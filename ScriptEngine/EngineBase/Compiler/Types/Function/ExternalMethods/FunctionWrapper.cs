using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods
{
    class FunctionWrapper<T> : IMethodWrapper
    {
        private T _instance;
        private Func<T, IValue[], IValue> _function;

        public FunctionWrapper(T instance, Func<T, IValue[], IValue> function)
        {
            _instance = instance;
            _function = function;
        }


        public FunctionWrapper(MethodInfo method)
        {
            _function = CreateFunction(method);
        }

        private Func<T, IValue[], IValue> CreateFunction(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(T), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");
            Expression call;
            if (!method.IsStatic)
                call = Expression.Call(instanceParameter, method, CreateParameterExpressions(method, argumentsParameter));
            else
                call = Expression.Call(method, CreateParameterExpressions(method, argumentsParameter));
            MethodInfo factory = typeof(ValueFactory).GetMethod("Create", new Type[] { method.ReturnType });
            Expression result = Expression.Call(factory, call);

            return Expression.Lambda<Func<T, IValue[],IValue>>(result, instanceParameter, argumentsParameter).Compile();
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
            return _function(_instance, param);
        }

        public IMethodWrapper Clone(object instance)
        {
            return new FunctionWrapper<T>((T)instance,_function);
        }
    }
}
