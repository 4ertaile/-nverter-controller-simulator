using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Invertor_sim
{

    public class InverterRegister
    {

        public ushort RegisterNumber { get; set; }

        private float _value;
        public float Value
        {
            get => _value;
            set
            {
                if ((value >= MinValue) && (value <= MaxValue))
                {
                    _value = value;
                }
            }
        }
        public string Unit { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public string Description { get; set; }

        public InverterRegister(ushort number, float value, string unit, float min, float max, string description)
        {
            RegisterNumber = number;
            Value = value;
            Unit = unit;
            MinValue = min;
            MaxValue = max;
            Description = description;
        }

        public ushort GetRegisterData()
        {
            return (ushort)(Value * 100);
        }
    }


}
