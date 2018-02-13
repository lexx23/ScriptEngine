using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptEngine.EngineBase.Library.Attributes
{
    public class EnumStringAttribute : System.Attribute
    {
        private readonly string _value;
        public string Value { get => _value; }

        public EnumStringAttribute(string value)
        {
            _value = value;
        }

        public static string GetStringValue(Enum value)
        {
            string output = string.Empty;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            EnumStringAttribute[] attrs = fi.GetCustomAttributes(typeof(EnumStringAttribute), false) as EnumStringAttribute[];
            if (attrs.Length > 0)
                output = attrs[0].Value;

            return output;
        }

    }
}
