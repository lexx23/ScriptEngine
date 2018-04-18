/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    class FunctionWrapper<T> : IMethodWrapper
    {
        private T _instance;
        private Func<T, IVariable[], IValue> _function;


        /// <summary>
        /// Конструктор используется при клонировании класса.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="function"></param>
        public FunctionWrapper(T instance, Func<T, IVariable[], IValue> function)
        {
            _instance = instance;
            _function = function;
        }

        /// <summary>
        /// Простой конструктор.
        /// </summary>
        /// <param name="method"></param>
        public FunctionWrapper(MethodInfo method)
        {
            _function = CreateFunction(method);
        }

        /// <summary>
        /// Создает прототип функции.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private Func<T, IVariable[], IValue> CreateFunction(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IVariable[]), "arguments");
            Expression call;
            if (!method.IsStatic)
                call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));
            else
                call = Expression.Call(method, CreateParameterExpressions(method, argumentsParameter));

            Expression result = ConvertExpression.ConvertToScript(method.ReturnType,call);
            return Expression.Lambda<Func<T, IVariable[],IValue>>(result, instanceParameter, argumentsParameter).Compile();
        }

        private Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            Expression[] list = new Expression[method.GetParameters().Length];
            int i = 0;

            foreach (ParameterInfo info in method.GetParameters())
            {
                list[i] = ConvertExpression.ConvertFromScript(argumentsParameter, info.ParameterType,i);
                i++;
            }
            return list;
        }

        /// <summary>
        /// Выполнить функцию.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IValue Run(IVariable[] param)
        {
            return _function(_instance, param);
        }

        /// <summary>
        /// Копия функции с указанием нового объекта.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IMethodWrapper Clone(object instance)
        {
            return new FunctionWrapper<T>((T)instance,_function);
        }
    }
}
