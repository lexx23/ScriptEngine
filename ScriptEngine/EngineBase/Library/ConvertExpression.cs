using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
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
            if (!to_type.IsArray)
                value = Expression.Call(Expression.ArrayIndex(variable, Expression.Constant(index)), typeof(IVariable).GetProperty("Value").GetGetMethod());

            if (to_type.IsEnum)
            {
                MethodInfo enum_converter = typeof(ConvertExpression).GetMethod("ConvertEnum").MakeGenericMethod(to_type);
                return Expression.Call(enum_converter, value);
            }

            switch (to_type.Name)
            {
                case "IValue":
                    return value;

                case "IValue[]":
                    return Expression.Call(typeof(ConvertExpression).GetMethod("CreateValueArray"), variable);

                case "Decimal":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsDecimal"));

                case "DateTime":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsDateTime"));

                case "Int32":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsInt"));

                case "String":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsString"));

                case "IVariable":
                    return Expression.ArrayIndex(variable, Expression.Constant(index));

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
        /// Конвертер IValue в перечисление. Используется в вызовах "внешних" методов.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static T ConvertEnum<T>(IValue value)
        {
            if (value != null)
                foreach (FieldInfo field in typeof(T).GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
                {
                    EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First();
                    if (attr.Value == value.AsString() || field.Name == value.AsString())
                        return (T)field.GetValue(null);
                }

            throw new Exception("Несоответствие типов.");
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

            if (type == typeof(IValue))
                return value;

            if (type.IsEnum)
                type = typeof(object);

            if (type == typeof(object))
                value = Expression.Convert(value, typeof(object));

            method = typeof(ValueFactory).GetMethod("Create", new Type[] { type });

            if (method == null)
            {
                LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryClassAttribute), false);
                if (attribute != null)
                    return Expression.Convert(value, typeof(IValue));
            }
            else
                return Expression.Call(method, value);

            throw new Exception($"Тип {type.ToString()} не поддерживается.");
        }

    }
}
