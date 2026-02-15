using System;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MilkAnalyzerTest.DataAccess;
using MilkAnalyzerTest.Services;
using MilkAnalyzerTest.Tests;
using Microsoft.Data.SqlClient;
using System.Net.Http;
using System.Text.Json;
using System.Device.Location;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace MilkAnalyzerTest
{
    public partial class MainForm : Form
    {
        // runtime-only state; UI controls are declared in Form1.Designer.cs
        private SerialPort _serialPort;
        private readonly StringBuilder _buffer = new();
        private int? _currentProfileId;
        private string? _lastPhoneLookupValue;
        private bool _phoneLookupInFlight;

        public MainForm()
        {
            InitializeComponent();
            InitializeSerial();

            // Wire designer controls to handlers
            try { _testButton.Click += RunTestButton_Click; } catch { }
            try { _pdfButton.Click += PdfButton_Click; } catch { }
            try { _btnNewTest.Click += (s, e) => OnNewTestClicked(); } catch { }
            try { _btnPortToggle.Click += (s, e) => TogglePort(); UpdatePortButtonText(); } catch { }
            try { _btnSetLocation.Click += (s, e) => { using var dlg = new LocationPickerForm(); if (dlg.ShowDialog(this) == DialogResult.OK) { SetLocationText($"Location: {dlg.SelectedLocation}"); _btnSetLocation.Visible = false; } }; } catch { }

            // Attach Enter key handlers to perform profile lookup
            try { txtPhone.KeyDown += LookupField_KeyDown; } catch { }
            try { txtNIC.KeyDown += LookupField_KeyDown; } catch { }
            try { txtEmail.KeyDown += LookupField_KeyDown; } catch { }
            // wire leave handler for phone
            try { txtPhone.Leave += Phone_Leave; } catch { }
            // ensure customer type default
            try { if (cmbCustomerType != null && cmbCustomerType.Items.Count > 0) cmbCustomerType.SelectedIndex = 0; } catch { }

            // adjust splitter initially
            try { if (_bottomSplit != null && _bottomSplit.Width > 0) _bottomSplit.SplitterDistance = _bottomSplit.Width / 2; } catch { }

            // Load analyzer parameters into parameter grids moved to MainForm_Load
            // initial button states
            try { _pdfButton.Enabled = false; } catch { }
            try { btnSendEmail.Enabled = false; } catch { }

            // hook selection changed for previous tests grid
            try { _gridPreviousTests.SelectionChanged += PreviousTests_SelectionChanged; } catch { }
        }

        // Click handler for the top "Run Test Insert" button
        private void RunTestButton_Click(object? sender, EventArgs e)
        {
            if (_currentProfileId.HasValue)
            {
                SerialSimulationTest.RunAndNotify(_currentProfileId.Value);
            }
            else
            {
                MessageBox.Show("No profile selected. Please fill the profile form and click 'Start Test' first.", "Profile Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void BtnStartTest_Click(object? sender, EventArgs e)
        {
            // Prevent double-clicks / concurrent runs
            try
            {
                try { if (btnStartTest != null) btnStartTest.Enabled = false; } catch { }

                var name = txtName.Text.Trim();
                var nic = txtNIC.Text.Trim();
                var phone = txtPhone.Text.Trim();
                var email = txtEmail.Text.Trim();
                var whatsapp = txtWhatsapp.Text.Trim();
                var address = txtAddress.Text.Trim();

                // Validation: require Name AND at least one of CNIC, Phone or Whatsapp
                var hasContact = !string.IsNullOrWhiteSpace(nic) || !string.IsNullOrWhiteSpace(phone) || !string.IsNullOrWhiteSpace(whatsapp);
                if (string.IsNullOrWhiteSpace(name) || !hasContact)
                {
                    MessageBox.Show("Please enter required fields: Name, and at least one of CNIC Number, Phone Number or Whatsapp Number.", "Missing Required Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    // set focus to first missing field for convenience
                    if (string.IsNullOrWhiteSpace(name))
                    {
                        try { txtName.Focus(); } catch { }
                    }
                    else if (!hasContact)
                    {
                        try
                        {
                            if (string.IsNullOrWhiteSpace(nic)) txtNIC.Focus();
                            else if (string.IsNullOrWhiteSpace(phone)) txtPhone.Focus();
                            else txtWhatsapp.Focus();
                        }
                        catch { }
                    }

                    return;
                }

                object nicVal = string.IsNullOrWhiteSpace(nic) ? DBNull.Value : (object)nic;
                object phoneVal = string.IsNullOrWhiteSpace(phone) ? DBNull.Value : (object)phone;
                object emailVal = string.IsNullOrWhiteSpace(email) ? DBNull.Value : (object)email;
                object whatsappVal = string.IsNullOrWhiteSpace(whatsapp) ? DBNull.Value : (object)whatsapp;

                // Find existing profile by WhatsappNumber or Phone or NIC
                var existingId = await Database.ExecuteScalarAsync<int?>(
                    "SELECT TOP 1 Id FROM dbo.Profile WHERE (@Whatsapp IS NOT NULL AND WhatsappNumber = @Whatsapp) OR (@Phone IS NOT NULL AND Phone = @Phone) OR (@NIC IS NOT NULL AND NIC = @NIC)",
                    new SqlParameter("@Whatsapp", System.Data.SqlDbType.NVarChar, 50) { Value = whatsappVal },
                    new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = phoneVal },
                    new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = nicVal }
                );

                int profileId;
                if (existingId.HasValue)
                {
                    profileId = existingId.Value;
                    // Update existing
                    await Database.ExecuteNonQueryAsync(
                        "UPDATE dbo.Profile SET Name = @Name, NIC = @NIC, Phone = @Phone, WhatsappNumber = @Whatsapp, Address = @Address, Email = @Email, UpdatedAt = SYSUTCDATETIME() WHERE Id = @Id",
                        new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = (object?)name ?? DBNull.Value },
                        new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = nicVal },
                        new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = phoneVal },
                        new SqlParameter("@Whatsapp", System.Data.SqlDbType.NVarChar, 50) { Value = whatsappVal },
                        new SqlParameter("@Address", System.Data.SqlDbType.NVarChar, 500) { Value = (object?)address ?? DBNull.Value },
                        new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 200) { Value = emailVal },
                        new SqlParameter("@Id", System.Data.SqlDbType.Int) { Value = existingId.Value }
                    );
                }
                else
                {
                    // Insert new and return inserted id using SCOPE_IDENTITY()
                    var newId = await Database.ExecuteScalarAsync<int?>(
                        "INSERT INTO dbo.Profile (Name, NIC, Phone, WhatsappNumber, Address, Email) VALUES (@Name, @NIC, @Phone, @Whatsapp, @Address, @Email); SELECT CAST(SCOPE_IDENTITY() AS int);",
                        new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = (object?)name ?? DBNull.Value },
                        new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = nicVal },
                        new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = phoneVal },
                        new SqlParameter("@Whatsapp", System.Data.SqlDbType.NVarChar, 50) { Value = whatsappVal },
                        new SqlParameter("@Address", System.Data.SqlDbType.NVarChar, 500) { Value = (object?)address ?? DBNull.Value },
                        new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 200) { Value = emailVal }
                    );

                    if (!newId.HasValue)
                        throw new InvalidOperationException("Failed to retrieve newly created profile id.");

                    profileId = newId.Value;
                }

                // store current profile id for subsequent serial operations
                _currentProfileId = profileId;

                // Run simulation (insert test data) but do NOT show additional message boxes here
                long insertedTestId = await SerialSimulationTest.RunSimulationAsync(profileId);

                // Reload and display latest result values for the profile used (ensures grids/columns exist)
                await LoadLatestResultsForProfileAsync(profileId);

                // Create PDF report for the latest result
                var latestId = await Database.GetLatestResultIdForProfileAsync(profileId);
                string? pdfFilePath = null;
                if (latestId.HasValue)
                {
                    var values = await Database.GetResultValuesByResultIdAsync(latestId.Value);
                    var customerName = txtName.Text.Trim();
                    var nicText = txtNIC.Text.Trim();
                    var date = DateTime.UtcNow;

                    // Ensure TestResults directory
                    var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
                    Directory.CreateDirectory(dir);

                    var fileName = $"MilkTest_{latestId.Value}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                    pdfFilePath = Path.Combine(dir, fileName);

                    PdfReportGenerator.GeneratePdfReport(pdfFilePath, latestId.Value, customerName, nicText, date, values);
                }

                // Perform optional WhatsApp send and API submit similar to PdfButton_Click
                var whatsappNumber = txtWhatsapp.Text.Trim();

                string? waError = null;
                string? apiError = null;

                var waTask = Task.CompletedTask;
                if (!string.IsNullOrWhiteSpace(whatsappNumber) && pdfFilePath != null)
                {
                    waTask = Task.Run(async () =>
                    {
                        try
                        {
                            await WhatsAppService.SendPdfReportToCustomerAsync(whatsappNumber, txtName.Text.Trim(), pdfFilePath);
                        }
                        catch (Exception ex)
                        {
                            waError = ex.Message;
                        }
                    });
                }

                var apiTask = Task.Run(async () =>
                {
                    try
                    {
                        var client = new MilkApiClient();
                        var token = await client.LoginAsync("ali@milkanalyzer.com", "Ali123!");

                        // Build submit DTO expected by the API
                        var parameters = new System.Collections.Generic.List<object>();
                        if (latestId.HasValue)
                        {
                            var values = await Database.GetResultValuesByResultIdAsync(latestId.Value);
                            foreach (var v in values)
                            {
                                double? parsed = null;
                                if (double.TryParse(Convert.ToString(v.Value), out var d)) parsed = d;
                                parameters.Add(new { KeyName = v.ParameterName, Value = parsed });
                            }
                        }

                        var submitDto = new
                        {
                            Email = txtEmail.Text.Trim(),
                            PhoneNumber = txtPhone.Text.Trim(),
                            FullName = txtName.Text.Trim(),
                            RawLine = string.Empty,
                            Parameters = parameters
                        };

                        await client.SubmitMilkTestAsync(token, submitDto);
                    }
                    catch (Exception ex)
                    {
                        apiError = ex.Message;
                    }
                });

                await Task.WhenAll(waTask, apiTask);

                // Single consolidated message at the end
                if (waError == null && apiError == null)
                {
                    MessageBox.Show("Profile saved, test started, PDF created (if applicable), and external submissions completed successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (waError == null && apiError != null)
                {
                    MessageBox.Show($"Profile saved and test started. PDF created (if applicable). API submit failed: {apiError}", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (waError != null && apiError == null)
                {
                    MessageBox.Show($"Profile saved and test started. PDF created (if applicable). WhatsApp send failed: {waError}", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Profile saved and test started. PDF created (if applicable). Both WhatsApp and API submit failed. WhatsApp: {waError} | API: {apiError}", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save profile or start test: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                try { if (btnStartTest != null) btnStartTest.Enabled = true; } catch { }
            }

            // refresh previous tests
            try { if (_currentProfileId.HasValue) await PopulatePreviousTestsAsync(_currentProfileId, true); else await PopulatePreviousTestsAsync(null); } catch { }
        }

        private async void PreviousTests_SelectionChanged(object? sender, EventArgs e)
        {
            try
            {
                if (_gridPreviousTests == null || _gridPreviousTests.SelectedRows.Count == 0)
                {
                    try { _pdfButton.Enabled = false; } catch { }
                    try { btnSendEmail.Enabled = false; } catch { }
                    return;
                }

                var row = _gridPreviousTests.SelectedRows[0];
                if (row.Cells.Count == 0) return;
                var idObj = row.Cells[0].Value;
                if (idObj == null) return;
                if (!int.TryParse(Convert.ToString(idObj), out var resultId)) return;

                // populate params grid values from this result
                var values = await Database.GetResultValuesWithIdsByResultIdAsync(resultId);
                var map = new System.Collections.Generic.Dictionary<int, double?>();
                foreach (var v in values) map[v.ParameterId] = v.Value;

                foreach (DataGridViewRow prow in _gridParams.Rows)
                {
                    try
                    {
                        if (prow.Cells.Count < 3) continue;
                        var pidObj = prow.Cells[0].Value;
                        if (pidObj == null) { prow.Cells[2].Value = string.Empty; continue; }
                        if (!int.TryParse(Convert.ToString(pidObj), out var pid)) { prow.Cells[2].Value = string.Empty; continue; }
                        if (map.TryGetValue(pid, out var val)) prow.Cells[2].Value = val.HasValue ? val.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
                        else prow.Cells[2].Value = string.Empty;
                    }
                    catch { }
                }

                // enable PDF send
                try { _pdfButton.Enabled = true; } catch { }

                // enable email if profile loaded and email present
                try
                {
                    var hasEmail = !string.IsNullOrWhiteSpace(txtEmail.Text);
                    btnSendEmail.Enabled = _currentProfileId.HasValue && hasEmail;
                }
                catch { }
            }
            catch { }
        }

        private async Task LoadAnalyzerParametersAsync()
        {
            try
            {
                // Prepopulate designer parameter grid with Id and Name (Value empty)
                _gridParams.Rows.Clear();
                // clear previous tests grid; it will be populated from API/local DB
                _gridPreviousTests.Rows.Clear();

                var rows = await MilkAnalyzerTest.DataAccess.Database.QueryAsync<(int Id, string Name, string KeyName)>(
                    "SELECT Id, Name, KeyName FROM dbo.MilkAnalyzerParameters WHERE IsActive = 1 ORDER BY SortOrder, Name",
                    reader => (
                        reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                        reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                        reader.IsDBNull(2) ? string.Empty : reader.GetString(2)));

                foreach (var r in rows)
                {
                    // Designer columns: [ParameterId(hidden), Parameter(Name), Value]
                    _gridParams.Rows.Add(r.Id, r.Name, string.Empty);
                }
            }
            catch
            {
                // ignore load errors
            }
        }

        private void ClearParameterValues()
        {
            if (_gridParams != null)
            {
                foreach (DataGridViewRow row in _gridParams.Rows)
                {
                    if (row.Cells.Count > 2) row.Cells[2].Value = string.Empty;
                }
            }
            // previous tests grid values are independent; do not clear rows here
        }

        private void OnNewTestClicked()
        {
            try
            {
                // Clear inputs
                txtName.Text = string.Empty;
                txtNIC.Text = string.Empty;
                txtPhone.Text = string.Empty;
                txtWhatsapp.Text = string.Empty;
                txtEmail.Text = string.Empty;
                txtAddress.Text = string.Empty;
                _currentProfileId = null;

                // Clear parameter value columns but keep names
                ClearParameterValues();

                // Clear previous tests list
                try { if (_gridPreviousTests != null) _gridPreviousTests.Rows.Clear(); } catch { }

                // Disable actions until a test/profile is selected
                try { _pdfButton.Enabled = false; } catch { }
                try { btnSendEmail.Enabled = false; } catch { }
            }
            catch { }
        }

        private void TogglePort()
        {
            try
            {
                if (_serialPort == null)
                {
                    InitializeSerial();
                    UpdatePortButtonText();
                    return;
                }

                if (_serialPort.IsOpen)
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    _serialPort.Close();
                    UpdatePortButtonText();
                }
                else
                {
                    _serialPort.DataReceived += SerialPort_DataReceived;
                    _serialPort.Open();
                    UpdatePortButtonText();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Port toggle failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void UpdatePortButtonText()
        {
            try
            {
                if (_btnPortToggle == null) return;
                if (_serialPort != null && _serialPort.IsOpen)
                {
                    _btnPortToggle.Text = "Disconnect";
                }
                else
                {
                    _btnPortToggle.Text = "Connect";
                }
            }
            catch { }
        }

        private async Task LoadLatestResultsForProfileAsync(int profileId)
        {
            try
            {
                // Update parameter grids' Value column using parameter ids
                if (_gridParams == null) return;
                if (_gridParams.Rows.Count == 0) return; // nothing to map

                var resultId = await Database.GetLatestResultIdForProfileAsync(profileId);
                if (!resultId.HasValue) return;

                var values = await Database.GetResultValuesWithIdsByResultIdAsync(resultId.Value);
                // build map by parameter id
                var map = new System.Collections.Generic.Dictionary<int, double?>();
                foreach (var v in values)
                {
                    map[v.ParameterId] = v.Value;
                }

                // For each row in _gridParams find matching param id in first cell and set value in third cell
                foreach (DataGridViewRow row in _gridParams.Rows)
                {
                    try
                    {
                        if (row.Cells.Count < 3) continue;
                        var pidObj = row.Cells[0].Value;
                        if (pidObj == null) { row.Cells[2].Value = string.Empty; continue; }
                        if (!int.TryParse(Convert.ToString(pidObj), out var pid)) { row.Cells[2].Value = string.Empty; continue; }
                        if (map.TryGetValue(pid, out var val))
                        {
                            row.Cells[2].Value = val.HasValue ? val.Value.ToString(System.Globalization.CultureInfo.InvariantCulture) : string.Empty;
                        }
                        else
                        {
                            row.Cells[2].Value = string.Empty;
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load results: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Keep design-time columns; runtime ensure not needed anymore
        private void EnsureGridColumns(DataGridView? grid) { }

        // Initialize serial port and subscribe to data events
        private void InitializeSerial()
        {
            try
            {
                if (_serialPort != null) return;
                _serialPort = new SerialPort("COM8", 9600, Parity.None, 8, StopBits.One)
                {
                    ReadTimeout = 1000,
                    DtrEnable = false,
                    RtsEnable = false,
                    NewLine = "\n"
                };
                _serialPort.DataReceived += SerialPort_DataReceived;
                try
                {
                    _serialPort.Open();
                }
                catch
                {
                    // ignore open errors; UpdatePortButtonText will show Connect
                }
                UpdatePortButtonText();
            }
            catch { }
        }

        // Serial data handler
        private void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                var data = _serialPort?.ReadExisting();
                if (string.IsNullOrEmpty(data)) return;

                _buffer.Append(data);
                var all = _buffer.ToString();
                int idx;
                while ((idx = all.IndexOf('\n')) >= 0)
                {
                    var message = all.Substring(0, idx).Trim('\r', '\n');
                    all = all.Substring(idx + 1);
                    _buffer.Clear();
                    if (!string.IsNullOrEmpty(all)) _buffer.Append(all);
                    _ = ProcessMessageAsync(message);
                }

                File.AppendAllLines("C:\\serial_log.txt" , new[] { $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} - Received: {data.Trim()}" });
            }
            catch (Exception ex)
            {
                try { BeginInvoke((Action)(() => MessageBox.Show($"Error reading serial data: {ex.Message}", "Serial Port Error", MessageBoxButtons.OK, MessageBoxIcon.Error))); } catch { }
            }
        }

        private async Task ProcessMessageAsync(string message)
        {
            try
            {
                if (!_currentProfileId.HasValue)
                {
                    BeginInvoke((Action)(() =>
                    {
                        MessageBox.Show("No profile selected. Please create/select a profile before receiving serial data.", "Profile Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }));
                    return;
                }

                // Insert into DB. No parsed values for now.
                var values = new (string name, string value, string type)[] { ("raw", message, "string") };
                var id = await Database.InsertMilkTestResultAsync(message, values, _currentProfileId.Value);

                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Saved message with id {id}", "Serial Data Saved", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }));

                // Reload and display latest result values in the grid for the profile used
                await LoadLatestResultsForProfileAsync(_currentProfileId.Value);
            }
            catch (Exception ex)
            {
                BeginInvoke((Action)(() =>
                {
                    MessageBox.Show($"Failed to save message to DB: {ex.Message}", "DB Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }));
            }
        }

        private async void PdfButton_Click(object? sender, EventArgs e)
        {
            try
            {
                if (!_currentProfileId.HasValue)
                {
                    MessageBox.Show("Select a profile first.", "No Profile", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var latestId = await Database.GetLatestResultIdForProfileAsync(_currentProfileId.Value);
                if (!latestId.HasValue)
                {
                    MessageBox.Show("No test result found for the selected profile.", "No Result", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                var customerName = txtName.Text.Trim();
                var nic = txtNIC.Text.Trim();
                var date = DateTime.UtcNow;
                var values = await Database.GetResultValuesByResultIdAsync(latestId.Value);

                // Ensure TestResults directory
                var dir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestResults");
                Directory.CreateDirectory(dir);

                var fileName = $"MilkTest_{latestId.Value}_{DateTime.Now:yyyyMMdd_HHmmss}.pdf";
                var filePath = Path.Combine(dir, fileName);

                PdfReportGenerator.GeneratePdfReport(filePath, latestId.Value, customerName, nic, date, values);

                MessageBox.Show($"PDF created: {filePath}", "PDF Generated", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Prepare independent operations: WhatsApp send and API submit
                var whatsappNumber = txtWhatsapp.Text.Trim();

                string? waError = null;
                string? apiError = null;

                // Build submit DTO expected by the API
                var parameters = new System.Collections.Generic.List<object>();
                foreach (var v in values)
                {
                    double? parsed = null;
                    if (double.TryParse(Convert.ToString(v.Value), out var d)) parsed = d;
                    parameters.Add(new { KeyName = v.ParameterName, Value = parsed });
                }

                var submitDto = new
                {
                    Email = txtEmail.Text.Trim(),
                    PhoneNumber = txtPhone.Text.Trim(),
                    FullName = customerName,
                    RawLine = string.Empty,
                    Parameters = parameters
                };

                // Start both tasks and wait for both to finish, errors handled independently
                var waTask = Task.CompletedTask;
                if (!string.IsNullOrWhiteSpace(whatsappNumber))
                {
                    waTask = Task.Run(async () =>
                    {
                        try
                        {
                            await WhatsAppService.SendPdfReportToCustomerAsync(whatsappNumber, customerName, filePath);
                        }
                        catch (Exception ex)
                        {
                            waError = ex.Message;
                        }
                    });
                }
                else
                {
                    waError = "WhatsApp number not found.";
                }

                var apiTask = Task.Run(async () =>
                {
                    try
                    {
                        var client = new MilkApiClient();
                        // Use configured credentials
                        var token = await client.LoginAsync("ali@milkanalyzer.com", "Ali123!");
                        await client.SubmitMilkTestAsync(token, submitDto);
                    }
                    catch (Exception ex)
                    {
                        apiError = ex.Message;
                    }
                });

                await Task.WhenAll(waTask, apiTask);

                // Report combined result without letting one failure cancel the other
                if (waError == null && apiError == null)
                {
                    MessageBox.Show("PDF created, sent via WhatsApp, and test submitted to API successfully.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                else if (waError == null && apiError != null)
                {
                    MessageBox.Show($"PDF sent via WhatsApp. API submit failed: {apiError}", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (waError != null && apiError == null)
                {
                    MessageBox.Show($"PDF submitted to API. WhatsApp send failed: {waError}", "Partial Success", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show($"Both WhatsApp and API submit failed. WhatsApp: {waError} | API: {apiError}\nFile saved at: {filePath}", "Failure", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to generate PDF: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void InitializeLocation()
        {
            SetLocationText("Location: fetching...");

            try
            {
                var watcher = new GeoCoordinateWatcher(GeoPositionAccuracy.Default)
                {
                    MovementThreshold = 1
                };

                watcher.PositionChanged += (s, e) =>
                {
                    try
                    {
                        var coord = e.Position.Location;
                        if (coord != null && !coord.IsUnknown)
                        {
                            SetLocationText($"Location: {coord.Latitude:F6}, {coord.Longitude:F6}");
                            try { watcher.Stop(); } catch { }
                        }
                    }
                    catch { }
                };

                watcher.StatusChanged += (s, e) =>
                {
                    if (e.Status == GeoPositionStatus.Disabled || e.Status == GeoPositionStatus.NoData)
                    {
                        SetLocationText("Location unavilable");
                        try { watcher.Stop(); } catch { }
                    }
                };

                _ = Task.Run(() =>
                {
                    try
                    {
                        if (!watcher.TryStart(false, TimeSpan.FromSeconds(5)))
                        {
                            SetLocationText("Location unavilable");
                            try { watcher.Stop(); } catch { }
                        }
                    }
                    catch
                    {
                        SetLocationText("Location unavilable");
                        try { watcher.Stop(); } catch { }
                    }
                });

                _ = Task.Run(async () =>
                {
                    await Task.Delay(TimeSpan.FromSeconds(8));
                    if (lblLocation.Text.Contains("fetching")) SetLocationText("Location unavilable");
                    try { watcher.Stop(); } catch { }
                });
            }
            catch
            {
                SetLocationText("Location unavilable");
            }
        }

        private void SetLocationText(string text)
        {
            if (lblLocation == null) return;
            if (lblLocation.InvokeRequired)
            {
                lblLocation.Invoke(new Action(() => lblLocation.Text = text));
            }
            else
            {
                lblLocation.Text = text;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);

            if (_serialPort != null)
            {
                try
                {
                    _serialPort.DataReceived -= SerialPort_DataReceived;
                    if (_serialPort.IsOpen) _serialPort.Close();
                    _serialPort.Dispose();
                }
                catch { }
            }
        }

        private async void LookupField_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.Enter) return;
            e.SuppressKeyPress = true;

            try
            {
                string? phone = null, nic = null, email = null;
                if (sender == txtPhone) phone = txtPhone.Text?.Trim();
                else if (sender == txtNIC) nic = txtNIC.Text?.Trim();
                else if (sender == txtEmail) email = txtEmail.Text?.Trim();

                var info = await TryAutoFillProfileAsync(phone, nic, email);
                if (info != null)
                {
                    // apply directly on UI thread
                    if (!string.IsNullOrWhiteSpace(info.FullName)) txtName.Text = info.FullName;
                    if (!string.IsNullOrWhiteSpace(info.Cnic)) txtNIC.Text = info.Cnic;
                    if (!string.IsNullOrWhiteSpace(info.Phone)) txtPhone.Text = info.Phone;
                    if (!string.IsNullOrWhiteSpace(info.Email)) txtEmail.Text = info.Email;
                    if (info.Id.HasValue) _currentProfileId = info.Id.Value;
                    if (!string.IsNullOrWhiteSpace(info.Phone)) _lastPhoneLookupValue = info.Phone;

                    // Load previous tests for this profile and select latest (try API then DB fallback)
                    try { await PopulatePreviousTestsAsync(info.Id, true); } catch (Exception ex) { Debug.WriteLine($"PopulatePreviousTestsAsync failed: {ex}"); }
                }
            }
            catch (Exception ex)
            {
                try { MessageBox.Show($"Lookup failed: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); } catch { }
            }
        }

        private class ProfileInfo
        {
            public int? Id { get; set; }
            public string? FullName { get; set; }
            public string? Email { get; set; }
            public string? Phone { get; set; }
            public string? Cnic { get; set; }
        }

        private async Task<ProfileInfo?> TryAutoFillProfileAsync(string? phoneNumber, string? cnicNumber, string? email)
        {
            phoneNumber = string.IsNullOrWhiteSpace(phoneNumber) ? null : phoneNumber.Trim();
            cnicNumber = string.IsNullOrWhiteSpace(cnicNumber) ? null : cnicNumber.Trim();
            email = string.IsNullOrWhiteSpace(email) ? null : email.Trim();

            if (phoneNumber == null && cnicNumber == null && email == null) return null;

            using var http = new HttpClient();
            var baseUrl = MilkAnalyzerTest.Config.Settings.ApiBaseUrl ?? string.Empty;
            if (!baseUrl.EndsWith("/")) baseUrl += '/';
            http.BaseAddress = new Uri(baseUrl);

            var qs = new System.Text.StringBuilder();
            if (phoneNumber != null) qs.Append($"phoneNumber={Uri.EscapeDataString(phoneNumber)}");
            else if (cnicNumber != null) qs.Append($"cnicNumber={Uri.EscapeDataString(cnicNumber)}");
            else if (email != null) qs.Append($"email={Uri.EscapeDataString(email)}");

            var url = $"users/profile/info?{qs}";

            using var resp = await http.GetAsync(url);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();

            try
            {
                using var doc = JsonDocument.Parse(body);
                var root = doc.RootElement;

                var info = new ProfileInfo();

                // Accept multiple possible id property names
                if (root.TryGetProperty("Id", out var idElem) && idElem.ValueKind == JsonValueKind.Number) info.Id = idElem.GetInt32();
                else if (root.TryGetProperty("id", out var idElem2) && idElem2.ValueKind == JsonValueKind.Number) info.Id = idElem2.GetInt32();
                else if (root.TryGetProperty("userId", out var idElem3) && idElem3.ValueKind == JsonValueKind.Number) info.Id = idElem3.GetInt32();
                else if (root.TryGetProperty("user_id", out var idElem4) && idElem4.ValueKind == JsonValueKind.Number) info.Id = idElem4.GetInt32();
                else if (root.TryGetProperty("sub", out var idElem5) && idElem5.ValueKind == JsonValueKind.Number) info.Id = idElem5.GetInt32();

                info.FullName = root.TryGetProperty("fullName", out var fn) ? fn.GetString() : (root.TryGetProperty("userName", out var un) ? un.GetString() : null);
                info.Email = root.TryGetProperty("email", out var em) ? em.GetString() : null;
                info.Phone = root.TryGetProperty("phoneNumber", out var pn) ? pn.GetString() : null;
                info.Cnic = root.TryGetProperty("cnicNumber", out var cn) ? cn.GetString() : (root.TryGetProperty("cNICNumber", out var cn2) ? cn2.GetString() : null);

                return info;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"TryAutoFillProfileAsync: failed to parse profile response: {ex}");
                return null;
            }
        }

        // Called when phone textbox loses focus; performs lookup if value changed and none in-flight
        private void Phone_Leave(object? sender, EventArgs e)
        {
            try
            {
                var val = txtPhone.Text?.Trim();
                if (string.IsNullOrWhiteSpace(val)) return;
                if (_phoneLookupInFlight) return;
                if (_lastPhoneLookupValue != null && string.Equals(_lastPhoneLookupValue, val, StringComparison.OrdinalIgnoreCase)) return;

                _phoneLookupInFlight = true;
                _ = Task.Run(async () =>
                {
                    try
                    {
                        var info = await TryAutoFillProfileAsync(val, null, null);
                        if (info != null)
                        {
                            try { BeginInvoke((Action)(() =>
                            {
                                if (!string.IsNullOrWhiteSpace(info.FullName)) txtName.Text = info.FullName;
                                if (!string.IsNullOrWhiteSpace(info.Cnic)) txtNIC.Text = info.Cnic;
                                if (!string.IsNullOrWhiteSpace(info.Phone)) txtPhone.Text = info.Phone;
                                if (!string.IsNullOrWhiteSpace(info.Email)) txtEmail.Text = info.Email;
                                if (info.Id.HasValue) _currentProfileId = info.Id.Value;
                                if (!string.IsNullOrWhiteSpace(info.Phone)) _lastPhoneLookupValue = info.Phone;
                                // enable Send Email if email available
                                try { btnSendEmail.Enabled = info.Id.HasValue && !string.IsNullOrWhiteSpace(info.Email); } catch { }
                            })); } catch { }
                            // After autofill, load previous tests for this profile and select latest
                            try
                            {
                                BeginInvoke((Action)(() => { _ = PopulatePreviousTestsAsync(info.Id, true); }));
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"PopulatePreviousTestsAsync invoke failed: {ex}");
                            }
                        }
                    }
                    finally
                    {
                        _phoneLookupInFlight = false;
                    }
                });
            }
            catch { }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            // call async loader and ignore
            MainForm_LoadAsync(sender, e);
        }

        private async void MainForm_LoadAsync(object sender, EventArgs e)
        {
            // Load parameters after form creation
            try
            {
                await LoadAnalyzerParametersAsync();
                // populate previous tests (try API with token, fallback to local DB)
                await PopulatePreviousTestsAsync(_currentProfileId);
            }
            catch (Exception ex)
            {
                try { MessageBox.Show($"Failed to load analyzer parameters: {ex.Message}", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning); } catch { }
            }
        }

        // Populate the previous tests grid using API (my-results) using userId from token, fallback to local DB
        private async Task PopulatePreviousTestsAsync(int? profileId, bool selectLatest = false)
        {
            try
            {
                // Clear grid
                _gridPreviousTests.Rows.Clear();

                // Try API using token to extract userId
                int? userIdFromToken = null;
                try
                {
                    var token = MilkAnalyzerTest.Services.TokenStore.Token;
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        var parts = token.Split('.');
                        if (parts.Length >= 2)
                        {
                            var payload = parts[1];
                            var padded = payload.PadRight(payload.Length + (4 - payload.Length % 4) % 4, '=');
                            var bytes = Convert.FromBase64String(padded.Replace('-', '+').Replace('_', '/'));
                            var json = System.Text.Encoding.UTF8.GetString(bytes);
                            using var doc = JsonDocument.Parse(json);
                            if (doc.RootElement.TryGetProperty("id", out var idElem) && idElem.ValueKind == JsonValueKind.Number)
                            {
                                userIdFromToken = idElem.GetInt32();
                            }
                        }
                    }
                }
                catch { }

                bool loadedFromApi = false;
                if (userIdFromToken.HasValue)
                {
                    try
                    {
                        var client = new MilkAnalyzerTest.Services.MilkApiClient();
                        // call my-results?userId=...
                        using var http = new System.Net.Http.HttpClient();
                        var baseUrl = MilkAnalyzerTest.Config.Settings.ApiBaseUrl ?? string.Empty;
                        if (!baseUrl.EndsWith("/")) baseUrl += '/';
                        http.BaseAddress = new Uri(baseUrl);
                        var reqUrl = $"my-results?userId={userIdFromToken.Value}&page=1&pageSize=50";
                        using var req = new System.Net.Http.HttpRequestMessage(System.Net.Http.HttpMethod.Get, reqUrl);
                        var token = MilkAnalyzerTest.Services.TokenStore.Token;
                        if (!string.IsNullOrWhiteSpace(token)) req.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
                        using var resp = await http.SendAsync(req);
                        if (resp.IsSuccessStatusCode)
                        {
                            var body = await resp.Content.ReadAsStringAsync();
                            using var doc = JsonDocument.Parse(body);
                            // assume root is array or { data: [ ... ] }
                            var arr = doc.RootElement.ValueKind == JsonValueKind.Array ? doc.RootElement : (doc.RootElement.TryGetProperty("data", out var d) ? d : default(JsonElement));
                            if (arr.ValueKind == JsonValueKind.Array)
                            {
                                foreach (var item in arr.EnumerateArray())
                                {
                                    var id = item.TryGetProperty("id", out var idp) && idp.ValueKind == JsonValueKind.Number ? idp.GetInt32() : 0;
                                    var ts = item.TryGetProperty("timestampUtc", out var tsp) && tsp.ValueKind == JsonValueKind.String ? tsp.GetString() : null;
                                    var display = ts ?? item.ToString();
                                    _gridPreviousTests.Rows.Add(id, display, string.Empty);
                                }
                                loadedFromApi = true;
                            }
                        }
                    }
                    catch { }
                }

                if (!loadedFromApi)
                {
                    // fallback to local DB: determine profile id (use provided profileId or lookup by phone)
                    int? pidToUse = profileId;
                    if (!pidToUse.HasValue)
                    {
                        try
                        {
                            var phone = txtPhone?.Text?.Trim();
                            if (!string.IsNullOrWhiteSpace(phone))
                            {
                                // Normalize phone by removing non-digits for lookup
                                var norm = Regex.Replace(phone, "\\D", string.Empty);
                                if (string.IsNullOrWhiteSpace(norm)) norm = phone;

                                pidToUse = await Database.ExecuteScalarAsync<int?>(
                                    "SELECT TOP 1 Id FROM dbo.Profile WHERE REPLACE(REPLACE(REPLACE(Phone, ' ', ''), '-', ''), '+', '') = @Phone OR REPLACE(REPLACE(REPLACE(WhatsappNumber, ' ', ''), '-', ''), '+', '') = @Phone",
                                    new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = norm }
                                );
                            }
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine($"PopulatePreviousTestsAsync fallback lookup failed: {ex}");
                            pidToUse = null;
                        }
                    }

                    if (pidToUse.HasValue)
                    {
                        var rows = await Database.GetResultsForProfileAsync(pidToUse.Value);
                        foreach (var r in rows)
                        {
                            _gridPreviousTests.Rows.Add(r.ResultId, r.TimestampUtc.ToString("s"), string.Empty);
                        }
                    }
                }

                // If requested, select the latest (first) row and load its parameters
                if (selectLatest)
                {
                    try
                    {
                        if (_gridPreviousTests.Rows.Count > 0)
                        {
                            _gridPreviousTests.ClearSelection();
                            var first = _gridPreviousTests.Rows[0];
                            first.Selected = true;
                            _gridPreviousTests.CurrentCell = first.Cells.Count > 1 ? first.Cells[1] : first.Cells[0];
                            // Trigger selection handler to load parameters
                            PreviousTests_SelectionChanged(null, EventArgs.Empty);
                        }
                    }
                    catch { }
                }
            }
            catch { }
        }
    }
}
