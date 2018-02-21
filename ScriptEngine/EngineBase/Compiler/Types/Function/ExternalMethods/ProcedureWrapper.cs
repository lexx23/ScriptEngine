using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods
{
    class ProcedureWrapper<T> : IMethodWrapper
    {

        private T _instance;
        private Action<T, IValue[]> _procedure;

        public ProcedureWrapper(T instance, Action<T, IValue[]> procedure)
        {
            _instance = instance;
            _procedure = procedure;
        }


        public ProcedureWrapper(MethodInfo method)
        {
            _procedure = Create(method);
        }

        private Action<T, IValue[]> Create(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");
            Expression call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));

            return Expression.Lambda<Action<T, IValue[]>>(call, instanceParameter, argumentsParameter).Compile();
        }

        private Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            MethodCallExpression call;
            Expression[] list = new Expression[method.GetParameters().Length];
            int i = 0;

            foreach (ParameterInfo info in method.GetParameters())
            {
                switch (info.ParameterType.Name)
                {
                    case "IValue[]":
                        list[i] = argumentsParameter;
                        break;

                    case "IValue":
                        list[i] = Expression.ArrayIndex(argumentsParameter, Expression.Constant(i));
                        break;

                    case "String":
                        call = Expression.Call(Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)), typeof(IValue).GetMethod("AsString"));
                        list[i] = call;
                        break;

                    default:
                        call = Expression.Call(Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)), typeof(IValue).GetMethod("AsObject"));
                        list[i] = Expression.Convert(call, info.ParameterType);
                        break;
                }
                i++;
            }

            return list;
        }


        public IValue Run(IValue[] param)
        {
            _procedure(_instance, param);
            return null;
        }

        public IMethodWrapper Clone(object instance)
        {
            return new ProcedureWrapper<T>((T)instance, _procedure);
        }

    }
}
