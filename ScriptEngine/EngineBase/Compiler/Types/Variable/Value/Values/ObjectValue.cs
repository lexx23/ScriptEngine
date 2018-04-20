/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using System.Reflection;
using System.Linq;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values
{
    class ObjectValue : IValue
    {
        private object _value;

        public ValueTypeEnum BaseType => ValueTypeEnum.OBJECT;

        public InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(_value);

        public ObjectValue(object value)
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
            throw new NotImplementedException();
        }

        public string AsString()
        {
            if (!_value.GetType().IsEnum)
                return _value.ToString();
            else
                return EnumToString(_value.GetType());
        }

        private string EnumToString(Type type)
        {
            foreach (FieldInfo field in type.GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
            {
                EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First();
                if (attr.Value == _value.ToString() || field.Name == _value.ToString())
                    return attr.Value;
            }
            return "";
        }


        public object AsObject()
        {
            return _value;
        }


        public int CompareTo(IValue other)
        {
            throw new Exception("Операции сравнения на больше-меньше допустимы только для значений совпадающих примитивных типов (Булево, Число, Строка, Дата)");
        }

        public bool Equals(IValue other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;

            return AsString() == other.AsString();
        }
    }
}
