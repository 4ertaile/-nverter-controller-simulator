using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using NModbus;
using NModbus.Serial;

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

        private Battery[] batteries = new Battery[]
        {
            new Battery("No battery", 0, 40, 60, 0),
            new Battery("Pylontech US5000", 100, 54, 42, 80),
            new Battery("Deye BOS-GM5.1", 100, 57, 46, 50)
        };
        public class InverterRegister
        {
            public ushort RegisterNumber { get; set; }

            private float _value;
            public float Value
            {
                get => _value;
                set
                {
                    if ((value >= MinValue) && (value <= MaxValue))
                    {
                        _value = value;
                    }
                }
            }
            public string Unit { get; set; }
            public float MinValue { get; set; }
            public float MaxValue { get; set; }
            public string Description { get; set; }

            public InverterRegister(ushort number, float value, string unit, float min, float max, string description)
            {
                RegisterNumber = number;
                Value = value;
                Unit = unit;
                MinValue = min;
                MaxValue = max;
                Description = description;
            }

            public ushort GetRegisterData()
            {
                return (ushort)(Value * 100);
            }
        }
        public class Battery
        {
            public string Name { get; set; }
            public float NominalCapacity { get; set; } // Ah
            public float MaxVoltage { get; set; } // V
            public float MinVoltage { get; set; } // V
            public float NominalChargeDischargeCurrent { get; set; } // A

            public Battery(string name, float capacity, float maxV, float minV, float chargeCurrent)
            {
                Name = name;
                NominalCapacity = capacity;
                MaxVoltage = maxV;
                MinVoltage = minV;
                NominalChargeDischargeCurrent = chargeCurrent;
            }
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
                new InverterRegister(10, 0, "Text", 0, 11, "Register OverLimitError"),
                new InverterRegister(11, 0, "Error", 0, 10, "Error"),
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

            comboBoxBattery.SelectedIndexChanged += ComboBoxBattery_SelectedIndexChanged;
            comboBoxBattery.DataSource = batteries;
            comboBoxBattery.DisplayMember = "Name";
        }
        
        private void ComboBoxBattery_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedBattery = (Battery)comboBoxBattery.SelectedItem;
            UpdateBatteryRegisters();
            UpdateMaxPowerCapacity();
        }
        private void numericUpDownBatteryBlocks_ValueChanged(object sender, EventArgs e)
        {
            UpdateBatteryBlockCount((int)numericUpDownBatteryBlocks.Value);
            UpdateMaxPowerCapacity();

        }
       
        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            currentTime = currentTime.AddSeconds(1); // Increment current time by one second
            labelCurrentTime.Text = currentTime.ToString("HH:mm:ss"); // Update time label
            UpdateRegisterDisplay();
            UpdateBatteryInfo();
            ChрeckRegisterOverLimitError();
            ShowError();
        }

        private void UpdateRegisterDisplay()
        {
            listBoxBatteryInfo.Items.Clear(); // Очищення попереднього тексту
            foreach (var register in registers)
            {
                listBoxBatteryInfo.Items.Add($"{register.Description}: {register.Value} {register.Unit}");
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

        private void PrintDataToConsole(byte[] data)
        {
            Console.WriteLine("Received Packet:");
            foreach (byte b in data)
            {
                Console.Write($"0x{b:X2} ");
            }
            Console.WriteLine();
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
        private void SetBattaryPower(int value)
        {
            registers[9].Value = value;
        }

        //register changes
        private void UpdateBatteryInfo()
        {
            registers[0].Value = registers[9].Value / registers[3].Value/ registers[2].Value;
            registers[1].Value = (registers[0].Value - registers[5].Value) / (registers[4].Value - registers[5].Value) * 100; // % заряду батареї
            

        }
        private void UpdateBatteryBlockCount(int blockCount)
        {
            registers[2].Value = blockCount;

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
        private void ShowError()
        {
            if (registers[11].Value != 0)
            {
                var error = registers[(int)registers[11].Value];
                var errorRegis = registers[(int)error.Value];
                label_Error.Text = "Error " + error.Description + " for " + errorRegis.Description;
            }
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
    }
}
