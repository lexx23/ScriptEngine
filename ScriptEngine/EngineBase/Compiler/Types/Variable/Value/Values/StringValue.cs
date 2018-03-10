using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class StringValue : IValue
    {
        private string _value;

        public ValueTypeEnum Type => ValueTypeEnum.STRING;

        public StringValue(string value)
        {
            if (value == null)
                value = "";
            _value = value;
        }

        public bool AsBoolean()
        {
            if (String.Equals(_value,"ложь",StringComparison.OrdinalIgnoreCase) || String.Equals(_value,"false", StringComparison.OrdinalIgnoreCase))
                return false;
            if (String.Equals(_value,"истина", StringComparison.OrdinalIgnoreCase) || String.Equals(_value,"true", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            return int.Parse(_value);
        }

        public decimal AsNumber()
        {
            if (decimal.TryParse(_value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                return result;
            throw new Exception($"Невозможно преобразовать [{_value}] в число.");
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value;
        }

        public object AsObject()
        {
            return _value;
        }


        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.STRING)
                return _value == other.AsString();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.Type)
            {
                case ValueTypeEnum.STRING:
                    return _value.CompareTo(other.AsString());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }

    }
}
