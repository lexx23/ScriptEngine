using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class ScriptObjectValue : IValue
    {
        private ScriptObjectContext _value;

        public ValueTypeEnum Type => ValueTypeEnum.SCRIPT_OBJECT;

        public bool ReadOnly => throw new NotImplementedException();

        public ScriptObjectValue(ScriptObjectContext value)
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
            throw new NotImplementedException();
        }

        public ScriptObjectContext AsScriptObject()
        {
            return _value;
        }

        public string AsString()
        {
            return _value.ModuleName;
        }

        public object AsObject()
        {
            return _value;
        }


        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.Type == ValueTypeEnum.SCRIPT_OBJECT)
                return _value.ModuleName == other.AsScriptObject().ModuleName && _value.Context == other.AsScriptObject().Context;

            return false;
        }

        public int CompareTo(IValue other)
        {

            throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
        }
    }
}
