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

        // Controls added so they are visible at design time
        private System.Windows.Forms.Button _testButton;
        private System.Windows.Forms.Button _pdfButton;
        private System.Windows.Forms.Button _btnNewTest;
        private System.Windows.Forms.Button _btnPortToggle;
        private System.Windows.Forms.Button _btnSetLocation;
        private System.Windows.Forms.SplitContainer _bottomSplit;
        private System.Windows.Forms.DataGridView _gridParams;
        private System.Windows.Forms.DataGridView _gridAdulteration;
        private System.Windows.Forms.Label lblCustomerType;
        private System.Windows.Forms.ComboBox cmbCustomerType;

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
            this.lblName = new System.Windows.Forms.Label();
            this.lblNIC = new System.Windows.Forms.Label();
            this.lblPhone = new System.Windows.Forms.Label();
            this.lblWhatsapp = new System.Windows.Forms.Label();
            this.lblAddress = new System.Windows.Forms.Label();
            this.lblEmail = new System.Windows.Forms.Label();
            this.txtName = new System.Windows.Forms.TextBox();
            this.txtNIC = new System.Windows.Forms.TextBox();
            this.txtPhone = new System.Windows.Forms.TextBox();
            this.txtEmail = new System.Windows.Forms.TextBox();
            this.txtWhatsapp = new System.Windows.Forms.TextBox();
            this.txtAddress = new System.Windows.Forms.TextBox();
            this.btnStartTest = new System.Windows.Forms.Button();
            this.lblLocation = new System.Windows.Forms.Label();
            this._testButton = new System.Windows.Forms.Button();
            this._pdfButton = new System.Windows.Forms.Button();
            this._btnNewTest = new System.Windows.Forms.Button();
            this._btnPortToggle = new System.Windows.Forms.Button();
            this._btnSetLocation = new System.Windows.Forms.Button();
            this._bottomSplit = new System.Windows.Forms.SplitContainer();
            this._gridParams = new System.Windows.Forms.DataGridView();
            this._gridAdulteration = new System.Windows.Forms.DataGridView();
            this.lblCustomerType = new System.Windows.Forms.Label();
            this.cmbCustomerType = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this._bottomSplit)).BeginInit();
            this._bottomSplit.Panel1.SuspendLayout();
            this._bottomSplit.Panel2.SuspendLayout();
            this._bottomSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridAdulteration)).BeginInit();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.Location = new System.Drawing.Point(9, 91);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(89, 16);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // lblNIC
            // 
            this.lblNIC.Location = new System.Drawing.Point(9, 134);
            this.lblNIC.Name = "lblNIC";
            this.lblNIC.Size = new System.Drawing.Size(89, 16);
            this.lblNIC.TabIndex = 1;
            this.lblNIC.Text = "NIC:";
            // 
            // lblPhone
            // 
            this.lblPhone.Location = new System.Drawing.Point(9, 174);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(89, 16);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Phone:";
            // 
            // lblWhatsapp
            // 
            this.lblWhatsapp.Location = new System.Drawing.Point(9, 247);
            this.lblWhatsapp.Name = "lblWhatsapp";
            this.lblWhatsapp.Size = new System.Drawing.Size(89, 16);
            this.lblWhatsapp.TabIndex = 4;
            this.lblWhatsapp.Text = "Whatsapp:";
            // 
            // lblAddress
            // 
            this.lblAddress.Location = new System.Drawing.Point(9, 274);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(89, 16);
            this.lblAddress.TabIndex = 5;
            this.lblAddress.Text = "Address:";
            // 
            // lblEmail
            // 
            this.lblEmail.Location = new System.Drawing.Point(9, 211);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(89, 16);
            this.lblEmail.TabIndex = 3;
            this.lblEmail.Text = "Email:";
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(107, 89);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(400, 22);
            this.txtName.TabIndex = 6;
            // 
            // txtNIC
            // 
            this.txtNIC.Location = new System.Drawing.Point(107, 132);
            this.txtNIC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNIC.Name = "txtNIC";
            this.txtNIC.Size = new System.Drawing.Size(223, 22);
            this.txtNIC.TabIndex = 7;
            // 
            // txtPhone
            // 
            this.txtPhone.Location = new System.Drawing.Point(107, 172);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(223, 22);
            this.txtPhone.TabIndex = 8;
            // 
            // txtEmail
            // 
            this.txtEmail.Location = new System.Drawing.Point(107, 209);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(223, 22);
            this.txtEmail.TabIndex = 9;
            // 
            // txtWhatsapp
            // 
            this.txtWhatsapp.Location = new System.Drawing.Point(107, 245);
            this.txtWhatsapp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWhatsapp.Name = "txtWhatsapp";
            this.txtWhatsapp.Size = new System.Drawing.Size(223, 22);
            this.txtWhatsapp.TabIndex = 10;
            // 
            // txtAddress
            // 
            this.txtAddress.Location = new System.Drawing.Point(107, 271);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(400, 57);
            this.txtAddress.TabIndex = 11;
            // 
            // btnStartTest
            // 
            this.btnStartTest.Location = new System.Drawing.Point(107, 332);
            this.btnStartTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(107, 21);
            this.btnStartTest.TabIndex = 12;
            this.btnStartTest.Text = "Start Test";
            this.btnStartTest.UseVisualStyleBackColor = true;
            // 
            // lblLocation
            // 
            this.lblLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(890, 8);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(119, 16);
            this.lblLocation.TabIndex = 12;
            this.lblLocation.Text = "Location: fetching...";
            // 
            // _testButton
            // 
            this._testButton.Location = new System.Drawing.Point(10, 10);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new System.Drawing.Size(120, 30);
            this._testButton.TabIndex = 13;
            this._testButton.Text = "Run Test Insert";
            this._testButton.UseVisualStyleBackColor = true;
            // 
            // _pdfButton
            // 
            this._pdfButton.Location = new System.Drawing.Point(300, 12);
            this._pdfButton.Name = "_pdfButton";
            this._pdfButton.Size = new System.Drawing.Size(120, 30);
            this._pdfButton.TabIndex = 17;
            this._pdfButton.Text = "Generate PDF";
            this._pdfButton.UseVisualStyleBackColor = true;
            // 
            // _btnNewTest
            // 
            this._btnNewTest.Location = new System.Drawing.Point(140, 10);
            this._btnNewTest.Name = "_btnNewTest";
            this._btnNewTest.Size = new System.Drawing.Size(100, 30);
            this._btnNewTest.TabIndex = 14;
            this._btnNewTest.Text = "New Test";
            this._btnNewTest.UseVisualStyleBackColor = true;
            // 
            // _btnPortToggle
            // 
            this._btnPortToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnPortToggle.Location = new System.Drawing.Point(808, 8);
            this._btnPortToggle.Name = "_btnPortToggle";
            this._btnPortToggle.Size = new System.Drawing.Size(80, 24);
            this._btnPortToggle.TabIndex = 15;
            this._btnPortToggle.Text = "Connect";
            this._btnPortToggle.UseVisualStyleBackColor = true;
            // 
            // _btnSetLocation
            // 
            this._btnSetLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSetLocation.Location = new System.Drawing.Point(898, 8);
            this._btnSetLocation.Name = "_btnSetLocation";
            this._btnSetLocation.Size = new System.Drawing.Size(100, 24);
            this._btnSetLocation.TabIndex = 16;
            this._btnSetLocation.Text = "Set Location";
            this._btnSetLocation.UseVisualStyleBackColor = true;
            this._btnSetLocation.Visible = false;
            // 
            // _bottomSplit
            // 
            this._bottomSplit.Dock = System.Windows.Forms.DockStyle.Bottom;
            this._bottomSplit.Location = new System.Drawing.Point(0, 267);
            this._bottomSplit.Name = "_bottomSplit";
            // 
            // _bottomSplit.Panel1
            // 
            this._bottomSplit.Panel1.Controls.Add(this._gridParams);
            // 
            // _bottomSplit.Panel2
            // 
            this._bottomSplit.Panel2.Controls.Add(this._gridAdulteration);
            this._bottomSplit.Size = new System.Drawing.Size(1068, 300);
            this._bottomSplit.SplitterDistance = 861;
            this._bottomSplit.TabIndex = 18;
            // 
            // _gridParams
            // 
            this._gridParams.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._gridParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridParams.Location = new System.Drawing.Point(0, 0);
            this._gridParams.Name = "_gridParams";
            this._gridParams.RowHeadersWidth = 51;
            this._gridParams.RowTemplate.Height = 24;
            this._gridParams.Size = new System.Drawing.Size(861, 300);
            this._gridParams.TabIndex = 0;
            // 
            // _gridAdulteration
            // 
            this._gridAdulteration.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this._gridAdulteration.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridAdulteration.Location = new System.Drawing.Point(0, 0);
            this._gridAdulteration.Name = "_gridAdulteration";
            this._gridAdulteration.RowHeadersWidth = 51;
            this._gridAdulteration.RowTemplate.Height = 24;
            this._gridAdulteration.Size = new System.Drawing.Size(203, 300);
            this._gridAdulteration.TabIndex = 0;
            // 
            // lblCustomerType
            // 
            this.lblCustomerType.AutoSize = true;
            this.lblCustomerType.Location = new System.Drawing.Point(352, 138);
            this.lblCustomerType.Name = "lblCustomerType";
            this.lblCustomerType.Size = new System.Drawing.Size(102, 16);
            this.lblCustomerType.TabIndex = 19;
            this.lblCustomerType.Text = "Customer Type:";
            // 
            // cmbCustomerType
            // 
            this.cmbCustomerType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCustomerType.FormattingEnabled = true;
            this.cmbCustomerType.Items.AddRange(new object[] {
            "Walk-in",
            "Shop Owner"});
            this.cmbCustomerType.Location = new System.Drawing.Point(452, 135);
            this.cmbCustomerType.Name = "cmbCustomerType";
            this.cmbCustomerType.Size = new System.Drawing.Size(120, 24);
            this.cmbCustomerType.TabIndex = 20;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 567);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblNIC);
            this.Controls.Add(this.lblPhone);
            this.Controls.Add(this.lblEmail);
            this.Controls.Add(this.lblWhatsapp);
            this.Controls.Add(this.lblAddress);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtNIC);
            this.Controls.Add(this.txtPhone);
            this.Controls.Add(this.txtEmail);
            this.Controls.Add(this.txtWhatsapp);
            this.Controls.Add(this.txtAddress);
            this.Controls.Add(this.lblLocation);
            this.Controls.Add(this.btnStartTest);
            this.Controls.Add(this._testButton);
            this.Controls.Add(this._btnNewTest);
            this.Controls.Add(this._btnPortToggle);
            this.Controls.Add(this._btnSetLocation);
            this.Controls.Add(this._pdfButton);
            this.Controls.Add(this._bottomSplit);
            this.Controls.Add(this.lblCustomerType);
            this.Controls.Add(this.cmbCustomerType);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Milk Test";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
            this.Load += new System.EventHandler(this.MainForm_Load);
            this._bottomSplit.Panel1.ResumeLayout(false);
            this._bottomSplit.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._bottomSplit)).EndInit();
            this._bottomSplit.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this._gridParams)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridAdulteration)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
    }
}
