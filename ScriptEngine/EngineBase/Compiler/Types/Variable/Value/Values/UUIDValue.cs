﻿using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    public class UUIDValue : IValue
    {
        protected Guid _value;

        public ValueTypeEnum Type => ValueTypeEnum.GUID;

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
            return _value.ToString();
        }

        public int CompareTo(IValue other)
        {
            return AsString().CompareTo(other.AsString());
        }

        public bool Equals(IValue other)
        {
            return AsString().Equals(other.AsString());
        }
    }
}