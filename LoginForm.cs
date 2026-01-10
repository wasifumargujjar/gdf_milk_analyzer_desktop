using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using MilkAnalyzerTest.Services;

namespace MilkAnalyzerTest
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
            // Allow pressing Enter to trigger the login button
            try { this.AcceptButton = btnLogin; } catch { }
        }

        private async void BtnLogin_Click(object? sender, EventArgs e)
        {
        }

        private async void btnLogin_Click_1(object sender, EventArgs e)
        {
            var username = txtUsername.Text.Trim();
            var password = txtPassword.Text;

            if (string.IsNullOrEmpty(username))
            {
                MessageBox.Show("Username is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrEmpty(password))
            {
                MessageBox.Show("Password is required", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            btnLogin.Enabled = false;
            try
            {
                var client = new MilkApiClient();
                var token = await client.LoginAsync(username, password);
                TokenStore.Token = token;

                // Signal success to Program.cs
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login failed: {ex.Message}", "Login Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnLogin.Enabled = true;
            }
        }
    }
}
