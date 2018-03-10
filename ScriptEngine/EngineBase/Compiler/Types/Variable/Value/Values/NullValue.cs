using System;
using System.Collections.Generic;
using System.Text;
using ScriptEngine.EngineBase.Interpreter.Context;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class NullValue : IValue
    {
        public ValueTypeEnum Type => ValueTypeEnum.NULL;

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

        public ScriptObjectContext AsScriptObject()
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

            if (other.Type == ValueTypeEnum.NULL)
                return true;

            return false;
        }

        public int CompareTo(IValue other)
        {
            switch (other.Type)
            {
                case ValueTypeEnum.NULL:
                    return 0;

                default:
                    throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
            }
        }

    }
}
