namespace MilkAnalyzerTest
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.TextBox txtNIC;
        private System.Windows.Forms.TextBox txtPhone;
        private System.Windows.Forms.TextBox txtWhatsapp;
        private System.Windows.Forms.TextBox txtAddress;
        private System.Windows.Forms.Button btnStartTest;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.Label lblNIC;
        private System.Windows.Forms.Label lblPhone;
        private System.Windows.Forms.Label lblWhatsapp;
        private System.Windows.Forms.Label lblAddress;
        private System.Windows.Forms.TextBox txtEmail;
        private System.Windows.Forms.Label lblEmail;
        private System.Windows.Forms.Label lblLocation;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            lblName = new System.Windows.Forms.Label();
            lblNIC = new System.Windows.Forms.Label();
            lblPhone = new System.Windows.Forms.Label();
            lblWhatsapp = new System.Windows.Forms.Label();
            lblAddress = new System.Windows.Forms.Label();
            lblEmail = new System.Windows.Forms.Label();
            txtName = new System.Windows.Forms.TextBox();
            txtNIC = new System.Windows.Forms.TextBox();
            txtPhone = new System.Windows.Forms.TextBox();
            txtEmail = new System.Windows.Forms.TextBox();
            txtWhatsapp = new System.Windows.Forms.TextBox();
            txtAddress = new System.Windows.Forms.TextBox();
            btnStartTest = new System.Windows.Forms.Button();
            lblLocation = new System.Windows.Forms.Label();
            SuspendLayout();
            // 
            // lblName
            // 
            lblName.Location = new System.Drawing.Point(10, 15);
            lblName.Name = "lblName";
            lblName.Size = new System.Drawing.Size(100, 23);
            lblName.TabIndex = 0;
            lblName.Text = "Name:";
            // 
            // lblNIC
            // 
            lblNIC.Location = new System.Drawing.Point(10, 50);
            lblNIC.Name = "lblNIC";
            lblNIC.Size = new System.Drawing.Size(100, 23);
            lblNIC.TabIndex = 1;
            lblNIC.Text = "NIC:";
            // 
            // lblPhone
            // 
            lblPhone.Location = new System.Drawing.Point(10, 85);
            lblPhone.Name = "lblPhone";
            lblPhone.Size = new System.Drawing.Size(100, 23);
            lblPhone.TabIndex = 2;
            lblPhone.Text = "Phone:";
            // 
            // lblEmail
            // 
            lblEmail.Location = new System.Drawing.Point(10, 120);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new System.Drawing.Size(100, 23);
            lblEmail.TabIndex = 3;
            lblEmail.Text = "Email:";
            // 
            // lblWhatsapp
            // 
            lblWhatsapp.Location = new System.Drawing.Point(10, 155);
            lblWhatsapp.Name = "lblWhatsapp";
            lblWhatsapp.Size = new System.Drawing.Size(100, 23);
            lblWhatsapp.TabIndex = 4;
            lblWhatsapp.Text = "Whatsapp:";
            // 
            // lblAddress
            // 
            lblAddress.Location = new System.Drawing.Point(10, 190);
            lblAddress.Name = "lblAddress";
            lblAddress.Size = new System.Drawing.Size(100, 23);
            lblAddress.TabIndex = 5;
            lblAddress.Text = "Address:";
            // 
            // txtName
            // 
            txtName.Location = new System.Drawing.Point(120, 12);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(450, 27);
            txtName.TabIndex = 6;
            // 
            // txtNIC
            // 
            txtNIC.Location = new System.Drawing.Point(120, 47);
            txtNIC.Name = "txtNIC";
            txtNIC.Size = new System.Drawing.Size(250, 27);
            txtNIC.TabIndex = 7;
            // 
            // txtPhone
            // 
            txtPhone.Location = new System.Drawing.Point(120, 82);
            txtPhone.Name = "txtPhone";
            txtPhone.Size = new System.Drawing.Size(250, 27);
            txtPhone.TabIndex = 8;
            // 
            // txtEmail
            // 
            txtEmail.Location = new System.Drawing.Point(120, 117);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new System.Drawing.Size(250, 27);
            txtEmail.TabIndex = 9;
            // 
            // txtWhatsapp
            // 
            txtWhatsapp.Location = new System.Drawing.Point(120, 152);
            txtWhatsapp.Name = "txtWhatsapp";
            txtWhatsapp.Size = new System.Drawing.Size(250, 27);
            txtWhatsapp.TabIndex = 10;
            // 
            // txtAddress
            // 
            txtAddress.Location = new System.Drawing.Point(120, 187);
            txtAddress.Name = "txtAddress";
            txtAddress.Size = new System.Drawing.Size(450, 80);
            txtAddress.Multiline = true;
            txtAddress.TabIndex = 11;
            // 
            // btnStartTest
            // 
            btnStartTest.Location = new System.Drawing.Point(120, 280);
            btnStartTest.Name = "btnStartTest";
            btnStartTest.Size = new System.Drawing.Size(120, 30);
            btnStartTest.TabIndex = 12;
            btnStartTest.Text = "Start Test";
            btnStartTest.UseVisualStyleBackColor = true;
            btnStartTest.Click += BtnStartTest_Click;
            // 
            // lblLocation
            // 
            lblLocation.AutoSize = true;
            lblLocation.Name = "lblLocation";
            lblLocation.Text = "Location: fetching...";
            lblLocation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
            lblLocation.Location = new System.Drawing.Point(800, 12);
            // 
            // Form1
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(9F, 23F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(1000, 700);
            Controls.Add(lblName);
            Controls.Add(lblNIC);
            Controls.Add(lblPhone);
            Controls.Add(lblEmail);
            Controls.Add(lblWhatsapp);
            Controls.Add(lblAddress);
            Controls.Add(txtName);
            Controls.Add(txtNIC);
            Controls.Add(txtPhone);
            Controls.Add(txtEmail);
            Controls.Add(txtWhatsapp);
            Controls.Add(txtAddress);
            Controls.Add(lblLocation);
            Controls.Add(btnStartTest);
            Name = "MainForm";
            Text = "Milk Test";
            WindowState = System.Windows.Forms.FormWindowState.Maximized;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
    }
}
