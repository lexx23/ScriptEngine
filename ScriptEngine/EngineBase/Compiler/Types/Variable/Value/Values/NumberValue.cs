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
    class NumberValue : IValue
    {
        private decimal _value;
        public ValueTypeEnum BaseType => ValueTypeEnum.NUMBER;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get("Число");

        public NumberValue(decimal value)
        {
            _value = value;
        }


        public bool AsBoolean()
        {
            if (_value != 0)
                return true;

            return false;
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            return (int)_value;
        }

        public decimal AsNumber()
        {
            return _value;
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value.ToString(System.Globalization.NumberFormatInfo.InvariantInfo);
        }

        public object AsObject()
        {
            return _value;
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.NUMBER)
                return _value == other.AsNumber();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.BaseType)
            {
                case ValueTypeEnum.NUMBER:
                    return _value.CompareTo(other.AsNumber());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }
    }
}
