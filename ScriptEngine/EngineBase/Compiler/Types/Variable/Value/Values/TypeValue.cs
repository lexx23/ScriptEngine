using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class TypeValue : IValue
    {
        private ValueTypeEnum _value;

        public ValueTypeEnum Type => ValueTypeEnum.TYPE;

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

        public object AsObject()
        {
            return _value;
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            throw new NotImplementedException();
        }

        public int CompareTo(IValue other)
        {
            throw new NotImplementedException();
        }

        public bool Equals(IValue other)
        {
            throw new NotImplementedException();
        }
    }
}
