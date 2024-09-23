using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invertor_sim
{
    public class SolarPanel
    {
        public SolarPanel(string name, float openCircuitVoltage, float maxWorkСurrent, float maxWorkVoltage)
        {
            Name = name;
            OpenCircuitVoltage = openCircuitVoltage;
            MaxWorkСurrent = maxWorkСurrent;
            MaxWorkVoltage = maxWorkVoltage;
        }

        public string Name { get; set; }
        public float OpenCircuitVoltage { get; set; }
        public float MaxWorkСurrent { get; set; }
        public float MaxWorkVoltage { get; set; }

    }
}
