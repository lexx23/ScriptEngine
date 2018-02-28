using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    class FunctionWrapper<T> : IMethodWrapper
    {
        private T _instance;
        private Func<T, IValue[], IValue> _function;


        /// <summary>
        /// Конструктор используется при клонировании класса.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="function"></param>
        public FunctionWrapper(T instance, Func<T, IValue[], IValue> function)
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
        private Func<T, IValue[], IValue> CreateFunction(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");
            Expression call;
            if (!method.IsStatic)
                call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));
            else
                call = Expression.Call(method, CreateParameterExpressions(method, argumentsParameter));

            Expression result = ConvertExpression.ConvertToScript(method.ReturnType,call);
            return Expression.Lambda<Func<T, IValue[],IValue>>(result, instanceParameter, argumentsParameter).Compile();
        }

        private Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            Expression[] list = new Expression[method.GetParameters().Length];
            int i = 0;

            foreach (ParameterInfo info in method.GetParameters())
            {
                if (info.ParameterType == typeof(IValue[]))
                    list[i] = argumentsParameter;
                else
                    list[i] = ConvertExpression.ConvertFromScript(Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)), info.ParameterType);
                i++;
            }
            return list;
        }

        /// <summary>
        /// Выполнить функцию.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IValue Run(IValue[] param)
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
