using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Invertor_sim
{
    public partial class MainForm : Form
    {
        
        const int minutsInDay = 1440;
        private List<InverterData> inverterDataList;
        private bool isDrawing = false;
        private int currentXValue = 0;
        private string currentParameter;
        private Dictionary<string, List<float>> parameterValues = new Dictionary<string, List<float>>();
        private Simulator simulator;

        public MainForm()
        {
            InitializeComponent();
            parameterComboBox.SelectedIndex = 0;
        }
        public void AddSimulatorForm(Simulator sim)
        {
            simulator = sim;
        }
        // Вибір параметра зі списку
        private void ParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentParameter = parameterComboBox.SelectedItem.ToString();

            // Перевіряємо, чи існує параметр у словнику, якщо ні — додаємо його
            if (!parameterValues.ContainsKey(currentParameter))
            {
                parameterValues[currentParameter] = new List<float>();
            }

            chart.Series["Data"].Points.Clear(); // Очищуємо графік
            currentXValue = 0;

            // Додаємо точки з параметра
            if (parameterValues.ContainsKey(currentParameter))
            {
                foreach (var value in parameterValues[currentParameter])
                {
                    chart.Series["Data"].Points.AddXY(currentXValue, value);
                    currentXValue++;
                }
            }

            UpdateAverageLabel();
        }

        // Подія натискання на кнопку "Draw"
        private void ChartButton_Click(object sender, EventArgs e)
        {
            // Перевірка, чи введені коректні мінімальні і максимальні значення
            if (!float.TryParse(minBox.Text, out float minValue) ||
                !float.TryParse(maxBox.Text, out float maxValue) || minValue >= maxValue)
            {
                MessageBox.Show("Please enter valid minimum and maximum values. Maximum must be greater than minimum.");
                return;
            }

            // Налаштування осі Y для вибраного чарта
            chart.ChartAreas[0].AxisY.Minimum = minValue;
            chart.ChartAreas[0].AxisY.Maximum = maxValue;

            // Почати малювання на чарті
            isDrawing = true;
            currentXValue = 0; // Починаємо з 0 по осі X
            chart.Series["Data"].Points.Clear(); // Очистити попередні точки
            parameterValues[currentParameter] = new List<float>(); // Очищуємо дані для поточного параметра
        }

        // Подія малювання на чарті за допомогою миші
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || e.Button != MouseButtons.Left)
                return;

            ChartArea chartArea = chart.ChartAreas[0];
            float yValue = (float)chartArea.AxisY.PixelPositionToValue(e.Y);

            float minValue = float.Parse(minBox.Text);
            float maxValue = float.Parse(maxBox.Text);
            yValue = Math.Max(minValue, Math.Min(maxValue, yValue));

            // Перевіряємо, чи існує параметр у словнику, якщо ні — додаємо його
            if (!parameterValues.ContainsKey(currentParameter))
            {
                parameterValues[currentParameter] = new List<float>();
            }

            if (currentXValue <= minutsInDay)
            {
                chart.Series["Data"].Points.AddXY(currentXValue, yValue);
                parameterValues[currentParameter].Add(yValue);
                currentXValue++;

                // Оновлюємо середнє значення
                UpdateAverageLabel();
            }

            // Якщо графік заповнений, завершити малювання
            if (currentXValue > minutsInDay)
            {
                isDrawing = false;
                MessageBox.Show("Chart drawing completed.");
            }
        }

        // Оновлення середнього значення для параметра
        private void UpdateAverageLabel()
        {
            if (parameterValues.ContainsKey(currentParameter))
            {
                float average = 0;
                if (parameterValues[currentParameter].Count > 0)
                {
                    average = parameterValues[currentParameter].Average();
                }
                averageLabel.Text = $"Average {currentParameter}: {average:F2}";
            }
            else
            {
                averageLabel.Text = $"Average {currentParameter}: N/A";
            }
        }

        // Генерація даних після заповнення всіх графіків
        private void GenerateButton_Click(object sender, EventArgs e)
        {
            inverterDataList = new List<InverterData>();

            DateTime startDate = dateTimePicker.Value.Date;
            DateTime endDate = startDate.AddDays(1);
            DateTime currentTime = startDate;

            int xValue = 0; // Initialize xValue to 0

            while (currentTime < endDate)
            {
                var data = new InverterData
                {
                    Time = currentTime.ToString("H:mm:ss"),
                    InputVoltage = GetParameterValueAtCurrentX(Parameter.Input_Voltage.ToString(), xValue),
                    BatteryVoltage = GetParameterValueAtCurrentX(Parameter.Battery_Voltage.ToString(), xValue),
                    BatteryPercentage = GetParameterValueAtCurrentX(Parameter.Battery_Percentage.ToString(), xValue),
                    SolarPanelVoltage = GetParameterValueAtCurrentX(Parameter.Solar_Panel_Voltage.ToString(), xValue),
                    SolarGenerationPower = GetParameterValueAtCurrentX(Parameter.Solar_Generation_Power.ToString(), xValue),
                    UserPowerUsage = GetParameterValueAtCurrentX(Parameter.User_Power_Usage.ToString(), xValue)
                };
                inverterDataList.Add(data);
                currentTime = currentTime.AddMinutes(5);
                xValue += 5; // Increment xValue for the next iteration
            }

            SaveDataToFile();
        }

        // Метод для отримання значення параметра в певний момент часу з округленням до 2 знаків після коми
        private float GetParameterValueAtCurrentX(string parameter, int xValue)
        {
            if (parameterValues.ContainsKey(parameter) && parameterValues[parameter].Count > xValue)
            {
                // Округлюємо до 2 знаків після коми
                return (float)Math.Round(parameterValues[parameter][xValue], 2);
            }
            return 0;
        }

        // Збереження даних у файл із округленими значеннями
        private void SaveDataToFile()
        {
            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");
            string fileName = $"InverterData_{date}.json";

            // Округлюємо кожне значення перед серіалізацією
            var roundedInverterDataList = inverterDataList.Select(data => new InverterData
            {
                Time = data.Time,
                InputVoltage = (float)Math.Round(data.InputVoltage, 2),
                BatteryVoltage = (float)Math.Round(data.BatteryVoltage, 2),
                BatteryPercentage = (float)Math.Round(data.BatteryPercentage, 2),
                SolarPanelVoltage = (float)Math.Round(data.SolarPanelVoltage, 2),
                SolarGenerationPower = (float)Math.Round(data.SolarGenerationPower, 2),
                UserPowerUsage = (float)Math.Round(data.UserPowerUsage, 2)
            }).ToList();

            string jsonString = JsonConvert.SerializeObject(roundedInverterDataList, Formatting.Indented);

            File.WriteAllText(fileName, jsonString);
            MessageBox.Show($"Data saved to {fileName}");
        }

        private void sim_Click(object sender, EventArgs e)
        {
            this.Hide();
            if (simulator == null || simulator.IsDisposed)
            {
                Simulator sim = new Simulator(this);
                simulator = sim;
            }
            simulator.Show();
            simulator.Location = this.Location;
        }
    }
}
