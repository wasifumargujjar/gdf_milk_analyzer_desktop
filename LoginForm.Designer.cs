namespace MilkAnalyzerTest
{
    partial class LoginForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label lblUsername;
        private System.Windows.Forms.Label lblPassword;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblUsername = new System.Windows.Forms.Label();
            lblPassword = new System.Windows.Forms.Label();
            txtUsername = new System.Windows.Forms.TextBox();
            txtPassword = new System.Windows.Forms.TextBox();
            btnLogin = new System.Windows.Forms.Button();
            SuspendLayout();
            // 
            // lblUsername
            // 
            lblUsername.Location = new System.Drawing.Point(12, 15);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new System.Drawing.Size(80, 23);
            lblUsername.Text = "Username:";
            // 
            // lblPassword
            // 
            lblPassword.Location = new System.Drawing.Point(12, 55);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new System.Drawing.Size(80, 23);
            lblPassword.Text = "Password:";
            // 
            // txtUsername
            // 
            txtUsername.Location = new System.Drawing.Point(100, 12);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new System.Drawing.Size(220, 27);
            // 
            // txtPassword
            // 
            txtPassword.Location = new System.Drawing.Point(100, 52);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new System.Drawing.Size(220, 27);
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.Location = new System.Drawing.Point(100, 95);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new System.Drawing.Size(100, 30);
            btnLogin.Text = "Login";
            btnLogin.UseVisualStyleBackColor = true;
            btnLogin.Click += BtnLogin_Click;
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(340, 140);
            Controls.Add(lblUsername);
            Controls.Add(lblPassword);
            Controls.Add(txtUsername);
            Controls.Add(txtPassword);
            Controls.Add(btnLogin);
            Name = "LoginForm";
            Text = "Login";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
