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
    class BooleanValue : IValue
    {
        private bool _value;

        public ValueTypeEnum BaseType => ValueTypeEnum.BOOLEAN;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get("Булево"); 

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

        public object AsObject()
        {
            return _value;
        }


        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.BOOLEAN)
                return _value == other.AsBoolean();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.BaseType)
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
