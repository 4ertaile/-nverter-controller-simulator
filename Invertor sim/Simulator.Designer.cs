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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(736, 7);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
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
            this.labelStatus.Location = new System.Drawing.Point(280, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(200, 28);
            this.labelStatus.TabIndex = 3;
            this.labelStatus.Text = "Status: Disconnected";
            // 
            // listBoxReceivedCommands
            // 
            this.listBoxReceivedCommands.Location = new System.Drawing.Point(20, 60);
            this.listBoxReceivedCommands.Name = "listBoxReceivedCommands";
            this.listBoxReceivedCommands.Size = new System.Drawing.Size(400, 134);
            this.listBoxReceivedCommands.TabIndex = 4;
            // 
            // listBoxSentCommands
            // 
            this.listBoxSentCommands.Location = new System.Drawing.Point(20, 220);
            this.listBoxSentCommands.Name = "listBoxSentCommands";
            this.listBoxSentCommands.Size = new System.Drawing.Size(400, 134);
            this.listBoxSentCommands.TabIndex = 5;
            // 
            // labelCurrentTime
            // 
            this.labelCurrentTime.Location = new System.Drawing.Point(431, 7);
            this.labelCurrentTime.Name = "labelCurrentTime";
            this.labelCurrentTime.Size = new System.Drawing.Size(49, 21);
            this.labelCurrentTime.TabIndex = 6;
            this.labelCurrentTime.Text = "00:00:00";
            // 
            // buttonSyncTime
            // 
            this.buttonSyncTime.Location = new System.Drawing.Point(415, 26);
            this.buttonSyncTime.Name = "buttonSyncTime";
            this.buttonSyncTime.Size = new System.Drawing.Size(84, 27);
            this.buttonSyncTime.TabIndex = 7;
            this.buttonSyncTime.Text = "Sync Time";
            this.buttonSyncTime.UseVisualStyleBackColor = true;
            this.buttonSyncTime.Click += new System.EventHandler(this.buttonSyncTime_Click);
            // 
            // textBoxSetTime
            // 
            this.textBoxSetTime.Location = new System.Drawing.Point(498, 4);
            this.textBoxSetTime.Name = "textBoxSetTime";
            this.textBoxSetTime.Size = new System.Drawing.Size(71, 20);
            this.textBoxSetTime.TabIndex = 8;
            this.textBoxSetTime.Text = "HH:mm:ss";
            this.textBoxSetTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // buttonConfirmTime
            // 
            this.buttonConfirmTime.Location = new System.Drawing.Point(575, 2);
            this.buttonConfirmTime.Name = "buttonConfirmTime";
            this.buttonConfirmTime.Size = new System.Drawing.Size(29, 24);
            this.buttonConfirmTime.TabIndex = 9;
            this.buttonConfirmTime.Text = "<--";
            this.buttonConfirmTime.UseVisualStyleBackColor = true;
            this.buttonConfirmTime.Click += new System.EventHandler(this.buttonConfirmTime_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(26, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Port";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(147, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Baud rate";
            // 
            // Simulator
            // 
            this.ClientSize = new System.Drawing.Size(839, 457);
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
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
    }
}
