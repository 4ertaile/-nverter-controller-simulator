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
        private System.Windows.Forms.TextBox textBoxSendCommand;
        private System.Windows.Forms.Button buttonSend;

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
            this.textBoxSendCommand = new System.Windows.Forms.TextBox();
            this.buttonSend = new System.Windows.Forms.Button();

            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(1035, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(131, 36);
            this.button1.TabIndex = 0;
            this.button1.Text = "Data generator";
            this.button1.UseVisualStyleBackColor = true;
            //this.button1.Click += new System.EventHandler(this.button1_Click);

            // 
            // comboBoxPorts
            // 
            this.comboBoxPorts.Location = new System.Drawing.Point(20, 20);
            this.comboBoxPorts.Name = "comboBoxPorts";
            this.comboBoxPorts.Size = new System.Drawing.Size(120, 28);
            this.comboBoxPorts.TabIndex = 1;

            // 
            // comboBoxBaudRate
            // 
            this.comboBoxBaudRate.Location = new System.Drawing.Point(150, 20);
            this.comboBoxBaudRate.Name = "comboBoxBaudRate";
            this.comboBoxBaudRate.Size = new System.Drawing.Size(120, 28);
            this.comboBoxBaudRate.TabIndex = 2;

            // 
            // labelStatus
            // 
            this.labelStatus.Location = new System.Drawing.Point(280, 20);
            this.labelStatus.Name = "labelStatus";
            this.labelStatus.Size = new System.Drawing.Size(200, 28);
            this.labelStatus.Text = "Status: Disconnected";

            // 
            // listBoxReceivedCommands
            // 
            this.listBoxReceivedCommands.Location = new System.Drawing.Point(20, 60);
            this.listBoxReceivedCommands.Size = new System.Drawing.Size(400, 150);
            this.listBoxReceivedCommands.TabIndex = 3;

            // 
            // listBoxSentCommands
            // 
            this.listBoxSentCommands.Location = new System.Drawing.Point(20, 220);
            this.listBoxSentCommands.Size = new System.Drawing.Size(400, 150);
            this.listBoxSentCommands.TabIndex = 4;

            // 
            // textBoxSendCommand
            // 
            this.textBoxSendCommand.Location = new System.Drawing.Point(20, 380);
            this.textBoxSendCommand.Size = new System.Drawing.Size(320, 28);
            this.textBoxSendCommand.TabIndex = 5;

            // 
            // buttonSend
            // 
            this.buttonSend.Location = new System.Drawing.Point(350, 380);
            this.buttonSend.Name = "buttonSend";
            this.buttonSend.Size = new System.Drawing.Size(70, 28);
            this.buttonSend.TabIndex = 6;
            this.buttonSend.Text = "Send";
            this.buttonSend.Click += new System.EventHandler(this.buttonSend_Click);

            // 
            // Simulator
            // 
            this.ClientSize = new System.Drawing.Size(1178, 844);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.comboBoxPorts);
            this.Controls.Add(this.comboBoxBaudRate);
            this.Controls.Add(this.labelStatus);
            this.Controls.Add(this.listBoxReceivedCommands);
            this.Controls.Add(this.listBoxSentCommands);
            this.Controls.Add(this.textBoxSendCommand);
            this.Controls.Add(this.buttonSend);
            this.Name = "Simulator";
            this.Text = "Simulator";
            this.ResumeLayout(false);
        }
    }
}
