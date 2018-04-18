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
    class StringValue : IValue
    {
        private string _value;
        public ValueTypeEnum BaseType => ValueTypeEnum.STRING;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get("Строка");

        public StringValue(string value)
        {
            if (value == null)
                value = "";
            _value = value;
        }

        public bool AsBoolean()
        {
            if (String.Equals(_value,"ложь",StringComparison.OrdinalIgnoreCase) || String.Equals(_value,"false", StringComparison.OrdinalIgnoreCase))
                return false;
            if (String.Equals(_value,"истина", StringComparison.OrdinalIgnoreCase) || String.Equals(_value,"true", StringComparison.OrdinalIgnoreCase))
                return true;

            return false;
        }

        public DateTime AsDate()
        {
            throw new NotImplementedException();
        }

        public int AsInt()
        {
            if (int.TryParse(_value, out int value))
                return value;
            else
                throw new Exception($"Невозможно преобразовать [{_value}] в число.");
        }

        public decimal AsNumber()
        {
            if (decimal.TryParse(_value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                return result;
            throw new Exception($"Невозможно преобразовать [{_value}] в число.");
        }

        public ScriptObjectContext AsScriptObject()
        {
            throw new NotImplementedException();
        }

        public string AsString()
        {
            return _value;
        }

        public object AsObject()
        {
            return _value;
        }


        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.STRING)
                return _value == other.AsString();

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.BaseType)
            {
                case ValueTypeEnum.STRING:
                    return _value.CompareTo(other.AsString());

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }

    }
}
