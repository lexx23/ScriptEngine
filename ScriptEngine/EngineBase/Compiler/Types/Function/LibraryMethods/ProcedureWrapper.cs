using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    class ProcedureWrapper<T> : IMethodWrapper
    {

        private T _instance;
        private Action<T, IValue[]> _procedure;

        /// <summary>
        /// Конструктор используется при клонировании класса.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="procedure"></param>
        public ProcedureWrapper(T instance, Action<T, IValue[]> procedure)
        {
            _instance = instance;
            _procedure = procedure;
        }

        /// <summary>
        /// Простой конструктор.
        /// </summary>
        /// <param name="method"></param>
        public ProcedureWrapper(MethodInfo method)
        {
            _procedure = Create(method);
        }


        /// <summary>
        /// Создает прототип процедуры.
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        private Action<T, IValue[]> Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");
            Expression call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));

            return Expression.Lambda<Action<T, IValue[]>>(call, instanceParameter, argumentsParameter).Compile();
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
        /// Выполнить процедуру.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IValue Run(IValue[] param)
        {
            _procedure(_instance, param);
            return null;
        }

        /// <summary>
        /// Копия функции с указанием нового объекта.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IMethodWrapper Clone(object instance)
        {
            return new ProcedureWrapper<T>((T)instance, _procedure);
        }

    }
}
