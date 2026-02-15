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
        private System.Windows.Forms.Button _pdfButton;
        private System.Windows.Forms.Button _btnNewTest;
        private System.Windows.Forms.Button _btnPortToggle;
        private System.Windows.Forms.Button _btnSetLocation;
        private System.Windows.Forms.SplitContainer _bottomSplit;
        private System.Windows.Forms.DataGridView _gridParams;
        private System.Windows.Forms.DataGridView _gridPreviousTests;
        private System.Windows.Forms.Label lblCustomerType;
        private System.Windows.Forms.ComboBox cmbCustomerType;

        // New layout containers (design-time)
        private System.Windows.Forms.Panel _topCommandPanel;
        private System.Windows.Forms.GroupBox _groupCustomer;
        private System.Windows.Forms.TableLayoutPanel _customerTable;
        private System.Windows.Forms.GroupBox _groupResults;

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
            this._pdfButton = new System.Windows.Forms.Button();
            this._btnNewTest = new System.Windows.Forms.Button();
            this._btnPortToggle = new System.Windows.Forms.Button();
            this._btnSetLocation = new System.Windows.Forms.Button();
            this._bottomSplit = new System.Windows.Forms.SplitContainer();
            this._gridParams = new System.Windows.Forms.DataGridView();
            this.ParameterId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Parameter = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Value = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this._gridPreviousTests = new System.Windows.Forms.DataGridView();
            this.ResultId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TestDateTime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Summary = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lblCustomerType = new System.Windows.Forms.Label();
            this.cmbCustomerType = new System.Windows.Forms.ComboBox();
            this._topCommandPanel = new System.Windows.Forms.Panel();
            this.btnSendEmail = new System.Windows.Forms.Button();
            this._testButton = new System.Windows.Forms.Button();
            this._groupCustomer = new System.Windows.Forms.GroupBox();
            this._customerTable = new System.Windows.Forms.TableLayoutPanel();
            this._groupResults = new System.Windows.Forms.GroupBox();
            ((System.ComponentModel.ISupportInitialize)(this._bottomSplit)).BeginInit();
            this._bottomSplit.Panel1.SuspendLayout();
            this._bottomSplit.Panel2.SuspendLayout();
            this._bottomSplit.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this._gridParams)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this._gridPreviousTests)).BeginInit();
            this._topCommandPanel.SuspendLayout();
            this._groupCustomer.SuspendLayout();
            this._customerTable.SuspendLayout();
            this._groupResults.SuspendLayout();
            this.SuspendLayout();
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(3, 0);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(47, 16);
            this.lblName.TabIndex = 0;
            this.lblName.Text = "Name:";
            // 
            // lblNIC
            // 
            this.lblNIC.AutoSize = true;
            this.lblNIC.Location = new System.Drawing.Point(3, 30);
            this.lblNIC.Name = "lblNIC";
            this.lblNIC.Size = new System.Drawing.Size(32, 16);
            this.lblNIC.TabIndex = 1;
            this.lblNIC.Text = "NIC:";
            // 
            // lblPhone
            // 
            this.lblPhone.AutoSize = true;
            this.lblPhone.Location = new System.Drawing.Point(522, 30);
            this.lblPhone.Name = "lblPhone";
            this.lblPhone.Size = new System.Drawing.Size(49, 16);
            this.lblPhone.TabIndex = 2;
            this.lblPhone.Text = "Phone:";
            // 
            // lblWhatsapp
            // 
            this.lblWhatsapp.AutoSize = true;
            this.lblWhatsapp.Location = new System.Drawing.Point(522, 60);
            this.lblWhatsapp.Name = "lblWhatsapp";
            this.lblWhatsapp.Size = new System.Drawing.Size(72, 16);
            this.lblWhatsapp.TabIndex = 4;
            this.lblWhatsapp.Text = "Whatsapp:";
            // 
            // lblAddress
            // 
            this.lblAddress.AutoSize = true;
            this.lblAddress.Location = new System.Drawing.Point(3, 90);
            this.lblAddress.Name = "lblAddress";
            this.lblAddress.Size = new System.Drawing.Size(61, 16);
            this.lblAddress.TabIndex = 5;
            this.lblAddress.Text = "Address:";
            // 
            // lblEmail
            // 
            this.lblEmail.AutoSize = true;
            this.lblEmail.Location = new System.Drawing.Point(3, 60);
            this.lblEmail.Name = "lblEmail";
            this.lblEmail.Size = new System.Drawing.Size(44, 16);
            this.lblEmail.TabIndex = 3;
            this.lblEmail.Text = "Email:";
            // 
            // txtName
            // 
            this.txtName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtName.Location = new System.Drawing.Point(113, 2);
            this.txtName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(403, 22);
            this.txtName.TabIndex = 6;
            // 
            // txtNIC
            // 
            this.txtNIC.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtNIC.Location = new System.Drawing.Point(113, 32);
            this.txtNIC.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtNIC.Name = "txtNIC";
            this.txtNIC.Size = new System.Drawing.Size(403, 22);
            this.txtNIC.TabIndex = 7;
            // 
            // txtPhone
            // 
            this.txtPhone.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtPhone.Location = new System.Drawing.Point(642, 32);
            this.txtPhone.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPhone.Name = "txtPhone";
            this.txtPhone.Size = new System.Drawing.Size(403, 22);
            this.txtPhone.TabIndex = 8;
            // 
            // txtEmail
            // 
            this.txtEmail.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtEmail.Location = new System.Drawing.Point(113, 62);
            this.txtEmail.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEmail.Name = "txtEmail";
            this.txtEmail.Size = new System.Drawing.Size(403, 22);
            this.txtEmail.TabIndex = 9;
            // 
            // txtWhatsapp
            // 
            this.txtWhatsapp.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtWhatsapp.Location = new System.Drawing.Point(642, 62);
            this.txtWhatsapp.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtWhatsapp.Name = "txtWhatsapp";
            this.txtWhatsapp.Size = new System.Drawing.Size(403, 22);
            this.txtWhatsapp.TabIndex = 10;
            // 
            // txtAddress
            // 
            this._customerTable.SetColumnSpan(this.txtAddress, 3);
            this.txtAddress.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtAddress.Location = new System.Drawing.Point(113, 92);
            this.txtAddress.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtAddress.Multiline = true;
            this.txtAddress.Name = "txtAddress";
            this.txtAddress.Size = new System.Drawing.Size(932, 151);
            this.txtAddress.TabIndex = 11;
            // 
            // btnStartTest
            // 
            this.btnStartTest.Dock = System.Windows.Forms.DockStyle.Left;
            this.btnStartTest.Location = new System.Drawing.Point(108, 8);
            this.btnStartTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnStartTest.Name = "btnStartTest";
            this.btnStartTest.Size = new System.Drawing.Size(120, 28);
            this.btnStartTest.TabIndex = 12;
            this.btnStartTest.Text = "Start Test";
            this.btnStartTest.UseVisualStyleBackColor = true;
            this.btnStartTest.Click += new System.EventHandler(this.BtnStartTest_Click);
            // 
            // lblLocation
            // 
            this.lblLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLocation.AutoSize = true;
            this.lblLocation.Location = new System.Drawing.Point(1758, 8);
            this.lblLocation.Name = "lblLocation";
            this.lblLocation.Size = new System.Drawing.Size(119, 16);
            this.lblLocation.TabIndex = 12;
            this.lblLocation.Text = "Location: fetching...";
            // 
            // _pdfButton
            // 
            this._pdfButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._pdfButton.Location = new System.Drawing.Point(940, 8);
            this._pdfButton.Name = "_pdfButton";
            this._pdfButton.Size = new System.Drawing.Size(120, 28);
            this._pdfButton.TabIndex = 17;
            this._pdfButton.Text = "Send PDF";
            this._pdfButton.UseVisualStyleBackColor = true;
            // 
            // _btnNewTest
            // 
            this._btnNewTest.Dock = System.Windows.Forms.DockStyle.Left;
            this._btnNewTest.Location = new System.Drawing.Point(8, 8);
            this._btnNewTest.Name = "_btnNewTest";
            this._btnNewTest.Size = new System.Drawing.Size(100, 28);
            this._btnNewTest.TabIndex = 14;
            this._btnNewTest.Text = "New Test";
            this._btnNewTest.UseVisualStyleBackColor = true;
            // 
            // _btnPortToggle
            // 
            this._btnPortToggle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnPortToggle.Location = new System.Drawing.Point(1636, 10);
            this._btnPortToggle.Name = "_btnPortToggle";
            this._btnPortToggle.Size = new System.Drawing.Size(80, 24);
            this._btnPortToggle.TabIndex = 15;
            this._btnPortToggle.Text = "Connect";
            this._btnPortToggle.UseVisualStyleBackColor = true;
            // 
            // _btnSetLocation
            // 
            this._btnSetLocation.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this._btnSetLocation.Location = new System.Drawing.Point(1722, 10);
            this._btnSetLocation.Name = "_btnSetLocation";
            this._btnSetLocation.Size = new System.Drawing.Size(100, 24);
            this._btnSetLocation.TabIndex = 16;
            this._btnSetLocation.Text = "Set Location";
            this._btnSetLocation.UseVisualStyleBackColor = true;
            this._btnSetLocation.Visible = false;
            // 
            // _bottomSplit
            // 
            this._bottomSplit.Dock = System.Windows.Forms.DockStyle.Fill;
            this._bottomSplit.Location = new System.Drawing.Point(8, 23);
            this._bottomSplit.Name = "_bottomSplit";
            // 
            // _bottomSplit.Panel1
            // 
            this._bottomSplit.Panel1.Controls.Add(this._gridParams);
            // 
            // _bottomSplit.Panel2
            // 
            this._bottomSplit.Panel2.Controls.Add(this._gridPreviousTests);
            this._bottomSplit.Size = new System.Drawing.Size(1052, 212);
            this._bottomSplit.SplitterDistance = 848;
            this._bottomSplit.TabIndex = 0;
            // 
            // _gridParams
            // 
            this._gridParams.AllowUserToAddRows = false;
            this._gridParams.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._gridParams.ColumnHeadersHeight = 32;
            this._gridParams.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ParameterId,
            this.Parameter,
            this.Value});
            this._gridParams.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridParams.Location = new System.Drawing.Point(0, 0);
            this._gridParams.Name = "_gridParams";
            this._gridParams.RowHeadersVisible = false;
            this._gridParams.RowHeadersWidth = 57;
            this._gridParams.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridParams.Size = new System.Drawing.Size(848, 212);
            this._gridParams.TabIndex = 0;
            // 
            // ParameterId
            // 
            this.ParameterId.HeaderText = "Id";
            this.ParameterId.MinimumWidth = 6;
            this.ParameterId.Name = "ParameterId";
            this.ParameterId.Visible = false;
            // 
            // Parameter
            // 
            this.Parameter.HeaderText = "Parameter";
            this.Parameter.MinimumWidth = 6;
            this.Parameter.Name = "Parameter";
            this.Parameter.ReadOnly = true;
            // 
            // Value
            // 
            this.Value.HeaderText = "Value";
            this.Value.MinimumWidth = 6;
            this.Value.Name = "Value";
            // 
            // _gridPreviousTests
            // 
            this._gridPreviousTests.AllowUserToAddRows = false;
            this._gridPreviousTests.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this._gridPreviousTests.ColumnHeadersHeight = 32;
            this._gridPreviousTests.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ResultId,
            this.TestDateTime,
            this.Summary});
            this._gridPreviousTests.Dock = System.Windows.Forms.DockStyle.Fill;
            this._gridPreviousTests.Location = new System.Drawing.Point(0, 0);
            this._gridPreviousTests.Name = "_gridPreviousTests";
            this._gridPreviousTests.RowHeadersVisible = false;
            this._gridPreviousTests.RowHeadersWidth = 57;
            this._gridPreviousTests.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this._gridPreviousTests.Size = new System.Drawing.Size(200, 212);
            this._gridPreviousTests.TabIndex = 0;
            // 
            // ResultId
            // 
            this.ResultId.HeaderText = "ResultId";
            this.ResultId.MinimumWidth = 6;
            this.ResultId.Name = "ResultId";
            this.ResultId.Visible = false;
            // 
            // TestDateTime
            // 
            this.TestDateTime.HeaderText = "Date/Time";
            this.TestDateTime.MinimumWidth = 6;
            this.TestDateTime.Name = "TestDateTime";
            this.TestDateTime.ReadOnly = true;
            // 
            // Summary
            // 
            this.Summary.HeaderText = "Summary";
            this.Summary.MinimumWidth = 6;
            this.Summary.Name = "Summary";
            // 
            // lblCustomerType
            // 
            this.lblCustomerType.AutoSize = true;
            this.lblCustomerType.Location = new System.Drawing.Point(522, 0);
            this.lblCustomerType.Name = "lblCustomerType";
            this.lblCustomerType.Size = new System.Drawing.Size(0, 16);
            this.lblCustomerType.TabIndex = 7;
            // 
            // cmbCustomerType
            // 
            this.cmbCustomerType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbCustomerType.Location = new System.Drawing.Point(642, 3);
            this.cmbCustomerType.Name = "cmbCustomerType";
            this.cmbCustomerType.Size = new System.Drawing.Size(403, 24);
            this.cmbCustomerType.TabIndex = 8;
            // 
            // _topCommandPanel
            // 
            this._topCommandPanel.Controls.Add(this.btnSendEmail);
            this._topCommandPanel.Controls.Add(this._testButton);
            this._topCommandPanel.Controls.Add(this.btnStartTest);
            this._topCommandPanel.Controls.Add(this._btnNewTest);
            this._topCommandPanel.Controls.Add(this._pdfButton);
            this._topCommandPanel.Controls.Add(this._btnPortToggle);
            this._topCommandPanel.Controls.Add(this._btnSetLocation);
            this._topCommandPanel.Controls.Add(this.lblLocation);
            this._topCommandPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this._topCommandPanel.Location = new System.Drawing.Point(0, 0);
            this._topCommandPanel.Name = "_topCommandPanel";
            this._topCommandPanel.Padding = new System.Windows.Forms.Padding(8);
            this._topCommandPanel.Size = new System.Drawing.Size(1068, 44);
            this._topCommandPanel.TabIndex = 14;
            // 
            // btnSendEmail
            // 
            this.btnSendEmail.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSendEmail.Location = new System.Drawing.Point(700, 8);
            this.btnSendEmail.Name = "btnSendEmail";
            this.btnSendEmail.Size = new System.Drawing.Size(120, 28);
            this.btnSendEmail.TabIndex = 18;
            this.btnSendEmail.Text = "Send Email";
            this.btnSendEmail.UseVisualStyleBackColor = true;
            // 
            // _testButton
            // 
            this._testButton.Dock = System.Windows.Forms.DockStyle.Right;
            this._testButton.Location = new System.Drawing.Point(820, 8);
            this._testButton.Name = "_testButton";
            this._testButton.Size = new System.Drawing.Size(120, 28);
            this._testButton.TabIndex = 13;
            this._testButton.Text = "Run Test Insert";
            this._testButton.UseVisualStyleBackColor = true;
            this._testButton.Visible = false;
            // 
            // _groupCustomer
            // 
            this._groupCustomer.Controls.Add(this._customerTable);
            this._groupCustomer.Dock = System.Windows.Forms.DockStyle.Top;
            this._groupCustomer.Location = new System.Drawing.Point(0, 44);
            this._groupCustomer.Name = "_groupCustomer";
            this._groupCustomer.Padding = new System.Windows.Forms.Padding(10);
            this._groupCustomer.Size = new System.Drawing.Size(1068, 280);
            this._groupCustomer.TabIndex = 13;
            this._groupCustomer.TabStop = false;
            this._groupCustomer.Text = "Customer Details";
            // 
            // _customerTable
            // 
            this._customerTable.AutoSize = true;
            this._customerTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this._customerTable.ColumnCount = 4;
            this._customerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 110F));
            this._customerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._customerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this._customerTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this._customerTable.Controls.Add(this.lblName, 0, 0);
            this._customerTable.Controls.Add(this.txtName, 1, 0);
            this._customerTable.Controls.Add(this.lblCustomerType, 2, 0);
            this._customerTable.Controls.Add(this.cmbCustomerType, 3, 0);
            this._customerTable.Controls.Add(this.lblNIC, 0, 1);
            this._customerTable.Controls.Add(this.txtNIC, 1, 1);
            this._customerTable.Controls.Add(this.lblPhone, 2, 1);
            this._customerTable.Controls.Add(this.txtPhone, 3, 1);
            this._customerTable.Controls.Add(this.lblEmail, 0, 2);
            this._customerTable.Controls.Add(this.txtEmail, 1, 2);
            this._customerTable.Controls.Add(this.lblWhatsapp, 2, 2);
            this._customerTable.Controls.Add(this.txtWhatsapp, 3, 2);
            this._customerTable.Controls.Add(this.lblAddress, 0, 3);
            this._customerTable.Controls.Add(this.txtAddress, 1, 3);
            this._customerTable.Dock = System.Windows.Forms.DockStyle.Fill;
            this._customerTable.Location = new System.Drawing.Point(10, 25);
            this._customerTable.Name = "_customerTable";
            this._customerTable.RowCount = 4;
            this._customerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._customerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._customerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 30F));
            this._customerTable.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this._customerTable.Size = new System.Drawing.Size(1048, 245);
            this._customerTable.TabIndex = 0;
            // 
            // _groupResults
            // 
            this._groupResults.Controls.Add(this._bottomSplit);
            this._groupResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this._groupResults.Location = new System.Drawing.Point(0, 324);
            this._groupResults.Name = "_groupResults";
            this._groupResults.Padding = new System.Windows.Forms.Padding(8);
            this._groupResults.Size = new System.Drawing.Size(1068, 243);
            this._groupResults.TabIndex = 0;
            this._groupResults.TabStop = false;
            this._groupResults.Text = "Test Results";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1068, 567);
            this.Controls.Add(this._groupResults);
            this.Controls.Add(this._groupCustomer);
            this.Controls.Add(this._topCommandPanel);
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
            ((System.ComponentModel.ISupportInitialize)(this._gridPreviousTests)).EndInit();
            this._topCommandPanel.ResumeLayout(false);
            this._topCommandPanel.PerformLayout();
            this._groupCustomer.ResumeLayout(false);
            this._groupCustomer.PerformLayout();
            this._customerTable.ResumeLayout(false);
            this._customerTable.PerformLayout();
            this._groupResults.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnSendEmail;
        private System.Windows.Forms.Button _testButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ParameterId;
        private System.Windows.Forms.DataGridViewTextBoxColumn Parameter;
        private System.Windows.Forms.DataGridViewTextBoxColumn Value;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResultId;
        private System.Windows.Forms.DataGridViewTextBoxColumn TestDateTime;
        private System.Windows.Forms.DataGridViewTextBoxColumn Summary;
    }
}
