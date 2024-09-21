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

        public class InverterRegister
        {
            public ushort RegisterNumber { get; set; }
            public float Value { get; set; }
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

        public Simulator(MainForm main)
        {
            InitializeComponent();
            this.main = main;

            modbusFactory = new ModbusFactory();

            registers = new InverterRegister[]
            {
                new InverterRegister(0, 25.6f, "V", 0, 100, "Battery Voltage"),
                new InverterRegister(1, 80.0f, "%", 0, 100, "State of Charge (SOC)")
            };

            portCheckTimer = new System.Windows.Forms.Timer();
            portCheckTimer.Interval = 5000;
            portCheckTimer.Tick += new EventHandler(CheckAvailablePorts);
            portCheckTimer.Start();

            previousPorts = SerialPort.GetPortNames();
            comboBoxPorts.Items.Add("Port Not Selected");
            comboBoxPorts.Items.AddRange(previousPorts);

            comboBoxBaudRate.Items.Add("9600");
            comboBoxBaudRate.Items.Add("115200");
            comboBoxBaudRate.SelectedIndex = 0;

            comboBoxPorts.SelectedIndexChanged += ComboBoxPorts_SelectedIndexChanged;

            // Initialize and start clock timer
            clockTimer = new System.Windows.Forms.Timer();
            clockTimer.Interval = 1000; // Update every second
            clockTimer.Tick += ClockTimer_Tick;
            clockTimer.Start();
        }

        private void ClockTimer_Tick(object sender, EventArgs e)
        {
            currentTime = currentTime.AddSeconds(1); // Increment current time by one second
            labelCurrentTime.Text = currentTime.ToString("HH:mm:ss"); // Update time label
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
    }
}
