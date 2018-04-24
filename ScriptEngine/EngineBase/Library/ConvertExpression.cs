/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System;
using System.Diagnostics;
using ScriptEngine.EngineBase.Interpreter.Context;

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
                    return SafeCall<string>(value, typeof(IValue).GetMethod("AsString"));//Expression.Call(value, typeof(IValue).GetMethod("AsString"));

                case "IScriptObjectContext":
                case "ScriptObjectContext":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsScriptObject"));

                case "Boolean":
                    return Expression.Call(value, typeof(IValue).GetMethod("AsBoolean"));

                case "IVariable":
                    return Expression.ArrayIndex(variable, Expression.Constant(index));

                default:
                    //Expression call = Expression.Call(value, typeof(IValue).GetMethod("AsObject"));
                    //return Expression.Convert(call, to_type);
                    //value = Expression.Call(value, typeof(IVariable).GetProperty("Value").GetGetMethod());
                    return Expression.Convert(SafeCall<object>(value, typeof(IValue).GetMethod("AsObject")), to_type);

            }
        }

        private static Expression SafeCall<T>(Expression value,MethodInfo method)
        {
            var value_internal = Expression.Parameter(typeof(IValue));

            Expression base_type = Expression.Call(value_internal, typeof(IValue).GetProperty("BaseType").GetGetMethod());
            Expression method_call = Expression.Call(value_internal, method);
            LabelTarget return_value = Expression.Label(typeof(T));
            Expression if_expression = Expression.IfThenElse(
                Expression.Equal(base_type, Expression.Constant(ValueTypeEnum.NULL)),
                Expression.Return(return_value,Expression.Convert(Expression.Constant(null),typeof(T))),
                Expression.Return(return_value, method_call)
                );

            var ex = Expression.Block(
                if_expression,
                Expression.Label(return_value, Expression.Convert(Expression.Constant(null), typeof(T))));

            Expression func = Expression.Lambda<Func<IValue,T>>(ex, value_internal);
            return Expression.Invoke(func, value);
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
