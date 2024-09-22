namespace Invertor_sim
{
    partial class Simulator
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox comboBoxPorts;
        private System.Windows.Forms.ComboBox comboBoxBaudRate;
        private System.Windows.Forms.Label labelStatus;
        private System.Windows.Forms.ListBox listBoxReceivedCommands;
        private System.Windows.Forms.ListBox listBoxSentCommands;
        private System.Windows.Forms.Label labelCurrentTime;
        private System.Windows.Forms.Button buttonSyncTime;
        private System.Windows.Forms.TextBox textBoxSetTime;
        private System.Windows.Forms.Button buttonConfirmTime;
        private System.Windows.Forms.ComboBox comboBoxBattery;
        private System.Windows.Forms.NumericUpDown numericUpDownBatteryBlocks;
        private System.Windows.Forms.ListBox listBoxBatteryInfo; // Заміна тут

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
            this.button1 = new System.Windows.Forms.Button();
            this.comboBoxPorts = new System.Windows.Forms.ComboBox();
            this.comboBoxBaudRate = new System.Windows.Forms.ComboBox();
            this.labelStatus = new System.Windows.Forms.Label();
            this.listBoxReceivedCommands = new System.Windows.Forms.ListBox();
            this.listBoxSentCommands = new System.Windows.Forms.ListBox();
            this.labelCurrentTime = new System.Windows.Forms.Label();
            this.buttonSyncTime = new System.Windows.Forms.Button();
            this.textBoxSetTime = new System.Windows.Forms.TextBox();
            this.buttonConfirmTime = new System.Windows.Forms.Button();
            this.comboBoxBattery = new System.Windows.Forms.ComboBox();
            this.numericUpDownBatteryBlocks = new System.Windows.Forms.NumericUpDown();
            this.listBoxBatteryInfo = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.battaryPowerInput = new System.Windows.Forms.TextBox();
            this.BatteryPower = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_Error = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBatteryBlocks)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(736, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 24);
            this.button1.TabIndex = 0;
            this.button1.Text = "Data generator";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.Location = new System.Drawing.Point(20, 20);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(120, 21);
            this.comboBoxPorts.TabIndex = 1;
            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.Location = new System.Drawing.Point(150, 20);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(120, 21);
            this.comboBoxBaudRate.TabIndex = 2;
            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(280, 24);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(113, 21);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Status: Disconnected";
            // 
            // listBoxReceivedCommands
            // 
            this.listBoxReceivedCommands.Location = new System.Drawing.Point(20, 60);
            this.listBoxReceivedCommands.Name = "listBoxReceivedCommands";
            this.listBoxReceivedCommands.Size = new System.Drawing.Size(400, 121);
            this.listBoxReceivedCommands.TabIndex = 4;
            // 
            // listBoxSentCommands
            // 
            this.listBoxSentCommands.Location = new System.Drawing.Point(20, 220);
            this.listBoxSentCommands.Name = "listBoxSentCommands";
            this.listBoxSentCommands.Size = new System.Drawing.Size(400, 121);
            this.listBoxSentCommands.TabIndex = 5;
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.Location = new System.Drawing.Point(422, 2);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(49, 21);
            this.labelCurrentTime.TabIndex = 6;
            this.labelCurrentTime.Text = "00:00:00";
            // 
            // buttonSyncTime
            // 
            this.buttonSyncTime.Location = new System.Drawing.Point(414, 20);
            this.buttonSyncTime.Name = "buttonSyncTime";
            this.buttonSyncTime.Size = new System.Drawing.Size(66, 25);
            this.buttonSyncTime.TabIndex = 7;
            this.buttonSyncTime.Text = "Sync Time";
            this.buttonSyncTime.UseVisualStyleBackColor = true;
            this.buttonSyncTime.Click += new System.EventHandler(this.buttonSyncTime_Click);
            // 
            // textBoxSetTime
            // 
            this.textBoxSetTime.Location = new System.Drawing.Point(481, 1);
            this.textBoxSetTime.Name = "textBoxSetTime";
            this.textBoxSetTime.Size = new System.Drawing.Size(71, 20);
            this.textBoxSetTime.TabIndex = 8;
            this.textBoxSetTime.Text = "HH:mm:ss";
            this.textBoxSetTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonConfirmTime
            // 
            this.buttonConfirmTime.Location = new System.Drawing.Point(550, 0);
            this.buttonConfirmTime.Name = "buttonConfirmTime";
            this.buttonConfirmTime.Size = new System.Drawing.Size(31, 22);
            this.buttonConfirmTime.TabIndex = 9;
            this.buttonConfirmTime.Text = "<--";
            this.buttonConfirmTime.UseVisualStyleBackColor = true;
            this.buttonConfirmTime.Click += new System.EventHandler(this.buttonConfirmTime_Click);
            // 
            // comboBoxBattery
            // 
            this.comboBoxBattery.FormattingEnabled = true;
            this.comboBoxBattery.Location = new System.Drawing.Point(20, 360);
            this.comboBoxBattery.Name = "comboBoxBattery";
            this.comboBoxBattery.Size = new System.Drawing.Size(150, 21);
            this.comboBoxBattery.TabIndex = 10;
            // 
            // numericUpDownBatteryBlocks
            // 
            this.numericUpDownBatteryBlocks.Location = new System.Drawing.Point(180, 360);
            this.numericUpDownBatteryBlocks.Name = "numericUpDownBatteryBlocks";
            this.numericUpDownBatteryBlocks.ReadOnly = true;
            this.numericUpDownBatteryBlocks.Size = new System.Drawing.Size(120, 20);
            this.numericUpDownBatteryBlocks.TabIndex = 11;
            this.numericUpDownBatteryBlocks.ValueChanged += new System.EventHandler(this.numericUpDownBatteryBlocks_ValueChanged);
            // 
            // listBoxBatteryInfo
            // 
            this.listBoxBatteryInfo.Location = new System.Drawing.Point(20, 390);
            this.listBoxBatteryInfo.Name = "listBoxBatteryInfo";
            this.listBoxBatteryInfo.Size = new System.Drawing.Size(400, 147);
            this.listBoxBatteryInfo.TabIndex = 12;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 13;
            this.label1.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 14;
            this.label2.Text = "Baud rate";
            // 
            // battaryPowerInput
            // 
            this.battaryPowerInput.Location = new System.Drawing.Point(115, 543);
            this.battaryPowerInput.Name = "battaryPowerInput";
            this.battaryPowerInput.Size = new System.Drawing.Size(100, 20);
            this.battaryPowerInput.TabIndex = 15;
            // 
            // BatteryPower
            // 
            this.BatteryPower.Location = new System.Drawing.Point(224, 541);
            this.BatteryPower.Name = "BatteryPower";
            this.BatteryPower.Size = new System.Drawing.Size(46, 23);
            this.BatteryPower.TabIndex = 16;
            this.BatteryPower.Text = "Set";
            this.BatteryPower.UseVisualStyleBackColor = true;
            this.BatteryPower.Click += new System.EventHandler(this.BatteryPower_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 548);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Set Battery Power";
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Location = new System.Drawing.Point(280, 546);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(61, 13);
            this.label_Error.TabIndex = 18;
            this.label_Error.Text = "Label_Error";
            // 
            // Simulator
            // 
            this.ClientSize = new System.Drawing.Size(839, 644);
            this.Controls.Add(this.label_Error);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.BatteryPower);
            this.Controls.Add(this.battaryPowerInput);
            this.Controls.Add(this.listBoxBatteryInfo);
            this.Controls.Add(this.numericUpDownBatteryBlocks);
            this.Controls.Add(this.comboBoxBattery);
            this.Controls.Add(this.textBoxSetTime);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.buttonSyncTime);
            this.Controls.Add(this.labelCurrentTime);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listBoxReceivedCommands);
            this.Controls.Add(this.listBoxSentCommands);
            this.Controls.Add(this.buttonConfirmTime);
            this.Name = "Simulator";
            this.Text = "Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownBatteryBlocks)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox battaryPowerInput;
        private System.Windows.Forms.Button BatteryPower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_Error;
    }
}
