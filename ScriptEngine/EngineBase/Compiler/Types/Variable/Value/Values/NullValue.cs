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
    class NullValue : IValue
    {
        public ValueTypeEnum BaseType => ValueTypeEnum.NULL;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get("Неопределено");

        public bool AsBoolean()
        {
            throw new Exception("Преобразование значения к типу Булево не может быть выполнено");
        }

        public DateTime AsDate()
        {
            throw new Exception("Преобразование значения к типу Дата не может быть выполнено"); 
        }

        public int AsInt()
        {
            throw new Exception("Преобразование значения к типу Число не может быть выполнено");
        }

        public decimal AsNumber()
        {
            throw new Exception("Преобразование значения к типу Число не может быть выполнено");
        }

        public IScriptObjectContext AsScriptObject()
        {
            throw new Exception("Преобразование значения к типу Обьект не может быть выполнено");
        }

        public string AsString()
        {
            return "";
        }

        public object AsObject()
        {
            return null;
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            if (other.BaseType == ValueTypeEnum.NULL)
                return true;

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.BaseType)
            {
                case ValueTypeEnum.NULL:
                    return 0;

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }

    }
}
