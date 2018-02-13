using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Reflection;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library
{

    public static class MethodInfoExtensions
    {
        private static readonly MethodInfo _convert_method = typeof(MethodInfoExtensions).GetMethod("Convert");

        private static Func<Object, IValue[], IValue> CreateForNonVoidInstanceMethod(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(Object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<Func<Object, IValue[], IValue>> lambda = Expression.Lambda<Func<Object, IValue[], IValue>>(
                Expression.Convert(call, typeof(IValue)),
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        private static Func<IValue[], IValue> CreateForNonVoidStaticMethod(MethodInfo method)
        {
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");

            MethodCallExpression call = Expression.Call(
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<Func<IValue[], IValue>> lambda = Expression.Lambda<Func<IValue[], IValue>>(
                Expression.Convert(call, typeof(IValue)),
                argumentsParameter);

            return lambda.Compile();
        }

        private static Action<Object, IValue[]> CreateForVoidInstanceMethod(MethodInfo method)
        {
            ParameterExpression instanceParameter = Expression.Parameter(typeof(Object), "target");
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");

            MethodCallExpression call = Expression.Call(
                Expression.Convert(instanceParameter, method.DeclaringType),
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<Action<Object, IValue[]>> lambda = Expression.Lambda<Action<Object, IValue[]>>(
                call,
                instanceParameter,
                argumentsParameter);

            return lambda.Compile();
        }

        private static Action<IValue[]> CreateForVoidStaticMethod(MethodInfo method)
        {
            ParameterExpression argumentsParameter = Expression.Parameter(typeof(IValue[]), "arguments");

            MethodCallExpression call = Expression.Call(
                method,
                CreateParameterExpressions(method, argumentsParameter));

            Expression<Action<IValue[]>> lambda = Expression.Lambda<Action<IValue[]>>(
                call,
                argumentsParameter);

            return lambda.Compile();
        }

        //public static T Convert<T>(IValue value)
        //{
        //    return (T)value.GetRawValue();
        //}


        private static Expression[] CreateParameterExpressions(MethodInfo method, Expression argumentsParameter)
        {
            IList<Expression> list = new List<Expression>();
            int i = 0;
            foreach (ParameterInfo info in method.GetParameters())
            {
                if (info.ParameterType.IsEnum)
                {
                    MethodInfo generic_method = _convert_method.MakeGenericMethod(info.ParameterType);
                    MethodCallExpression call = Expression.Call(generic_method, Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)));
                    list.Add(call);
                }
                else
                    list.Add(Expression.Convert(Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)), info.ParameterType));
                    //MethodInfo convert_method = typeof(IValue).GetMethod("AsString");
                    //MethodCallExpression call = Expression.Call(Expression.ArrayIndex(argumentsParameter, Expression.Constant(i)), convert_method);
                    //list.Add(call);
                i++;
            }

            return list.ToArray();
        }

        public static Func<IValue[], IValue> Bind(this MethodInfo method, object target)
        {

            if (method.IsStatic)
            {
                if (method.ReturnType == typeof(void))
                {
                    Action<IValue[]> wrapped = CreateForVoidStaticMethod(method);
                    return (parameters) =>
                    {
                        wrapped(parameters);
                        return null;
                    };
                }
                else
                {
                    Func<IValue[], IValue> wrapped = CreateForNonVoidStaticMethod(method);
                    return (parameters) => wrapped(parameters);
                }
            }
            if (method.ReturnType == typeof(void))
            {
                Action<object, IValue[]> wrapped = CreateForVoidInstanceMethod(method);
                return (parameters) =>
                {
                    wrapped(target, parameters);
                    return null;
                };
            }
            else
            {
                Func<object, IValue[], IValue> wrapped = CreateForNonVoidInstanceMethod(method);
                return (parameters) => wrapped(target, parameters);
            }
        }

        public static Type LambdaType(this MethodInfo method)
        {
            if (method.ReturnType == typeof(void))
            {
                Type actionGenericType;
                switch (method.GetParameters().Length)
                {
                    case 0:
                        return typeof(Action);
                    case 1:
                        actionGenericType = typeof(Action<>);
                        break;
                    case 2:
                        actionGenericType = typeof(Action<,>);
                        break;
                    case 3:
                        actionGenericType = typeof(Action<,,>);
                        break;
                    case 4:
                        actionGenericType = typeof(Action<,,,>);
                        break;
                    case 5:
                        actionGenericType = typeof(Action<,,,,>);
                        break;
                    case 6:
                        actionGenericType = typeof(Action<,,,,,>);
                        break;
                    case 7:
                        actionGenericType = typeof(Action<,,,,,,>);
                        break;
                    case 8:
                        actionGenericType = typeof(Action<,,,,,,,>);
                        break;
                    case 9:
                        actionGenericType = typeof(Action<,,,,,,,,>);
                        break;
                    case 10:
                        actionGenericType = typeof(Action<,,,,,,,,,>);
                        break;
                    case 11:
                        actionGenericType = typeof(Action<,,,,,,,,,,>);
                        break;
                    case 12:
                        actionGenericType = typeof(Action<,,,,,,,,,,,>);
                        break;
                    case 13:
                        actionGenericType = typeof(Action<,,,,,,,,,,,,>);
                        break;
                    case 14:
                        actionGenericType = typeof(Action<,,,,,,,,,,,,,>);
                        break;
                    case 15:
                        actionGenericType = typeof(Action<,,,,,,,,,,,,,,>);
                        break;
                    case 16:
                        actionGenericType = typeof(Action<,,,,,,,,,,,,,,,>);
                        break;
                    default:
                        throw new NotSupportedException("Lambdas may only have up to 16 parameters.");
                }
                return actionGenericType.MakeGenericType(method.GetParameters().Select(_ => _.ParameterType).ToArray());
            }
            Type functionGenericType;
            switch (method.GetParameters().Length)
            {
                case 0:
                    return typeof(Func<>);
                case 1:
                    functionGenericType = typeof(Func<,>);
                    break;
                case 2:
                    functionGenericType = typeof(Func<,,>);
                    break;
                case 3:
                    functionGenericType = typeof(Func<,,,>);
                    break;
                case 4:
                    functionGenericType = typeof(Func<,,,,>);
                    break;
                case 5:
                    functionGenericType = typeof(Func<,,,,,>);
                    break;
                case 6:
                    functionGenericType = typeof(Func<,,,,,,>);
                    break;
                case 7:
                    functionGenericType = typeof(Func<,,,,,,,>);
                    break;
                case 8:
                    functionGenericType = typeof(Func<,,,,,,,,>);
                    break;
                case 9:
                    functionGenericType = typeof(Func<,,,,,,,,,>);
                    break;
                case 10:
                    functionGenericType = typeof(Func<,,,,,,,,,,>);
                    break;
                case 11:
                    functionGenericType = typeof(Func<,,,,,,,,,,,>);
                    break;
                case 12:
                    functionGenericType = typeof(Func<,,,,,,,,,,,,>);
                    break;
                case 13:
                    functionGenericType = typeof(Func<,,,,,,,,,,,,,>);
                    break;
                case 14:
                    functionGenericType = typeof(Func<,,,,,,,,,,,,,,>);
                    break;
                case 15:
                    functionGenericType = typeof(Func<,,,,,,,,,,,,,,,>);
                    break;
                case 16:
                    functionGenericType = typeof(Func<,,,,,,,,,,,,,,,,>);
                    break;
                default:
                    throw new NotSupportedException("Lambdas may only have up to 16 parameters.");
            }
            var parametersAndReturnType = new Type[method.GetParameters().Length + 1];
            method.GetParameters().Select(_ => _.ParameterType).ToArray().CopyTo(parametersAndReturnType, 0);
            parametersAndReturnType[parametersAndReturnType.Length - 1] = method.ReturnType;
            return functionGenericType.MakeGenericType(parametersAndReturnType);
        }
    }
}

