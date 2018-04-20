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
    public class ScriptObjectValue : IValue
    {
        protected IScriptObjectContext _value;

        public ValueTypeEnum BaseType => ValueTypeEnum.SCRIPT_OBJECT;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(_value);

        public ScriptObjectValue()
        {
        }


        public ScriptObjectValue(IScriptObjectContext value)
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

        public IScriptObjectContext AsScriptObject()
        {
            return _value;
        }

        public string AsString()
        {
            if (_value != null)
                return _value.Module?.Name;
            else
                return "Неопределено";
        }

        public object AsObject()
        {
            return _value.Instance;
        }


        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.SCRIPT_OBJECT)
                return _value.Module?.Name == other.AsScriptObject().Module?.Name && _value.Instance == other.AsScriptObject().Instance;

            return false;
        }

        public int CompareTo(IValue other)
        {

            throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
        }
    }
}
