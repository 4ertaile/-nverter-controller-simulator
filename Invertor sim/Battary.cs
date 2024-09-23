using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invertor_sim
{
    public class Battery
    {
        public string Name { get; set; }
        public float NominalCapacity { get; set; } // Ah
        public float MaxVoltage { get; set; } // V
        public float MinVoltage { get; set; } // V
        public float NominalChargeDischargeCurrent { get; set; } // A

        public Battery(string name, float capacity, float maxV, float minV, float chargeCurrent)
        {
            Name = name;
            NominalCapacity = capacity;
            MaxVoltage = maxV;
            MinVoltage = minV;
            NominalChargeDischargeCurrent = chargeCurrent;
        }
    }
}
