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
            this.chartWriteButton = new System.Windows.Forms.Button();
            this.averageLabel = new System.Windows.Forms.Label();
            this.generateFileBut = new System.Windows.Forms.Button();
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
            this.dateTimePicker.Location = new System.Drawing.Point(7, 8);
            this.dateTimePicker.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.dateTimePicker.Name = "dateTimePicker";
            this.dateTimePicker.Size = new System.Drawing.Size(151, 20);
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
            this.parameterComboBox.Location = new System.Drawing.Point(7, 32);
            this.parameterComboBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.parameterComboBox.Name = "parameterComboBox";
            this.parameterComboBox.Size = new System.Drawing.Size(151, 21);
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
            this.chart.Location = new System.Drawing.Point(7, 57);
            this.chart.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chart.Name = "chart";
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Name = "Data";
            this.chart.Series.Add(series1);
            this.chart.Size = new System.Drawing.Size(600, 325);
            this.chart.TabIndex = 2;
            this.chart.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Chart_MouseMove);
            // 
            // minBox
            // 
            this.minBox.Location = new System.Drawing.Point(39, 391);
            this.minBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.minBox.Name = "minBox";
            this.minBox.Size = new System.Drawing.Size(76, 20);
            this.minBox.TabIndex = 3;
            // 
            // maxBox
            // 
            this.maxBox.Location = new System.Drawing.Point(151, 391);
            this.maxBox.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.maxBox.Name = "maxBox";
            this.maxBox.Size = new System.Drawing.Size(76, 20);
            this.maxBox.TabIndex = 4;
            // 
            // chartWriteButton
            // 
            this.chartWriteButton.Location = new System.Drawing.Point(241, 391);
            this.chartWriteButton.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chartWriteButton.Name = "chartWriteButton";
            this.chartWriteButton.Size = new System.Drawing.Size(56, 19);
            this.chartWriteButton.TabIndex = 5;
            this.chartWriteButton.Text = "Draw";
            this.chartWriteButton.UseVisualStyleBackColor = true;
            this.chartWriteButton.Click += new System.EventHandler(this.ChartButton_Click);
            // 
            // averageLabel
            // 
            this.averageLabel.AutoSize = true;
            this.averageLabel.Location = new System.Drawing.Point(11, 421);
            this.averageLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.averageLabel.Name = "averageLabel";
            this.averageLabel.Size = new System.Drawing.Size(24, 13);
            this.averageLabel.TabIndex = 6;
            this.averageLabel.Text = "text";
            // 
            // generateFileBut
            // 
            this.generateFileBut.Location = new System.Drawing.Point(241, 418);
            this.generateFileBut.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.generateFileBut.Name = "generateFileBut";
            this.generateFileBut.Size = new System.Drawing.Size(56, 19);
            this.generateFileBut.TabIndex = 7;
            this.generateFileBut.Text = "Generate";
            this.generateFileBut.UseVisualStyleBackColor = true;
            this.generateFileBut.Click += new System.EventHandler(this.GenerateButton_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 393);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(27, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "Min:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(119, 393);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(30, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "Max:";
            // 
            // sim
            // 
            this.sim.Location = new System.Drawing.Point(569, 6);
            this.sim.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.sim.Name = "sim";
            this.sim.Size = new System.Drawing.Size(84, 23);
            this.sim.TabIndex = 10;
            this.sim.Text = "Simulator";
            this.sim.UseVisualStyleBackColor = true;
            this.sim.Click += new System.EventHandler(this.sim_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(661, 455);
            this.Controls.Add(this.sim);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.generateFileBut);
            this.Controls.Add(this.averageLabel);
            this.Controls.Add(this.chartWriteButton);
            this.Controls.Add(this.maxBox);
            this.Controls.Add(this.minBox);
            this.Controls.Add(this.chart);
            this.Controls.Add(this.parameterComboBox);
            this.Controls.Add(this.dateTimePicker);
            this.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
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
        private System.Windows.Forms.Button chartWriteButton;
        private System.Windows.Forms.Label averageLabel;
        private System.Windows.Forms.Button generateFileBut;
        private Label label1;
        private Label label2;
        private Button sim;
    }
}