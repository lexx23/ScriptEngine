using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    class BooleanValue : IValue
    {
        private bool _value;

        public ValueTypeEnum Type => ValueTypeEnum.BOOLEAN;
        public bool ReadOnly { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public BooleanValue(bool value)
        {
            _value = value;
        }


        public bool AsBoolean()
        {
            return _value;
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            return _value == true ? 1 : 0;
        }

        public decimal AsNumber()
        {
            return _value == true ? 1 : 0;
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value == true ? "Да" : "Нет";
        }

        public IValue Clone()
        {
            return ValueFactory.Create(_value);
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.BOOLEAN)
                return _value == other.AsBoolean();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.Type)
            {
                case ValueTypeEnum.BOOLEAN:
                    return _value.CompareTo(other.AsBoolean());

                case ValueTypeEnum.NUMBER:
                    return AsNumber().CompareTo(other.AsNumber());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }
    }
}
