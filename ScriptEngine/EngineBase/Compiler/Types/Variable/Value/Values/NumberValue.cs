using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    class NumberValue : IValue
    {
        private decimal _value;

        public ValueTypeEnum Type => ValueTypeEnum.NUMBER;

        public NumberValue(decimal value)
        {
            _value = value;
        }

        public bool AsBoolean()
        {
            if (_value != 0)
                return true;

            return false;
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            return (int)_value;
        }

        public decimal AsNumber()
        {
            return _value;
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value.ToString("n3");
        }

        public object AsObject()
        {
            return _value;
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.NUMBER)
                return _value == other.AsNumber();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.Type)
            {
                case ValueTypeEnum.NUMBER:
                    return _value.CompareTo(other.AsNumber());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }
    }
}
