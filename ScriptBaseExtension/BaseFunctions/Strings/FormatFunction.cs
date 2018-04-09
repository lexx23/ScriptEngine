﻿using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System;

//ОРИГИНАЛ КЛАССА ЛЕЖИТ ТУТ: https://github.com/EvilBeaver/OneScript
//TODO: пересмотреть класс.
namespace ScriptBaseFunctionsLibrary.BaseFunctions.Strings
{
    public class FormatParametersList
    {
        private struct FormatParameter
        {
            private readonly string _name;
            private readonly string _value;

            public FormatParameter(string name, string value)
            {
                _name = name;
                _value = value;
            }

            public string Name
            {
                get { return _name; }
            }

            public string Value
            {
                get { return _value; }
            }


        }

        readonly List<FormatParameter> _paramList = new List<FormatParameter>();

        public FormatParametersList(string format)
        {
            ParseParameters(format);
        }
        private void ParseParameters(string format)
        {
            int index = 0;
            int len = format.Length;
            while (index < len)
            {
                SkipWhitespace(format, ref index);

                string param = ReadParameter(format, ref index);
                if (param == null)
                    break;

                SkipWhitespace(format, ref index);

                string value = ReadValue(format, ref index);
                if (value == null)
                    value = "";

                _paramList.Add(new FormatParameter(param, value));
            }

        }

        private static void SkipWhitespace(string format, ref int index)
        {
            while (index < format.Length)
            {
                if (Char.IsWhiteSpace(format, index))
                    index++;
                else
                    break;
            }
        }

        private static string ReadParameter(string format, ref int index)
        {
            if (!Char.IsLetter(format, index))
                return null;

            int start = index;
            while (index < format.Length)
            {
                if (Char.IsLetterOrDigit(format, index) || format[index] == '.' || format[index] == '_')
                    index++;
                else if (format[index] == '=')
                {
                    var param = format.Substring(start, GetLength(start, index));
                    index++;
                    return param.Trim();
                }
                else if (Char.IsWhiteSpace(format, index))
                {
                    SkipWhitespace(format, ref index);
                }
                else
                    return null;

            }

            return null;
        }

        private static string ReadValue(string format, ref int index)
        {
            const char SINGLE_QUOTE = '\'';
            const char DOUBLE_QUOTE = '\"';
            const char SPACE = ' ';

            if (index >= format.Length)
                return null;

            StringBuilder valueBuffer = new StringBuilder();
            char valueEnd = '\0';

            if (format[index] == SINGLE_QUOTE)
            {
                valueEnd = SINGLE_QUOTE;
                index++;
            }
            else if (format[index] == DOUBLE_QUOTE)
            {
                valueEnd = DOUBLE_QUOTE;
                index++;
            }
            else
            {
                valueEnd = SPACE;
            }

            int start = index;

            while (index < format.Length)
            {
                if (format[index] == valueEnd)
                {
                    if (index + 1 < format.Length && format[index + 1] == valueEnd)
                    {
                        index += 2;
                        valueBuffer.Append(valueEnd);
                        continue;
                    }
                    break;
                }
                else if (valueEnd == SPACE && format[index] == ';')
                {
                    break;
                }
                else
                    valueBuffer.Append(format[index++]);
            }

            if (index < format.Length)
            {
                if (valueEnd == DOUBLE_QUOTE || valueEnd == SINGLE_QUOTE)
                {
                    index++;
                    SkipWhitespace(format, ref index);
                }
                else
                {
                    SkipWhitespace(format, ref index);
                }

                if (index < format.Length && format[index] == ';')
                    index++;
            }

            return valueBuffer.ToString();

        }

        private static int GetLength(int start, int index)
        {
            if (index > start)
                return index - start;
            else if (index == start)
                return 1;
            else
                return 0;
        }

        public string GetParamValue(string paramName)
        {
            return FindParamValue(x => String.Compare(x.Name, paramName, true) == 0);
        }

        public string GetParamValue(string paramNameRus, string paramNameEng)
        {
            return FindParamValue(x => String.Compare(x.Name, paramNameRus, true) == 0
                || String.Compare(x.Name, paramNameEng, true) == 0);

        }

        public string GetParamValue(string[] biLingua)
        {
            return GetParamValue(biLingua[0], biLingua[1]);

        }

        public bool HasParam(string[] biLingua, out string value)
        {
            var paramValue = GetParamValue(biLingua);
            if (paramValue != null)
            {
                value = paramValue;
                return true;
            }
            else
            {
                value = null;
                return false;
            }

        }

        public Dictionary<string, string> ToDictionary()
        {
            var result = new Dictionary<string, string>();
            foreach (var item in _paramList)
            {
                result[item.Name] = item.Value;
            }

            return result;
        }

        public IEnumerable<string> EnumerateValues()
        {
            return _paramList.Select(x => x.Value);
        }

        private string FindParamValue(Predicate<FormatParameter> criteria)
        {
            var param = _paramList.Find(criteria);
            if (param.Name == null)
                return null;

            return param.Value;
        }

    }


    public static class FormatFunction
    {
        static readonly string[] BOOLEAN_FALSE = { "БЛ", "BF" };
        static readonly string[] BOOLEAN_TRUE = { "БИ", "BT" };
        static readonly string[] LOCALE = { "Л", "L" };
        static readonly string[] NUM_MAX_SIZE = { "ЧЦ", "ND" };
        static readonly string[] NUM_DECIMAL_SIZE = { "ЧДЦ", "NFD" };
        static readonly string[] NUM_FRACTION_DELIMITER = { "ЧРД", "NDS" };
        static readonly string[] NUM_GROUPS_DELIMITER = { "ЧРГ", "NGS" };
        static readonly string[] NUM_ZERO_APPEARANCE = { "ЧН", "NZ" };
        static readonly string[] NUM_GROUPING = { "ЧГ", "NG" };
        static readonly string[] NUM_LEADING_ZERO = { "ЧВН", "NLZ" };
        static readonly string[] NUM_NEGATIVE_APPEARANCE = { "ЧО", "NN" };
        static readonly string[] DATE_EMPTY = { "ДП", "DE" };
        static readonly string[] DATE_FORMAT = { "ДФ", "DF" };
        static readonly string[] DATE_LOCAL_FORMAT = { "ДЛФ", "DLF" };

        // Длины разрядов мантиссы типа decimal в строковом десятичном представлении
        static readonly int[] decimal_digits_sizes = { 10, 20, 29 };

        public static string Format(IValue value, string format)
        {
            var formatParameters = ParseParameters(format);

            string formattedValue;

            switch (value.BaseType)
            {
                case ValueTypeEnum.BOOLEAN:
                    formattedValue = FormatBoolean(value.AsBoolean(), formatParameters);
                    break;
                case ValueTypeEnum.NUMBER:
                    formattedValue = FormatNumber(value.AsNumber(), formatParameters);
                    break;
                case ValueTypeEnum.DATE:
                    formattedValue = FormatDate(value.AsDate(), formatParameters);
                    break;
                default:
                    formattedValue = DefaultFormat(value, formatParameters);
                    break;
            }

            return formattedValue;

        }

        private static string FormatBoolean(bool p, FormatParametersList formatParameters)
        {
            if (p)
            {
                var truePresentation = formatParameters.GetParamValue(BOOLEAN_TRUE);
                if (truePresentation != null)
                    return truePresentation;
            }
            else
            {
                var falsePresentation = formatParameters.GetParamValue(BOOLEAN_FALSE);
                if (falsePresentation != null)
                    return falsePresentation;
            }

            return ValueFactory.Create(p).AsString();
        }

        #region Number formatting

        private static string FormatNumber(decimal p, FormatParametersList formatParameters)
        {
            var locale = formatParameters.GetParamValue(LOCALE);
            NumberFormatInfo nf;
            if (locale != null)
            {
                // culture codes in 1C-style
                var culture = CreateCulture(locale);
                nf = (NumberFormatInfo)culture.NumberFormat.Clone();
            }
            else
            {
                nf = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            }

            string param;

            if (p == 0)
            {
                if (formatParameters.HasParam(NUM_ZERO_APPEARANCE, out param))
                {
                    if (param == "")
                        return "0";
                    else
                        return param;
                }
                else
                    return "";
            }

            bool hasDigitLimits = false;
            int totalDigits = 0;
            int fractionDigits = 0;

            if (formatParameters.HasParam(NUM_MAX_SIZE, out param))
            {
                int paramToInt;
                if (Int32.TryParse(param, out paramToInt))
                {
                    if (paramToInt < 0)
                        paramToInt = 0;

                    hasDigitLimits = true;
                    totalDigits = paramToInt;
                }
            }

            if (formatParameters.HasParam(NUM_DECIMAL_SIZE, out param))
            {
                int paramToInt;
                if (Int32.TryParse(param, out paramToInt))
                {
                    if (paramToInt < 0)
                        paramToInt = 0;

                    hasDigitLimits = true;
                    fractionDigits = paramToInt;
                }
            }

            if (formatParameters.HasParam(NUM_FRACTION_DELIMITER, out param))
            {
                nf.NumberDecimalSeparator = param;
            }

            if (formatParameters.HasParam(NUM_GROUPS_DELIMITER, out param))
            {
                nf.NumberGroupSeparator = param;
            }
            else
            {
                nf.NumberGroupSeparator = " ";
            }

            if (formatParameters.HasParam(NUM_GROUPING, out param))
            {
                nf.NumberGroupSizes = ParseGroupSizes(param);
            }

            if (formatParameters.HasParam(NUM_NEGATIVE_APPEARANCE, out param))
            {
                int pattern;
                if (int.TryParse(param, out pattern))
                    nf.NumberNegativePattern = pattern;
            }

            char leadingFormatSpecifier = '#';
            if (formatParameters.HasParam(NUM_LEADING_ZERO, out param))
            {
                leadingFormatSpecifier = '0';
            }

            StringBuilder formatBuilder = new StringBuilder();

            if (hasDigitLimits)
            {
                ApplyNumericSizeRestrictions(ref p, totalDigits, fractionDigits);

                int repeatCount = totalDigits - fractionDigits;
                if (repeatCount < 0)
                    repeatCount = 1;

                formatBuilder.Append(leadingFormatSpecifier, repeatCount);

                ApplyDigitsGrouping(formatBuilder, nf);

                formatBuilder.Append('.');
                formatBuilder.Append('#', fractionDigits);

            }
            else
            {
                int precision = GetDecimalPrecision(Decimal.GetBits(p));
                nf.NumberDecimalDigits = precision;
                formatBuilder.Append("N");
            }

            return p.ToString(formatBuilder.ToString(), nf);

        }

        private static int[] ParseGroupSizes(string param)
        {
            if (param == "" || param == "0")
                return new[] { 0 };

            List<int> sizes = new List<int>();
            for (int i = 0; i < param.Length; i++)
            {
                if (Char.IsNumber(param, i))
                {
                    sizes.Add(GetCharInteger(param[i]));
                }
            }

            return sizes.ToArray();
        }

        private static int GetCharInteger(char digit)
        {
            return (int)digit - 0x30; // keycode offset
        }

        public static void ApplyNumericSizeRestrictions(ref decimal p, int totalDigits, int fractionDigits)
        {
            if (totalDigits == 0)
                return;

            decimal value = Math.Round(p, fractionDigits);
            int sign = Math.Sign(value);
            value = Math.Abs(value);

            if (totalDigits < fractionDigits)
                totalDigits = fractionDigits;

            if (totalDigits > decimal_digits_sizes[2])
                totalDigits = decimal_digits_sizes[2];

            var bits = Decimal.GetBits(value);

            int digits = 1;

            for (int i = 0; i < 3; i++)
            {
                if ((uint)bits[i] == uint.MaxValue)
                {
                    digits = decimal_digits_sizes[i];
                }
                else
                {
                    uint divided = (uint)bits[i];
                    if (divided == 0 && digits == 1)
                    {
                        digits = 0;
                        break;
                    }

                    while ((divided /= 10) >= 1)
                    {
                        digits++;
                    }
                    break;
                }
            }

            int power = GetDecimalPrecision(bits);
            int digitsLengthAvailable = totalDigits - power;

            if (digits - power > digitsLengthAvailable)
            {
                string fake = new String('9', totalDigits);
                int pointPos = totalDigits - fractionDigits;
                fake = fake.Insert(pointPos, ".");
                value = Decimal.Parse(fake, NumberStyles.AllowDecimalPoint, NumberFormatInfo.InvariantInfo);
            }

            p = value * sign;
        }

        private static void ApplyDigitsGrouping(StringBuilder builder, NumberFormatInfo nf)
        {
            const char SEPARATOR_PLACEHOLDER = ',';

            int firstIndex = builder.Length;
            if (firstIndex <= 0)
                return;

            int offset = firstIndex;
            for (int i = 0; i < nf.NumberGroupSizes.Length; i++)
            {
                int size = nf.NumberGroupSizes[i];
                int insertionPoint = offset - size;
                if (insertionPoint <= 0)
                    break;

                builder.Insert(insertionPoint, SEPARATOR_PLACEHOLDER);
                offset -= size;
            }

            if (offset > 0)
            {
                int size = nf.NumberGroupSizes[nf.NumberGroupSizes.Length - 1];
                while (offset > 0)
                {
                    int insertionPoint = offset - size;
                    if (insertionPoint <= 0)
                        break;

                    builder.Insert(insertionPoint, SEPARATOR_PLACEHOLDER);
                    offset -= size;
                }
            }

        }

        private static int GetDecimalPrecision(int[] bits)
        {
            uint power = 0;
            unchecked
            {
                power = (uint)bits[3] & 0x00FF0000;
                power >>= 16;
            }

            return (int)power;
        }

        #endregion

        #region Date formatting

        private static string FormatDate(DateTime dateTime, FormatParametersList formatParameters)
        {
            var locale = formatParameters.GetParamValue(LOCALE);
            DateTimeFormatInfo df;
            if (locale != null)
            {
                // culture codes in 1C-style
                var culture = CreateCulture(locale);
                df = (DateTimeFormatInfo)culture.DateTimeFormat.Clone();
            }
            else
            {
                var currentDF = DateTimeFormatInfo.CurrentInfo;
                if (currentDF == null)
                    df = new DateTimeFormatInfo();
                else
                    df = (DateTimeFormatInfo)currentDF.Clone();
            }

            string param;

            if (dateTime == DateTime.MinValue)
            {
                if (formatParameters.HasParam(DATE_EMPTY, out param))
                {
                    return param;
                }

                return String.Empty;
            }

            string formatString = "G";

            if (formatParameters.HasParam(DATE_FORMAT, out param))
            {
                formatString = ProcessDateFormat(param);
            }

            if (formatParameters.HasParam(DATE_LOCAL_FORMAT, out param))
            {
                formatString = ProcessLocalDateFormat(param);
            }

            return dateTime.ToString(formatString, df);

        }

        private static string ProcessDateFormat(string param)
        {
            var builder = new StringBuilder(param);
            for (int i = 0; i < param.Length; i++)
            {
                if (param[i] == '\'')
                {
                    i++;
                    while (param[i] != '\'' && i < param.Length)
                        i++;
                    continue;
                }

                if (param[i] == 'д')
                {
                    builder[i] = 'd';
                    continue;
                }

                if (param[i] == 'М')
                {
                    builder[i] = 'M';
                    continue;
                }

                if (param[i] == 'г')
                {
                    builder[i] = 'y';
                    continue;
                }

                if (param[i] == 'к')
                {
                    builder[i] = 'q';
                    continue;
                }

                if (param[i] == 'ч')
                {
                    builder[i] = 'h';
                    continue;
                }
                if (param[i] == 'Ч')
                {
                    builder[i] = 'H';
                    continue;
                }
                if (param[i] == 'м')
                {
                    builder[i] = 'm';
                    continue;
                }
                if (param[i] == 'с')
                {
                    builder[i] = 's';
                    continue;
                }

                if (param[i] == 'в' && i + 1 < param.Length && param[i + 1] == 'в')
                {
                    builder[i] = 't';
                    builder[i + 1] = 't';
                    i++;
                    continue;
                }

            }
            builder.Replace("/", "\\/");
            builder.Replace("%", "\\%");

            return builder.ToString();
        }

        private static string ProcessLocalDateFormat(string param)
        {
            const string DATETIME_RU = "ДВ";
            const string DATETIME_EN = "DT";
            const string DATE_RU = "Д";
            const string DATE_EN = "D";
            const string LONG_DATE_RU = "ДД";
            const string LONG_DATE_EN = "DD";
            const string LONG_DATETIME_RU = "ДДВ";
            const string LONG_DATETIME_EN = "DDT";
            const string TIME_RU = "В";
            const string TIME_EN = "T";

            param = param.ToUpper();
            switch (param)
            {
                case DATETIME_RU:
                case DATETIME_EN:
                    return "G";
                case LONG_DATETIME_EN:
                case LONG_DATETIME_RU:
                    return "F";
                case LONG_DATE_EN:
                case LONG_DATE_RU:
                    return "D";
                case DATE_EN:
                case DATE_RU:
                    return "d";
                case TIME_EN:
                case TIME_RU:
                    return "T";
                default:
                    return "G";
            }
        }

        #endregion

        private static string DefaultFormat(IValue value, FormatParametersList formatParameters)
        {
            throw new NotImplementedException();
        }

        private static FormatParametersList ParseParameters(string format)
        {
            return new FormatParametersList(format);
        }

        private static CultureInfo CreateCulture(string locale)
        {
            locale = locale.Replace('_', '-');
            var culture = System.Globalization.CultureInfo.CreateSpecificCulture(locale);
            return culture;
        }



    }
}
