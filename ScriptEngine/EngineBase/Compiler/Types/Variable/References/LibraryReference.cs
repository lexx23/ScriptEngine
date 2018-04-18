/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Linq.Expressions;
using System.Reflection;
using System;
using ScriptEngine.EngineBase.Library;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class LibraryReference<T> : IVariableReference
    {
        private T _instance;
        private Func<T, IValue> _getter;
        private Action<T, IValue> _setter;

        /// <summary>
        /// Конструктор используется при клонировании класса.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public LibraryReference(T instance, Func<T, IValue> getter, Action<T, IValue> setter)
        {
            _instance = instance;
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Простой конструктор.
        /// </summary>
        /// <param name="instance_type"></param>
        /// <param name="property"></param>
        public LibraryReference(Type instance_type, PropertyInfo property)
        {
            _getter = CreateGetter(property);
            _setter = CreateSetter(property); ;
        }

        /// <summary>
        /// Создается геттер.
        /// </summary>
        /// <param name="property_info"></param>
        /// <returns></returns>
        private Func<T, IValue> CreateGetter(PropertyInfo property_info)
        {
            ParameterExpression instance_parameter = Expression.Parameter(typeof(T), "target");
            var getter = property_info.GetGetMethod();
            Expression call = Expression.Call(instance_parameter, getter);
            Expression result = ConvertExpression.ConvertToScript(getter.ReturnType, call);

            return Expression.Lambda<Func<T, IValue>>(result, instance_parameter).Compile();
        }

        /// <summary>
        /// Создается сеттер.
        /// </summary>
        /// <param name="property_info"></param>
        /// <returns></returns>
        private Action<T, IValue> CreateSetter(PropertyInfo property_info)
        {
            ParameterExpression instance_parameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argument_parameter = Expression.Parameter(typeof(IValue), "argument");

            var setter = property_info.GetSetMethod();
            Expression call;
            // Если свойство только для чтения, то вызов функции выдаст ошибку.
            if (setter == null)
                call = Expression.Throw(Expression.Constant(new Exception($"Поле [{property_info.Name}] объекта недоступно для записи")));
            else
                call = Expression.Call(instance_parameter, setter, ConvertExpression.ConvertFromScript(argument_parameter, property_info.PropertyType,0));

            return Expression.Lambda<Action<T, IValue>>(call, instance_parameter, argument_parameter).Compile();
        }

        /// <summary>
        /// Получить значение.
        /// </summary>
        /// <returns></returns>
        public IValue Get()
        {
            return _getter(_instance);
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void Set(IValue value)
        {
            _setter(_instance, value);
        }

        /// <summary>
        /// Клонировать класс с новым объектом.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IVariableReference Clone(object instance)
        {
            return new LibraryReference<T>((T)instance, _getter, _setter);
        }
    }
}
