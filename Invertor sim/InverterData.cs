using System;

namespace Invertor_sim
{

    public class InverterData
    {
        public string Time { get; set; }               // Час запису
        public float InputVoltage { get; set; }         // Вольтаж на вході
        public float BatteryVoltage { get; set; }       // Вольтаж батареї
        public float BatteryPercentage { get; set; }    // Запас потужності батареї у відсотках
        public float SolarPanelVoltage { get; set; }    // Вхідний вольтаж з сонячних панелей
        public float SolarGenerationPower { get; set; } // Потужність генерації сонячних панелей
        public float UserPowerUsage { get; set; }       // Потужність, яку використовує користувач
    }
}