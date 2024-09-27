using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using NModbus;
using NModbus.Serial;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Invertor_sim
{
    public partial class Simulator : Form
    {
        private MainForm main;
        private SerialPort serialPort;
        private System.Windows.Forms.Timer portCheckTimer;
        private System.Windows.Forms.Timer clockTimer;
        private string[] previousPorts;
        private IModbusSerialMaster modbusMaster;
        private IModbusFactory modbusFactory;
        private InverterRegister[] registers;
        private DateTime currentTime;
        private Battery selectedBattery;

        private SolarPanel selectedPanelGrid1;
        private SolarPanel selectedPanelGrid2;

        private int PanelCountGrid1 = 0;
        private int PanelCountGrid2 = 0;

        private int MaxPanelCountGrid1 = 0;
        private int MaxPanelCountGrid2 = 0;

        private const int InvertorPowerUsage = 7;//5-7%

        private  bool useFileSourse = false;



        private Battery[] batteries = new Battery[]
        {
            new Battery("No battery", 0, 40, 60, 0),
            new Battery("Pylontech US5000", 100, 54, 42, 80),
            new Battery("Deye BOS-GM5.1", 100, 57, 46, 50)
        };
        private SolarPanel[] solarPanels1 = new SolarPanel[]
        {
            new SolarPanel("No Solar Plane", 0, 0, 0),
            new SolarPanel("ANDO QN-550HT", 50.03f,12.99f, 42.34f),
            new SolarPanel("RISEN 400W RSM40-8-400M", 41.3f,11.64f, 34.39f),
            new SolarPanel("V-TAC AU410-27V-MH", 37.45f,13.04f, 31.46f),
            
        };
        private SolarPanel[] solarPanels2 = new SolarPanel[]
        {
            new SolarPanel("No Solar Plane", 0, 0, 0),
            new SolarPanel("ANDO QN-550HT", 50.03f,12.99f, 42.34f),
            new SolarPanel("RISEN 400W RSM40-8-400M", 41.3f,11.64f, 34.39f),
            new SolarPanel("V-TAC AU410-27V-MH", 37.45f,13.04f, 31.46f),

        };

        private enum Error
        {
            OverLimit_Error = 0,
            SolarGridInput_exceeded = 1,
            Output_power_exceeded = 2,
            Battery_has_discharged_to_specified_minimum = 3,
            Infussicient_power_at_generator_input = 4,

        }

        public Simulator(MainForm main)
        {
            InitializeComponent();
            this.main = main;

            modbusFactory = new ModbusFactory();

            registers = new InverterRegister[]
            {
                new InverterRegister(0, 0f, "V", 0, 62, "Battery Voltage"),
                new InverterRegister(1, 0f, "%", 0, 100, "State of Charge (SOC)"),
                new InverterRegister(2, 1, "Blocks", 0, 50, "Battery Blocks"),
                new InverterRegister(3, 0, "Ah", 0, 500, "Nominal Capacity"),
                new InverterRegister(4, 54, "V", 40, 60, "Max Voltage"),
                new InverterRegister(5, 42, "V", 40, 60, "Min Voltage"),
                new InverterRegister(6, 0, "A", 0, 135, "Nominal Charge current"),
                new InverterRegister(7, 0, "A", 0, 190, "Nominal discharge current"),
                new InverterRegister(8, 0, "W", 0, 450400, "Max Power Capacity"),
                new InverterRegister(9, 0, "W", 0, 0, "Battery Power"),
                new InverterRegister(10, 0, "Text", 0, 22, "OverLimit Error"),
                new InverterRegister(11, 0, "Error", 0, 10, "Error"),
                new InverterRegister(12, 0, "W", 0, 16000, "InputGrid Power"),//Мережевий вход
                new InverterRegister(13, 0, "V", 165, 290, "InputGrid Voltage"),
                new InverterRegister(14, 0, "W", 0, 16000, "InputGenerator Power"),
                new InverterRegister(15, 0, "V", 165, 290, "InputGenerator Voltage"),
                new InverterRegister(16, 0, "W", 0, 7800, "InputSolarPower"),//all solar grid input
                new InverterRegister(17, 0, "V", 0, 500, "InputSolarGrid1 Voltage"),
                new InverterRegister(18, 0, "V", 0, 500, "InputSolarGrid2 Voltage"),
                new InverterRegister(19, 0, "W", 0, 5525, "InputSolarGrid1 Power"),
                new InverterRegister(20, 0, "W", 0, 5525, "InputSolarGrid2 Power"),
                new InverterRegister(21, 0, "%", 0, 100, "Minimum Battery discharge"),
                new InverterRegister(22, 0, "A", 0, 135, "Real Charge current"),
                new InverterRegister(23, 0, "A", 0, 190, "Real discharge current"),
                new InverterRegister(24, 0, "V", 125, 425, "MPPT Voltage"),
            };
            UpdateRegisterDisplay();
            //timer
            portCheckTimer = new System.Windows.Forms.Timer();
            portCheckTimer.Interval = 5000;
            portCheckTimer.Tick += new EventHandler(CheckAvailablePorts);
            portCheckTimer.Start();
            //ports
            previousPorts = SerialPort.GetPortNames();
            comboBoxPorts.Items.Add("Port Not Selected");
            comboBoxPorts.Items.AddRange(previousPorts);
            //BaudRate
            comboBoxBaudRate.Items.AddRange(new object[] { "9600", "19200", "38400", "57600", "115200" });
            comboBoxBaudRate.SelectedIndex = 0;
            comboBoxPorts.SelectedIndexChanged += ComboBoxPorts_SelectedIndexChanged;

            // Initialize and start clock timer
            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000; // Update every second
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();

            batteryType.SelectedIndexChanged += ComboBoxBattery_SelectedIndexChanged;
            batteryType.DataSource = batteries;
            batteryType.DisplayMember = "Name";

            panelTypeGrid1.SelectedIndexChanged += PanelTypeGrid1_SelectedIndexChanged;
            panelTypeGrid1.DataSource = solarPanels1;
            panelTypeGrid1.DisplayMember = "Name";

            panelTypeGrid2.SelectedIndexChanged += PanelTypeGrid2_SelectedIndexChanged;
            panelTypeGrid2.DataSource = solarPanels2;
            panelTypeGrid2.DisplayMember = "Name";

        }

        
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            currentTime = currentTime.AddSeconds(1); // Increment current time by one second
            labelCurrentTime.Text = currentTime.ToString("HH:mm:ss"); // Update time label
            UpdateRegisterDisplay();
            
            UpdateFormLabels();


            FindError();
            ShowError();
        }

        private void UpdateFormLabels()
        {

            UpdateBatteryInfo();
            UpdateGridsPanelCount();
            UpdateGridsPanelMaxPower();
            ShowInvertorInfo();

            UpdateGrid1PanelCount();
            UpdateGrid2PanelCount();

            UpdateSolarGridSumInfo();
            UpdateSolarGridInputInfo();

            if(!useFileSourse)
            {
                UpdateVoltageSolarGrid();
            }

        }
        
        private void ComboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBoxPorts.SelectedItem.ToString() == "Port Not Selected")
            {
                if (serialPort != null && serialPort.IsOpen)
                {
                    serialPort.Close();
                    serialPort = null;
                }
                labelStatus.Text = "Port Not Selected";
                return;
            }

            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.DataReceived -= DataReceivedHandler;
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    labelStatus.Text = "Error closing port: " + ex.Message;
                }
            }

            string selectedPort = comboBoxPorts.SelectedItem.ToString();
            int baudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());

            try
            {
                serialPort = new SerialPort(selectedPort, baudRate, Parity.None, 8, StopBits.One);
                serialPort.Open();

                modbusMaster = modbusFactory.CreateRtuMaster(serialPort);
                serialPort.DataReceived += DataReceivedHandler;

                labelStatus.Text = "Connected to " + selectedPort;
            }
            catch (UnauthorizedAccessException)
            {
                labelStatus.Text = "Error: Port is already in use.";
            }
            catch (IOException)
            {
                labelStatus.Text = "Error: Failed to open port.";
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null || !serialPort.IsOpen)
                return;

            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            PrintDataToConsole(buffer);

            Invoke(new Action(() =>
            {
                listBoxReceivedCommands.Items.Add("Received: " + BitConverter.ToString(buffer));
            }));

            try
            {
                var request = modbusMaster.ReadHoldingRegisters(1, 0, (ushort)registers.Length);
                ProcessRequest(request);
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    listBoxReceivedCommands.Items.Add("Error: " + ex.Message);
                }));
            }
        }

        private void ProcessRequest(ushort[] request)
        {
            for (int i = 0; i < request.Length && i < registers.Length; i++)
            {
                registers[i].Value = request[i] / 100f;
            }

            byte[] byteArray = registers.SelectMany(r => BitConverter.GetBytes(r.GetRegisterData())).ToArray();

            Invoke(new Action(() =>
            {
                listBoxSentCommands.Items.Add("Sent: " + BitConverter.ToString(byteArray));
            }));
        }



        //Form methods
        //////////////
        //////////////


        private void PanelTypeGrid1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPanelGrid1 = (SolarPanel)panelTypeGrid1.SelectedItem;
            UpdateSolarGridPanelCount1();
        }

        private void PanelTypeGrid2_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPanelGrid2 = (SolarPanel)panelTypeGrid2.SelectedItem;
            UpdateSolarGridPanelCount2();
        }

        private void ComboBoxBattery_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBattery = (Battery)batteryType.SelectedItem;
            UpdateBatteryRegisters();
            UpdateMaxPowerCapacity();
        }
        private void numericUpDownBatteryBlocks_ValueChanged(object sender, EventArgs e)
        {
            UpdateBatteryBlockCount((int)batteryElemntCount.Value);
            UpdateMaxPowerCapacity();

        }
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.DataReceived -= DataReceivedHandler;
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    labelStatus.Text = "Error closing port: " + ex.Message;
                }
            }
            Application.Exit();
        }

        private void buttonSyncTime_Click(object sender, EventArgs e)
        {
            currentTime = DateTime.Now; // Synchronize current time
            labelCurrentTime.Text = currentTime.ToString("HH:mm:ss"); // Update time label
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (main == null || main.IsDisposed)
            {
                MainForm mainForm = new MainForm();
                mainForm.AddSimulatorForm(this);
                main = mainForm;
            }
            main.Show();
            main.Location = this.Location;
        }
        private void buttonSetnputSolPower1_Click(object sender, EventArgs e)
        {
            if (int.TryParse(setSolarGridPower1.Text, out int gridPower))
            {
                GetSolarGridPower(gridPower, registers[19], registers[20].Value, registers[16].MaxValue);
            }
        }

        private void buttonSetnputSolPower2_Click(object sender, EventArgs e)
        {
            if (int.TryParse(setSolarGridPower2.Text, out int gridPower))
            {
                GetSolarGridPower(gridPower, registers[20], registers[19].Value, registers[16].MaxValue);
            }
        }
        private void UpdateVoltageSolarGrid()
        {
            if (int.TryParse(setSolarGridPower1.Text, out int gridPower1))
            {
                SetVoltageGrid1(gridPower1);
            }
            if (int.TryParse(setSolarGridPower2.Text, out int gridPower2))
            {
                SetVoltageGrid2(gridPower2);
            }
        }
        private void SetVoltageGrid1(int gridPower) {

            registers[17].Value = CalculateMPPT(gridPower, PanelCountGrid1, selectedPanelGrid1.MaxWorkСurrent, selectedPanelGrid1.MaxWorkVoltage).Voltage;
        }
        private void SetVoltageGrid2(int gridPower)
        {

            registers[18].Value = CalculateMPPT(gridPower, PanelCountGrid2, selectedPanelGrid2.MaxWorkСurrent, selectedPanelGrid2.MaxWorkVoltage).Voltage;
        }
        private void GetSolarGridPower(int gridPower, InverterRegister solGridPowerRegister, float secondSolGridPowerRegister, float maxSolGridInputSum)
        {
            if (gridPower < 0)
                return;

            UpdateVoltageSolarGrid();
            if (gridPower > solGridPowerRegister.MaxValue || !SolarGrid1CanGenerate())
            {
                solGridPowerRegister.Value = 0;
                return;
            }
            solGridPowerRegister.Value = (secondSolGridPowerRegister + gridPower <= maxSolGridInputSum)
            ? gridPower
            : maxSolGridInputSum - secondSolGridPowerRegister;
        }


        private void buttonConfirmTime_Click(object sender, EventArgs e)
        {
            if (TimeSpan.TryParse(textBoxSetTime.Text, out TimeSpan newTime))
            {
                currentTime = DateTime.Today.Add(newTime); // Задаємо новий час
                labelCurrentTime.Text = currentTime.ToString("HH:mm:ss"); // Оновлюємо відображення
            }
            else
            {
                MessageBox.Show("Invalid time format. Please enter in HH:mm:ss format.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void countPanelGrid1_ValueChanged(object sender, EventArgs e)
        {
            UpdateGrid1PanelCount();
        }
        private void countPanelGrid2_ValueChanged(object sender, EventArgs e)
        {
            UpdateGrid2PanelCount();
        }
        private void checkBoxSolar1_MouseDown(object sender, MouseEventArgs e)
        {
            ((CheckBox)sender).Enabled = false;
            ((CheckBox)sender).Enabled = true;
        }

        private void checkBoxSolar2_MouseDown(object sender, MouseEventArgs e)
        {
            ((CheckBox)sender).Enabled = false;
            ((CheckBox)sender).Enabled = true;
        }
        private void BatteryPower_Click(object sender, EventArgs e)
        {
            // Проверка корректности введённого значения
            if (int.TryParse(battaryPowerInput.Text, out int batteryPower))
            {
                // Если значение корректное, устанавливаем мощность батареи
                SetBattaryPower(batteryPower);
            }
            else
            {
                // Если ввод некорректен, выводим сообщение об ошибке
                MessageBox.Show("Please enter a valid number for battery power.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        //Add data to form

       
        private void UpdateRegisterDisplay()//Замінити на форму з інформацією про регістри
        {
            listBoxBatteryInfo.Items.Clear(); // Очищення попереднього тексту
            foreach (var register in registers)
            {
                listBoxBatteryInfo.Items.Add($"{register.Description}: {register.Value} {register.Unit}");
            }
        }
        private void UpdateSolarGridInputInfo()
        {

            solarGridVoltage1.Text = registers[17].Value.ToString();
            solarGridVoltage2.Text = registers[18].Value.ToString();

            if (SolarGrid1CanGenerate())
            {
                checkBoxSolar1.Checked = true;
                solarGridPower1.Text = registers[19].Value.ToString();
            }
            else
            {
                checkBoxSolar1.Checked = false;
                solarGridPower1.Text = "0";
            }

            if (SolarGrid2CanGenerate())
            {
                checkBoxSolar2.Checked = true;
                solarGridPower2.Text = registers[20].Value.ToString();
            }
            else
            {
                checkBoxSolar2.Checked = false;
                solarGridPower2.Text = "0";
            }

        }

        private bool SolarGrid1CanGenerate()//voltage must be between lowValue and hight value registr MPPT Voltage
        {
            return registers[24].MinValue < registers[17].Value && registers[17].Value <= registers[24].MaxValue;
        }
        private bool SolarGrid2CanGenerate()//voltage must be between lowValue and hight value registr MPPT Voltage
        {
            return registers[24].MinValue < registers[18].Value && registers[18].Value <= registers[24].MaxValue;
        }
        private void UpdateSolarGridSumInfo()
        {
            registers[16].Value = registers[19].Value + registers[20].Value;
        }
        private void UpdateGridsPanelCount()
        {
            labelGrid1PanelCount.Text = PanelCountGrid1.ToString();
            labelGrid2PanelCount.Text = PanelCountGrid2.ToString();
        }

        private void UpdateGridsPanelMaxPower()
        {
            var Grid1MaxInput = GetMaxSolarGridPower1();
            var Grid2MaxInput = GetMaxSolarGridPower2();

            grid1MaxPower.Text = Grid1MaxInput.ToString();
            grid2MaxPower.Text = Grid2MaxInput.ToString();
            maxPosiibleSolarGridPower.Text = (Grid1MaxInput + Grid2MaxInput).ToString();
        }
        private void ShowInvertorInfo()
        {
            GridVoltageLimit.Text = "Grid Voltage Generation Limit: " + registers[24].MinValue + "-" + registers[24].MaxValue + " V";
            MaxGridPowerSum.Text = "Max Grid Power Sum: " + registers[16].MaxValue;
            MaxGridPower.Text = "Max Grid Power: " + registers[19].MaxValue;
        }
        private void UpdateBatteryBlockCount(int blockCount)
        {
            registers[2].Value = blockCount;

        }
        private float GetMaxSolarGridPower1()
        {
            return PanelCountGrid1 * selectedPanelGrid1.MaxWorkСurrent * selectedPanelGrid1.MaxWorkVoltage;
        }
        private float GetMaxSolarGridPower2()
        {
            return PanelCountGrid2 * selectedPanelGrid2.MaxWorkСurrent * selectedPanelGrid2.MaxWorkVoltage;
        }
        private void PrintDataToConsole(byte[] data)
        {
            Console.WriteLine("Received Packet:");
            foreach (byte b in data)
            {
                Console.Write($"0x{b:X2} ");
            }
            Console.WriteLine();
        }

        private void ShowError()
        {
            if (registers[11].Value != 0)
            {
                var error = registers[(int)registers[11].Value];
                var errorRegis = registers[(int)error.Value];
                label_Error.Text = "Error " + error.Description + " for " + errorRegis.Description;
            }
        }

        private void SetBattaryPower(int value)
        {
            registers[9].Value = value;
        }

        //register changes
        private void UpdateBatteryInfo()
        {
            if (registers[4].Value == registers[5].Value && registers[3].Value != 0 && registers[2].Value != 0)
            {
                registers[0].Value = registers[9].Value / registers[3].Value / registers[2].Value;
                registers[1].Value = (registers[0].Value - registers[5].Value) / (registers[4].Value - registers[5].Value) * 100; // % заряду батареї
            }
            

        }

        private void UpdateMaxPowerCapacity()
        {
            registers[8].Value = registers[4].Value * registers[2].Value * registers[3].Value; // Макс запас енергії
            registers[9].MaxValue = registers[8].Value;
        }
        private void UpdateBatteryRegisters()
        {
            registers[3].Value = selectedBattery.NominalCapacity; // Ємність
            registers[4].Value = selectedBattery.MaxVoltage; // Максимальний вольтаж
            registers[5].Value = selectedBattery.MinVoltage; // Мінімальний вольтаж
            registers[6].Value = selectedBattery.NominalChargeDischargeCurrent; // Ток заряду/розряду
            registers[7].Value = selectedBattery.NominalChargeDischargeCurrent;
            

        }
        
        private void UpdateSolarGridPanelCount1()
        {
            MaxPanelCountGrid1 = UpdateSolarGrid1Data(selectedPanelGrid1, registers[17].MaxValue, registers[24].MaxValue);
        }
        

        private void UpdateSolarGridPanelCount2()
        {
            MaxPanelCountGrid2 = UpdateSolarGrid1Data(selectedPanelGrid2, registers[18].MaxValue, registers[24].MaxValue);
        }
        
        private bool UpdateSolarGrid1PanelCount(NumericUpDown countPanelGrid, int  maxPanelCountGrid, SolarPanel selectedPanelGrid, float maxGridPower, float minGridVoltage, float maxSecondGridPower, float maxSolGridInputSum)
        {
            int formPanelCount = (int)countPanelGrid.Value;
            if(formPanelCount <= maxPanelCountGrid)
            {
                var gridPower = formPanelCount * selectedPanelGrid.MaxWorkVoltage * selectedPanelGrid.MaxWorkСurrent;
                if (maxGridPower >= gridPower && maxSolGridInputSum >= gridPower + maxSecondGridPower && minGridVoltage < formPanelCount * selectedPanelGrid.MaxWorkVoltage)
                {
                    return true;
                }
            }
            return false;

        }
        private void UpdateGrid1PanelCount()
        {
            if (!UpdateSolarGrid1PanelCount(countPanelGrid1,
                                            MaxPanelCountGrid1,
                                            selectedPanelGrid1,
                                            registers[19].MaxValue,
                                            registers[24].MinValue,
                                            selectedPanelGrid2.MaxWorkСurrent * PanelCountGrid2 * selectedPanelGrid2.MaxWorkVoltage,
                                            registers[16].MaxValue))
            {
                PanelCountGrid1 = 0;
            }
            else
            {
                PanelCountGrid1 = (int)countPanelGrid1.Value;
            }
        }
        private void UpdateGrid2PanelCount()
        {
            
            if (!UpdateSolarGrid1PanelCount(countPanelGrid2,
                                           MaxPanelCountGrid2,
                                           selectedPanelGrid2,
                                           registers[20].MaxValue,
                                           registers[24].MinValue,
                                           selectedPanelGrid1.MaxWorkСurrent * PanelCountGrid1 * selectedPanelGrid1.MaxWorkVoltage,
                                           registers[16].MaxValue))
            {
                PanelCountGrid2 = 0;
            }
            else
            {
                PanelCountGrid2 = (int)countPanelGrid2.Value;
            }
        }

        private void CheckAvailablePorts(object sender, EventArgs e)
        {
            string[] currentPorts = SerialPort.GetPortNames();
            if (!currentPorts.SequenceEqual(previousPorts))
            {
                previousPorts = currentPorts;
                comboBoxPorts.Items.Clear();
                comboBoxPorts.Items.Add("Port Not Selected");
                comboBoxPorts.Items.AddRange(currentPorts);

                if (serialPort != null && serialPort.IsOpen && !currentPorts.Contains(serialPort.PortName))
                {
                    try
                    {
                        serialPort.DataReceived -= DataReceivedHandler;
                        serialPort.Close();
                        labelStatus.Text = "Disconnected";
                    }
                    catch (Exception ex)
                    {
                        labelStatus.Text = "Error closing port: " + ex.Message;
                    }
                }
            }
        }
        public (float Voltage, float Current) CalculateMPPT(float power, int panelCount, float maxPanelCurrent, float maxPanelVoltage) //перевірити правильність формули
        {
            if (panelCount == 0 || maxPanelCurrent == 0 || maxPanelVoltage == 0)
                return (0, 0);
            // Максимальна потужність однієї панелі
            float maxPowerPerPanel = maxPanelVoltage * maxPanelCurrent;

            // Загальна максимальна потужність від усіх панелей
            float totalMaxPower = maxPowerPerPanel * panelCount;

            // Якщо задана потужність більше максимальної, обмежуємо її
            if (power > totalMaxPower)
            {
                power = totalMaxPower;
            }

            // Знаходимо відносне навантаження від максимальної потужності (частка потужності)
            float powerRatio = power / totalMaxPower;

            // Використовуємо пропорцію для визначення напруги та струму
            float voltage = maxPanelVoltage * (float)Math.Sqrt(powerRatio) * panelCount;  // Пропорційне зменшення напруги
            float current = maxPanelCurrent * powerRatio * panelCount;              // Пропорційне зменшення струму

            return (voltage, current);
        }
        private int UpdateSolarGrid1Data(SolarPanel panel, float maxVoltage, float maxMPPT)//
        {
            int MaxPanelCountByVoltage = 0;//максимальна кількість панелей в межах вольтажу при холостому ході
            int MaxPanelCountByMPPT = 0;//маскимальна кількість панеелй з якими інвертор може генерувати

            if (panel.OpenCircuitVoltage != 0)
            {
                MaxPanelCountByVoltage = (int)(maxVoltage / panel.OpenCircuitVoltage);
                MaxPanelCountByMPPT = (int)(maxMPPT / panel.MaxWorkVoltage);

            }

            if (MaxPanelCountByVoltage == MaxPanelCountByMPPT)
                return MaxPanelCountByMPPT;
            else if (MaxPanelCountByVoltage > MaxPanelCountByMPPT)
                return MaxPanelCountByMPPT;
            else
                return MaxPanelCountByVoltage;

        }
        //Errors check


        private void FindError()
        {
            ChрeckRegisterOverLimitError();
        }


        private void ChрeckRegisterOverLimitError()
        {
            foreach (var register in registers)
            {
                if (register.Value > register.MaxValue)
                {
                    register.Value = register.MaxValue;
                    registers[10].Value = register.RegisterNumber;//Привязать к RegisterNumber
                    registers[11].Value = registers[10].RegisterNumber;//поиск ошибки OverLimit, если есть, добавление в регистр ошибок, регистр RegisterOverLimitError
                    return;
                }
            }

        }

        
    }
}
