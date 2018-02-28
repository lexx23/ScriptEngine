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
            _value = value;
        }

        public bool AsBoolean()
        {
            if (_value.ToLower() == "ложь" || _value.ToLower() == "false")
                return false;
            if (_value.ToLower() == "истина" || _value.ToLower() == "true")
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
            return 0;
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
