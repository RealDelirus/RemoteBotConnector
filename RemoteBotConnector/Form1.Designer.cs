namespace RemoteBotConnector
{
    partial class Form1
    {
        /// <summary>
        /// Erforderliche Designervariable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Verwendete Ressourcen bereinigen.
        /// </summary>
        /// <param name="disposing">True, wenn verwaltete Ressourcen gelöscht werden sollen; andernfalls False.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Vom Windows Form-Designer generierter Code

        /// <summary>
        /// Erforderliche Methode für die Designerunterstützung.
        /// Der Inhalt der Methode darf nicht mit dem Code-Editor geändert werden.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.rtb_log = new System.Windows.Forms.RichTextBox();
            this.groupBox_connection = new System.Windows.Forms.GroupBox();
            this.checkBox_clientlessToClient = new System.Windows.Forms.CheckBox();
            this.groupBox_manualPortOverride = new System.Windows.Forms.GroupBox();
            this.textBox_manualPortOverride = new System.Windows.Forms.TextBox();
            this.checkBox_manualPortOverride = new System.Windows.Forms.CheckBox();
            this.checkBox_ignoreState = new System.Windows.Forms.CheckBox();
            this.button_refresh = new System.Windows.Forms.Button();
            this.listBox_botClients = new System.Windows.Forms.ListBox();
            this.radioButton_manual = new System.Windows.Forms.RadioButton();
            this.radioButton_autoSelect = new System.Windows.Forms.RadioButton();
            this.groupBox_connection.SuspendLayout();
            this.groupBox_manualPortOverride.SuspendLayout();
            this.SuspendLayout();
            // 
            // rtb_log
            // 
            this.rtb_log.Location = new System.Drawing.Point(455, 12);
            this.rtb_log.Name = "rtb_log";
            this.rtb_log.ReadOnly = true;
            this.rtb_log.Size = new System.Drawing.Size(407, 359);
            this.rtb_log.TabIndex = 0;
            this.rtb_log.Text = "";
            // 
            // groupBox_connection
            // 
            this.groupBox_connection.Controls.Add(this.checkBox_clientlessToClient);
            this.groupBox_connection.Controls.Add(this.groupBox_manualPortOverride);
            this.groupBox_connection.Controls.Add(this.checkBox_ignoreState);
            this.groupBox_connection.Controls.Add(this.button_refresh);
            this.groupBox_connection.Controls.Add(this.listBox_botClients);
            this.groupBox_connection.Controls.Add(this.radioButton_manual);
            this.groupBox_connection.Controls.Add(this.radioButton_autoSelect);
            this.groupBox_connection.Location = new System.Drawing.Point(3, 12);
            this.groupBox_connection.Name = "groupBox_connection";
            this.groupBox_connection.Size = new System.Drawing.Size(446, 359);
            this.groupBox_connection.TabIndex = 1;
            this.groupBox_connection.TabStop = false;
            this.groupBox_connection.Text = "Connection";
            // 
            // checkBox_clientlessToClient
            // 
            this.checkBox_clientlessToClient.AutoSize = true;
            this.checkBox_clientlessToClient.Enabled = false;
            this.checkBox_clientlessToClient.Location = new System.Drawing.Point(6, 327);
            this.checkBox_clientlessToClient.Name = "checkBox_clientlessToClient";
            this.checkBox_clientlessToClient.Size = new System.Drawing.Size(111, 30);
            this.checkBox_clientlessToClient.TabIndex = 6;
            this.checkBox_clientlessToClient.Text = "Clientless -> Client\r\n(phBot only)";
            this.checkBox_clientlessToClient.UseVisualStyleBackColor = true;
            // 
            // groupBox_manualPortOverride
            // 
            this.groupBox_manualPortOverride.Controls.Add(this.textBox_manualPortOverride);
            this.groupBox_manualPortOverride.Controls.Add(this.checkBox_manualPortOverride);
            this.groupBox_manualPortOverride.Location = new System.Drawing.Point(268, 12);
            this.groupBox_manualPortOverride.Name = "groupBox_manualPortOverride";
            this.groupBox_manualPortOverride.Size = new System.Drawing.Size(172, 47);
            this.groupBox_manualPortOverride.TabIndex = 5;
            this.groupBox_manualPortOverride.TabStop = false;
            this.groupBox_manualPortOverride.Text = "Manual port override";
            // 
            // textBox_manualPortOverride
            // 
            this.textBox_manualPortOverride.Enabled = false;
            this.textBox_manualPortOverride.Location = new System.Drawing.Point(83, 16);
            this.textBox_manualPortOverride.Name = "textBox_manualPortOverride";
            this.textBox_manualPortOverride.Size = new System.Drawing.Size(80, 20);
            this.textBox_manualPortOverride.TabIndex = 1;
            // 
            // checkBox_manualPortOverride
            // 
            this.checkBox_manualPortOverride.AutoSize = true;
            this.checkBox_manualPortOverride.Location = new System.Drawing.Point(6, 18);
            this.checkBox_manualPortOverride.Name = "checkBox_manualPortOverride";
            this.checkBox_manualPortOverride.Size = new System.Drawing.Size(71, 17);
            this.checkBox_manualPortOverride.TabIndex = 0;
            this.checkBox_manualPortOverride.Text = "Activated";
            this.checkBox_manualPortOverride.UseVisualStyleBackColor = true;
            this.checkBox_manualPortOverride.CheckedChanged += new System.EventHandler(this.checkBox_manualPortOverride_CheckedChanged);
            // 
            // checkBox_ignoreState
            // 
            this.checkBox_ignoreState.AutoSize = true;
            this.checkBox_ignoreState.Enabled = false;
            this.checkBox_ignoreState.Location = new System.Drawing.Point(6, 307);
            this.checkBox_ignoreState.Name = "checkBox_ignoreState";
            this.checkBox_ignoreState.Size = new System.Drawing.Size(84, 17);
            this.checkBox_ignoreState.TabIndex = 4;
            this.checkBox_ignoreState.Text = "Ignore State";
            this.checkBox_ignoreState.UseVisualStyleBackColor = true;
            this.checkBox_ignoreState.CheckedChanged += new System.EventHandler(this.checkBox_ignoreState_CheckedChanged);
            // 
            // button_refresh
            // 
            this.button_refresh.Enabled = false;
            this.button_refresh.Location = new System.Drawing.Point(123, 304);
            this.button_refresh.Name = "button_refresh";
            this.button_refresh.Size = new System.Drawing.Size(317, 50);
            this.button_refresh.TabIndex = 3;
            this.button_refresh.Text = "Refresh";
            this.button_refresh.UseVisualStyleBackColor = true;
            this.button_refresh.Click += new System.EventHandler(this.button_refresh_Click);
            // 
            // listBox_botClients
            // 
            this.listBox_botClients.Enabled = false;
            this.listBox_botClients.FormattingEnabled = true;
            this.listBox_botClients.Location = new System.Drawing.Point(6, 65);
            this.listBox_botClients.Name = "listBox_botClients";
            this.listBox_botClients.Size = new System.Drawing.Size(434, 238);
            this.listBox_botClients.TabIndex = 2;
            // 
            // radioButton_manual
            // 
            this.radioButton_manual.AutoSize = true;
            this.radioButton_manual.Location = new System.Drawing.Point(9, 42);
            this.radioButton_manual.Name = "radioButton_manual";
            this.radioButton_manual.Size = new System.Drawing.Size(60, 17);
            this.radioButton_manual.TabIndex = 1;
            this.radioButton_manual.Text = "Manual";
            this.radioButton_manual.UseVisualStyleBackColor = true;
            this.radioButton_manual.CheckedChanged += new System.EventHandler(this.radioButton_manual_CheckedChanged);
            // 
            // radioButton_autoSelect
            // 
            this.radioButton_autoSelect.AutoSize = true;
            this.radioButton_autoSelect.Checked = true;
            this.radioButton_autoSelect.Location = new System.Drawing.Point(9, 19);
            this.radioButton_autoSelect.Name = "radioButton_autoSelect";
            this.radioButton_autoSelect.Size = new System.Drawing.Size(77, 17);
            this.radioButton_autoSelect.TabIndex = 0;
            this.radioButton_autoSelect.TabStop = true;
            this.radioButton_autoSelect.Text = "AutoSelect";
            this.radioButton_autoSelect.UseVisualStyleBackColor = true;
            this.radioButton_autoSelect.CheckedChanged += new System.EventHandler(this.radioButton_autoSelect_CheckedChanged);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(874, 377);
            this.Controls.Add(this.groupBox_connection);
            this.Controls.Add(this.rtb_log);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "RemoteBotConnector v3 by Delirus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox_connection.ResumeLayout(false);
            this.groupBox_connection.PerformLayout();
            this.groupBox_manualPortOverride.ResumeLayout(false);
            this.groupBox_manualPortOverride.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        public System.Windows.Forms.RichTextBox rtb_log;
        private System.Windows.Forms.GroupBox groupBox_connection;
        private System.Windows.Forms.Button button_refresh;
        private System.Windows.Forms.ListBox listBox_botClients;
        private System.Windows.Forms.RadioButton radioButton_manual;
        private System.Windows.Forms.RadioButton radioButton_autoSelect;
        private System.Windows.Forms.CheckBox checkBox_ignoreState;
        private System.Windows.Forms.GroupBox groupBox_manualPortOverride;
        public System.Windows.Forms.CheckBox checkBox_clientlessToClient;
        public System.Windows.Forms.TextBox textBox_manualPortOverride;
        public System.Windows.Forms.CheckBox checkBox_manualPortOverride;
    }
}

