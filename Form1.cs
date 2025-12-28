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
        private SerialPort _serialPort;
        private readonly StringBuilder _buffer = new();
        private Button _testButton;
        private Button _pdfButton;
        private int? _currentProfileId;
        private DataGridView _grid;
        private DataGridView _gridParams;
        private DataGridView _gridAdulteration;
        private Button _btnNewTest;
        private Button _btnPortToggle;
        private Button _btnSetLocation;
        private SplitContainer _bottomSplit;

        public MainForm()
        {
            InitializeComponent();
            InitializeSerial();
            AddTestButton();
            AddResultsGrid();
            AddTopButtons();
            // Load analyzer parameters into parameter grids
            _ = LoadAnalyzerParametersAsync();
        }

        private void AddTopButtons()
        {
            // New Test button at top-left
            _btnNewTest = new Button
            {
                Text = "New Test",
                Left = 140,
                Top = 10,
                Width = 100,
                Height = 30
            };
            _btnNewTest.Click += (s, e) =>
            {
                // Clear values column in parameter grids
                ClearParameterValues();
            };
            Controls.Add(_btnNewTest);
            _btnNewTest.BringToFront();

            // Port toggle button at top-right (placed before location label)
            _btnPortToggle = new Button
            {
                Width = 80,
                Height = 24,
                Top = 8,
                Left = this.ClientSize.Width - 260,
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };
            _btnPortToggle.Click += (s, e) => TogglePort();
            Controls.Add(_btnPortToggle);
            UpdatePortButtonText();
            _btnPortToggle.BringToFront();

            // Set Location button (hidden by default)
            _btnSetLocation = new Button
            {
                Text = "Set Location",
                Width = 100,
                Height = 24,
                Top = 8,
                Left = this.ClientSize.Width - 170,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Visible = false
            };
            _btnSetLocation.Click += (s, e) =>
            {
                using var dlg = new LocationPickerForm();
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    SetLocationText($"Location: {dlg.SelectedLocation}");
                    _btnSetLocation.Visible = false;
                }
            };
            Controls.Add(_btnSetLocation);
            _btnSetLocation.BringToFront();
        }

        private void AddResultsGrid()
        {
            // Use SplitContainer so each grid gets 50% width reliably
            var split = new SplitContainer
            {
                Dock = DockStyle.Bottom,
                Height = 300,
                Orientation = Orientation.Vertical,
                SplitterDistance = this.ClientSize.Width / 2,
                IsSplitterFixed = false
            };

            _gridParams = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = false,
                AllowUserToAddRows = false,
                ColumnCount = 2,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _gridParams.Columns[0].Name = "Parameter";
            _gridParams.Columns[1].Name = "Value";

            _gridAdulteration = new DataGridView
            {
                Dock = DockStyle.Fill,
                ReadOnly = false,
                AllowUserToAddRows = false,
                ColumnCount = 2,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };
            _gridAdulteration.Columns[0].Name = "Parameter";
            _gridAdulteration.Columns[1].Name = "Value";

            split.Panel1.Controls.Add(_gridParams);
            split.Panel2.Controls.Add(_gridAdulteration);
            Controls.Add(split);
            _bottomSplit = split;
            // Ensure initial equal split after layout
            this.Resize += (s, e) =>
            {
                try
                {
                    if (_bottomSplit != null && _bottomSplit.Width > 0)
                        _bottomSplit.SplitterDistance = _bottomSplit.Width / 2;
                    if (_btnPortToggle != null)
                        _btnPortToggle.Left = this.ClientSize.Width - 260;
                    if (_btnSetLocation != null)
                        _btnSetLocation.Left = this.ClientSize.Width - 170;
                }
                catch { }
            };
            // Trigger once to set positions
            try { if (_bottomSplit != null && _bottomSplit.Width > 0) _bottomSplit.SplitterDistance = _bottomSplit.Width / 2; } catch { }

            // PDF button (position it next to the designer Start Test button)
            _pdfButton = new Button
            {
                Width = 120,
                Height = 30,
                Text = "Generate PDF"
            };
            // If the designer button exists, position relative to it; otherwise use defaults.
            try
            {
                _pdfButton.Left = btnStartTest.Left + btnStartTest.Width + 10;
                _pdfButton.Top = btnStartTest.Top;
            }
            catch
            {
                _pdfButton.Left = 300;
                _pdfButton.Top = 12;
            }
            _pdfButton.Click += PdfButton_Click;
            Controls.Add(_pdfButton);
            _pdfButton.BringToFront();
        }

        private void AddTestButton()
        {
            _testButton = new Button
            {
                Text = "Run Test Insert",
                Width = 120,
                Height = 30,
                Left = 10,
                Top = 10
            };
            _testButton.Click += RunTestButton_Click;
            Controls.Add(_testButton);
        }

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
    }
}
