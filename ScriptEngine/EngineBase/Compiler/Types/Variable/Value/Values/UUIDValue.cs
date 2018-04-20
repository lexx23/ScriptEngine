/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    public class UUIDValue : IValue
    {
        protected Guid _value;
        public ValueTypeEnum BaseType => ValueTypeEnum.GUID;

        public virtual InternalScriptType ScriptType { get; set; }

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

        public IScriptObjectContext AsScriptObject()
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
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.GUID)
                return AsString() == other.AsString();

            return false;
        }
    }
}
