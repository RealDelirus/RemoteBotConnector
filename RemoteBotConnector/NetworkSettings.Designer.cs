namespace RemoteBotConnector
{
    partial class NetworkSettings
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(NetworkSettings));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.textBox_network_agport = new System.Windows.Forms.TextBox();
            this.comboBox_network_address = new System.Windows.Forms.ComboBox();
            this.label_network_agport = new System.Windows.Forms.Label();
            this.label1_network_address = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label_network_gwport = new System.Windows.Forms.Label();
            this.textBox_network_gwport = new System.Windows.Forms.TextBox();
            this.button_save = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox_network_agport);
            this.groupBox1.Controls.Add(this.comboBox_network_address);
            this.groupBox1.Controls.Add(this.label_network_agport);
            this.groupBox1.Controls.Add(this.label1_network_address);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label_network_gwport);
            this.groupBox1.Controls.Add(this.textBox_network_gwport);
            this.groupBox1.Location = new System.Drawing.Point(4, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(269, 122);
            this.groupBox1.TabIndex = 10;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Local Endpoint";
            // 
            // textBox_network_agport
            // 
            this.textBox_network_agport.Location = new System.Drawing.Point(63, 94);
            this.textBox_network_agport.Name = "textBox_network_agport";
            this.textBox_network_agport.Size = new System.Drawing.Size(200, 20);
            this.textBox_network_agport.TabIndex = 5;
            this.textBox_network_agport.Text = "20009";
            // 
            // comboBox_network_address
            // 
            this.comboBox_network_address.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox_network_address.FormattingEnabled = true;
            this.comboBox_network_address.Location = new System.Drawing.Point(63, 41);
            this.comboBox_network_address.Name = "comboBox_network_address";
            this.comboBox_network_address.Size = new System.Drawing.Size(200, 21);
            this.comboBox_network_address.TabIndex = 0;
            this.comboBox_network_address.DropDown += new System.EventHandler(this.comboBox_network_address_DropDown);
            // 
            // label_network_agport
            // 
            this.label_network_agport.AutoSize = true;
            this.label_network_agport.Location = new System.Drawing.Point(10, 97);
            this.label_network_agport.Name = "label_network_agport";
            this.label_network_agport.Size = new System.Drawing.Size(47, 13);
            this.label_network_agport.TabIndex = 6;
            this.label_network_agport.Text = "AG Port:";
            // 
            // label1_network_address
            // 
            this.label1_network_address.AutoSize = true;
            this.label1_network_address.Location = new System.Drawing.Point(5, 44);
            this.label1_network_address.Name = "label1_network_address";
            this.label1_network_address.Size = new System.Drawing.Size(52, 13);
            this.label1_network_address.TabIndex = 1;
            this.label1_network_address.Text = "Interface:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 16);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(192, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Choose which address you want to use";
            // 
            // label_network_gwport
            // 
            this.label_network_gwport.AutoSize = true;
            this.label_network_gwport.Location = new System.Drawing.Point(5, 71);
            this.label_network_gwport.Name = "label_network_gwport";
            this.label_network_gwport.Size = new System.Drawing.Size(51, 13);
            this.label_network_gwport.TabIndex = 4;
            this.label_network_gwport.Text = "GW Port:";
            // 
            // textBox_network_gwport
            // 
            this.textBox_network_gwport.Location = new System.Drawing.Point(63, 68);
            this.textBox_network_gwport.Name = "textBox_network_gwport";
            this.textBox_network_gwport.Size = new System.Drawing.Size(200, 20);
            this.textBox_network_gwport.TabIndex = 3;
            this.textBox_network_gwport.Text = "20008";
            // 
            // button_save
            // 
            this.button_save.Location = new System.Drawing.Point(102, 131);
            this.button_save.Name = "button_save";
            this.button_save.Size = new System.Drawing.Size(75, 23);
            this.button_save.TabIndex = 9;
            this.button_save.Text = "Save";
            this.button_save.UseVisualStyleBackColor = true;
            this.button_save.Click += new System.EventHandler(this.button_save_Click);
            // 
            // NetworkSettings
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 156);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.button_save);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "NetworkSettings";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Network Settings";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox textBox_network_agport;
        private System.Windows.Forms.ComboBox comboBox_network_address;
        private System.Windows.Forms.Label label_network_agport;
        private System.Windows.Forms.Label label1_network_address;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label_network_gwport;
        private System.Windows.Forms.TextBox textBox_network_gwport;
        private System.Windows.Forms.Button button_save;
    }
}