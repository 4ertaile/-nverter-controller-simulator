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
        private System.Windows.Forms.ComboBox batteryType;
        private System.Windows.Forms.NumericUpDown batteryElemntCount;
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
            this.batteryType = new System.Windows.Forms.ComboBox();
            this.batteryElemntCount = new System.Windows.Forms.NumericUpDown();
            this.listBoxBatteryInfo = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.battaryPowerInput = new System.Windows.Forms.TextBox();
            this.batteryPower = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.label_Error = new System.Windows.Forms.Label();
            this.panelTypeGrid1 = new System.Windows.Forms.ComboBox();
            this.panelTypeGrid2 = new System.Windows.Forms.ComboBox();
            this.grid1Label = new System.Windows.Forms.Label();
            this.gridLabel2 = new System.Windows.Forms.Label();
            this.countPanelGrid1 = new System.Windows.Forms.NumericUpDown();
            this.countPanelGrid2 = new System.Windows.Forms.NumericUpDown();
            this.labelGrid1PanelCount = new System.Windows.Forms.Label();
            this.labelGrid2PanelCount = new System.Windows.Forms.Label();
            this.discription1 = new System.Windows.Forms.Label();
            this.desription2 = new System.Windows.Forms.Label();
            this.description3 = new System.Windows.Forms.Label();
            this.description4 = new System.Windows.Forms.Label();
            this.grid2MaxPower = new System.Windows.Forms.Label();
            this.grid1MaxPower = new System.Windows.Forms.Label();
            this.description5 = new System.Windows.Forms.Label();
            this.description7 = new System.Windows.Forms.Label();
            this.description8 = new System.Windows.Forms.Label();
            this.maxGripInput = new System.Windows.Forms.Label();
            this.description10 = new System.Windows.Forms.Label();
            this.description9 = new System.Windows.Forms.Label();
            this.description11 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.batteryElemntCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid2)).BeginInit();
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
            this.listBoxReceivedCommands.Size = new System.Drawing.Size(400, 95);
            this.listBoxReceivedCommands.TabIndex = 4;
            // 
            // listBoxSentCommands
            // 
            this.listBoxSentCommands.Location = new System.Drawing.Point(20, 220);
            this.listBoxSentCommands.Name = "listBoxSentCommands";
            this.listBoxSentCommands.Size = new System.Drawing.Size(400, 95);
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
            // batteryType
            // 
            this.batteryType.FormattingEnabled = true;
            this.batteryType.Location = new System.Drawing.Point(20, 360);
            this.batteryType.Name = "batteryType";
            this.batteryType.Size = new System.Drawing.Size(150, 21);
            this.batteryType.TabIndex = 10;
            // 
            // batteryElemntCount
            // 
            this.batteryElemntCount.Location = new System.Drawing.Point(180, 360);
            this.batteryElemntCount.Name = "batteryElemntCount";
            this.batteryElemntCount.ReadOnly = true;
            this.batteryElemntCount.Size = new System.Drawing.Size(120, 20);
            this.batteryElemntCount.TabIndex = 11;
            this.batteryElemntCount.ValueChanged += new System.EventHandler(this.numericUpDownBatteryBlocks_ValueChanged);
            // 
            // listBoxBatteryInfo
            // 
            this.listBoxBatteryInfo.Location = new System.Drawing.Point(20, 390);
            this.listBoxBatteryInfo.Name = "listBoxBatteryInfo";
            this.listBoxBatteryInfo.Size = new System.Drawing.Size(400, 368);
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
            this.battaryPowerInput.Location = new System.Drawing.Point(115, 763);
            this.battaryPowerInput.Name = "battaryPowerInput";
            this.battaryPowerInput.Size = new System.Drawing.Size(100, 20);
            this.battaryPowerInput.TabIndex = 15;
            // 
            // batteryPower
            // 
            this.batteryPower.Location = new System.Drawing.Point(224, 761);
            this.batteryPower.Name = "batteryPower";
            this.batteryPower.Size = new System.Drawing.Size(46, 23);
            this.batteryPower.TabIndex = 16;
            this.batteryPower.Text = "Set";
            this.batteryPower.UseVisualStyleBackColor = true;
            this.batteryPower.Click += new System.EventHandler(this.BatteryPower_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(17, 768);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(92, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Set Battery Power";
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Location = new System.Drawing.Point(280, 766);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(61, 13);
            this.label_Error.TabIndex = 18;
            this.label_Error.Text = "Label_Error";
            // 
            // panelTypeGrid1
            // 
            this.panelTypeGrid1.FormattingEnabled = true;
            this.panelTypeGrid1.Location = new System.Drawing.Point(498, 84);
            this.panelTypeGrid1.Name = "panelTypeGrid1";
            this.panelTypeGrid1.Size = new System.Drawing.Size(121, 21);
            this.panelTypeGrid1.TabIndex = 19;
            // 
            // panelTypeGrid2
            // 
            this.panelTypeGrid2.FormattingEnabled = true;
            this.panelTypeGrid2.Location = new System.Drawing.Point(705, 84);
            this.panelTypeGrid2.Name = "panelTypeGrid2";
            this.panelTypeGrid2.Size = new System.Drawing.Size(121, 21);
            this.panelTypeGrid2.TabIndex = 20;
            // 
            // grid1Label
            // 
            this.grid1Label.AutoSize = true;
            this.grid1Label.Location = new System.Drawing.Point(498, 65);
            this.grid1Label.Name = "grid1Label";
            this.grid1Label.Size = new System.Drawing.Size(32, 13);
            this.grid1Label.TabIndex = 21;
            this.grid1Label.Text = "Grid1";
            // 
            // gridLabel2
            // 
            this.gridLabel2.AutoSize = true;
            this.gridLabel2.Location = new System.Drawing.Point(708, 65);
            this.gridLabel2.Name = "gridLabel2";
            this.gridLabel2.Size = new System.Drawing.Size(32, 13);
            this.gridLabel2.TabIndex = 22;
            this.gridLabel2.Text = "Grid2";
            // 
            // countPanelGrid1
            // 
            this.countPanelGrid1.Location = new System.Drawing.Point(498, 112);
            this.countPanelGrid1.Name = "countPanelGrid1";
            this.countPanelGrid1.ReadOnly = true;
            this.countPanelGrid1.Size = new System.Drawing.Size(120, 20);
            this.countPanelGrid1.TabIndex = 23;
            this.countPanelGrid1.ValueChanged += new System.EventHandler(this.countPanelGrid1_ValueChanged);
            // 
            // countPanelGrid2
            // 
            this.countPanelGrid2.Location = new System.Drawing.Point(705, 112);
            this.countPanelGrid2.Name = "countPanelGrid2";
            this.countPanelGrid2.ReadOnly = true;
            this.countPanelGrid2.Size = new System.Drawing.Size(120, 20);
            this.countPanelGrid2.TabIndex = 24;
            this.countPanelGrid2.ValueChanged += new System.EventHandler(this.countPanelGrid2_ValueChanged);
            // 
            // labelGrid1PanelCount
            // 
            this.labelGrid1PanelCount.AutoSize = true;
            this.labelGrid1PanelCount.Location = new System.Drawing.Point(482, 87);
            this.labelGrid1PanelCount.Name = "labelGrid1PanelCount";
            this.labelGrid1PanelCount.Size = new System.Drawing.Size(13, 13);
            this.labelGrid1PanelCount.TabIndex = 25;
            this.labelGrid1PanelCount.Text = "c";
            // 
            // labelGrid2PanelCount
            // 
            this.labelGrid2PanelCount.AutoSize = true;
            this.labelGrid2PanelCount.Location = new System.Drawing.Point(683, 86);
            this.labelGrid2PanelCount.Name = "labelGrid2PanelCount";
            this.labelGrid2PanelCount.Size = new System.Drawing.Size(13, 13);
            this.labelGrid2PanelCount.TabIndex = 26;
            this.labelGrid2PanelCount.Text = "c";
            // 
            // discription1
            // 
            this.discription1.AutoSize = true;
            this.discription1.Location = new System.Drawing.Point(424, 87);
            this.discription1.Name = "discription1";
            this.discription1.Size = new System.Drawing.Size(56, 13);
            this.discription1.TabIndex = 27;
            this.discription1.Text = "PlateNum:";
            // 
            // desription2
            // 
            this.desription2.AutoSize = true;
            this.desription2.Location = new System.Drawing.Point(625, 87);
            this.desription2.Name = "desription2";
            this.desription2.Size = new System.Drawing.Size(56, 13);
            this.desription2.TabIndex = 28;
            this.desription2.Text = "PlateNum:";
            // 
            // description3
            // 
            this.description3.AutoSize = true;
            this.description3.Location = new System.Drawing.Point(426, 114);
            this.description3.Name = "description3";
            this.description3.Size = new System.Drawing.Size(63, 13);
            this.description3.TabIndex = 29;
            this.description3.Text = "MaxPrower:";
            // 
            // description4
            // 
            this.description4.AutoSize = true;
            this.description4.Location = new System.Drawing.Point(625, 114);
            this.description4.Name = "description4";
            this.description4.Size = new System.Drawing.Size(63, 13);
            this.description4.TabIndex = 30;
            this.description4.Text = "MaxPrower:";
            // 
            // grid2MaxPower
            // 
            this.grid2MaxPower.AutoSize = true;
            this.grid2MaxPower.Location = new System.Drawing.Point(624, 135);
            this.grid2MaxPower.Name = "grid2MaxPower";
            this.grid2MaxPower.Size = new System.Drawing.Size(63, 13);
            this.grid2MaxPower.TabIndex = 32;
            this.grid2MaxPower.Text = "MaxPrower:";
            // 
            // grid1MaxPower
            // 
            this.grid1MaxPower.AutoSize = true;
            this.grid1MaxPower.Location = new System.Drawing.Point(425, 135);
            this.grid1MaxPower.Name = "grid1MaxPower";
            this.grid1MaxPower.Size = new System.Drawing.Size(63, 13);
            this.grid1MaxPower.TabIndex = 31;
            this.grid1MaxPower.Text = "MaxPrower:";
            // 
            // description5
            // 
            this.description5.AutoSize = true;
            this.description5.Location = new System.Drawing.Point(577, 158);
            this.description5.Name = "description5";
            this.description5.Size = new System.Drawing.Size(55, 13);
            this.description5.TabIndex = 33;
            this.description5.Text = "Grid setup";
            // 
            // description7
            // 
            this.description7.AutoSize = true;
            this.description7.Location = new System.Drawing.Point(445, 179);
            this.description7.Name = "description7";
            this.description7.Size = new System.Drawing.Size(95, 13);
            this.description7.TabIndex = 35;
            this.description7.Text = "Max posible power";
            // 
            // description8
            // 
            this.description8.AutoSize = true;
            this.description8.Location = new System.Drawing.Point(681, 158);
            this.description8.Name = "description8";
            this.description8.Size = new System.Drawing.Size(59, 13);
            this.description8.TabIndex = 36;
            this.description8.Text = "Grid Power";
            // 
            // maxGripInput
            // 
            this.maxGripInput.AutoSize = true;
            this.maxGripInput.Location = new System.Drawing.Point(445, 201);
            this.maxGripInput.Name = "maxGripInput";
            this.maxGripInput.Size = new System.Drawing.Size(85, 13);
            this.maxGripInput.TabIndex = 38;
            this.maxGripInput.Text = "Max Real Power";
            // 
            // description10
            // 
            this.description10.AutoSize = true;
            this.description10.Location = new System.Drawing.Point(577, 179);
            this.description10.Name = "description10";
            this.description10.Size = new System.Drawing.Size(62, 13);
            this.description10.TabIndex = 39;
            this.description10.Text = "Solar Grid 1";
            // 
            // description9
            // 
            this.description9.AutoSize = true;
            this.description9.Location = new System.Drawing.Point(577, 201);
            this.description9.Name = "description9";
            this.description9.Size = new System.Drawing.Size(62, 13);
            this.description9.TabIndex = 40;
            this.description9.Text = "Solar Grid 2";
            // 
            // description11
            // 
            this.description11.AutoSize = true;
            this.description11.Location = new System.Drawing.Point(766, 158);
            this.description11.Name = "description11";
            this.description11.Size = new System.Drawing.Size(55, 13);
            this.description11.TabIndex = 41;
            this.description11.Text = "Grid input ";
            // 
            // Simulator
            // 
            this.ClientSize = new System.Drawing.Size(839, 963);
            this.Controls.Add(this.description11);
            this.Controls.Add(this.description9);
            this.Controls.Add(this.description10);
            this.Controls.Add(this.maxGripInput);
            this.Controls.Add(this.description8);
            this.Controls.Add(this.description7);
            this.Controls.Add(this.description5);
            this.Controls.Add(this.grid2MaxPower);
            this.Controls.Add(this.grid1MaxPower);
            this.Controls.Add(this.description4);
            this.Controls.Add(this.description3);
            this.Controls.Add(this.desription2);
            this.Controls.Add(this.discription1);
            this.Controls.Add(this.labelGrid2PanelCount);
            this.Controls.Add(this.labelGrid1PanelCount);
            this.Controls.Add(this.countPanelGrid2);
            this.Controls.Add(this.countPanelGrid1);
            this.Controls.Add(this.gridLabel2);
            this.Controls.Add(this.grid1Label);
            this.Controls.Add(this.panelTypeGrid2);
            this.Controls.Add(this.panelTypeGrid1);
            this.Controls.Add(this.label_Error);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.batteryPower);
            this.Controls.Add(this.battaryPowerInput);
            this.Controls.Add(this.listBoxBatteryInfo);
            this.Controls.Add(this.batteryElemntCount);
            this.Controls.Add(this.batteryType);
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
            ((System.ComponentModel.ISupportInitialize)(this.batteryElemntCount)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid2)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox battaryPowerInput;
        private System.Windows.Forms.Button batteryPower;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label_Error;
        private System.Windows.Forms.ComboBox panelTypeGrid1;
        private System.Windows.Forms.ComboBox panelTypeGrid2;
        private System.Windows.Forms.Label grid1Label;
        private System.Windows.Forms.Label gridLabel2;
        private System.Windows.Forms.NumericUpDown countPanelGrid1;
        private System.Windows.Forms.NumericUpDown countPanelGrid2;
        private System.Windows.Forms.Label labelGrid1PanelCount;
        private System.Windows.Forms.Label labelGrid2PanelCount;
        private System.Windows.Forms.Label discription1;
        private System.Windows.Forms.Label desription2;
        private System.Windows.Forms.Label description3;
        private System.Windows.Forms.Label description4;
        private System.Windows.Forms.Label grid2MaxPower;
        private System.Windows.Forms.Label grid1MaxPower;
        private System.Windows.Forms.Label description5;
        private System.Windows.Forms.Label description7;
        private System.Windows.Forms.Label description8;
        private System.Windows.Forms.Label maxGripInput;
        private System.Windows.Forms.Label description10;
        private System.Windows.Forms.Label description9;
        private System.Windows.Forms.Label description11;
    }
}
