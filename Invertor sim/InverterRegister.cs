using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Invertor_sim
{

    public class InverterRegister
    {

        public ushort RegisterNumber { get; set; }

        public float Value { get; set; }
        public string Unit { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public string Description { get; set; }

        public InverterRegister(RegisterEnum name, float value, string unit, float min, float max)
        {
            RegisterNumber = (ushort)name;
            Value = value;
            Unit = unit;
            MinValue = min;
            MaxValue = max;
            Description = GetEnumDescription(name);
        }

        public ushort GetRegisterData()
        {
            return (ushort)(Value * 100);
        }


        private string GetEnumDescription(RegisterEnum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);

            return (attributes.Length > 0) ? attributes[0].Description : value.ToString();
        }
    }
}

    
