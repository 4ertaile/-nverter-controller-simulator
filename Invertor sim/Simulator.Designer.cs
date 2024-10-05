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
            this.maxPosiibleSolarGridPower = new System.Windows.Forms.Label();
            this.description8 = new System.Windows.Forms.Label();
            this.description10 = new System.Windows.Forms.Label();
            this.description9 = new System.Windows.Forms.Label();
            this.description11 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.setSolarGridPower1 = new System.Windows.Forms.TextBox();
            this.setSolarGridPower2 = new System.Windows.Forms.TextBox();
            this.description13 = new System.Windows.Forms.Label();
            this.checkBoxSolar1 = new System.Windows.Forms.CheckBox();
            this.checkBoxSolar2 = new System.Windows.Forms.CheckBox();
            this.solarGridPower1 = new System.Windows.Forms.Label();
            this.solarGridPower2 = new System.Windows.Forms.Label();
            this.solarGridVoltage2 = new System.Windows.Forms.Label();
            this.solarGridVoltage1 = new System.Windows.Forms.Label();
            this.MaxGridPowerSum = new System.Windows.Forms.Label();
            this.GridVoltageLimit = new System.Windows.Forms.Label();
            this.MaxGridPower = new System.Windows.Forms.Label();
            this.buttonSetnputSolPower1 = new System.Windows.Forms.Button();
            this.buttonSetnputSolPower2 = new System.Windows.Forms.Button();
            this.possibleGrid2Voltage = new System.Windows.Forms.Label();
            this.possibleGrid1Voltage = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.description23 = new System.Windows.Forms.Label();
            this.buttonSetGenPower = new System.Windows.Forms.Button();
            this.setGenVoltage = new System.Windows.Forms.TextBox();
            this.description17 = new System.Windows.Forms.Label();
            this.genPowerIn = new System.Windows.Forms.Label();
            this.description16 = new System.Windows.Forms.Label();
            this.genVoltage = new System.Windows.Forms.Label();
            this.genMaxPower = new System.Windows.Forms.Label();
            this.description18 = new System.Windows.Forms.Label();
            this.mainsGridVoltage = new System.Windows.Forms.Label();
            this.mainsGridPowerIn = new System.Windows.Forms.Label();
            this.description20 = new System.Windows.Forms.Label();
            this.description21 = new System.Windows.Forms.Label();
            this.description24 = new System.Windows.Forms.Label();
            this.buttonSetMainsPower = new System.Windows.Forms.Button();
            this.setGenPower = new System.Windows.Forms.TextBox();
            this.setMainsGridVoltaage = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.maxChargePower = new System.Windows.Forms.Label();
            this.desription23 = new System.Windows.Forms.Label();
            this.chargePowerBattery = new System.Windows.Forms.NumericUpDown();
            this.dischargePowerBatery = new System.Windows.Forms.NumericUpDown();
            this.description25 = new System.Windows.Forms.Label();
            this.maxBatteryPowerOut = new System.Windows.Forms.Label();
            this.batteryPowerOut = new System.Windows.Forms.Label();
            this.description30 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.description29 = new System.Windows.Forms.Label();
            this.buttonSetPowerOut = new System.Windows.Forms.Button();
            this.setInvertorPowerOut = new System.Windows.Forms.TextBox();
            this.invertor_status = new System.Windows.Forms.Label();
            this.invertorVoltageOut = new System.Windows.Forms.Label();
            this.description27 = new System.Windows.Forms.Label();
            this.invertorPowerOut = new System.Windows.Forms.Label();
            this.description28 = new System.Windows.Forms.Label();
            this.maxPowerOut = new System.Windows.Forms.Label();
            this.powerSale = new System.Windows.Forms.Label();
            this.haveInvertorOut = new System.Windows.Forms.CheckBox();
            this.setBatteryCapacity = new System.Windows.Forms.NumericUpDown();
            this.startSimulation = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.buttonClearError = new System.Windows.Forms.Button();
            this.maximumCharge = new System.Windows.Forms.NumericUpDown();
            this.minimalCharge = new System.Windows.Forms.NumericUpDown();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.maintainingCharge = new System.Windows.Forms.NumericUpDown();
            this.useMainsGrid = new System.Windows.Forms.CheckBox();
            this.label15 = new System.Windows.Forms.Label();
            this.useGenerator = new System.Windows.Forms.CheckBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.label18 = new System.Windows.Forms.Label();
            this.label19 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.batteryElemntCount)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countPanelGrid2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.chargePowerBattery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dischargePowerBatery)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.setBatteryCapacity)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximumCharge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimalCharge)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintainingCharge)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(736, 6);
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
            this.listBoxReceivedCommands.Size = new System.Drawing.Size(328, 95);
            this.listBoxReceivedCommands.TabIndex = 4;
            // 
            // listBoxSentCommands
            // 
            this.listBoxSentCommands.Location = new System.Drawing.Point(20, 220);
            this.listBoxSentCommands.Name = "listBoxSentCommands";
            this.listBoxSentCommands.Size = new System.Drawing.Size(328, 95);
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
            this.batteryType.Location = new System.Drawing.Point(388, 209);
            this.batteryType.Name = "batteryType";
            this.batteryType.Size = new System.Drawing.Size(150, 21);
            this.batteryType.TabIndex = 10;
            // 
            // batteryElemntCount
            // 
            this.batteryElemntCount.Location = new System.Drawing.Point(548, 209);
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
            this.listBoxBatteryInfo.Size = new System.Drawing.Size(328, 537);
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
            // batteryPower
            // 
            this.batteryPower.Location = new System.Drawing.Point(596, 235);
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
            this.label3.Location = new System.Drawing.Point(389, 240);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(103, 13);
            this.label3.TabIndex = 17;
            this.label3.Text = "Set Battery Capacity";
            // 
            // label_Error
            // 
            this.label_Error.AutoSize = true;
            this.label_Error.Location = new System.Drawing.Point(375, 642);
            this.label_Error.Name = "label_Error";
            this.label_Error.Size = new System.Drawing.Size(32, 13);
            this.label_Error.TabIndex = 18;
            this.label_Error.Text = "Error:";
            // 
            // panelTypeGrid1
            // 
            this.panelTypeGrid1.FormattingEnabled = true;
            this.panelTypeGrid1.Location = new System.Drawing.Point(457, 73);
            this.panelTypeGrid1.Name = "panelTypeGrid1";
            this.panelTypeGrid1.Size = new System.Drawing.Size(121, 21);
            this.panelTypeGrid1.TabIndex = 19;
            // 
            // panelTypeGrid2
            // 
            this.panelTypeGrid2.FormattingEnabled = true;
            this.panelTypeGrid2.Location = new System.Drawing.Point(664, 73);
            this.panelTypeGrid2.Name = "panelTypeGrid2";
            this.panelTypeGrid2.Size = new System.Drawing.Size(121, 21);
            this.panelTypeGrid2.TabIndex = 20;
            // 
            // grid1Label
            // 
            this.grid1Label.AutoSize = true;
            this.grid1Label.Location = new System.Drawing.Point(457, 54);
            this.grid1Label.Name = "grid1Label";
            this.grid1Label.Size = new System.Drawing.Size(32, 13);
            this.grid1Label.TabIndex = 21;
            this.grid1Label.Text = "Grid1";
            // 
            // gridLabel2
            // 
            this.gridLabel2.AutoSize = true;
            this.gridLabel2.Location = new System.Drawing.Point(667, 54);
            this.gridLabel2.Name = "gridLabel2";
            this.gridLabel2.Size = new System.Drawing.Size(32, 13);
            this.gridLabel2.TabIndex = 22;
            this.gridLabel2.Text = "Grid2";
            // 
            // countPanelGrid1
            // 
            this.countPanelGrid1.Location = new System.Drawing.Point(457, 101);
            this.countPanelGrid1.Name = "countPanelGrid1";
            this.countPanelGrid1.ReadOnly = true;
            this.countPanelGrid1.Size = new System.Drawing.Size(120, 20);
            this.countPanelGrid1.TabIndex = 23;
            this.countPanelGrid1.ValueChanged += new System.EventHandler(this.countPanelGrid1_ValueChanged);
            // 
            // countPanelGrid2
            // 
            this.countPanelGrid2.Location = new System.Drawing.Point(664, 101);
            this.countPanelGrid2.Name = "countPanelGrid2";
            this.countPanelGrid2.ReadOnly = true;
            this.countPanelGrid2.Size = new System.Drawing.Size(120, 20);
            this.countPanelGrid2.TabIndex = 24;
            this.countPanelGrid2.ValueChanged += new System.EventHandler(this.countPanelGrid2_ValueChanged);
            // 
            // labelGrid1PanelCount
            // 
            this.labelGrid1PanelCount.AutoSize = true;
            this.labelGrid1PanelCount.Location = new System.Drawing.Point(441, 76);
            this.labelGrid1PanelCount.Name = "labelGrid1PanelCount";
            this.labelGrid1PanelCount.Size = new System.Drawing.Size(13, 13);
            this.labelGrid1PanelCount.TabIndex = 25;
            this.labelGrid1PanelCount.Text = "c";
            // 
            // labelGrid2PanelCount
            // 
            this.labelGrid2PanelCount.AutoSize = true;
            this.labelGrid2PanelCount.Location = new System.Drawing.Point(642, 75);
            this.labelGrid2PanelCount.Name = "labelGrid2PanelCount";
            this.labelGrid2PanelCount.Size = new System.Drawing.Size(13, 13);
            this.labelGrid2PanelCount.TabIndex = 26;
            this.labelGrid2PanelCount.Text = "c";
            // 
            // discription1
            // 
            this.discription1.AutoSize = true;
            this.discription1.Location = new System.Drawing.Point(383, 76);
            this.discription1.Name = "discription1";
            this.discription1.Size = new System.Drawing.Size(56, 13);
            this.discription1.TabIndex = 27;
            this.discription1.Text = "PlateNum:";
            // 
            // desription2
            // 
            this.desription2.AutoSize = true;
            this.desription2.Location = new System.Drawing.Point(584, 76);
            this.desription2.Name = "desription2";
            this.desription2.Size = new System.Drawing.Size(56, 13);
            this.desription2.TabIndex = 28;
            this.desription2.Text = "PlateNum:";
            // 
            // description3
            // 
            this.description3.AutoSize = true;
            this.description3.Location = new System.Drawing.Point(385, 103);
            this.description3.Name = "description3";
            this.description3.Size = new System.Drawing.Size(63, 13);
            this.description3.TabIndex = 29;
            this.description3.Text = "MaxPrower:";
            // 
            // description4
            // 
            this.description4.AutoSize = true;
            this.description4.Location = new System.Drawing.Point(584, 103);
            this.description4.Name = "description4";
            this.description4.Size = new System.Drawing.Size(63, 13);
            this.description4.TabIndex = 30;
            this.description4.Text = "MaxPrower:";
            // 
            // grid2MaxPower
            // 
            this.grid2MaxPower.AutoSize = true;
            this.grid2MaxPower.Location = new System.Drawing.Point(583, 124);
            this.grid2MaxPower.Name = "grid2MaxPower";
            this.grid2MaxPower.Size = new System.Drawing.Size(63, 13);
            this.grid2MaxPower.TabIndex = 32;
            this.grid2MaxPower.Text = "MaxPrower:";
            // 
            // grid1MaxPower
            // 
            this.grid1MaxPower.AutoSize = true;
            this.grid1MaxPower.Location = new System.Drawing.Point(384, 124);
            this.grid1MaxPower.Name = "grid1MaxPower";
            this.grid1MaxPower.Size = new System.Drawing.Size(63, 13);
            this.grid1MaxPower.TabIndex = 31;
            this.grid1MaxPower.Text = "MaxPrower:";
            // 
            // description5
            // 
            this.description5.AutoSize = true;
            this.description5.Location = new System.Drawing.Point(384, 383);
            this.description5.Name = "description5";
            this.description5.Size = new System.Drawing.Size(26, 13);
            this.description5.TabIndex = 33;
            this.description5.Text = "Grid";
            // 
            // maxPosiibleSolarGridPower
            // 
            this.maxPosiibleSolarGridPower.AutoSize = true;
            this.maxPosiibleSolarGridPower.Location = new System.Drawing.Point(505, 148);
            this.maxPosiibleSolarGridPower.Name = "maxPosiibleSolarGridPower";
            this.maxPosiibleSolarGridPower.Size = new System.Drawing.Size(37, 13);
            this.maxPosiibleSolarGridPower.TabIndex = 35;
            this.maxPosiibleSolarGridPower.Text = "Power";
            // 
            // description8
            // 
            this.description8.AutoSize = true;
            this.description8.Location = new System.Drawing.Point(674, 383);
            this.description8.Name = "description8";
            this.description8.Size = new System.Drawing.Size(71, 13);
            this.description8.TabIndex = 36;
            this.description8.Text = "Grid Power In";
            // 
            // description10
            // 
            this.description10.AutoSize = true;
            this.description10.Location = new System.Drawing.Point(365, 404);
            this.description10.Name = "description10";
            this.description10.Size = new System.Drawing.Size(62, 13);
            this.description10.TabIndex = 39;
            this.description10.Text = "Solar Grid 1";
            // 
            // description9
            // 
            this.description9.AutoSize = true;
            this.description9.Location = new System.Drawing.Point(365, 426);
            this.description9.Name = "description9";
            this.description9.Size = new System.Drawing.Size(62, 13);
            this.description9.TabIndex = 40;
            this.description9.Text = "Solar Grid 2";
            // 
            // description11
            // 
            this.description11.AutoSize = true;
            this.description11.Location = new System.Drawing.Point(448, 383);
            this.description11.Name = "description11";
            this.description11.Size = new System.Drawing.Size(55, 13);
            this.description11.TabIndex = 41;
            this.description11.Text = "Grid input ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(385, 148);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(116, 13);
            this.label4.TabIndex = 43;
            this.label4.Text = "Max Work Power Sum:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(740, 383);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(65, 13);
            this.label5.TabIndex = 44;
            this.label5.Text = "Grid Voltage";
            // 
            // setSolarGridPower1
            // 
            this.setSolarGridPower1.Location = new System.Drawing.Point(444, 399);
            this.setSolarGridPower1.Name = "setSolarGridPower1";
            this.setSolarGridPower1.Size = new System.Drawing.Size(59, 20);
            this.setSolarGridPower1.TabIndex = 45;
            // 
            // setSolarGridPower2
            // 
            this.setSolarGridPower2.Location = new System.Drawing.Point(444, 422);
            this.setSolarGridPower2.Name = "setSolarGridPower2";
            this.setSolarGridPower2.Size = new System.Drawing.Size(59, 20);
            this.setSolarGridPower2.TabIndex = 46;
            // 
            // description13
            // 
            this.description13.AutoSize = true;
            this.description13.Location = new System.Drawing.Point(592, 383);
            this.description13.Name = "description13";
            this.description13.Size = new System.Drawing.Size(76, 13);
            this.description13.TabIndex = 49;
            this.description13.Text = "Grid Can Used";
            // 
            // checkBoxSolar1
            // 
            this.checkBoxSolar1.AutoSize = true;
            this.checkBoxSolar1.Location = new System.Drawing.Point(625, 403);
            this.checkBoxSolar1.Name = "checkBoxSolar1";
            this.checkBoxSolar1.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSolar1.TabIndex = 50;
            this.checkBoxSolar1.UseVisualStyleBackColor = true;
            this.checkBoxSolar1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBoxSolar1_MouseDown);
            // 
            // checkBoxSolar2
            // 
            this.checkBoxSolar2.AutoSize = true;
            this.checkBoxSolar2.Location = new System.Drawing.Point(625, 426);
            this.checkBoxSolar2.Name = "checkBoxSolar2";
            this.checkBoxSolar2.Size = new System.Drawing.Size(15, 14);
            this.checkBoxSolar2.TabIndex = 51;
            this.checkBoxSolar2.UseVisualStyleBackColor = true;
            this.checkBoxSolar2.MouseDown += new System.Windows.Forms.MouseEventHandler(this.checkBoxSolar2_MouseDown);
            // 
            // solarGridPower1
            // 
            this.solarGridPower1.AutoSize = true;
            this.solarGridPower1.Location = new System.Drawing.Point(691, 404);
            this.solarGridPower1.Name = "solarGridPower1";
            this.solarGridPower1.Size = new System.Drawing.Size(13, 13);
            this.solarGridPower1.TabIndex = 52;
            this.solarGridPower1.Text = "0";
            // 
            // solarGridPower2
            // 
            this.solarGridPower2.AutoSize = true;
            this.solarGridPower2.Location = new System.Drawing.Point(691, 425);
            this.solarGridPower2.Name = "solarGridPower2";
            this.solarGridPower2.Size = new System.Drawing.Size(13, 13);
            this.solarGridPower2.TabIndex = 53;
            this.solarGridPower2.Text = "0";
            // 
            // solarGridVoltage2
            // 
            this.solarGridVoltage2.AutoSize = true;
            this.solarGridVoltage2.Location = new System.Drawing.Point(752, 425);
            this.solarGridVoltage2.Name = "solarGridVoltage2";
            this.solarGridVoltage2.Size = new System.Drawing.Size(13, 13);
            this.solarGridVoltage2.TabIndex = 55;
            this.solarGridVoltage2.Text = "0";
            // 
            // solarGridVoltage1
            // 
            this.solarGridVoltage1.AutoSize = true;
            this.solarGridVoltage1.Location = new System.Drawing.Point(752, 402);
            this.solarGridVoltage1.Name = "solarGridVoltage1";
            this.solarGridVoltage1.Size = new System.Drawing.Size(13, 13);
            this.solarGridVoltage1.TabIndex = 54;
            this.solarGridVoltage1.Text = "0";
            // 
            // MaxGridPowerSum
            // 
            this.MaxGridPowerSum.AutoSize = true;
            this.MaxGridPowerSum.Location = new System.Drawing.Point(614, 148);
            this.MaxGridPowerSum.Name = "MaxGridPowerSum";
            this.MaxGridPowerSum.Size = new System.Drawing.Size(109, 13);
            this.MaxGridPowerSum.TabIndex = 56;
            this.MaxGridPowerSum.Text = "Max Grid Power Sum:";
            // 
            // GridVoltageLimit
            // 
            this.GridVoltageLimit.AutoSize = true;
            this.GridVoltageLimit.Location = new System.Drawing.Point(385, 174);
            this.GridVoltageLimit.Name = "GridVoltageLimit";
            this.GridVoltageLimit.Size = new System.Drawing.Size(147, 13);
            this.GridVoltageLimit.TabIndex = 57;
            this.GridVoltageLimit.Text = "Grid Voltage Generation Limit:";
            // 
            // MaxGridPower
            // 
            this.MaxGridPower.AutoSize = true;
            this.MaxGridPower.Location = new System.Drawing.Point(614, 174);
            this.MaxGridPower.Name = "MaxGridPower";
            this.MaxGridPower.Size = new System.Drawing.Size(85, 13);
            this.MaxGridPower.TabIndex = 58;
            this.MaxGridPower.Text = "Max Grid Power:";
            // 
            // buttonSetnputSolPower1
            // 
            this.buttonSetnputSolPower1.Location = new System.Drawing.Point(501, 398);
            this.buttonSetnputSolPower1.Name = "buttonSetnputSolPower1";
            this.buttonSetnputSolPower1.Size = new System.Drawing.Size(15, 22);
            this.buttonSetnputSolPower1.TabIndex = 59;
            this.buttonSetnputSolPower1.Text = "<";
            this.buttonSetnputSolPower1.UseVisualStyleBackColor = true;
            this.buttonSetnputSolPower1.Click += new System.EventHandler(this.buttonSetnputSolPower1_Click);
            // 
            // buttonSetnputSolPower2
            // 
            this.buttonSetnputSolPower2.Location = new System.Drawing.Point(501, 421);
            this.buttonSetnputSolPower2.Name = "buttonSetnputSolPower2";
            this.buttonSetnputSolPower2.Size = new System.Drawing.Size(15, 22);
            this.buttonSetnputSolPower2.TabIndex = 60;
            this.buttonSetnputSolPower2.Text = "<";
            this.buttonSetnputSolPower2.UseVisualStyleBackColor = true;
            this.buttonSetnputSolPower2.Click += new System.EventHandler(this.buttonSetnputSolPower2_Click);
            // 
            // possibleGrid2Voltage
            // 
            this.possibleGrid2Voltage.AutoSize = true;
            this.possibleGrid2Voltage.Location = new System.Drawing.Point(525, 425);
            this.possibleGrid2Voltage.Name = "possibleGrid2Voltage";
            this.possibleGrid2Voltage.Size = new System.Drawing.Size(13, 13);
            this.possibleGrid2Voltage.TabIndex = 63;
            this.possibleGrid2Voltage.Text = "0";
            // 
            // possibleGrid1Voltage
            // 
            this.possibleGrid1Voltage.AutoSize = true;
            this.possibleGrid1Voltage.Location = new System.Drawing.Point(525, 402);
            this.possibleGrid1Voltage.Name = "possibleGrid1Voltage";
            this.possibleGrid1Voltage.Size = new System.Drawing.Size(13, 13);
            this.possibleGrid1Voltage.TabIndex = 62;
            this.possibleGrid1Voltage.Text = "0";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(521, 383);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(65, 13);
            this.label8.TabIndex = 61;
            this.label8.Text = "Grid Voltage";
            // 
            // description23
            // 
            this.description23.AutoSize = true;
            this.description23.Location = new System.Drawing.Point(369, 478);
            this.description23.Name = "description23";
            this.description23.Size = new System.Drawing.Size(49, 13);
            this.description23.TabIndex = 65;
            this.description23.Text = "Gen Grid";
            // 
            // buttonSetGenPower
            // 
            this.buttonSetGenPower.Location = new System.Drawing.Point(501, 474);
            this.buttonSetGenPower.Name = "buttonSetGenPower";
            this.buttonSetGenPower.Size = new System.Drawing.Size(15, 22);
            this.buttonSetGenPower.TabIndex = 69;
            this.buttonSetGenPower.Text = "<";
            this.buttonSetGenPower.UseVisualStyleBackColor = true;
            this.buttonSetGenPower.Click += new System.EventHandler(this.buttonSetGenPower_Click);
            // 
            // setGenVoltage
            // 
            this.setGenVoltage.Location = new System.Drawing.Point(424, 475);
            this.setGenVoltage.Name = "setGenVoltage";
            this.setGenVoltage.Size = new System.Drawing.Size(40, 20);
            this.setGenVoltage.TabIndex = 67;
            // 
            // description17
            // 
            this.description17.AutoSize = true;
            this.description17.Location = new System.Drawing.Point(621, 458);
            this.description17.Name = "description17";
            this.description17.Size = new System.Drawing.Size(65, 13);
            this.description17.TabIndex = 70;
            this.description17.Text = "Grid Voltage";
            // 
            // genPowerIn
            // 
            this.genPowerIn.AutoSize = true;
            this.genPowerIn.Location = new System.Drawing.Point(555, 479);
            this.genPowerIn.Name = "genPowerIn";
            this.genPowerIn.Size = new System.Drawing.Size(13, 13);
            this.genPowerIn.TabIndex = 72;
            this.genPowerIn.Text = "0";
            // 
            // description16
            // 
            this.description16.AutoSize = true;
            this.description16.Location = new System.Drawing.Point(524, 458);
            this.description16.Name = "description16";
            this.description16.Size = new System.Drawing.Size(71, 13);
            this.description16.TabIndex = 71;
            this.description16.Text = "Grid Power In";
            // 
            // genVoltage
            // 
            this.genVoltage.AutoSize = true;
            this.genVoltage.Location = new System.Drawing.Point(649, 479);
            this.genVoltage.Name = "genVoltage";
            this.genVoltage.Size = new System.Drawing.Size(13, 13);
            this.genVoltage.TabIndex = 73;
            this.genVoltage.Text = "0";
            // 
            // genMaxPower
            // 
            this.genMaxPower.AutoSize = true;
            this.genMaxPower.Location = new System.Drawing.Point(732, 479);
            this.genMaxPower.Name = "genMaxPower";
            this.genMaxPower.Size = new System.Drawing.Size(13, 13);
            this.genMaxPower.TabIndex = 77;
            this.genMaxPower.Text = "0";
            // 
            // description18
            // 
            this.description18.AutoSize = true;
            this.description18.Location = new System.Drawing.Point(711, 458);
            this.description18.Name = "description18";
            this.description18.Size = new System.Drawing.Size(82, 13);
            this.description18.TabIndex = 76;
            this.description18.Text = "Gen max Power";
            // 
            // mainsGridVoltage
            // 
            this.mainsGridVoltage.AutoSize = true;
            this.mainsGridVoltage.Location = new System.Drawing.Point(649, 521);
            this.mainsGridVoltage.Name = "mainsGridVoltage";
            this.mainsGridVoltage.Size = new System.Drawing.Size(13, 13);
            this.mainsGridVoltage.TabIndex = 83;
            this.mainsGridVoltage.Text = "0";
            // 
            // mainsGridPowerIn
            // 
            this.mainsGridPowerIn.AutoSize = true;
            this.mainsGridPowerIn.Location = new System.Drawing.Point(555, 522);
            this.mainsGridPowerIn.Name = "mainsGridPowerIn";
            this.mainsGridPowerIn.Size = new System.Drawing.Size(13, 13);
            this.mainsGridPowerIn.TabIndex = 82;
            this.mainsGridPowerIn.Text = "0";
            // 
            // description20
            // 
            this.description20.AutoSize = true;
            this.description20.Location = new System.Drawing.Point(524, 501);
            this.description20.Name = "description20";
            this.description20.Size = new System.Drawing.Size(71, 13);
            this.description20.TabIndex = 81;
            this.description20.Text = "Grid Power In";
            // 
            // description21
            // 
            this.description21.AutoSize = true;
            this.description21.Location = new System.Drawing.Point(621, 500);
            this.description21.Name = "description21";
            this.description21.Size = new System.Drawing.Size(65, 13);
            this.description21.TabIndex = 80;
            this.description21.Text = "Grid Voltage";
            // 
            // description24
            // 
            this.description24.AutoSize = true;
            this.description24.Location = new System.Drawing.Point(369, 522);
            this.description24.Name = "description24";
            this.description24.Size = new System.Drawing.Size(60, 13);
            this.description24.TabIndex = 78;
            this.description24.Text = "Mains  Grid";
            // 
            // buttonSetMainsPower
            // 
            this.buttonSetMainsPower.Location = new System.Drawing.Point(501, 517);
            this.buttonSetMainsPower.Name = "buttonSetMainsPower";
            this.buttonSetMainsPower.Size = new System.Drawing.Size(15, 22);
            this.buttonSetMainsPower.TabIndex = 88;
            this.buttonSetMainsPower.Text = "<";
            this.buttonSetMainsPower.UseVisualStyleBackColor = true;
            this.buttonSetMainsPower.Click += new System.EventHandler(this.buttonSetMainsPower_Click);
            // 
            // setGenPower
            // 
            this.setGenPower.Location = new System.Drawing.Point(463, 475);
            this.setGenPower.Name = "setGenPower";
            this.setGenPower.Size = new System.Drawing.Size(40, 20);
            this.setGenPower.TabIndex = 89;
            // 
            // setMainsGridVoltaage
            // 
            this.setMainsGridVoltaage.Location = new System.Drawing.Point(444, 518);
            this.setMainsGridVoltaage.Name = "setMainsGridVoltaage";
            this.setMainsGridVoltaage.Size = new System.Drawing.Size(59, 20);
            this.setMainsGridVoltaage.TabIndex = 90;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(421, 458);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 92;
            this.label6.Text = "Voltage";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(448, 502);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(43, 13);
            this.label7.TabIndex = 93;
            this.label7.Text = "Voltage";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(460, 458);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(60, 13);
            this.label13.TabIndex = 95;
            this.label13.Text = "Max Power";
            // 
            // maxChargePower
            // 
            this.maxChargePower.AutoSize = true;
            this.maxChargePower.Location = new System.Drawing.Point(389, 269);
            this.maxChargePower.Name = "maxChargePower";
            this.maxChargePower.Size = new System.Drawing.Size(0, 13);
            this.maxChargePower.TabIndex = 96;
            // 
            // desription23
            // 
            this.desription23.AutoSize = true;
            this.desription23.Location = new System.Drawing.Point(389, 269);
            this.desription23.Name = "desription23";
            this.desription23.Size = new System.Drawing.Size(74, 13);
            this.desription23.TabIndex = 97;
            this.desription23.Text = "Charge Power";
            // 
            // chargePowerBattery
            // 
            this.chargePowerBattery.Location = new System.Drawing.Point(469, 264);
            this.chargePowerBattery.Name = "chargePowerBattery";
            this.chargePowerBattery.Size = new System.Drawing.Size(73, 20);
            this.chargePowerBattery.TabIndex = 98;
            this.chargePowerBattery.ValueChanged += new System.EventHandler(this.chargePowerBattery_ValueChanged);
            // 
            // dischargePowerBatery
            // 
            this.dischargePowerBatery.Location = new System.Drawing.Point(469, 291);
            this.dischargePowerBatery.Name = "dischargePowerBatery";
            this.dischargePowerBatery.Size = new System.Drawing.Size(73, 20);
            this.dischargePowerBatery.TabIndex = 100;
            this.dischargePowerBatery.ValueChanged += new System.EventHandler(this.dischargePowerBatery_ValueChanged);
            // 
            // description25
            // 
            this.description25.AutoSize = true;
            this.description25.Location = new System.Drawing.Point(389, 294);
            this.description25.Name = "description25";
            this.description25.Size = new System.Drawing.Size(79, 13);
            this.description25.TabIndex = 99;
            this.description25.Text = "Discharge Limit";
            // 
            // maxBatteryPowerOut
            // 
            this.maxBatteryPowerOut.AutoSize = true;
            this.maxBatteryPowerOut.Location = new System.Drawing.Point(549, 269);
            this.maxBatteryPowerOut.Name = "maxBatteryPowerOut";
            this.maxBatteryPowerOut.Size = new System.Drawing.Size(168, 13);
            this.maxBatteryPowerOut.TabIndex = 101;
            this.maxBatteryPowerOut.Text = "Max Battery Power out in moment:";
            // 
            // batteryPowerOut
            // 
            this.batteryPowerOut.AutoSize = true;
            this.batteryPowerOut.Location = new System.Drawing.Point(549, 296);
            this.batteryPowerOut.Name = "batteryPowerOut";
            this.batteryPowerOut.Size = new System.Drawing.Size(94, 13);
            this.batteryPowerOut.TabIndex = 102;
            this.batteryPowerOut.Text = "Battery Power out:";
            // 
            // description30
            // 
            this.description30.AutoSize = true;
            this.description30.Location = new System.Drawing.Point(369, 550);
            this.description30.Name = "description30";
            this.description30.Size = new System.Drawing.Size(63, 13);
            this.description30.TabIndex = 103;
            this.description30.Text = "Invertor Out";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(717, 522);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(76, 13);
            this.label14.TabIndex = 106;
            this.label14.Text = "Invertor Status";
            // 
            // description29
            // 
            this.description29.AutoSize = true;
            this.description29.Location = new System.Drawing.Point(448, 549);
            this.description29.Name = "description29";
            this.description29.Size = new System.Drawing.Size(37, 13);
            this.description29.TabIndex = 105;
            this.description29.Text = "Power";
            // 
            // buttonSetPowerOut
            // 
            this.buttonSetPowerOut.Location = new System.Drawing.Point(500, 564);
            this.buttonSetPowerOut.Name = "buttonSetPowerOut";
            this.buttonSetPowerOut.Size = new System.Drawing.Size(15, 22);
            this.buttonSetPowerOut.TabIndex = 108;
            this.buttonSetPowerOut.Text = "<";
            this.buttonSetPowerOut.UseVisualStyleBackColor = true;
            this.buttonSetPowerOut.Click += new System.EventHandler(this.buttonSetPowerOut_Click);
            // 
            // setInvertorPowerOut
            // 
            this.setInvertorPowerOut.Location = new System.Drawing.Point(444, 565);
            this.setInvertorPowerOut.Name = "setInvertorPowerOut";
            this.setInvertorPowerOut.Size = new System.Drawing.Size(59, 20);
            this.setInvertorPowerOut.TabIndex = 107;
            // 
            // invertor_status
            // 
            this.invertor_status.AutoSize = true;
            this.invertor_status.Location = new System.Drawing.Point(717, 542);
            this.invertor_status.Name = "invertor_status";
            this.invertor_status.Size = new System.Drawing.Size(76, 13);
            this.invertor_status.TabIndex = 109;
            this.invertor_status.Text = "Invertor Status";
            // 
            // invertorVoltageOut
            // 
            this.invertorVoltageOut.AutoSize = true;
            this.invertorVoltageOut.Location = new System.Drawing.Point(553, 569);
            this.invertorVoltageOut.Name = "invertorVoltageOut";
            this.invertorVoltageOut.Size = new System.Drawing.Size(13, 13);
            this.invertorVoltageOut.TabIndex = 111;
            this.invertorVoltageOut.Text = "0";
            // 
            // description27
            // 
            this.description27.AutoSize = true;
            this.description27.Location = new System.Drawing.Point(525, 550);
            this.description27.Name = "description27";
            this.description27.Size = new System.Drawing.Size(65, 13);
            this.description27.TabIndex = 110;
            this.description27.Text = "Grid Voltage";
            // 
            // invertorPowerOut
            // 
            this.invertorPowerOut.AutoSize = true;
            this.invertorPowerOut.Location = new System.Drawing.Point(649, 572);
            this.invertorPowerOut.Name = "invertorPowerOut";
            this.invertorPowerOut.Size = new System.Drawing.Size(13, 13);
            this.invertorPowerOut.TabIndex = 113;
            this.invertorPowerOut.Text = "0";
            // 
            // description28
            // 
            this.description28.AutoSize = true;
            this.description28.Location = new System.Drawing.Point(614, 550);
            this.description28.Name = "description28";
            this.description28.Size = new System.Drawing.Size(77, 13);
            this.description28.TabIndex = 112;
            this.description28.Text = "Grid Power out";
            // 
            // maxPowerOut
            // 
            this.maxPowerOut.AutoSize = true;
            this.maxPowerOut.Location = new System.Drawing.Point(711, 618);
            this.maxPowerOut.Name = "maxPowerOut";
            this.maxPowerOut.Size = new System.Drawing.Size(77, 13);
            this.maxPowerOut.TabIndex = 114;
            this.maxPowerOut.Text = "MaxPowerOut:";
            // 
            // powerSale
            // 
            this.powerSale.AutoSize = true;
            this.powerSale.Location = new System.Drawing.Point(711, 598);
            this.powerSale.Name = "powerSale";
            this.powerSale.Size = new System.Drawing.Size(70, 13);
            this.powerSale.TabIndex = 115;
            this.powerSale.Text = "Amount Sold:";
            // 
            // haveInvertorOut
            // 
            this.haveInvertorOut.AutoSize = true;
            this.haveInvertorOut.Location = new System.Drawing.Point(392, 568);
            this.haveInvertorOut.Name = "haveInvertorOut";
            this.haveInvertorOut.Size = new System.Drawing.Size(15, 14);
            this.haveInvertorOut.TabIndex = 116;
            this.haveInvertorOut.UseVisualStyleBackColor = true;
            // 
            // setBatteryCapacity
            // 
            this.setBatteryCapacity.Location = new System.Drawing.Point(510, 236);
            this.setBatteryCapacity.Name = "setBatteryCapacity";
            this.setBatteryCapacity.Size = new System.Drawing.Size(73, 20);
            this.setBatteryCapacity.TabIndex = 117;
            // 
            // startSimulation
            // 
            this.startSimulation.AutoSize = true;
            this.startSimulation.Location = new System.Drawing.Point(392, 618);
            this.startSimulation.Name = "startSimulation";
            this.startSimulation.Size = new System.Drawing.Size(15, 14);
            this.startSimulation.TabIndex = 119;
            this.startSimulation.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(356, 598);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(80, 13);
            this.label9.TabIndex = 118;
            this.label9.Text = "Start Simulation";
            // 
            // buttonClearError
            // 
            this.buttonClearError.Location = new System.Drawing.Point(372, 658);
            this.buttonClearError.Name = "buttonClearError";
            this.buttonClearError.Size = new System.Drawing.Size(46, 23);
            this.buttonClearError.TabIndex = 120;
            this.buttonClearError.Text = "Clear";
            this.buttonClearError.UseVisualStyleBackColor = true;
            this.buttonClearError.Click += new System.EventHandler(this.buttonClearError_Click);
            // 
            // maximumCharge
            // 
            this.maximumCharge.Location = new System.Drawing.Point(469, 344);
            this.maximumCharge.Name = "maximumCharge";
            this.maximumCharge.Size = new System.Drawing.Size(73, 20);
            this.maximumCharge.TabIndex = 122;
            // 
            // minimalCharge
            // 
            this.minimalCharge.Location = new System.Drawing.Point(469, 317);
            this.minimalCharge.Name = "minimalCharge";
            this.minimalCharge.Size = new System.Drawing.Size(73, 20);
            this.minimalCharge.TabIndex = 121;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(389, 347);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(64, 13);
            this.label10.TabIndex = 124;
            this.label10.Text = "Max Charge";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(389, 321);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 123;
            this.label11.Text = "Min Charge";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(549, 321);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(111, 13);
            this.label12.TabIndex = 126;
            this.label12.Text = "Maintaining Charge lvl";
            // 
            // maintainingCharge
            // 
            this.maintainingCharge.Location = new System.Drawing.Point(555, 344);
            this.maintainingCharge.Name = "maintainingCharge";
            this.maintainingCharge.Size = new System.Drawing.Size(73, 20);
            this.maintainingCharge.TabIndex = 125;
            this.maintainingCharge.ValueChanged += new System.EventHandler(this.maintainingCharge_ValueChanged);
            // 
            // useMainsGrid
            // 
            this.useMainsGrid.AutoSize = true;
            this.useMainsGrid.Location = new System.Drawing.Point(612, 617);
            this.useMainsGrid.Name = "useMainsGrid";
            this.useMainsGrid.Size = new System.Drawing.Size(15, 14);
            this.useMainsGrid.TabIndex = 130;
            this.useMainsGrid.UseVisualStyleBackColor = true;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(555, 598);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(128, 13);
            this.label15.TabIndex = 129;
            this.label15.Text = "Use MainsGrid for Charge";
            // 
            // useGenerator
            // 
            this.useGenerator.AutoSize = true;
            this.useGenerator.Location = new System.Drawing.Point(486, 617);
            this.useGenerator.Name = "useGenerator";
            this.useGenerator.Size = new System.Drawing.Size(15, 14);
            this.useGenerator.TabIndex = 128;
            this.useGenerator.UseVisualStyleBackColor = true;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(442, 598);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(103, 13);
            this.label16.TabIndex = 127;
            this.label16.Text = "Use Gen For charge";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(385, 193);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 13);
            this.label17.TabIndex = 131;
            this.label17.Text = "Battery Type";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(555, 193);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(71, 13);
            this.label18.TabIndex = 132;
            this.label18.Text = "Battery Count";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(17, 374);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(51, 13);
            this.label19.TabIndex = 133;
            this.label19.Text = "Registers";
            // 
            // Simulator
            // 
            this.ClientSize = new System.Drawing.Size(839, 963);
            this.Controls.Add(this.label19);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.label17);
            this.Controls.Add(this.useMainsGrid);
            this.Controls.Add(this.label15);
            this.Controls.Add(this.useGenerator);
            this.Controls.Add(this.label16);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.maintainingCharge);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.maximumCharge);
            this.Controls.Add(this.minimalCharge);
            this.Controls.Add(this.buttonClearError);
            this.Controls.Add(this.startSimulation);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.setBatteryCapacity);
            this.Controls.Add(this.haveInvertorOut);
            this.Controls.Add(this.powerSale);
            this.Controls.Add(this.buttonSetMainsPower);
            this.Controls.Add(this.maxPowerOut);
            this.Controls.Add(this.invertorPowerOut);
            this.Controls.Add(this.description28);
            this.Controls.Add(this.invertorVoltageOut);
            this.Controls.Add(this.description27);
            this.Controls.Add(this.invertor_status);
            this.Controls.Add(this.buttonSetPowerOut);
            this.Controls.Add(this.setInvertorPowerOut);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.description29);
            this.Controls.Add(this.description30);
            this.Controls.Add(this.batteryPowerOut);
            this.Controls.Add(this.maxBatteryPowerOut);
            this.Controls.Add(this.dischargePowerBatery);
            this.Controls.Add(this.description25);
            this.Controls.Add(this.chargePowerBattery);
            this.Controls.Add(this.desription23);
            this.Controls.Add(this.maxChargePower);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.setMainsGridVoltaage);
            this.Controls.Add(this.setGenPower);
            this.Controls.Add(this.mainsGridVoltage);
            this.Controls.Add(this.mainsGridPowerIn);
            this.Controls.Add(this.description20);
            this.Controls.Add(this.description21);
            this.Controls.Add(this.description24);
            this.Controls.Add(this.genMaxPower);
            this.Controls.Add(this.description18);
            this.Controls.Add(this.genVoltage);
            this.Controls.Add(this.genPowerIn);
            this.Controls.Add(this.description16);
            this.Controls.Add(this.description17);
            this.Controls.Add(this.buttonSetGenPower);
            this.Controls.Add(this.setGenVoltage);
            this.Controls.Add(this.description23);
            this.Controls.Add(this.possibleGrid2Voltage);
            this.Controls.Add(this.possibleGrid1Voltage);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.buttonSetnputSolPower2);
            this.Controls.Add(this.buttonSetnputSolPower1);
            this.Controls.Add(this.MaxGridPower);
            this.Controls.Add(this.GridVoltageLimit);
            this.Controls.Add(this.MaxGridPowerSum);
            this.Controls.Add(this.solarGridVoltage2);
            this.Controls.Add(this.solarGridVoltage1);
            this.Controls.Add(this.solarGridPower2);
            this.Controls.Add(this.solarGridPower1);
            this.Controls.Add(this.checkBoxSolar2);
            this.Controls.Add(this.checkBoxSolar1);
            this.Controls.Add(this.description13);
            this.Controls.Add(this.setSolarGridPower2);
            this.Controls.Add(this.setSolarGridPower1);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.description11);
            this.Controls.Add(this.description9);
            this.Controls.Add(this.description10);
            this.Controls.Add(this.description8);
            this.Controls.Add(this.maxPosiibleSolarGridPower);
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
            ((System.ComponentModel.ISupportInitialize)(this.chargePowerBattery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dischargePowerBatery)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.setBatteryCapacity)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maximumCharge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.minimalCharge)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.maintainingCharge)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
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
        private System.Windows.Forms.Label maxPosiibleSolarGridPower;
        private System.Windows.Forms.Label description8;
        private System.Windows.Forms.Label description10;
        private System.Windows.Forms.Label description9;
        private System.Windows.Forms.Label description11;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox setSolarGridPower1;
        private System.Windows.Forms.TextBox setSolarGridPower2;
        private System.Windows.Forms.Label description13;
        private System.Windows.Forms.CheckBox checkBoxSolar1;
        private System.Windows.Forms.CheckBox checkBoxSolar2;
        private System.Windows.Forms.Label solarGridPower1;
        private System.Windows.Forms.Label solarGridPower2;
        private System.Windows.Forms.Label solarGridVoltage2;
        private System.Windows.Forms.Label solarGridVoltage1;
        private System.Windows.Forms.Label MaxGridPowerSum;
        private System.Windows.Forms.Label GridVoltageLimit;
        private System.Windows.Forms.Label MaxGridPower;
        private System.Windows.Forms.Button buttonSetnputSolPower1;
        private System.Windows.Forms.Button buttonSetnputSolPower2;
        private System.Windows.Forms.Label possibleGrid2Voltage;
        private System.Windows.Forms.Label possibleGrid1Voltage;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label description23;
        private System.Windows.Forms.Button buttonSetGenPower;
        private System.Windows.Forms.TextBox setGenVoltage;
        private System.Windows.Forms.Label description17;
        private System.Windows.Forms.Label genPowerIn;
        private System.Windows.Forms.Label description16;
        private System.Windows.Forms.Label genVoltage;
        private System.Windows.Forms.Label genMaxPower;
        private System.Windows.Forms.Label description18;
        private System.Windows.Forms.Label mainsGridVoltage;
        private System.Windows.Forms.Label mainsGridPowerIn;
        private System.Windows.Forms.Label description20;
        private System.Windows.Forms.Label description21;
        private System.Windows.Forms.Label description24;
        private System.Windows.Forms.Button buttonSetMainsPower;
        private System.Windows.Forms.TextBox setGenPower;
        private System.Windows.Forms.TextBox setMainsGridVoltaage;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Label maxChargePower;
        private System.Windows.Forms.Label desription23;
        private System.Windows.Forms.NumericUpDown chargePowerBattery;
        private System.Windows.Forms.NumericUpDown dischargePowerBatery;
        private System.Windows.Forms.Label description25;
        private System.Windows.Forms.Label maxBatteryPowerOut;
        private System.Windows.Forms.Label batteryPowerOut;
        private System.Windows.Forms.Label description30;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label description29;
        private System.Windows.Forms.Button buttonSetPowerOut;
        private System.Windows.Forms.TextBox setInvertorPowerOut;
        private System.Windows.Forms.Label invertor_status;
        private System.Windows.Forms.Label invertorVoltageOut;
        private System.Windows.Forms.Label description27;
        private System.Windows.Forms.Label invertorPowerOut;
        private System.Windows.Forms.Label description28;
        private System.Windows.Forms.Label maxPowerOut;
        private System.Windows.Forms.Label powerSale;
        private System.Windows.Forms.CheckBox haveInvertorOut;
        private System.Windows.Forms.NumericUpDown setBatteryCapacity;
        private System.Windows.Forms.CheckBox startSimulation;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button buttonClearError;
        private System.Windows.Forms.NumericUpDown maximumCharge;
        private System.Windows.Forms.NumericUpDown minimalCharge;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.CheckBox useMainsGrid;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.CheckBox useGenerator;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.NumericUpDown maintainingCharge;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label19;
    }
}
