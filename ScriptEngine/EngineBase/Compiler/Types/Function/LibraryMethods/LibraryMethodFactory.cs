/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    class LibraryMethodFactory
    {
        /// <summary>
        /// Создает класс который содержит метод вызова (внешней функции). Используется при загрузке внешних dll библиотек.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static IMethodWrapper Create(Type target, MethodInfo method)
        {
            ParameterExpression target_expr = Expression.Parameter(typeof(Type), "target");
            ParameterExpression method_expr = Expression.Parameter(typeof(MethodInfo), "method");

            Type instance_type;
            // Функция или процедура.
            if (method.ReturnType != typeof(void))
                instance_type = typeof(FunctionWrapper<>).MakeGenericType(target);
            else
                instance_type = typeof(ProcedureWrapper<>).MakeGenericType(target);
            // Выбор конструктора у класса.
            var constructor_type = instance_type.GetConstructor(new Type[] { typeof(MethodInfo) });
            var constructor = Expression.New(constructor_type, method_expr);

            Func<Type, MethodInfo, IMethodWrapper> result = Expression.Lambda<Func<Type, MethodInfo, IMethodWrapper>>(constructor, target_expr, method_expr).Compile();
            return result(target, method);
        }
    }
}
