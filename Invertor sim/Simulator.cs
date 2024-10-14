using Microsoft.Win32;
using NModbus;
using NModbus.Serial;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

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
        private static InverterRegister[] registers;
        private DateTime currentTime;
        private Battery selectedBattery;

        private SolarPanel selectedPanelGrid1;
        private SolarPanel selectedPanelGrid2;

        private int PanelCountGrid1 = 0;
        private int PanelCountGrid2 = 0;

        private int MaxPanelCountGrid1 = 0;
        private int MaxPanelCountGrid2 = 0;

        private const int InvertorPowerUsage = 200;
        private const float Inverter_efficiency = 0.94f;//5-7%
        private const float Generator_efficiency = 0.9f;
        private const float invertorWorkPower = 200f;
        private bool useFileSourse = false;
        private static string recivetmess = "";
        private static bool messageRecived = false;
        //modbus
        private static SlaveStorage storage;
        private static CancellationTokenSource cts = new CancellationTokenSource();
        private Task listenTask;

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

        public enum UnitEnum
        {
            V,    // Вольти
            A,    // Ампери
            W,    // Ватти
            Wh,   // Ват-години
            Ah,   // Ампер-години
            Blocks,  // Блоки
            N,    // Кількість
            Error, // Помилки
            Percent // Відсотки
        }


        public Simulator(MainForm main)
        {
            InitializeComponent();
            this.main = main;

            modbusFactory = new ModbusFactory();

            registers = new InverterRegister[Enum.GetValues(typeof(RegisterEnum)).Length];

            registers[(int)RegisterEnum.BatteryVoltage] = new InverterRegister(RegisterEnum.BatteryVoltage, 0, UnitEnum.V.ToString(), 0, 62);//*
            registers[(int)RegisterEnum.StateOfCharge] = new InverterRegister(RegisterEnum.StateOfCharge, 0, UnitEnum.Percent.ToString(), 0, 100);//*
            registers[(int)RegisterEnum.BatteryBlocks] = new InverterRegister(RegisterEnum.BatteryBlocks, 0, UnitEnum.Blocks.ToString(), 0, 50);
            registers[(int)RegisterEnum.BatteryCurrency] = new InverterRegister(RegisterEnum.BatteryCurrency, 0, UnitEnum.Ah.ToString(), 0, 500);
            registers[(int)RegisterEnum.MaxVoltage] = new InverterRegister(RegisterEnum.MaxVoltage, 54, UnitEnum.V.ToString(), 40, 60);
            registers[(int)RegisterEnum.MinVoltage] = new InverterRegister(RegisterEnum.MinVoltage, 42, UnitEnum.V.ToString(), 40, 60);
            registers[(int)RegisterEnum.NominalChargeCurrent] = new InverterRegister(RegisterEnum.NominalChargeCurrent, 0, UnitEnum.A.ToString(), 0, 135);
            registers[(int)RegisterEnum.NominalDischargeCurrent] = new InverterRegister(RegisterEnum.NominalDischargeCurrent, 0, UnitEnum.A.ToString(), 0, 190);
            registers[(int)RegisterEnum.MaxBatteryPowerCapacity] = new InverterRegister(RegisterEnum.MaxBatteryPowerCapacity, 0, UnitEnum.W.ToString(), 0, 450400);
            registers[(int)RegisterEnum.BatteryPower] = new InverterRegister(RegisterEnum.BatteryPower, 0, UnitEnum.Wh.ToString(), 0, 0);
            registers[(int)RegisterEnum.OverLimitError] = new InverterRegister(RegisterEnum.OverLimitError, 0, UnitEnum.N.ToString(), 0, 36);
            registers[(int)RegisterEnum.Error] = new InverterRegister(RegisterEnum.Error, 0, UnitEnum.Error.ToString(), 0, 36);
            registers[(int)RegisterEnum.InputGridPower] = new InverterRegister(RegisterEnum.InputGridPower, 0, UnitEnum.W.ToString(), -16000, 16000); // Мережевий вхід
            registers[(int)RegisterEnum.InputGridVoltage] = new InverterRegister(RegisterEnum.InputGridVoltage, 0, UnitEnum.V.ToString(), 165, 290);
            registers[(int)RegisterEnum.InputGeneratorPower] = new InverterRegister(RegisterEnum.InputGeneratorPower, 0, UnitEnum.W.ToString(), 0, 0);
            registers[(int)RegisterEnum.InputGeneratorVoltage] = new InverterRegister(RegisterEnum.InputGeneratorVoltage, 0, UnitEnum.V.ToString(), 165, 290);
            registers[(int)RegisterEnum.InputSolarPower] = new InverterRegister(RegisterEnum.InputSolarPower, 0, UnitEnum.W.ToString(), 0, 7800); // Весь сонячний вхід
            registers[(int)RegisterEnum.InputSolarGrid1Voltage] = new InverterRegister(RegisterEnum.InputSolarGrid1Voltage, 0, UnitEnum.V.ToString(), 0, 500);
            registers[(int)RegisterEnum.InputSolarGrid2Voltage] = new InverterRegister(RegisterEnum.InputSolarGrid2Voltage, 0, UnitEnum.V.ToString(), 0, 500);
            registers[(int)RegisterEnum.InputSolarGrid1Power] = new InverterRegister(RegisterEnum.InputSolarGrid1Power, 0, UnitEnum.W.ToString(), 0, 5525);
            registers[(int)RegisterEnum.InputSolarGrid2Power] = new InverterRegister(RegisterEnum.InputSolarGrid2Power, 0, UnitEnum.W.ToString(), 0, 5525);
            registers[(int)RegisterEnum.MinimumBatteryDischarge] = new InverterRegister(RegisterEnum.MinimumBatteryDischarge, 20, UnitEnum.Percent.ToString(), 0, 100);
            registers[(int)RegisterEnum.ChargeCurrent] = new InverterRegister(RegisterEnum.ChargeCurrent, 0, UnitEnum.A.ToString(), 0, 135);
            registers[(int)RegisterEnum.DischargeCurrent] = new InverterRegister(RegisterEnum.DischargeCurrent, 0, UnitEnum.A.ToString(), 0, 190);
            registers[(int)RegisterEnum.MPPTVoltage] = new InverterRegister(RegisterEnum.MPPTVoltage, 0, UnitEnum.V.ToString(), 125, 425);
            registers[(int)RegisterEnum.GeneratorConnected] = new InverterRegister(RegisterEnum.GeneratorConnected, 0, UnitEnum.Error.ToString(), 0, 1);
            registers[(int)RegisterEnum.SalePower] = new InverterRegister(RegisterEnum.SalePower, 0, UnitEnum.W.ToString(), 0, 99999999999);
            registers[(int)RegisterEnum.GridVoltageOut] = new InverterRegister(RegisterEnum.GridVoltageOut, 0, UnitEnum.V.ToString(), 165, 290);
            registers[(int)RegisterEnum.GridPowerOut] = new InverterRegister(RegisterEnum.GridPowerOut, 0, UnitEnum.W.ToString(), 0, 6000);
            registers[(int)RegisterEnum.BatteryPowerUsage] = new InverterRegister(RegisterEnum.BatteryPowerUsage, 0, UnitEnum.W.ToString(), 0, 0);
            registers[(int)RegisterEnum.UseGeneratorForChargeBattery] = new InverterRegister(RegisterEnum.UseGeneratorForChargeBattery, 0, UnitEnum.Error.ToString(), 0, 1);
            registers[(int)RegisterEnum.UseMainsGridForChargeBattery] = new InverterRegister(RegisterEnum.UseMainsGridForChargeBattery, 0, UnitEnum.Error.ToString(), 0, 1);
            registers[(int)RegisterEnum.MaxChargeCurrent] = new InverterRegister(RegisterEnum.MaxChargeCurrent, 135, UnitEnum.A.ToString(), 0, 200);
            registers[(int)RegisterEnum.MaxDischargeCurrent] = new InverterRegister(RegisterEnum.MaxDischargeCurrent, 190, UnitEnum.A.ToString(), 0, 200);
            registers[(int)RegisterEnum.MaximumBatteryCharge] = new InverterRegister(RegisterEnum.MaximumBatteryCharge, 100, UnitEnum.Percent.ToString(), 0, 100);
            registers[(int)RegisterEnum.MaintainingBatteryChargeLevel] = new InverterRegister(RegisterEnum.MaintainingBatteryChargeLevel, 100, UnitEnum.Percent.ToString(), 0, 100);
            registers[(int)RegisterEnum.LowWorkPower] = new InverterRegister(RegisterEnum.LowWorkPower, 0, UnitEnum.W.ToString(), 0, 1);
            registers[(int)RegisterEnum.PowerConsumption] = new InverterRegister(RegisterEnum.PowerConsumption, 0, UnitEnum.W.ToString(), 0, 16200);


            storage = new SlaveStorage(registers);
            storage.HoldingRegisters.StorageOperationOccurred += OnStorageOperationOccurred;


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
            if (messageRecived)
            {
                listBoxReceivedCommands.Items.Add(recivetmess);
                messageRecived = false;
            }
            storage.SyncInverterToModbus();
            UpdateBatteryInfo();
            UpdateBatteryFormInfo();
            UpdateGridsPanelCount();
            UpdateGridsPanelMaxPower();
            ShowInvertorInfo();

            UpdateGrid1PanelCount();
            UpdateGrid2PanelCount();
            UpdateUseGeneratorToCharge();
            UpdateUseMainsGridToCharge();

            UpdateSolarGridSumInfo();
            UpdateSolarGridInputInfo();
            UpdateGridOutStatus();
            UpdateGeneratorInfo();
            UpdateMainsGridInfo();
            UpdateOutGridInfo();

            CalculatedSellPower();
            if (!useFileSourse)
            {
                UpdateVoltageSolarGrid();
                UpdateWorkStatus();
            }

        }

        private void ComboBoxPorts_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Отримуємо обраний порт
            string selectedPort = comboBoxPorts.SelectedItem?.ToString();

            // Перевірка, чи порт не обрано
            if (string.IsNullOrEmpty(selectedPort) || selectedPort == "Port Not Selected")
            {
                StopModbusTasks();
                labelStatus.Text = "Port Not Selected";
                return;
            }

            // Закриття попереднього порту і запуск нового підключення
            StopModbusTasks();

            try
            {
                // Перевірка вибраного baudRate
                if (comboBoxBaudRate.SelectedItem == null)
                {
                    labelStatus.Text = "Error: Baud rate not selected.";
                    return;
                }

                // Створення і відкриття нового серійного порту
                int baudRate = int.Parse(comboBoxBaudRate.SelectedItem.ToString());
                serialPort = new SerialPort(selectedPort, baudRate, Parity.None, 8, StopBits.One);
                serialPort.Open();

                // Ініціалізація Modbus
                modbusFactory = new ModbusFactory();
                var slaveNetwork = modbusFactory.CreateRtuSlaveNetwork(serialPort);
                IModbusSlave slave = modbusFactory.CreateSlave(1, storage);
                slaveNetwork.AddSlave(slave);

                // Запуск прослуховування і оновлення реєстрів
                cts = new CancellationTokenSource();
                listenTask = Task.Run(async () =>
                {
                    try
                    {
                        await slaveNetwork.ListenAsync(cts.Token);
                    }
                    catch (OperationCanceledException)
                    {
                        // Ігноруємо помилку, оскільки це результат скасування
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Error during ListenAsync: " + ex.Message);
                    }
                });

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

        private void StopModbusTasks()
        {
            // Зупиняємо прослуховування і оновлення реєстрів
            if (cts != null)
            {
                cts.Cancel(); // Скасування задачі

                try
                {
                    // Закриття порту без очікування завершення задачі
                    if (serialPort != null && serialPort.IsOpen)
                    {
                        serialPort.Close(); // Закриваємо порт без чекаючої задачі
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error closing port: " + ex.Message);
                }
                finally
                {
                    cts.Dispose();
                    cts = null;
                }
            }
        }


        //код який необхідно переписати, щоб він вказував який запут надійшов, що ми надіслали, виконую синхронізацію значення після отримання запиту
        private static void OnStorageOperationOccurred(object sender, StorageEventArgs<ushort> e)
        {
            // Оновлення даних у реєстрах при зміні Modbus
            recivetmess = "Operation:" + e.Operation  + ", Starting Address: " + e.StartingAddress + ", Points: " + string.Join(", ", e.Points);
            messageRecived = true;
            if (e.Operation == PointOperation.Read)
            {
                storage.SyncInverterToModbus();
            }
            else if (e.Operation == PointOperation.Write)
            {
                foreach (var point in e.Points)
                {
                    storage.SyncModbusToInverter(e.StartingAddress, point);
                }
            }

            Console.WriteLine($"Current Register Values: {storage.HoldingRegisters[0]}, {storage.HoldingRegisters[1]}");
        }


        //переписати і використати в методі отримання повідомлення
        private void DataReceivedHandler(object sender, SerialDataReceivedEventArgs e)
        {
            if (serialPort == null || !serialPort.IsOpen)
                return;

            byte[] buffer = new byte[serialPort.BytesToRead];
            serialPort.Read(buffer, 0, buffer.Length);


            Invoke(new Action(() =>
            {
                listBoxReceivedCommands.Items.Add("Received: " + BitConverter.ToString(buffer));
            }));

            try
            {
                // Припускаємо, що запит зчитує Holding регістри
                var request = modbusMaster.ReadHoldingRegisters(1, 0, (ushort)registers.Length);

                if (request.Length > 0)
                {
                    ProcessRequest(request);
                }
                else
                {
                    // Якщо запит на запис
                    HandleWriteRequest(buffer);
                }
            }
            catch (Exception ex)
            {
                Invoke(new Action(() =>
                {
                    listBoxReceivedCommands.Items.Add("Error: " + ex.Message);
                }));
            }
        }
        //видалити через непотрібність
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

        //видалити метод через непотрібність
        private void HandleWriteRequest(byte[] buffer)
        {
            // Створення пакету для запису в регістри на основі даних із буфера
            ushort startingAddress = BitConverter.ToUInt16(buffer, 0); // Перші 2 байти — це стартова адреса
            ushort[] valuesToWrite = new ushort[(buffer.Length - 2) / 2];

            for (int i = 0; i < valuesToWrite.Length; i++)
            {
                valuesToWrite[i] = BitConverter.ToUInt16(buffer, 2 + i * 2);
            }

            // Оновлення значень регістрів
            for (int i = 0; i < valuesToWrite.Length; i++)
            {
                int registerIndex = startingAddress + i;
                if (registerIndex < registers.Length)
                {
                    registers[registerIndex].Value = valuesToWrite[i] / 100f;
                }
            }

            Invoke(new Action(() =>
            {
                listBoxSentCommands.Items.Add("Processed Write Request for Registers starting at: " + startingAddress);
            }));
        }


        ///////////////
        ///////////////
        //modbus
        

    

    public class SlaveStorage : ISlaveDataStore
    {
        private readonly SparsePointSource<ushort> _holdingRegisters;
        private readonly InverterRegister[] _inverterRegisters;

        public byte[] GetRegisterData(InverterRegister register)
        {
            var json = JsonSerializer.Serialize(register);
            return Encoding.UTF8.GetBytes(json);
        }

        public void SetRegisterData(byte[] data, InverterRegister register)
        {
            var json = Encoding.UTF8.GetString(data);
            register = JsonSerializer.Deserialize<InverterRegister>(json);
        }

        public SlaveStorage(InverterRegister[] inverterRegisters)
        {
            _holdingRegisters = new SparsePointSource<ushort>();
            _inverterRegisters = inverterRegisters;

            // Ініціалізація значень регістрів з InverterRegister
            foreach (var reg in inverterRegisters)
            {
                _holdingRegisters[reg.RegisterNumber] = reg.GetRegisterData();
            }
        }

        public SparsePointSource<ushort> HoldingRegisters => _holdingRegisters;

        IPointSource<ushort> ISlaveDataStore.HoldingRegisters => _holdingRegisters;
        IPointSource<bool> ISlaveDataStore.CoilDiscretes => throw new NotImplementedException();
        IPointSource<bool> ISlaveDataStore.CoilInputs => throw new NotImplementedException();
        IPointSource<ushort> ISlaveDataStore.InputRegisters => throw new NotImplementedException();

        // Метод для синхронізації даних
        public void SyncInverterToModbus()
        {
            foreach (var reg in _inverterRegisters)
            {
                _holdingRegisters[reg.RegisterNumber] = reg.GetRegisterData();
            }
        }
        public void SyncInverterToModbus(ushort registerNumber)
        {
            var reg = Array.Find(_inverterRegisters, r => r.RegisterNumber == registerNumber);
            if (reg != null)
            {
                _holdingRegisters[reg.RegisterNumber] = reg.GetRegisterData();
            }
        }

        public void SyncModbusToInverter(ushort registerNumber, ushort value)
        {
            var inverterReg = Array.Find(_inverterRegisters, r => r.RegisterNumber == registerNumber);
            if (inverterReg != null)
            {
                inverterReg.Value = value / 100.0f; // Конвертація назад до float
            }
        }

    }

   

    public class SparsePointSource<TPoint> : IPointSource<TPoint>
    {
        private readonly Dictionary<ushort, TPoint> _values = new Dictionary<ushort, TPoint>();

        public event EventHandler<StorageEventArgs<TPoint>> StorageOperationOccurred;

        public TPoint this[ushort registerIndex]
        {
            get
            {
                if (_values.TryGetValue(registerIndex, out var value))
                    return value;
                return default;
            }
            set => _values[registerIndex] = value;
        }

        public TPoint[] ReadPoints(ushort startAddress, ushort numberOfPoints)
        {
            var points = new TPoint[numberOfPoints];
            for (ushort i = 0; i < numberOfPoints; i++)
            {
                points[i] = this[(ushort)(i + startAddress)];
            }
            StorageOperationOccurred?.Invoke(this, new StorageEventArgs<TPoint>(PointOperation.Read, startAddress, points));
            return points;
        }

        public void WritePoints(ushort startAddress, TPoint[] points)
        {
            for (ushort i = 0; i < points.Length; i++)
            {
                this[(ushort)(i + startAddress)] = points[i];
            }
            StorageOperationOccurred?.Invoke(this, new StorageEventArgs<TPoint>(PointOperation.Write, startAddress, points));
        }
    }

    public class StorageEventArgs<TPoint> : EventArgs
    {
        public StorageEventArgs(PointOperation operation, ushort startingAddress, TPoint[] points)
        {
            Operation = operation;
            StartingAddress = startingAddress;
            Points = points;
        }

        public PointOperation Operation { get; }
        public ushort StartingAddress { get; }
        public TPoint[] Points { get; }
    }

    public enum PointOperation
    {
        Read,
        Write
    }




    ///////////////
    ///////////////


    //Form methods
    //////////////
    //////////////


    private void PanelTypeGrid1_SelectedIndexChanged(object sender, EventArgs e)
        {
            selectedPanelGrid1 = (SolarPanel)panelTypeGrid1.SelectedItem;
            UpdateSolarGridPanelCount1();
        }
        private void maintainingCharge_ValueChanged(object sender, EventArgs e)
        {
            if ((float)maintainingCharge.Value >= registers[21].Value && (float)maintainingCharge.Value <= registers[34].Value)
                registers[35].Value = (float)maintainingCharge.Value;
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
        }
        private void numericUpDownBatteryBlocks_ValueChanged(object sender, EventArgs e)
        {
            UpdateBatteryBlockCount((int)batteryElemntCount.Value);
            UpdateBatteryRegisters();

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
        private void buttonSetGenPower_Click(object sender, EventArgs e)
        {
            if (float.TryParse(setGenVoltage.Text, out float gridVoltage))
            {

                SetGeneratorVoltage(gridVoltage);
            }
            if (float.TryParse(setGenPower.Text, out float gridMaxPower))
            {
                SetGeneratorMaxPower(gridMaxPower);
            }
        }

        private void buttonSetPowerOut_Click(object sender, EventArgs e)
        {
            if (float.TryParse(setInvertorPowerOut.Text, out float gridPower))
            {
                SetInvertorPowerOut(gridPower);
            }
        }

        private void buttonSetMainsPower_Click(object sender, EventArgs e)
        {
            if (float.TryParse(setMainsGridVoltaage.Text, out float gridVoltage))
            {
                SetMainsGridVoltage(gridVoltage);
            }
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
            if (float.TryParse(setSolarGridPower1.Text, out float gridPower))
            {
                GetSolarGridPower(gridPower, registers[19], registers[20].Value, registers[16].MaxValue, GetVoltageAndCurrentGrid1(gridPower).Item1);
            }
        }

        private void buttonSetnputSolPower2_Click(object sender, EventArgs e)
        {
            if (float.TryParse(setSolarGridPower2.Text, out float gridPower))
            {
                GetSolarGridPower(gridPower, registers[20], registers[19].Value, registers[16].MaxValue, GetVoltageAndCurrentGrid2(gridPower).Item1);
            }
        }
        private (float, float) GetVoltageAndCurrentGrid1(float gridPower)
        {

            return CalculateMPPT(gridPower, PanelCountGrid1, selectedPanelGrid1.MaxWorkСurrent, selectedPanelGrid1.MaxWorkVoltage);
        }
        private (float, float) GetVoltageAndCurrentGrid2(float gridPower)
        {

            return CalculateMPPT(gridPower, PanelCountGrid2, selectedPanelGrid2.MaxWorkСurrent, selectedPanelGrid2.MaxWorkVoltage);
        }

        private void UpdateVoltageSolarGrid()
        {
            registers[17].Value = GetVoltageAndCurrentGrid1(registers[19].Value).Item1;
            registers[18].Value = GetVoltageAndCurrentGrid2(registers[20].Value).Item1;
        }


        private void GetSolarGridPower(float gridPower, InverterRegister solGridPowerRegister, float secondSolGridPowerRegister, float maxSolGridInputSum, float inputVoltage)
        {
            if (gridPower < 0)
                return;


            if (gridPower > solGridPowerRegister.MaxValue || !SolarGridCanGenerate(inputVoltage))
            {
                solGridPowerRegister.Value = 0;
                return;
            }
            if ((secondSolGridPowerRegister + gridPower) <= maxSolGridInputSum)
            {
                solGridPowerRegister.Value = gridPower;
            }

        }

        private void UpdateWorkStatus()
        {
            if (registers[11].Value != 0)
            {
                startSimulation.Checked = false;
                SetSimulatePowerUsageParametersToZero();
            }
            if (startSimulation.Checked == true)
            {
                SimulatePowerUsage();
            }
        }
        private void UpdateUseGeneratorToCharge()
        {
            if (useGenerator.Checked == true)
            {
                registers[30].Value = 1;
            }
            else
            {
                registers[30].Value = 0;
            }
        }
        private void UpdateUseMainsGridToCharge()
        {
            if (useMainsGrid.Checked == true)
            {
                registers[31].Value = 1;
            }
            else
            {
                registers[31].Value = 0;
            }
        }
        private void UpdateGridOutStatus()
        {

            if (haveInvertorOut.Checked && startSimulation.Checked)
            {
                SetInvertorVoltageOut(220);
            }
            else
            {
                SetInvertorPowerOut(0);
                SetInvertorVoltageOut(0);

            }
        }
        private void UpdateSolarGridInputInfo()
        {
            if (float.TryParse(setSolarGridPower1.Text, out float solGridPower1))
            {
                if (solGridPower1 >= selectedPanelGrid1.MaxWorkVoltage * selectedPanelGrid1.MaxWorkСurrent * PanelCountGrid1)
                {
                    possibleGrid1Voltage.Text = "out_of_range";
                    checkBoxSolar1.Checked = false;
                    return;
                }
                float possibleVoltage1 = CalculateMPPT(solGridPower1, PanelCountGrid1, selectedPanelGrid1.MaxWorkСurrent, selectedPanelGrid1.MaxWorkVoltage).Voltage;
                possibleGrid1Voltage.Text = possibleVoltage1.ToString();

                if (SolarGridCanGenerate(possibleVoltage1))
                {
                    checkBoxSolar1.Checked = true;
                    solarGridPower1.Text = registers[19].Value.ToString();
                }
                else
                {
                    checkBoxSolar1.Checked = false;
                }
            }
            if (float.TryParse(setSolarGridPower2.Text, out float solGridPower2))
            {
                if (solGridPower2 >= selectedPanelGrid2.MaxWorkVoltage * selectedPanelGrid2.MaxWorkСurrent * PanelCountGrid2)
                {
                    possibleGrid2Voltage.Text = "out_of_range";
                    checkBoxSolar2.Checked = false;
                    return;
                }
                float possibleVoltage2 = CalculateMPPT(solGridPower1, PanelCountGrid2, selectedPanelGrid2.MaxWorkСurrent, selectedPanelGrid2.MaxWorkVoltage).Voltage;
                possibleGrid2Voltage.Text = possibleVoltage2.ToString();

                if (SolarGridCanGenerate(possibleVoltage2))
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

            solarGridVoltage1.Text = registers[17].Value.ToString();
            solarGridVoltage2.Text = registers[18].Value.ToString();



        }
        private void buttonClearError_Click(object sender, EventArgs e)
        {
            registers[11].Value = 0;
            label_Error.Text = "Error:";
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
        private void UpdateBatteryFormInfo()
        {
            maxBatteryPowerOut.Text = "Max Battery Power out in moment: " + registers[29].MaxValue;
            batteryPowerOut.Text = "Battery Power out: " + registers[29].Value;//замена на value
            SetupBatteryChargeDischargeBoxs();
            SetupBatteryChargeLvls();
        }
        private void SetupBatteryChargeDischargeBoxs()
        {
            chargePowerBattery.Maximum = (decimal)registers[6].MaxValue;
            dischargePowerBatery.Maximum = (decimal)registers[7].MaxValue;
        }
        private void SetupBatteryChargeLvls()
        {


            if (minimalCharge.Value < maximumCharge.Value)
            {
                registers[34].Value = (float)maximumCharge.Value;
                registers[21].Value = (float)minimalCharge.Value;
            }
            else
            {
                maximumCharge.Value = (decimal)registers[34].Value;
                minimalCharge.Value = (decimal)registers[21].Value;
            }

            maintainingCharge.Maximum = (decimal)registers[34].Value;
            maintainingCharge.Minimum = (decimal)registers[21].Value;


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
            if (int.TryParse(setBatteryCapacity.Text, out int batteryPower))
            {
                // Если значение корректное, устанавливаем мощность батареи
                SetBattaryStateOfCharge(batteryPower);
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
        private bool SolarGridCanGenerate(float gridVoltage)//voltage must be between lowValue and hight value registr MPPT Voltage
        {
            return registers[24].MinValue < gridVoltage && gridVoltage <= registers[24].MaxValue;
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
                return;
            }
        }

        private void SetBattaryStateOfCharge(int value)
        {
            registers[1].Value = value;
        }

        //register changes
        private void UpdateBatteryInfo()
        {
            if (registers[4].Value != registers[5].Value && registers[2].Value != 0)
            {
                registers[3].Value = registers[3].MaxValue * registers[1].Value / 100;
                if (registers[3].MaxValue != 0)
                    registers[0].Value = CalculateVoltageByMaxMinVoltageAndAh();
                else
                    registers[0].Value = 0;
                registers[29].MaxValue = registers[7].Value * registers[0].Value;

                registers[8].Value = registers[4].Value * registers[3].MaxValue; // Макс запас енергії
                registers[9].MaxValue = registers[8].Value;
                registers[9].Value = registers[0].Value * registers[3].Value;
            }


        }
        private float CalculateVoltageByMaxMinVoltageAndAh()
        {
            return registers[5].Value + (registers[4].Value - registers[5].Value) * (registers[3].Value / registers[3].MaxValue);
        }
        private float CalculateBatteryCapacityInPercents()
        {
            return registers[3].Value / registers[3].MaxValue * 100;
        }
        private void UpdateBatteryRegisters()
        {
            registers[3].MaxValue = selectedBattery.NominalCapacity * registers[2].Value; // Ємність
            registers[4].Value = selectedBattery.MaxVoltage; // Максимальний вольтаж
            registers[5].Value = selectedBattery.MinVoltage; // Мінімальний вольтаж

            var charge = selectedBattery.NominalChargeDischargeCurrent * registers[2].Value;
            var discharge = selectedBattery.NominalChargeDischargeCurrent * registers[2].Value;

            registers[6].MaxValue = Math.Min(charge, registers[32].Value);// Ток заряду/розряду
            registers[7].MaxValue = Math.Min(discharge, registers[33].Value);

        }

        private void UpdateSolarGridPanelCount1()
        {
            MaxPanelCountGrid1 = UpdateSolarGrid1Data(selectedPanelGrid1, registers[17].MaxValue, registers[24].MaxValue);
        }


        private void UpdateSolarGridPanelCount2()
        {
            MaxPanelCountGrid2 = UpdateSolarGrid1Data(selectedPanelGrid2, registers[18].MaxValue, registers[24].MaxValue);
        }
        private void CalculatedSellPower()
        {
            if (registers[12].Value < 0)
            {
                registers[26].Value += -1 * registers[12].Value;
            }
        }
        private void UpdateGeneratorInfo()
        {
            genPowerIn.Text = registers[14].Value.ToString();
            genVoltage.Text = registers[15].Value.ToString();
            genMaxPower.Text = registers[14].MaxValue.ToString();

        }
        private void UpdateMainsGridInfo()
        {
            mainsGridVoltage.Text = registers[13].Value.ToString();
            mainsGridPowerIn.Text = registers[12].Value.ToString();
        }
        private void UpdateOutGridInfo()
        {
            invertorPowerOut.Text = registers[28].Value.ToString();
            invertorVoltageOut.Text = registers[27].Value.ToString();
            maxPowerOut.Text = "MaxPowerOut:" + registers[28].MaxValue.ToString();
            powerSale.Text = "Amount Sold:" + registers[26].Value.ToString();
        }

        private void UpdateMaxChargeCurrency(float currency)
        {
            registers[6].Value = currency;
            registers[22].MaxValue = currency;
        }

        private void UpdateMaxDisChargeCurrency(float currency)
        {
            registers[7].Value = currency;
            registers[23].MaxValue = currency;
        }
        private void chargePowerBattery_ValueChanged(object sender, EventArgs e)
        {
            UpdateMaxChargeCurrency((float)chargePowerBattery.Value);
        }

        private void dischargePowerBatery_ValueChanged(object sender, EventArgs e)
        {
            UpdateMaxDisChargeCurrency((float)dischargePowerBatery.Value);
        }
        private bool UpdateSolarGrid1PanelCount(NumericUpDown countPanelGrid, int maxPanelCountGrid, SolarPanel selectedPanelGrid, float maxGridPower, float minGridVoltage, float maxSecondGridPower, float maxSolGridInputSum)
        {
            int formPanelCount = (int)countPanelGrid.Value;
            if (formPanelCount <= maxPanelCountGrid)
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
            CheckRegisterOverLimitError();
        }


        private void CheckRegisterOverLimitError()
        {
            foreach (var register in registers)
            {
                if (register.Value > register.MaxValue || register.Value < register.MinValue && register.Value != 0)
                {
                    register.Value = register.MaxValue;
                    SetRegisterErrorOverLimit(register);
                    return;
                }
            }

        }
        private bool BatteryConnected()
        {
            if (registers[0].Value == 0)
            {
                return false;
            }
            return true;
        }
        private void SetOutPowerLowError(float deficitePower)
        {
            registers[11].Value = registers[36].RegisterNumber;
            registers[36].Value = deficitePower;

        }
        private void SetRegisterErrorOverLimit(InverterRegister register)//поиск ошибки OverLimit, если есть, добавление в регистр ошибок, регистр RegisterOverLimitError
        {
            registers[10].Value = register.RegisterNumber;//Привязать к RegisterNumber
            registers[11].Value = registers[10].RegisterNumber;
        }


        //generation

        private void SetGeneratorMaxPower(float power)
        {
            if (power > 0)
            {
                registers[14].MaxValue = power;
                registers[25].Value = 1;
            }
            else
            {
                registers[14].MaxValue = 0;
                registers[25].Value = 0;
            }

        }
        private void SetGeneratorPower(float power)
        {

            if (registers[25].Value == 1 && registers[15].Value > 0)
            {
                if (registers[14].MaxValue > power)
                {
                    registers[14].Value = power;
                }
                else
                {
                    registers[14].Value = registers[14].MaxValue;
                }
            }

        }
        private void GeneratorStart()
        {
            if (registers[14].MaxValue > 0)
                SetGeneratorVoltage(220);
        }



        private void SetMainsGridPower(float power)
        {
            if (registers[13].Value > 0)
            {
                if (registers[12].MaxValue > power)
                    registers[12].Value = power;
                else
                    registers[12].Value = registers[12].MaxValue;

            }
            else
            {
                registers[12].Value = 0;
            }

        }

        private void SetInvertorPowerOut(float power)
        {
            if (registers[27].Value > 0)
                registers[28].Value = power;

        }
        private void SetGeneratorVoltage(float voltage)
        {
            if (voltage == 0)
            {
                SetGeneratorPower(0);
                registers[15].Value = voltage;
                return;
            }

            if (registers[15].MaxValue >= voltage && voltage >= registers[15].MinValue)
            {
                registers[15].Value = voltage;
            }
            else
            {
                SetRegisterErrorOverLimit(registers[15]);
                SetGeneratorPower(0);
                registers[15].Value = 0;
            }
        }
        private void SetMainsGridVoltage(float voltage)
        {
            if (voltage == 0)
            {
                SetMainsGridPower(0);
                registers[13].Value = voltage;
                return;
            }
            if (registers[13].MaxValue >= voltage && voltage >= registers[13].MinValue)
            {
                registers[13].Value = voltage;

            }
            else
            {
                SetRegisterErrorOverLimit(registers[15]);
                SetMainsGridPower(0);
                registers[13].Value = 0;
            }
        }
        private void SetInvertorVoltageOut(float voltage)//220/230
        {
            if (voltage == 0)
            {
                SetInvertorPowerOut(0);
                registers[27].Value = voltage;
                return;
            }
            if (registers[27].MaxValue >= voltage && voltage >= registers[27].MinValue)
            {
                registers[27].Value = voltage;
            }
            else
            {
                SetRegisterErrorOverLimit(registers[27]);
                SetInvertorPowerOut(0);
                registers[27].Value = 0;

            }
        }


        ////simulator
        ///


        private float DischargeBattery(float dischargeI, InverterRegister dischargeLimit)
        {
            float dischargeCur = registers[23].Value;
            registers[23].Value = Math.Min(registers[7].Value - registers[23].Value, dischargeI / Inverter_efficiency);
            dischargeI = registers[23].Value - dischargeCur;

            float batteryCharge = registers[3].Value;
            float dischargeIa = dischargeI / 3600f;
            float setBattarycharge = Math.Max(registers[3].MaxValue * dischargeLimit.Value / 100f, registers[3].Value - dischargeIa);

            registers[3].Value = setBattarycharge;

            registers[0].Value = CalculateVoltageByMaxMinVoltageAndAh();

            registers[1].Value = CalculateBatteryCapacityInPercents();

            return (batteryCharge - setBattarycharge) * Inverter_efficiency * 3600f + 0.1f;
        }
        private float ChargeBattery(float chargeI, InverterRegister chargeLimit)
        {
            float chargeCur = registers[23].Value;
            registers[22].Value += Math.Min(registers[6].Value - registers[22].Value, chargeI);
            chargeI = registers[22].Value - chargeCur;

            float batteryCharge = registers[3].Value;
            float chargeIa = chargeI / 3600f;
            float setBattarycharge = Math.Min((chargeIa * Inverter_efficiency + registers[3].Value), registers[3].MaxValue * chargeLimit.Value / 100f);
            registers[3].Value = setBattarycharge;


            registers[0].Value = CalculateVoltageByMaxMinVoltageAndAh();

            registers[1].Value = CalculateBatteryCapacityInPercents();
            return (setBattarycharge - batteryCharge) / Inverter_efficiency * 3600f + 0.1f;

        }
        private void SetSimulatePowerUsageParametersToZero()
        {
            registers[23].Value = 0f;
            registers[22].Value = 0f;
            registers[(int)RegisterEnum.PowerConsumption].Value = 0f;
            SetGeneratorPower(0);
            SetMainsGridPower(0);
        }
        private float FindIForBatteryCharge()
        {
            float neededIa = registers[3].MaxValue * registers[35].Value / 100 - registers[3].Value;//доводимо батарею до заданого рівня
            float deltaIa = Math.Min(registers[6].Value / 3600, neededIa);
            return deltaIa * 3600;
        }

        private void SimulatePowerUsage()
        {
            SetSimulatePowerUsageParametersToZero();
            registers[(int)RegisterEnum.PowerConsumption].Value = invertorWorkPower + registers[28].Value;
            float invertorWokrP = registers[(int)RegisterEnum.PowerConsumption].Value;

            if (!BatteryConnected())
            {
                if (registers[13].Value > 0)
                {
                    SetMainsGridPower(invertorWokrP);

                    invertorWokrP -= registers[12].Value;
                }
                if (registers[25].Value == 1)
                {
                    if (registers[15].Value == 0)
                        GeneratorStart();

                    SetGeneratorPower(invertorWokrP);

                    invertorWokrP -= registers[14].Value;

                }
                if (invertorWokrP > 0)
                    SetOutPowerLowError(invertorWokrP);


                return;
            }

            if (invertorWokrP <= registers[16].Value)
            {//використовуємо сонячні панелі
                float solarPowerAvailble = registers[16].Value - invertorWokrP;

                if (registers[1].Value >= registers[35].Value)
                {//використовуємо сонячні панелі для продажу
                    registers[26].Value += solarPowerAvailble / 1000;
                }
                else
                {//використовуємо сонячні панелі для зарядки
                    float chargeI = solarPowerAvailble / registers[0].Value;

                    float deltaI = FindIForBatteryCharge();

                    if (chargeI >= deltaI)
                    {


                        chargeI -= ChargeBattery(deltaI, registers[35]);

                        if (chargeI > 0)
                        {//використовуємо сонячні панелі для зарядки і продажу
                            solarPowerAvailble = chargeI * registers[0].Value;//не знаю на який вольтаж множити

                            registers[26].Value += solarPowerAvailble / 1000;//перевірити часовий проміжок генерації
                        }
                    }
                    if (registers[1].Value < registers[35].Value)
                    {//якщо не вистачає потужності для заряду
                        float deficiteI = (deltaI - chargeI);
                        float deficiteP = deficiteI * registers[0].Value;

                        if (registers[31].Value == 1)
                        {
                            SetMainsGridPower(deficiteP);

                            ChargeBattery(registers[12].Value / registers[0].Value + chargeI, registers[35]);
                        }
                        else if (registers[30].Value == 1)
                        {
                            if (registers[15].Value == 0)
                                GeneratorStart();

                            SetGeneratorPower(deficiteP);
                            ChargeBattery(registers[14].Value / registers[0].Value + chargeI, registers[35]);
                        }
                        else
                        {
                            ChargeBattery(chargeI, registers[35]);
                        }

                    }
                }
            }
            else
            {
                float deficiteP = invertorWokrP - registers[16].Value;

                if (registers[1].Value > registers[21].Value)
                {// використовуємо сонячні панелі + батареї

                    if (registers[1].Value > registers[35].Value)
                    {
                        float workVoltage = registers[0].Value;
                        deficiteP -= DischargeBattery(Math.Min(deficiteP / registers[0].Value, registers[7].Value), registers[21]) * workVoltage;

                        if (deficiteP > 0)
                        {
                            if (registers[31].Value == 1)
                            {
                                SetMainsGridPower(deficiteP);
                                deficiteP -= registers[12].Value;
                            }
                            if (registers[30].Value == 1)
                            {
                                if (registers[15].Value == 0)
                                    GeneratorStart();

                                SetGeneratorPower(deficiteP);

                                deficiteP -= registers[14].Value;


                            }
                            if (deficiteP > 0)
                            {
                                SetOutPowerLowError(deficiteP);
                            }
                        }
                    }
                    else
                    {
                        if (registers[13].Value > 0)
                        {
                            SetMainsGridPower(deficiteP);

                            deficiteP -= registers[12].Value;
                        }
                        if (registers[25].Value == 1)
                        {
                            if (registers[15].Value == 0)
                                GeneratorStart();

                            SetGeneratorPower(deficiteP);

                            deficiteP -= registers[14].Value;

                        }
                        if (deficiteP > 0)
                        {
                            float workVoltage = registers[0].Value;
                            deficiteP -= DischargeBattery(Math.Min(deficiteP / registers[0].Value, registers[7].Value), registers[21]) * workVoltage;
                        }
                        else
                        {
                            float deltaI = FindIForBatteryCharge();

                            if (registers[31].Value == 1 && registers[13].Value > 0 && deltaI > 0)
                            {
                                float bufferI = (registers[12].MaxValue - registers[12].Value) / registers[0].Value;
                                float charge = Math.Min(bufferI, deltaI);
                                SetMainsGridPower(charge * registers[0].Value + registers[12].Value);

                                deltaI -= ChargeBattery(charge, registers[35]);
                            }

                            if (registers[25].Value == 1 && registers[30].Value == 1 && deltaI > 0)
                            {
                                if (registers[15].Value == 0)
                                    GeneratorStart();
                                float bufferI = (registers[14].MaxValue - registers[14].Value) / registers[0].Value;
                                float charge = Math.Min(bufferI, deltaI);

                                SetGeneratorPower(ChargeBattery(charge, registers[35]) * registers[0].Value + registers[14].Value);

                            }


                        }
                        if (deficiteP > 0)
                        {
                            SetOutPowerLowError(deficiteP);
                        }


                    }
                }
                else
                {
                    float deltaI = FindIForBatteryCharge();

                    if (registers[13].Value > 0)
                    {
                        SetMainsGridPower(deficiteP);

                        deficiteP -= registers[12].Value;
                    }
                    if (registers[25].Value == 1)
                    {
                        if (registers[15].Value == 0)
                            GeneratorStart();

                        SetGeneratorPower(deficiteP);

                        deficiteP -= registers[14].Value;

                    }
                    if (deficiteP == 0)
                    {
                        if (registers[31].Value == 1 && registers[13].Value > 0 && deltaI > 0)
                        {
                            float bufferI = (registers[12].MaxValue - registers[12].Value) / registers[0].Value;
                            float charge = Math.Min(bufferI, deltaI);
                            SetMainsGridPower(charge * registers[0].Value + registers[12].Value);

                            deltaI -= ChargeBattery(charge, registers[35]);
                        }

                        if (registers[25].Value == 1 && registers[30].Value == 1 && deltaI > 0)
                        {
                            if (registers[15].Value == 0)
                                GeneratorStart();
                            float bufferI = (registers[14].MaxValue - registers[14].Value) / registers[0].Value;
                            float charge = Math.Min(bufferI, deltaI);

                            SetGeneratorPower(ChargeBattery(charge, registers[35]) * registers[0].Value + registers[14].Value);

                        }
                    }
                    else
                    {
                        SetOutPowerLowError(deficiteP);
                    }
                }
            }
        }
    }
}
