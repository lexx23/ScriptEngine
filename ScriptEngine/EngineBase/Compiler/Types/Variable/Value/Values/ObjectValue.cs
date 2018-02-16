using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class ObjectValue : IValue
    {
        private IValue _internal_value;
        private object _value;

        public ValueTypeEnum Type => ValueTypeEnum.OBJECT;


        public ObjectValue(object value,IValue internal_value)
        {
            _value = value;
            _internal_value = internal_value;
        }


        public bool AsBoolean()
        {
            return _internal_value.AsBoolean();
        }

        public DateTime AsDate()
        {
            return _internal_value.AsDate();
        }

        public int AsInt()
        {
            return _internal_value.AsInt();
        }

        public decimal AsNumber()
        {
            return _internal_value.AsNumber();
        }

        public ScriptObjectContext AsScriptObject()
        {
            return _internal_value.AsScriptObject();
        }

        public string AsString()
        {
            return _internal_value.AsString();
        }

        public object AsObject()
        {
            return _value;
        }


        public int CompareTo(IValue other)
        {
            throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.STRING)
                return _value == other.AsObject();

            return false;
        }
    }
}
