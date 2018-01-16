﻿using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public class Value
    {
        public ValueTypeEnum Type { get; set; }

        public string Content { get; set; }
        public decimal Number { get; set; }
        public bool Boolean { get; set; }
        public DateTime Date { get; set; }
        public ObjectContext Object { get; set; }


        #region Логические операции с значением
        /// <summary>
        /// Равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Value left, Value right)
        {
            if (object.ReferenceEquals(left, null) == true && object.ReferenceEquals(right, null) == true)
                return true;

            if (object.ReferenceEquals(left, null))
                return false;

            if (object.ReferenceEquals(right, null))
                return false;

            if (left.Type != right.Type)
                return false;

            switch (right.Type)
            {
                case ValueTypeEnum.STRING:
                    return left.Content == right.Content;

                case ValueTypeEnum.NUMBER:
                    decimal left_result,right_result;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return false;
                    else
                        return left_result == right_result;

                case ValueTypeEnum.BOOLEAN:
                    return left.ToBoolean() == right.ToBoolean();
            }
            return false;
        }

        /// <summary>
        /// Не равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Value left, Value right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator >(Value left, Value right)
        {
            Value result = new Value();
            result.Type = ValueTypeEnum.BOOLEAN;
            switch (right.Type)
            {
                case ValueTypeEnum.BOOLEAN:
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        result.Boolean = false;
                    else
                        result.Boolean = left_result > right_result;
                    result.Content = result.ToString();
                    return result;
            }

            result.Boolean = false;
            result.Content = result.ToString();
            return result;
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator >=(Value left, Value right)
        {
            Value result = new Value();
            result.Type = ValueTypeEnum.BOOLEAN;
            switch (right.Type)
            {
                case ValueTypeEnum.BOOLEAN:
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        result.Boolean = false;
                    else
                        result.Boolean = left_result >= right_result;
                    result.Content = result.ToString();
                    return result;


            }

            result.Boolean = false;
            result.Content = result.ToString();
            return result;
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator <(Value left, Value right)
        {
            Value result = new Value();
            result.Type = ValueTypeEnum.BOOLEAN;
            switch (right.Type)
            {
                case ValueTypeEnum.BOOLEAN:
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        result.Boolean = false;
                    else
                        result.Boolean = left_result < right_result;
                    result.Content = result.ToString();
                    return result;

            }

            result.Boolean = false;
            result.Content = result.ToString();
            return result;
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator <=(Value left, Value right)
        {
            Value result = new Value();
            result.Type = ValueTypeEnum.BOOLEAN;
            switch (right.Type)
            {
                case ValueTypeEnum.BOOLEAN:
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        result.Boolean = false;
                    else
                        result.Boolean = left_result <= right_result;
                    result.Content = result.ToString();
                    return result;

            }

            result.Boolean = false;
            result.Content = result.ToString();
            return result;
        }
        #endregion

        #region Арифметические операции с значением
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator +(Value left, Value right)
        {
            Value result = new Value();

            switch (right.Type)
            {
                case ValueTypeEnum.STRING:
                    result.Type = ValueTypeEnum.STRING;
                    result.Content = left.ToString() + right.ToString();
                    return result;

                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    result.Type = ValueTypeEnum.NUMBER;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return null;
                    else
                        result.Number = left_result + right_result;
                    result.Content = result.ToString();
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Разница
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator -(Value left, Value right)
        {
            Value result = new Value();

            switch (right.Type)
            {
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    result.Type = ValueTypeEnum.NUMBER;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return null;
                    else
                        result.Number = left_result - right_result;
                    result.Content = result.ToString();
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator *(Value left, Value right)
        {
            Value result = new Value();

            switch (right.Type)
            {
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    result.Type = ValueTypeEnum.NUMBER;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return null;
                    else
                        result.Number = left_result * right_result;
                    result.Content = result.ToString();
                    return result;
            }

            return null;

        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator *(Value left, int right)
        {
            Value result = new Value();

            Value right_tmp = new Value();
            right_tmp.Type = ValueTypeEnum.NUMBER;
            switch (right_tmp.Type)
            {
                case ValueTypeEnum.NUMBER:
                    decimal left_result;
                    if (!left.ToNumber(out left_result))
                        return null;
                    else
                        result.Number = left_result - right;
                    result.Content = result.ToString();
                    return result;
            }

            return null;
        }

        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator /(Value left, Value right)
        {
            Value result = new Value();
            switch (right.Type)
            {
                case ValueTypeEnum.NUMBER:
                    decimal left_result, right_result;
                    result.Type = ValueTypeEnum.NUMBER;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return null;
                    else
                    {
                        if (right_result == 0)
                            return null;
                        result.Number = left_result / right_result;
                        result.Content = result.ToString();
                        return result;
                    }
            }

            return null;
        }

        /// <summary>
        /// Остаток от деления
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static Value operator %(Value left, Value right)
        {
            Value result = new Value();
            decimal left_result, right_result;

            switch (right.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Type = ValueTypeEnum.NUMBER;
                    if (!left.ToNumber(out left_result) || !right.ToNumber(out right_result))
                        return null;
                    else
                    {
                        if (right_result == 0)
                            return null;
                        result.Number = left_result % right_result;
                        result.Content = result.ToString();
                        return result;
                    }
            }

            return null;
        }
        #endregion

        #region Преобразование значения

        /// <summary>
        /// Преобразовать в строку, не изменяя тип переменной.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    return Content;

                case ValueTypeEnum.NUMBER:
                    return Number.ToString("n3");

                case ValueTypeEnum.BOOLEAN:
                    return Boolean.ToString();

                case ValueTypeEnum.NULL:
                    return "null";

                case ValueTypeEnum.OBJECT:
                    return Object.Module.Name;

                case ValueTypeEnum.DATE:
                    return Date.ToString("dd.MM.yyyy hh.mm:ss");

            }

            return "";
        }

        /// <summary>
        /// Преобразовать в число , не изменяя тип переменной.
        /// </summary>
        /// <returns></returns>
        public bool ToNumber(out decimal result)
        {
            result = 0;
            if (Type == ValueTypeEnum.NULL)
                return false;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    if (decimal.TryParse(Content, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out result))
                        return true;
                    break;
                case ValueTypeEnum.NUMBER:
                    result = Number;
                    return true;

                case ValueTypeEnum.BOOLEAN:
                    if (Boolean)
                        result = 1;
                    else
                        result = 0;
                    return true;

            }

            return false;
        }

        /// <summary>
        /// Преобразовать в число, не изменяя тип переменной.
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            if (Type == ValueTypeEnum.NULL)
                return 0;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    return int.Parse(Content);

                case ValueTypeEnum.NUMBER:
                    return (int)Number;

                case ValueTypeEnum.BOOLEAN:
                    if (Boolean)
                        return 1;
                    else
                        return 0;
            }

            return 0;
        }

        /// <summary>
        /// Преобразовать в логический тип, не изменяя тип переменной.
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean()
        {
            if (Type == ValueTypeEnum.NULL)
                return false;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    if (Content.ToLower() == "ложь" || Content.ToLower() == "false")
                        return false;
                    if (Content.ToLower() == "истина" || Content.ToLower() == "true")
                        return true;
                    break;

                case ValueTypeEnum.NUMBER:
                    return Number != 0;

                case ValueTypeEnum.BOOLEAN:
                    return Boolean;

            }

            return false;
        }

        /// <summary>
        /// Преобразовать в дату, не изменяя тип переменной.
        /// </summary>
        /// <returns></returns>
        public DateTime ToDate()
        {
            if (Type == ValueTypeEnum.NULL)
                throw new Exception($"Значение с типом Null невозможно преобразовать в логическое значение.");

            string[] formats = { "yyyyMMddhhmmss", "yyyyMMdd", "yyyyMMddhhmm" };
            DateTime result;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    if (DateTime.TryParseExact(Content, formats, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out result))
                        return result;
                    break;
                // проверить возможно такого фунционала нет!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                case ValueTypeEnum.NUMBER:
                    return DateTime.FromFileTime((long)Number);
            }

            throw new Exception($"Невозможно преобразовать [{Content}] в дату.");
        }

        /// <summary>
        /// Преобразование значения к необходимому типу. И зменяется тип значения.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Value ConvertTo(ValueTypeEnum type)
        {
            string error = String.Empty;
            if (type == Type)
                return this;

            switch (type)
            {
                case ValueTypeEnum.STRING:
                    Content = ToString();
                    break;

                case ValueTypeEnum.NUMBER:
                    decimal result;
                    ToNumber(out result);
                    Number = result;
                    break;

                case ValueTypeEnum.BOOLEAN:
                    Boolean = ToBoolean();
                    break;

                case ValueTypeEnum.DATE:
                    Date = ToDate();
                    break;
            }

            Type = type;
            return this;
        }

        #endregion

        public Value()
        {
            Type = ValueTypeEnum.NULL;
        }

        /// <summary>
        /// Инициализация значения
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public Value(ValueTypeEnum type, string value)
        {
            Type = ValueTypeEnum.STRING;
            Content = value;
            ConvertTo(type);
        }


        public Value(string value)
        {
            Type = ValueTypeEnum.STRING;
            Content = value;
        }


        public Value(int value)
        {
            Type = ValueTypeEnum.NUMBER;
            Content = value.ToString();
            Number = value;
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(Value value)
        {
            Type = value.Type;
            Content = value.Content;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    Content = value.Content;
                    break;

                case ValueTypeEnum.NUMBER:
                    Number = value.Number;
                    break;

                case ValueTypeEnum.BOOLEAN:
                    Boolean = value.Boolean;
                    break;

                case ValueTypeEnum.OBJECT:
                    Object = value.Object;
                    Content = value.Object.Module.Name;
                    break;
            }
        }

        /// <summary>
        /// Копировать класс.
        /// </summary>
        /// <returns></returns>
        public Value Clone()
        {
            if (this == null)
                return null;

            return new Value()
            {
                Type = this.Type,
                Content = this.Content,
                Boolean = this.Boolean,
                Number = this.Number,
                Date = this.Date,
                Object = this.Object
            };
        }

        public override bool Equals(object obj)
        {
            var value = obj as Value;
            return value != null && this == value;
        }

        public override int GetHashCode()
        {
            var hashCode = 873661529;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Content);
            return hashCode;
        }
    }
}
