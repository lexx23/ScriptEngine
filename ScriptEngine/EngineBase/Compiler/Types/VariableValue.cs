﻿using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Exceptions;
using System;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Compiler.Types
{
    public class VariableValue
    {
        public ValueTypeEnum Type { get; set; }

        public string Content { get; set; }
        public int Integer { get; set; }
        public float Float { get; set; }
        public bool Boolean { get; set; }
        public DateTime Date { get; set; }
        public VariableValueObject Object { get; set; }


        #region Логические операции с значением
        /// <summary>
        /// Равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(VariableValue left, VariableValue right)
        {
            if (object.ReferenceEquals(left, null) == true && object.ReferenceEquals(right, null) == true)
                return true;

            if (object.ReferenceEquals(left, null))
                return false;

            if (object.ReferenceEquals(right, null))
                return false;

            if (left.Type != right.Type)
                return false;

            switch (left.Type)
            {
                case ValueTypeEnum.STRING:
                    return left.Content == right.Content;

                case ValueTypeEnum.NUMBER:
                    return left.Integer == right.Integer;

                case ValueTypeEnum.FLOAT:
                    return left.Float == right.Float;

                case ValueTypeEnum.BOOLEAN:
                    return left.Boolean == right.Boolean;
            }
            return false;
        }

        /// <summary>
        /// Не равенство
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(VariableValue left, VariableValue right)
        {
            return !(left == right);
        }


        /// <summary>
        /// Больше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator >(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            result.Type = ValueTypeEnum.BOOLEAN;
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Boolean = left.Integer > right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Boolean = left.Float > right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить сравнение (больше) {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator >=(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            result.Type = ValueTypeEnum.BOOLEAN;
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Boolean = left.Integer >= right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Boolean = left.Float >= right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить сравнение (больше либо равно) {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator <(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            result.Type = ValueTypeEnum.BOOLEAN;
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Boolean = left.Integer < right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Boolean = left.Float < right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить сравнение (меньше) {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Меньше
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator <=(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            result.Type = ValueTypeEnum.BOOLEAN;
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Boolean = left.Integer <= right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Boolean = left.Float <= right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить сравнение (меньше либо равно) {left.Content} и {right.Content}.");
        }
        #endregion

        #region Арифметические операции с значением
        /// <summary>
        /// Сложение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator +(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.STRING:
                    result.Type = ValueTypeEnum.STRING;
                    result.Content = left.Content + right.Content;
                    return result;

                case ValueTypeEnum.NUMBER:
                    result.Type = ValueTypeEnum.NUMBER;
                    result.Integer = left.Integer + right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Type = ValueTypeEnum.FLOAT;
                    result.Float = left.Float + right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить сложение {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Разница
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator -(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Type = ValueTypeEnum.NUMBER;
                    result.Integer = left.Integer - right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Type = ValueTypeEnum.FLOAT;
                    result.Float = left.Float - right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно вычислить разницу {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator *(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Type = ValueTypeEnum.NUMBER;
                    result.Integer = left.Integer * right.Integer;
                    result.Content = result.ToString();
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Type = ValueTypeEnum.FLOAT;
                    result.Float = left.Float * right.Float;
                    result.Content = result.ToString();
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить произведение {left.Content} и {right.Content}.");
        }

        /// <summary>
        /// Произведение
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator *(VariableValue left, int right)
        {
            VariableValue right_tmp = new VariableValue();
            right_tmp.Type = ValueTypeEnum.NUMBER;
            CommonType(left, right_tmp);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    left.Integer *= right;
                    left.Content = left.ToString();
                    return left;

                case ValueTypeEnum.FLOAT:
                    left.Float *= right;
                    left.Content = left.ToString();
                    return left;
            }

            throw new ExceptionBase($"Невозможно выполнить произведение {left.Content} и {right.ToString()}.");
        }

        /// <summary>
        /// Деление
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static VariableValue operator /(VariableValue left, VariableValue right)
        {
            VariableValue result = new VariableValue();
            CommonType(left, right);
            switch (left.Type)
            {
                case ValueTypeEnum.NUMBER:
                    result.Type = ValueTypeEnum.NUMBER;
                    result.Integer = left.Integer / right.Integer;
                    return result;

                case ValueTypeEnum.FLOAT:
                    result.Type = ValueTypeEnum.FLOAT;
                    result.Float = left.Float / right.Float;
                    return result;
            }

            throw new ExceptionBase($"Невозможно выполнить деление {left.Content} и {right.Content}.");
        }

        #endregion

        #region Преобразование значения

        /// <summary>
        /// Преобразовать значения к общему типу
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        public static void CommonType(VariableValue left, VariableValue right)
        {
            if (left.Type == right.Type)
                return;

            if (left.Type == ValueTypeEnum.FLOAT)
            {
                right = right.ConvertTo(ValueTypeEnum.FLOAT);
                return;
            }

            if (right.Type == ValueTypeEnum.FLOAT)
            {
                left = left.ConvertTo(ValueTypeEnum.FLOAT);
                return;
            }

            right = right.ConvertTo(left.Type);
        }

        /// <summary>
        /// Преобразовать в строку
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    return Content;

                case ValueTypeEnum.NUMBER:
                    return Integer.ToString();

                case ValueTypeEnum.FLOAT:
                    return Float.ToString("n3");

                case ValueTypeEnum.BOOLEAN:
                    return Boolean.ToString();

                case ValueTypeEnum.NULL:
                    return "null";
            }

            return string.Empty;
        }

        /// <summary>
        /// Преобразовать в число с плавающей точкой
        /// </summary>
        /// <returns></returns>
        public float ToFloat()
        {
            if (Type == ValueTypeEnum.NULL)
                throw new ExceptionBase($"Значение с типом Null невозможно преобразовать в число с плавающей точкой.");

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    try
                    {
                        return float.Parse(Content, System.Globalization.CultureInfo.InvariantCulture);
                    }
                    catch
                    {
                        throw new ExceptionBase($"Невозможно преобразовать [{Content}] в число с плавающей точкой.");
                    }


                case ValueTypeEnum.NUMBER:
                    return (float)Integer;

                case ValueTypeEnum.FLOAT:
                    return Float;

                case ValueTypeEnum.BOOLEAN:
                    if (Boolean)
                        return 1.0f;
                    else
                        return 0f;

            }
            return 0;
        }

        /// <summary>
        /// Преобразовать в число
        /// </summary>
        /// <returns></returns>
        public int ToInt()
        {
            if (Type == ValueTypeEnum.NULL)
                throw new ExceptionBase($"Значение с типом Null невозможно преобразовать в число.");

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    try
                    {
                        return int.Parse(Content);
                    }
                    catch
                    {
                        throw new ExceptionBase($"Невозможно преобразовать [{Content}] в число.");
                    }


                case ValueTypeEnum.NUMBER:
                    return Integer;

                case ValueTypeEnum.FLOAT:
                    return (int)Float;

                case ValueTypeEnum.BOOLEAN:
                    if (Boolean)
                        return 1;
                    else
                        return 0;

            }
            return 0;
        }

        /// <summary>
        /// Преобразовать в логический тип
        /// </summary>
        /// <returns></returns>
        public bool ToBoolean()
        {
            if (Type == ValueTypeEnum.NULL)
                throw new ExceptionBase($"Значение с типом Null невозможно преобразовать в логическое значение.");

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    if (Content.ToLower() == "ложь" || Content.ToLower() == "false")
                        return false;
                    if (Content.ToLower() == "истина" || Content.ToLower() == "true")
                        return true;
                    throw new ExceptionBase($"Невозможно преобразовать [{Content}] в логическое значение.");

                case ValueTypeEnum.NUMBER:
                    return Integer != 0;

                case ValueTypeEnum.FLOAT:
                    return Float != 0.0;

                case ValueTypeEnum.BOOLEAN:
                    return Boolean;

            }
            return false;
        }

        /// <summary>
        /// Преобразование значения к необходимому типу
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public VariableValue ConvertTo(ValueTypeEnum type)
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
                    Integer = ToInt();
                    break;

                case ValueTypeEnum.FLOAT:
                    Float = ToFloat();
                    break;

                case ValueTypeEnum.BOOLEAN:
                    Boolean = ToBoolean();
                    break;
            }

            Type = type;
            return this;
        }

        #endregion

        public VariableValue()
        {
            Type = ValueTypeEnum.NULL;
        }

        /// <summary>
        /// Инициализация значения
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        public VariableValue(ValueTypeEnum type, string value)
        {
            Type = ValueTypeEnum.STRING;
            Content = value;
            ConvertTo(type);
        }


        public VariableValue(string value)
        {
            Type = ValueTypeEnum.STRING;
            Content = value;
        }


        public VariableValue(int value)
        {
            Type = ValueTypeEnum.NUMBER;
            Content = value.ToString();
            Integer = value;
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void SetValue(VariableValue value)
        {
            Type = value.Type;
            Content = value.Content;

            switch (Type)
            {
                case ValueTypeEnum.STRING:
                    Content = value.Content;
                    break;

                case ValueTypeEnum.NUMBER:
                    Integer = value.Integer;
                    break;

                case ValueTypeEnum.FLOAT:
                    Float = value.Float;
                    break;

                case ValueTypeEnum.BOOLEAN:
                    Boolean = value.Boolean;
                    break;

                case ValueTypeEnum.OBJECT:
                    Object = value.Object;
                    break;
            }
        }

        /// <summary>
        /// Копировать класс.
        /// </summary>
        /// <returns></returns>
        public VariableValue Clone()
        {
            if (this == null)
                return null;

            return new VariableValue()
            {
                Type = this.Type,
                Content = this.Content,
                Boolean = this.Boolean,
                Integer = this.Integer,
                Float = this.Float,
                Date = this.Date,
                Object = this.Object
            };
        }

        public override bool Equals(object obj)
        {
            var value = obj as VariableValue;
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
