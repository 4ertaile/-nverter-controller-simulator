using System;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Threading;
using System.Windows.Forms;

namespace Invertor_sim
{
    public partial class Simulator : Form
    {
        private MainForm main;
        private SerialPort serialPort;
        private System.Windows.Forms.Timer portCheckTimer;
        private string[] previousPorts;
        private Thread simulationThread;  // Окрема нитка для симулятора інвертора
        private bool isRunning = false;   // Флаг для контролю роботи симулятора

        // Структура даних інвертора
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

            // Ініціалізуємо таймер для перевірки доступних портів
            portCheckTimer = new System.Windows.Forms.Timer();
            portCheckTimer.Interval = 5000;  // Перевіряємо кожні 5 секунд
            portCheckTimer.Tick += new EventHandler(CheckAvailablePorts);
            portCheckTimer.Start();

            // Заповнення ComboBox портів при старті
            previousPorts = SerialPort.GetPortNames();
            comboBoxPorts.Items.AddRange(previousPorts);

            // Встановлення дефолтної швидкості
            comboBoxBaudRate.Items.Add("9600");
            comboBoxBaudRate.Items.Add("115200");
            comboBoxBaudRate.SelectedIndex = 0; // За замовчуванням 9600

            // Підписуємось на подію вибору порту
            comboBoxPorts.SelectedIndexChanged += ComboBoxPorts_SelectedIndexChanged;

            // Кнопка для ручного відправлення команд
            buttonSend.Click += buttonSend_Click;
        }

        private void ComboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Закриваємо поточний порт, якщо відкритий
            if (serialPort != null && serialPort.IsOpen)
            {
                serialPort.Close();
            }

            string selectedPort = comboBoxPorts.SelectedItem.ToString();
            int baudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());

            try
            {
                // Створюємо новий екземпляр SerialPort
                serialPort = new SerialPort(selectedPort, baudRate, Parity.None, 8, StopBits.One);
                serialPort.DataReceived += new SerialDataReceivedEventHandler(DataReceivedHandler);

                // Пробуємо відкрити порт
                serialPort.Open();
                labelStatus.Text = "Connected to " + selectedPort;

                // Запускаємо симуляцію інвертора в окремій нитці
                isRunning = true;
                simulationThread = new Thread(SimulatorLoop);
                simulationThread.Start();
            }
            catch (UnauthorizedAccessException)
            {
                labelStatus.Text = "Error: Port is already in use.";
            }
            catch (IOException)
            {
                labelStatus.Text = "Error: Failed to open port. Port may not be available.";
            }
            catch (Exception ex)  // Для будь-яких інших помилок
            {
                labelStatus.Text = "Error: " + ex.Message;
            }
        }

        private void buttonSend_Click(object sender, EventArgs e)
        {
            if (serialPort != null && serialPort.IsOpen)
            {
                string command = textBoxSendCommand.Text;
                serialPort.WriteLine(command);
                listBoxSentCommands.Items.Add("Sent: " + command);
            }
        }

        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            string receivedData = serialPort.ReadLine();
            Invoke(new Action(() =>
            {
                listBoxReceivedCommands.Items.Add("Received: " + receivedData);
            }));

            // Обробка вхідних команд
            byte command = Convert.ToByte(receivedData);
            InverterData data = new InverterData
            {
                batteryVoltage = 48.5f,
                gridCurrent = 15.2f,
                solarVoltage = 320.7f,
                solarPower = 800.0f,
                batteryChargeDischargePower = -50.0f
            };

            if (command == 0x03)  // Запит всіх даних (GET_FULL_DATA)
            {
                SendData(data);  // Відправляємо всі дані
            }
            else if (command == 0x04)  // Запит вольтажу батареї (GET_BATTERY_VOLTAGE)
            {
                SendSingleValue(data.batteryVoltage);  // Відправляємо лише вольтаж
            }
        }

        private void SendData(InverterData data)
        {
            byte[] responseData = StructToByteArray(data);
            serialPort.Write(responseData, 0, responseData.Length);
            Invoke(new Action(() =>
            {
                listBoxSentCommands.Items.Add("Sent: Full data");
            }));
        }

        private void SendSingleValue(float value)
        {
            byte[] responseValue = BitConverter.GetBytes(value);
            serialPort.Write(responseValue, 0, responseValue.Length);
            Invoke(new Action(() =>
            {
                listBoxSentCommands.Items.Add("Sent: Battery voltage - " + value);
            }));
        }

        private byte[] StructToByteArray(InverterData data)
        {
            int size = sizeof(float) * 5;
            byte[] arr = new byte[size];

            Buffer.BlockCopy(BitConverter.GetBytes(data.batteryVoltage), 0, arr, 0, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.gridCurrent), 0, arr, 4, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.solarVoltage), 0, arr, 8, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.solarPower), 0, arr, 12, sizeof(float));
            Buffer.BlockCopy(BitConverter.GetBytes(data.batteryChargeDischargePower), 0, arr, 16, sizeof(float));

            return arr;
        }

        // Метод для перевірки нових доступних портів
        private void CheckAvailablePorts(object sender, EventArgs e)
        {
            string[] currentPorts = SerialPort.GetPortNames();

            // Якщо доступні порти змінилися, оновлюємо ComboBox
            if (!currentPorts.SequenceEqual(previousPorts))
            {
                previousPorts = currentPorts;

                comboBoxPorts.Items.Clear();
                comboBoxPorts.Items.AddRange(currentPorts);

                // Якщо порт був відкритий, а зараз зник, закриваємо його
                if (serialPort != null && serialPort.IsOpen && !currentPorts.Contains(serialPort.PortName))
                {
                    serialPort.Close();
                    labelStatus.Text = "Disconnected";
                }
            }
        }

        private void SimulatorLoop()
        {
            while (isRunning)
            {
                Thread.Sleep(100);  // Імітація очікування запитів
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            isRunning = false;  // Зупиняємо симулятор
            if (simulationThread != null && simulationThread.IsAlive)
            {
                simulationThread.Join();  // Очікуємо завершення потоку
            }
            Application.Exit();  // Завершуємо програму
        }
    }
}
