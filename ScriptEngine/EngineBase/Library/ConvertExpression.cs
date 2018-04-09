using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System;

namespace ScriptEngine.EngineBase.Library
{
    class ConvertExpression
    {
        /// <summary>
        /// Выбор выражения конвертации из IValue в базовые типы c#.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="to_type"></param>
        /// <returns></returns>
        public static Expression ConvertFromScript(Expression variable, Type to_type, int index)
        {
            Expression value = null;

            if (variable.Type.IsArray)
                value = Expression.ArrayIndex(variable, Expression.Constant(index));
            else
                value = variable;

            if (!to_type.IsArray && value.Type == typeof(IVariable))
                value = Expression.Call(value, typeof(IVariable).GetProperty("Value").GetGetMethod());

            switch (to_type.Name)
            {
                case "IValue":
                    return value;

                case "IValue[]":
                    return Expression.Call(typeof(ConvertExpression).GetMethod("CreateValueArray"), variable);

                case "Decimal":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsDecimal"));

                case "Int64":
                    return Expression.Convert(Expression.Call(value, typeof(IValue).GetMethod("AsDecimal")), typeof(long));

                case "DateTime":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsDate"));

                case "Int32":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsInt"));

                case "String":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsString"));

                case "Boolean":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsBoolean"));

                case "IVariable":
                    return Expression.ArrayIndex(variable, Expression.Constant(index));

                case "ScriptObjectContext":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsScriptObject"));

                default:
                    Expression call = Expression.Call(value, typeof(IValue).GetMethod("AsObject"));
                    return Expression.Convert(call, to_type);

            }
        }

        public static IValue[] CreateValueArray(IVariable[] array)
        {
            return array.Select(x => x.Value).ToArray();
        }


        /// <summary>
        /// Конструктор для типов c#.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Expression ConvertToScript(Type type, Expression value)
        {
            MethodInfo method;
            MethodInfo method_sample;

            if (type == typeof(IValue))
                return value;

            if (type.IsEnum)
                type = typeof(object);

            if (type == typeof(object))
                value = Expression.Convert(value, typeof(object));

            method_sample = typeof(ValueFactory).GetMethod("Create", new Type[] { typeof(object) });
            method = typeof(ValueFactory).GetMethod("Create", new Type[] { type });

            if (method == method_sample && type != typeof(object))
                throw new Exception($"Тип {type.ToString()} не является наследником IValue.");

            return Expression.Call(method, value);
        }

    }
}
