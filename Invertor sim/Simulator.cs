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
        private string[] previousPorts;
        private CancellationTokenSource cts; // Для контролю зупинки симулятора
        private IModbusSerialMaster modbusMaster;
        private IModbusFactory modbusFactory;
        private ushort[] holdingRegisters;

        public struct InverterData
        {
            public float batteryVoltage;
            public float gridCurrent;
            public float solarVoltage;
            public float solarPower;
            public float batteryChargeDischargePower;
        }

        public Simulator(MainForm main)
        {
            InitializeComponent();
            this.main = main;

            // Ініціалізація Modbus
            modbusFactory = new ModbusFactory();
            holdingRegisters = new ushort[10];
            holdingRegisters[0] = 80; // SOC
            holdingRegisters[1] = 2560; // Напруга батареї

            portCheckTimer = new System.Windows.Forms.Timer();
            portCheckTimer.Interval = 5000;
            portCheckTimer.Tick += new EventHandler(CheckAvailablePorts);
            portCheckTimer.Start();

            previousPorts = SerialPort.GetPortNames();
            comboBoxPorts.Items.AddRange(previousPorts);

            comboBoxBaudRate.Items.Add("9600");
            comboBoxBaudRate.Items.Add("115200");
            comboBoxBaudRate.SelectedIndex = 0;

            comboBoxPorts.SelectedIndexChanged += ComboBoxPorts_SelectedIndexChanged;
        }

        private async void ComboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                try
                {
                    serialPort.DataReceived -= DataReceivedHandler;
                    serialPort.Close();
                }
                catch (Exception ex)
                {
                    // Виводимо помилку, якщо не вдалося закрити порт
                    labelStatus.Text = "Error closing port: " + ex.Message;
                }
            }

            string selectedPort = comboBoxPorts.SelectedItem.ToString();
            int baudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());

            try
            {
                serialPort = new SerialPort(selectedPort, baudRate, Parity.None, 8, StopBits.One);
                serialPort.Open();

                // Ініціалізація Modbus
                modbusMaster = modbusFactory.CreateRtuMaster(serialPort);

                cts = new CancellationTokenSource();
                await Task.Run(() => MonitorPort(cts.Token), cts.Token);

                // Підключаємо обробник подій для отримання даних
                serialPort.DataReceived += DataReceivedHandler;

                labelStatus.Text = "Connected to " + selectedPort;
            }
            catch (UnauthorizedAccessException)
            {
                labelStatus.Text = "Error: Port is already in use.";
            }
            catch (IOException)
            {
                labelStatus.Text = "Error: Failed to open port. Port may not be available.";
            }
            catch (Exception ex)
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void PrintDataToConsole(byte[] data)
        {
            Console.WriteLine("Received Packet:");
            for (int i = 0; i < data.Length; i++)
            {
                Console.Write($"0x{data[i]:X2} ");
            }
            Console.WriteLine();
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null || !serialPort.IsOpen)
                return;

            // Зчитування даних з порту
            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);

            // Виведення даних на консоль
            PrintDataToConsole(buffer);

            // Виведення даних на форму
            Invoke(new Action(() =>
            {
                listBoxReceivedCommands.Items.Add("Received: " + BitConverter.ToString(buffer));
            }));

            // Обробка Modbus запитів
            try
            {
                var request = modbusMaster.ReadHoldingRegisters(1, 0, 10); // 1 - адреса раба, 0 - початкова адреса, 10 - кількість регістрів
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
            // Обробка запиту Modbus
            holdingRegisters = request;

            // Перетворення ushort[] на byte[]
            byte[] byteArray = holdingRegisters.SelectMany(BitConverter.GetBytes).ToArray();

            // Наприклад, відправляємо дані на запит
            Invoke(new Action(() =>
            {
                listBoxSentCommands.Items.Add("Sent: " + BitConverter.ToString(byteArray));
            }));
        }

        private async Task MonitorPort(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    // Постійне зчитування запитів
                    var request = modbusMaster.ReadHoldingRegisters(1, 0, 10); // 1 - адреса раба, 0 - початкова адреса, 10 - кількість регістрів
                    ProcessRequest(request);
                }
                catch (Exception ex)
                {
                    Invoke(new Action(() =>
                    {
                        listBoxReceivedCommands.Items.Add("Monitor Error: " + ex.Message);
                    }));
                }
                await Task.Delay(100); // Затримка для уникнення надмірного використання ресурсів
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            if (cts != null)
            {
                cts.Cancel(); // Зупиняємо моніторинг
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
                    // Виводимо помилку, якщо не вдалося закрити порт
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
    }
}
