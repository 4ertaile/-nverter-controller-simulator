using System;

namespace Invertor_sim
{

    public class InverterData
    {
        public DateTime Time { get; set; }               // Час запису
        public double InputVoltage { get; set; }         // Вольтаж на вході
        public double BatteryVoltage { get; set; }       // Вольтаж батареї
        public double BatteryPercentage { get; set; }    // Запас потужності батареї у відсотках
        public double SolarPanelVoltage { get; set; }    // Вхідний вольтаж з сонячних панелей
        public double SolarGenerationPower { get; set; } // Потужність генерації сонячних панелей
        public double UserPowerUsage { get; set; }       // Потужність, яку використовує користувач
    }
}