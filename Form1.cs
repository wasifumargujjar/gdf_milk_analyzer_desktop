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

            // Wire designer controls to handlers (safe for designer-time)
            try { _testButton.Click += RunTestButton_Click; } catch { }
            try { _pdfButton.Click += PdfButton_Click; } catch { }
            try { _btnNewTest.Click += (s, e) => ClearParameterValues(); } catch { }
            try { _btnPortToggle.Click += (s, e) => TogglePort(); UpdatePortButtonText(); } catch { }
            try { _btnSetLocation.Click += (s, e) => { using var dlg = new LocationPickerForm(); if (dlg.ShowDialog(this) == DialogResult.OK) { SetLocationText($"Location: {dlg.SelectedLocation}"); _btnSetLocation.Visible = false; } }; } catch { }

            // Load analyzer parameters into parameter grids
            _ = LoadAnalyzerParametersAsync();

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

            // Handle runtime resizing for Right-anchored controls and splitter (do not put these in InitializeComponent)
            this.Resize += MainForm_Resize;
            // set initial positions
            MainForm_Resize(this, EventArgs.Empty);
        }

        private void MainForm_Resize(object? sender, EventArgs e)
        {
            try
            {
                if (_btnPortToggle != null) _btnPortToggle.Left = Math.Max(0, this.ClientSize.Width - 260);
                if (_btnSetLocation != null) _btnSetLocation.Left = Math.Max(0, this.ClientSize.Width - 170);
                if (_bottomSplit != null && _bottomSplit.Width > 0) _bottomSplit.SplitterDistance = _bottomSplit.Width / 2;
            }
            catch { }
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
            try
            {
                var name = txtName.Text.Trim();
                var nic = txtNIC.Text.Trim();
                var phone = txtPhone.Text.Trim();
                var email = txtEmail.Text.Trim();
                var whatsapp = txtWhatsapp.Text.Trim();
                var address = txtAddress.Text.Trim();

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
                    // Insert new
                    await Database.ExecuteNonQueryAsync(
                        "INSERT INTO dbo.Profile (Name, NIC, Phone, WhatsappNumber, Address, Email) VALUES (@Name, @NIC, @Phone, @Whatsapp, @Address, @Email);",
                        new SqlParameter("@Name", System.Data.SqlDbType.NVarChar, 200) { Value = (object?)name ?? DBNull.Value },
                        new SqlParameter("@NIC", System.Data.SqlDbType.NVarChar, 50) { Value = nicVal },
                        new SqlParameter("@Phone", System.Data.SqlDbType.NVarChar, 50) { Value = phoneVal },
                        new SqlParameter("@Whatsapp", System.Data.SqlDbType.NVarChar, 50) { Value = whatsappVal },
                        new SqlParameter("@Address", System.Data.SqlDbType.NVarChar, 500) { Value = (object?)address ?? DBNull.Value },
                        new SqlParameter("@Email", System.Data.SqlDbType.NVarChar, 200) { Value = emailVal }
                    );

                    // retrieve newly inserted ID
                    var newId = await Database.ExecuteScalarAsync<int?>("SELECT TOP 1 Id FROM dbo.Profile WHERE WhatsappNumber = @Whatsapp ORDER BY Id DESC",
                        new SqlParameter("@Whatsapp", System.Data.SqlDbType.NVarChar, 50) { Value = whatsappVal });

                    if (!newId.HasValue)
                        throw new InvalidOperationException("Failed to retrieve newly created profile id.");

                    profileId = newId.Value;
                }

                // store current profile id for subsequent serial operations
                _currentProfileId = profileId;

                // After profile upsert, run the simulated serial test with the profileId
                SerialSimulationTest.RunAndNotify(profileId);

                // Load latest result values for this profile into grid
                await LoadLatestResultsForProfileAsync(profileId);

                MessageBox.Show("Profile saved and test started.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save profile or start test: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task LoadAnalyzerParametersAsync()
        {
            try
            {
                _gridParams.Rows.Clear();
                _gridAdulteration.Rows.Clear();

                var rows = await MilkAnalyzerTest.DataAccess.Database.QueryAsync<(string Name, string KeyName)>(
                    "SELECT Name, KeyName FROM dbo.MilkAnalyzerParameters WHERE IsActive = 1 ORDER BY SortOrder, Name",
                    reader => (
                        reader.IsDBNull(0) ? string.Empty : reader.GetString(0),
                        reader.IsDBNull(1) ? string.Empty : reader.GetString(1)));

                foreach (var r in rows)
                {
                    _gridParams.Rows.Add(r.Name, string.Empty);
                    _gridAdulteration.Rows.Add(r.Name, string.Empty);
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
                    if (row.Cells.Count > 1) row.Cells[1].Value = string.Empty;
                }
            }
            if (_gridAdulteration != null)
            {
                foreach (DataGridViewRow row in _gridAdulteration.Rows)
                {
                    if (row.Cells.Count > 1) row.Cells[1].Value = string.Empty;
                }
            }
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
                // Update parameter grids with latest result values
                if (_gridParams != null) _gridParams.Rows.Clear();
                if (_gridAdulteration != null) _gridAdulteration.Rows.Clear();
                var resultId = await Database.GetLatestResultIdForProfileAsync(profileId);
                if (!resultId.HasValue) return;

                var values = await Database.GetResultValuesByResultIdAsync(resultId.Value);
                // Map by parameter name
                foreach (var v in values)
                {
                    _gridParams.Rows.Add(v.ParameterName, v.Value?.ToString() ?? string.Empty);
                    _gridAdulteration.Rows.Add(v.ParameterName, v.Value?.ToString() ?? string.Empty);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load results: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

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

            var url = $"users/getProfileInfo?{qs}";

            using var resp = await http.GetAsync(url);
            if (resp.StatusCode == System.Net.HttpStatusCode.NotFound) return null;

            resp.EnsureSuccessStatusCode();
            var body = await resp.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            var info = new ProfileInfo();
            if (root.TryGetProperty("Id", out var idElem) && idElem.ValueKind == JsonValueKind.Number) info.Id = idElem.GetInt32();
            info.FullName = root.TryGetProperty("fullName", out var fn) ? fn.GetString() : (root.TryGetProperty("userName", out var un) ? un.GetString() : null);
            info.Email = root.TryGetProperty("email", out var em) ? em.GetString() : null;
            info.Phone = root.TryGetProperty("phoneNumber", out var pn) ? pn.GetString() : null;
            info.Cnic = root.TryGetProperty("cnicNumber", out var cn) ? cn.GetString() : (root.TryGetProperty("cNICNumber", out var cn2) ? cn2.GetString() : null);

            return info;
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
                            })); } catch { }
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

        }
    }
}
