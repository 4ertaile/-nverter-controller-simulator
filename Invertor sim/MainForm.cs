using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Invertor_sim
{
    public partial class MainForm : Form
    {
        private List<InverterData> inverterDataList;
        private bool isDrawing = false;
        private int currentXValue = 0;
        private string currentParameter;
        private Dictionary<string, List<double>> parameterValues = new Dictionary<string, List<double>>();

        public MainForm()
        {
            InitializeComponent();
        }

        // Вибір параметра зі списку
        private void ParameterComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            currentParameter = parameterComboBox.SelectedItem.ToString();
            chart.Series["Data"].Points.Clear(); // Очищуємо графік
            currentXValue = 0;

            // Якщо для параметра вже є дані, завантажуємо їх у графік
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
            if (!double.TryParse(minBox.Text, out double minValue) ||
                !double.TryParse(maxBox.Text, out double maxValue) || minValue >= maxValue)
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
            parameterValues[currentParameter] = new List<double>(); // Очищуємо дані для поточного параметра
        }

        // Подія малювання на чарті за допомогою миші
        private void Chart_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isDrawing || e.Button != MouseButtons.Left)
                return;

            ChartArea chartArea = chart.ChartAreas[0];
            double yValue = chartArea.AxisY.PixelPositionToValue(e.Y);

            // Обмежуємо значення Y в межах мінімальних і максимальних значень
            double minValue = double.Parse(minBox.Text);
            double maxValue = double.Parse(maxBox.Text);
            yValue = Math.Max(minValue, Math.Min(maxValue, yValue));

            // Додаємо точку до графіка повільніше (кожний 5-й піксель)
            if (currentXValue <= 100)//&& currentXValue % 5 == 0
            {
                chart.Series["Data"].Points.AddXY(currentXValue, yValue);
                parameterValues[currentParameter].Add(yValue);
                currentXValue++;

                // Оновлюємо середнє значення
                UpdateAverageLabel();
            }

            // Якщо графік заповнений, завершити малювання
            if (currentXValue > 100)
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
                double average = 0;
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

            while (currentTime < endDate)
            {
                var data = new InverterData
                {
                    Time = currentTime,
                    InputVoltage = GetParameterValue("Input Voltage"),
                    BatteryVoltage = GetParameterValue("Battery Voltage"),
                    BatteryPercentage = GetParameterValue("Battery Percentage"),
                    SolarPanelVoltage = GetParameterValue("Solar Panel Voltage"),
                    SolarGenerationPower = GetParameterValue("Solar Generation Power"),
                    UserPowerUsage = GetParameterValue("User Power Usage")
                };

                inverterDataList.Add(data);
                currentTime = currentTime.AddMinutes(5);
            }

            SaveDataToFile();
        }

        // Отримання середнього значення для параметра
        private double GetParameterValue(string parameter)
        {
            if (parameterValues.ContainsKey(parameter) && parameterValues[parameter].Count > 0)
            {
                return parameterValues[parameter].Average();
            }
            return 0; // Якщо даних немає, повертаємо 0
        }

        // Збереження даних у файл
        private void SaveDataToFile()
        {
            string date = dateTimePicker.Value.ToString("yyyy-MM-dd");
            string fileName = $"InverterData_{date}.json";
            string jsonString = System.Text.Json.JsonSerializer.Serialize(inverterDataList, new JsonSerializerOptions { WriteIndented = true });

            File.WriteAllText(fileName, jsonString);
            MessageBox.Show($"Data saved to {fileName}");
        }
    }

}
