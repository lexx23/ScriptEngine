using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library;
using System.Linq.Expressions;
using System.Reflection;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    class ProcedureWrapper<T> : IMethodWrapper
    {

        private T _instance;
        private Action<T, IVariable[]> _procedure;

        /// <summary>
        /// Конструктор используется при клонировании класса.
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="procedure"></param>
        public ProcedureWrapper(T instance, Action<T, IVariable[]> procedure)
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
        private Action<T, IVariable[]> Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IVariable[]), "arguments");
            Expression call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));

            return Expression.Lambda<Action<T, IVariable[]>>(call, instanceParameter, argumentsParameter).Compile();
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
        /// Выполнить процедуру.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public IValue Run(IVariable[] param)
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
