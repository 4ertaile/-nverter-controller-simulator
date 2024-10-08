using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
namespace Invertor_sim
{
    public enum RegisterEnum
    {
        [Description("Battery Voltage")]
        BatteryVoltage = 0,

        [Description("State of Charge (SOC)")]
        StateOfCharge = 1,

        [Description("Battery Blocks")]
        BatteryBlocks = 2,

        [Description("Battery Currency")]
        BatteryCurrency = 3,

        [Description("Max Voltage")]
        MaxVoltage = 4,

        [Description("Min Voltage")]
        MinVoltage = 5,

        [Description("Nominal Charge Current")]
        NominalChargeCurrent = 6,

        [Description("Nominal Discharge Current")]
        NominalDischargeCurrent = 7,

        [Description("Max Battery Power Capacity")]
        MaxBatteryPowerCapacity = 8,

        [Description("Battery Power")]
        BatteryPower = 9,

        [Description("OverLimit Error")]
        OverLimitError = 10,

        [Description("Error")]
        Error = 11,

        [Description("Input Grid Power")]
        InputGridPower = 12,

        [Description("Input Grid Voltage")]
        InputGridVoltage = 13,

        [Description("Input Generator Power")]
        InputGeneratorPower = 14,

        [Description("Input Generator Voltage")]
        InputGeneratorVoltage = 15,

        [Description("Input Solar Power")]
        InputSolarPower = 16,

        [Description("Input Solar Grid 1 Voltage")]
        InputSolarGrid1Voltage = 17,

        [Description("Input Solar Grid 2 Voltage")]
        InputSolarGrid2Voltage = 18,

        [Description("Input Solar Grid 1 Power")]
        InputSolarGrid1Power = 19,

        [Description("Input Solar Grid 2 Power")]
        InputSolarGrid2Power = 20,

        [Description("Minimum Battery Discharge")]
        MinimumBatteryDischarge = 21,

        [Description("Charge Current")]
        ChargeCurrent = 22,

        [Description("Discharge Current")]
        DischargeCurrent = 23,

        [Description("MPPT Voltage")]
        MPPTVoltage = 24,

        [Description("Generator Connected")]
        GeneratorConnected = 25,

        [Description("Sale Power")]
        SalePower = 26,

        [Description("Grid Voltage Out")]
        GridVoltageOut = 27,

        [Description("Grid Power Out")]
        GridPowerOut = 28,

        [Description("Battery Power Usage")]
        BatteryPowerUsage = 29,

        [Description("Use Generator For Charge Battery")]
        UseGeneratorForChargeBattery = 30,

        [Description("Use MainsGrid For Charge Battery")]
        UseMainsGridForChargeBattery = 31,

        [Description("Max Charge Current")]
        MaxChargeCurrent = 32,

        [Description("Max Discharge Current")]
        MaxDischargeCurrent = 33,

        [Description("Maximum Battery Charge")]
        MaximumBatteryCharge = 34,

        [Description("Maintaining The Battery Charge Level")]
        MaintainingBatteryChargeLevel = 35,

        [Description("Low Work Power")]
        LowWorkPower = 36
    }

}


