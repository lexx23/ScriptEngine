﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    class ReferenceFactory
    {
        /// <summary>
        /// Статические переменные.
        /// </summary>
        /// <returns></returns>
        public static IVariableReference Create() => new SimpleReference();

        /// <summary>
        /// Статические переменные с значением по умолчанию.
        /// </summary>
        /// <returns></returns>
        public static IVariableReference Create(IValue value)
        {
            IVariableReference reference = new SimpleReference();
            reference.Set(value);
            return reference;
        }

        /// <summary>
        /// Создает обертку для доступа к переменным которые ссылаются на другие переменные.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static IVariableReference Create(IVariableReference reference) => new ScriptReference(reference);

        /// <summary>
        /// Создает обертку для доступа к свойствам "внешних" классов, классы которые загружаются из dll.
        /// </summary>
        /// <param name="target_type"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IVariableReference Create(Type target_type, PropertyInfo property)
        {
            ParameterExpression target_expr = Expression.Parameter(typeof(Type), "target");
            ParameterExpression property_expr = Expression.Parameter(typeof(PropertyInfo), "property");

            var instance_type = typeof(LibraryReference<>).MakeGenericType(target_type);
            var constructor_type = instance_type.GetConstructor(new Type[] { typeof(Type), typeof(PropertyInfo) });
            var constructor = Expression.New(constructor_type, target_expr, property_expr);

            Func<Type, PropertyInfo, IVariableReference> result = Expression.Lambda<Func<Type, PropertyInfo, IVariableReference>>(constructor, target_expr, property_expr).Compile();
            return result(target_type, property);
        }


    }
}
