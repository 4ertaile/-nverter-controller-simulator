using System.Collections.Generic;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;

namespace Invertor_sim
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.dateTimePicker = new System.Windows.Forms.DateTimePicker();
            this.parameterComboBox = new System.Windows.Forms.ComboBox();
            this.chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.minBox = new System.Windows.Forms.TextBox();
            this.maxBox = new System.Windows.Forms.TextBox();
            this.chartButton = new System.Windows.Forms.Button();
            this.averageLabel = new System.Windows.Forms.Label();
            this.generateButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.Location = new System.Drawing.Point(11, 12);
            this.dateTimePicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(224, 26);
            this.dateTimePicker.TabIndex = 0;
            // Set the Format type and the CustomFormat string.
            this.dateTimePicker.Format = DateTimePickerFormat.Custom;
            dateTimePicker.CustomFormat = "MM-dd-yyyy dddd";
            // 
            // parameterComboBox
            // 
            this.parameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parameterComboBox.FormattingEnabled = true;
            this.parameterComboBox.Items.AddRange(new object[] {
            "Input Voltage",
            "Battery Voltage",
            "Battery Percentage",
            "Solar Panel Voltage",
            "Solar Generation Power",
            "User Power Usage"});
            this.parameterComboBox.Location = new System.Drawing.Point(11, 50);
            this.parameterComboBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.parameterComboBox.Name = "parameterComboBox";
            this.parameterComboBox.Size = new System.Drawing.Size(224, 28);
            this.parameterComboBox.TabIndex = 1;
            this.parameterComboBox.SelectedIndexChanged += new System.EventHandler(this.ParameterComboBox_SelectedIndexChanged);
            // 
            // chart
            // 
            chartArea1.AxisX.LabelStyle.Format = "F0";
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.AxisX.Maximum = 1440D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.LabelStyle.Format = "F2";
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.Silver;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dot;
            chartArea1.Name = "ChartArea1";
            this.chart.ChartAreas.Add(chartArea1);
            this.chart.Location = new System.Drawing.Point(11, 88);
            this.chart.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Data";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(900, 500);
            this.chart.TabIndex = 2;
            this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseMove);
            // 
            // minBox
            // 
            this.minBox.Location = new System.Drawing.Point(11, 600);
            this.minBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.minBox.Name = "minBox";
            this.minBox.Size = new System.Drawing.Size(112, 26);
            this.minBox.TabIndex = 3;
            // 
            // maxBox
            // 
            this.maxBox.Location = new System.Drawing.Point(135, 600);
            this.maxBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.maxBox.Name = "maxBox";
            this.maxBox.Size = new System.Drawing.Size(112, 26);
            this.maxBox.TabIndex = 4;
            // 
            // chartButton
            // 
            this.chartButton.Location = new System.Drawing.Point(259, 600);
            this.chartButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chartButton.Name = "chartButton";
            this.chartButton.Size = new System.Drawing.Size(84, 29);
            this.chartButton.TabIndex = 5;
            this.chartButton.Text = "Draw";
            this.chartButton.UseVisualStyleBackColor = true;
            this.chartButton.Click += new System.EventHandler(this.ChartButton_Click);
            // 
            // averageLabel
            // 
            this.averageLabel.AutoSize = true;
            this.averageLabel.Location = new System.Drawing.Point(11, 638);
            this.averageLabel.Name = "averageLabel";
            this.averageLabel.Size = new System.Drawing.Size(0, 20);
            this.averageLabel.TabIndex = 6;
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(259, 638);
            this.generateButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(84, 29);
            this.generateButton.TabIndex = 7;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 700);
            this.Controls.Add(this.generateButton);
            this.Controls.Add(this.averageLabel);
            this.Controls.Add(this.chartButton);
            this.Controls.Add(this.maxBox);
            this.Controls.Add(this.minBox);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.parameterComboBox);
            this.Controls.Add(this.dateTimePicker);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "MainForm";
            this.Text = "Invertor Simulator";
            ((System.ComponentModel.ISupportInitialize)(this.chart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker dateTimePicker;
        private System.Windows.Forms.ComboBox parameterComboBox;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart;
        private System.Windows.Forms.TextBox minBox;
        private System.Windows.Forms.TextBox maxBox;
        private System.Windows.Forms.Button chartButton;
        private System.Windows.Forms.Label averageLabel;
        private System.Windows.Forms.Button generateButton;
    }
}