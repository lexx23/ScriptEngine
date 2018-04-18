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
    class TypeValue : IValue
    {
        private InternalScriptType _value;
        public ValueTypeEnum BaseType => ValueTypeEnum.TYPE;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get("Тип"); 

        public TypeValue(InternalScriptType value)
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
            return _value.Index;
        }

        public decimal AsNumber()
        {
            return _value.Index; ;
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
            return _value.Description;
        }

        public int CompareTo(IValue other)
        {
            return _value.Index.CompareTo(other.AsInt());
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.TYPE)
                return _value.Index == other.AsInt();

            return false;
        }
    }
}
