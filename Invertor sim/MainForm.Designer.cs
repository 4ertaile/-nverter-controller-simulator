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
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.sim = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.chart)).BeginInit();
            this.SuspendLayout();
            // 
            // dateTimePicker
            // 
            this.dateTimePicker.CustomFormat = "MM-dd-yyyy dddd";
            this.dateTimePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker.Location = new System.Drawing.Point(11, 12);
            this.dateTimePicker.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(224, 26);
            this.dateTimePicker.TabIndex = 0;
            // 
            // parameterComboBox
            // 
            this.parameterComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.parameterComboBox.FormattingEnabled = true;
            this.parameterComboBox.Items.AddRange(new object[] {
            Invertor_sim.Parameter.Input_Voltage,
            Invertor_sim.Parameter.Battery_Voltage,
            Invertor_sim.Parameter.Battery_Percentage,
            Invertor_sim.Parameter.Solar_Panel_Voltage,
            Invertor_sim.Parameter.Solar_Generation_Power,
            Invertor_sim.Parameter.User_Power_Usage});
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
            this.minBox.Location = new System.Drawing.Point(59, 602);
            this.minBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.minBox.Name = "minBox";
            this.minBox.Size = new System.Drawing.Size(112, 26);
            this.minBox.TabIndex = 3;
            // 
            // maxBox
            // 
            this.maxBox.Location = new System.Drawing.Point(227, 602);
            this.maxBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.maxBox.Name = "maxBox";
            this.maxBox.Size = new System.Drawing.Size(112, 26);
            this.maxBox.TabIndex = 4;
            // 
            // chartButton
            // 
            this.chartButton.Location = new System.Drawing.Point(361, 602);
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
            this.averageLabel.Location = new System.Drawing.Point(17, 647);
            this.averageLabel.Name = "averageLabel";
            this.averageLabel.Size = new System.Drawing.Size(35, 20);
            this.averageLabel.TabIndex = 6;
            this.averageLabel.Text = "text";
            // 
            // generateButton
            // 
            this.generateButton.Location = new System.Drawing.Point(361, 643);
            this.generateButton.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.generateButton.Name = "generateButton";
            this.generateButton.Size = new System.Drawing.Size(84, 29);
            this.generateButton.TabIndex = 7;
            this.generateButton.Text = "Generate";
            this.generateButton.UseVisualStyleBackColor = true;
            this.generateButton.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(17, 605);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(38, 20);
            this.label1.TabIndex = 8;
            this.label1.Text = "Min:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(179, 605);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 20);
            this.label2.TabIndex = 9;
            this.label2.Text = "Max:";
            // 
            // sim
            // 
            this.sim.Location = new System.Drawing.Point(853, 9);
            this.sim.Name = "sim";
            this.sim.Size = new System.Drawing.Size(126, 36);
            this.sim.TabIndex = 10;
            this.sim.Text = "Simulator";
            this.sim.UseVisualStyleBackColor = true;
            this.sim.Click += new System.EventHandler(this.sim_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(991, 700);
            this.Controls.Add(this.sim);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
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
        private Label label1;
        private Label label2;
        private Button sim;
    }
}