/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public class ValueFactory
    {
        private static IValue _null_value = new NullValue();
        private static IValue _bool_value_true = new BooleanValue(true);
        private static IValue _bool_value_false = new BooleanValue(false);


        public static IValue Create() => _null_value;
        public static IValue Create(string value) => new StringValue(value);
        public static IValue Create(decimal value) => new NumberValue(value);
        public static IValue Create(Int64 value) => new NumberValue(value);
        public static IValue Create(int value) => new NumberValue(value);
        public static IValue Create(DateTime value) => new DateValue(value);
        public static IValue Create(object value) => new ObjectValue(value);
        public static IValue Create(InternalScriptType value) => new TypeValue(value);
        public static IValue Create(IScriptObjectContext value) => new ScriptObjectValue(value);
        public static IValue Create(bool value) => value == true ? _bool_value_true : _bool_value_false;
        public static IValue Create(IValue value) => value ?? Create();


        /// <summary>
        /// Конструктор для типов скрипта.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IValue Create(ValueTypeEnum type, string value)
        {
            switch (type)
            {
                case ValueTypeEnum.NUMBER:
                    if (decimal.TryParse(value, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out decimal result))
                        return Create(result);
                    else
                        throw new Exception($"Ошибка преобразования в число, значения [{value}]");

                case ValueTypeEnum.STRING:
                    return Create(value);

                case ValueTypeEnum.BOOLEAN:
                    if (String.Equals(value, "ложь", StringComparison.OrdinalIgnoreCase) || String.Equals(value, "false", StringComparison.OrdinalIgnoreCase))
                        return Create(false);
                    if (String.Equals(value, "истина", StringComparison.OrdinalIgnoreCase) || String.Equals(value, "true", StringComparison.OrdinalIgnoreCase))
                        return Create(true);

                    throw new Exception($"Ошибка преобразования в логический тип, значения [{value}]");

                case ValueTypeEnum.DATE:
                    string[] formats = { "yyyyMMddHHmmss", "yyyyMMdd", "yyyyMMddHHmm" };
                    if (DateTime.TryParseExact(value, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime date))
                        return Create(date);

                    throw new Exception($"Ошибка преобразования в тип даты, значения [{value}]");

            }
            return Create();
        }


        /// <summary>
        /// Конструктор для типов c#.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static IValue Create(Type type, object value)
        {
            if (type == typeof(void) || value == null)
                return Create();

            if (type == typeof(int))
                return Create(Convert.ToDecimal(value));

            if (type == typeof(string))
                return Create((string)value);

            if (type == typeof(DateTime))
                return Create((DateTime)value);

            if (type == typeof(bool))
                return Create((bool)value);

            if (type.IsEnum)
                return Create(value);

            throw new Exception($"Тип {type.ToString()} не поддерживается.");
        }

        #region Логические операции
        /// <summary>
        /// Равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue EQ(IValue left, IValue right)
        {
            return Create(left.Equals(right));
        }

        /// <summary>
        /// Не равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue UNEQ(IValue left, IValue right)
        {
            return Create(!left.Equals(right));
        }

        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue GT(IValue left, IValue right)
        {
            return Create(left.CompareTo(right) > 0);
        }


        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue GE(IValue left, IValue right)
        {
            return Create(left.CompareTo(right) >= 0);
        }


        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue LT(IValue left, IValue right)
        {
            return Create(left.CompareTo(right) < 0);
        }


        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue LE(IValue left, IValue right)
        {
            return Create(left.CompareTo(right) <= 0);
        }
        #endregion

        #region Арифметические операции
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue ADD(IValue left, IValue right)
        {
            if (left.BaseType == ValueTypeEnum.STRING)
                return Create(left.AsString() + right.AsString());
            if (left.BaseType == ValueTypeEnum.DATE && right.BaseType == ValueTypeEnum.NUMBER)
                return Create(left.AsDate().AddSeconds((double)right.AsNumber()));

            return Create(left.AsNumber() + right.AsNumber());
        }


        /// <summary>
        /// Разница
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue SUB(IValue left, IValue right)
        {
            if (left.BaseType == ValueTypeEnum.NUMBER)
                return Create(left.AsNumber() - right.AsNumber());

            if (left.BaseType == ValueTypeEnum.DATE && right.BaseType == ValueTypeEnum.NUMBER)
                return Create(left.AsDate().AddSeconds(-(double)right.AsNumber()));

            if (left.BaseType == ValueTypeEnum.DATE && right.BaseType == ValueTypeEnum.DATE)
            {
                TimeSpan result = right.AsDate() - left.AsDate();
                return Create((decimal)result.TotalSeconds);
            }

            return Create(right.AsNumber() - left.AsNumber());
        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue MUL(IValue left, IValue right)
        {
            if (right.AsNumber() == 0)
                throw new Exception("Деление на 0.");

            return Create(left.AsNumber() * right.AsNumber());
        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue MUL(IValue left, int right)
        {
            return Create(left.AsNumber() * right);
        }

        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue DIV(IValue left, IValue right)
        {
            if (right.AsNumber() == 0)
                throw new Exception("Деление на 0.");

            return Create(left.AsNumber() / right.AsNumber());
        }


        /// <summary>
        /// Остаток от деления
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static IValue MOD(IValue left, IValue right)
        {
            if (right.AsNumber() == 0)
                throw new Exception("Деление на 0.");

            return Create(left.AsNumber() % right.AsNumber());
        }
        #endregion
    }
}
