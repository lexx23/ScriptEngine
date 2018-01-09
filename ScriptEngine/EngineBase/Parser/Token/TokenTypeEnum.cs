using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public enum TokenTypeEnum
    {
        [StringValue("Число")]
        NUMBER = 0,
        [StringValue("Литерал")]
        LITERAL = 1,
        [StringValue("Идентификатор")]
        IDENTIFIER = 2,
        [StringValue("Символ")]
        PUNCTUATION = 5
    }



    public class StringValue : System.Attribute
    {
        private readonly string _value;
        public string Value { get => _value; }

        public StringValue(string value)
        {
            _value = value;
        }
    }


    public static class StringEnum
    {
        public static string GetStringValue(Enum value)
        {
            string output = string.Empty;
            Type type = value.GetType();

            FieldInfo fi = type.GetField(value.ToString());
            StringValue[] attrs = fi.GetCustomAttributes(typeof(StringValue),false) as StringValue[];
            if (attrs.Length > 0)
                output = attrs[0].Value;

            return output;
        }
    }
}
