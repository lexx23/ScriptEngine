using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class DateValue : IValue
    {
        private DateTime _value;

        public ValueTypeEnum Type => ValueTypeEnum.DATE;

        public bool ReadOnly => throw new NotImplementedException();

        public DateValue(DateTime value)
        {
            _value = value;
        }

        public bool AsBoolean()
        {
            throw new NotImplementedException();
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            throw new NotImplementedException();
        }

        public decimal AsNumber()
        {
            return _value.Ticks;
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value.ToString("dd.MM.yyyy hh.mm:ss");
        }

        public IValue Clone()
        {
            return ValueFactory.Create(_value);
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.DATE)
                return _value == other.AsDate();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.Type)
            {
                case ValueTypeEnum.DATE:
                    return _value.CompareTo(other.AsDate());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }
    }
}
