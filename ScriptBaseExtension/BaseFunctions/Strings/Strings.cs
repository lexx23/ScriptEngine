using ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections;
using ScriptBaseFunctionsLibrary.Enums;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using System;
using System.Linq;

namespace ScriptBaseFunctionsLibrary.BaseFunctions.Strings
{
    [LibraryClassAttribute(AsGlobal = true, Name = "strings_library")]
    public class Strings
    {
        [LibraryClassMethodAttribute(Alias = "Строка", Name = "String")]
        public IValue String(IValue value)
        {
            return ValueFactory.Create(value.AsString());
        }

        [LibraryClassMethodAttribute(Alias = "Формат", Name = "Format")]
        public string Format(IValue value, string format)
        {
            return FormatFunction.Format(value, format);
        }

        [LibraryClassMethodAttribute(Alias = "Найти", Name = "Find")]
        public int Find(IValue str, IValue substring)
        {
            return str.AsString().IndexOf(substring.AsString(), StringComparison.Ordinal) + 1;
        }

        [LibraryClassMethodAttribute(Alias = "НРег", Name = "Lower")]
        public string Lower(IValue str)
        {
            return str.AsString().ToLower();
        }

        [LibraryClassMethodAttribute(Alias = "СтрЗаменить", Name = "StrReplace")]
        public IValue StrReplace(IValue str, IValue substring_search, IValue substring_replace)
        {
            string result = str.AsString().Replace(substring_search.AsString(), substring_replace.AsString());
            return ValueFactory.Create(result);
        }

        [LibraryClassMethodAttribute(Alias = "СтрШаблон", Name = "StrTemplate")]
        private IValue StrTemplate(IValue[] arguments)
        {
            if (arguments.Length < 1)
                throw new Exception("Недостаточно фактических параметров (СтрШаблон)");

            var srcFormat = arguments[0].AsString();
            if (srcFormat == string.Empty)
                return ValueFactory.Create("");

            var re = new System.Text.RegularExpressions.Regex(@"(%%)|(%\d+)|(%\D)");
            int matchCount = 0;
            int passedArgsCount = arguments.Skip(1).Count(x => x != null && x.BaseType != ValueTypeEnum.NULL);
            var result = re.Replace(srcFormat, (m) =>
            {
                if (m.Groups[1].Success)
                    return "%";

                if (m.Groups[2].Success)
                {
                    matchCount++;
                    var number = int.Parse(m.Groups[2].Value.Substring(1));
                    if (number < 1 || number > 11)
                        throw new Exception("Ошибка при вызове метода контекста (СтрШаблон): Ошибка синтаксиса шаблона в позиции " + (m.Index + 1));

                    if (arguments[number] != null)
                        return arguments[number].AsString();
                    else
                        return "";
                }

                throw new Exception("Ошибка при вызове метода контекста (СтрШаблон): Ошибка синтаксиса шаблона в позиции " + (m.Index + 1));

            });

            if (passedArgsCount > matchCount)
                throw new Exception("Передано параметров больше чем объявлено в строке.");

            return ValueFactory.Create(result);
        }

        [LibraryClassMethodAttribute(Alias = "СтрЧислоСтрок", Name = "StrLineCount")]
        public int StrLineCount(IValue str)
        {
            int pos = 0;
            int line_count = 1;
            string data = str.AsString();

            while (pos >= 0 && pos < data.Length)
            {
                pos = data.IndexOf('\n', pos);
                if (pos >= 0)
                {
                    line_count++;
                    pos++;
                }
            }
            return line_count;
        }


        [LibraryClassMethodAttribute(Alias = "СтрДлина", Name = "StrLen")]
        public int StrLen(IValue str)
        {
            return str.AsString().Length;
        }

        [LibraryClassMethodAttribute(Alias = "СтрПолучитьСтроку", Name = "StrGetLine")]
        public IValue StrGetLine(IValue str, IValue number)
        {
            int line_number = number.AsInt();
            var str_arg = str.AsString();
            string result = "";
            if (line_number >= 1)
            {
                string[] subStrVals = str_arg.Split('\n', line_number + 1);
                result = subStrVals[line_number - 1];
            }

            return ValueFactory.Create(result);
        }

        [LibraryClassMethodAttribute(Alias = "СокрЛП", Name = "TrimAll")]
        public string TrimAll(IValue str)
        {
            return str.AsString().Trim();
        }

        [LibraryClassMethodAttribute(Alias = "СтрНачинаетсяС", Name = "StrStartsWith")]
        public bool StrStartsWith(string inputString, string searchString)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(inputString))
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    result = inputString.StartsWith(searchString);
                }
                else throw new Exception("Ошибка при вызове метода контекста (СтрНачинаетсяС): Недопустимое значение параметра (параметр номер '2')");
            }

            return result;
        }

        /// <summary>
        /// Определяет, заканчивается ли строка указанной подстрокой.
        /// </summary>
        /// <param name="inputString">Строка, окончание которой проверяется на совпадение с подстрокой поиска.</param>
        /// <param name="searchString">Строка, содержащая предполагаемое окончание строки. В случае если переданное значение является пустой строкой генерируется исключительная ситуация.</param>
        [LibraryClassMethodAttribute(Alias = "СтрЗаканчиваетсяНа", Name = "StrEndsWith")]
        public bool StrEndsWith(string inputString, string searchString)
        {
            bool result = false;

            if (!string.IsNullOrEmpty(inputString))
            {
                if (!string.IsNullOrEmpty(searchString))
                {
                    result = inputString.EndsWith(searchString);
                }
                else throw new Exception("Ошибка при вызове метода контекста (СтрЗаканчиваетсяНа): Недопустимое значение параметра (параметр номер '2')");
            }

            return result;
        }


        /// <summary>
        /// Разделяет строку на части по указанным символам-разделителям.
        /// </summary>
        /// <param name="inputString">Разделяемая строка.</param>
        /// <param name="stringDelimiter">Строка символов, каждый из которых является индивидуальным разделителем.</param>
        /// <param name="includeEmpty">Указывает необходимость включать в результат пустые строки, которые могут образоваться в результате разделения исходной строки. Значение по умолчанию: Истина. </param>
        [LibraryClassMethodAttribute(Alias = "СтрРазделить", Name = "StrSplit")]
        public ScriptArray StrSplit(string inputString, string stringDelimiter, IValue includeEmpty = null)
        {
            bool includeEmpty_bool = false;
            string[] arrParsed;
            if (includeEmpty.BaseType == ValueTypeEnum.NULL)
                includeEmpty_bool = true;
            else
                includeEmpty_bool = includeEmpty.AsBoolean();

            if (!string.IsNullOrEmpty(inputString))
            {
                if (!string.IsNullOrEmpty(stringDelimiter))
                {
                    arrParsed = inputString.Split(new string[] { stringDelimiter }, includeEmpty_bool ? StringSplitOptions.None : StringSplitOptions.RemoveEmptyEntries);
                }
                else
                {
                    arrParsed = new string[] { inputString };
                }
            }
            else
            {
                arrParsed = new string[] { string.Empty };
            }
            return new ScriptArray(arrParsed.Select(x => ValueFactory.Create(x)));
        }

        /// <summary>
        /// Соединяет массив переданных строк в одну строку с указанным разделителем
        /// </summary>
        /// <param name="input">Массив - соединяемые строки</param>
        /// <param name="delimiter">Разделитель. Если не указан, строки объединяются слитно</param>
        [LibraryClassMethodAttribute(Alias = "СтрСоединить", Name = "StrConcat")]
        public string StrConcat(ScriptArray input, string delimiter = null)
        {
            var strings = input.Select(x => x.AsString());

            return string.Join(delimiter, strings);
        }

        /// <summary>
        /// Сравнивает строки без учета регистра.
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <returns>-1 первая строка больше, 1 - вторая строка больше. 0 - строки равны</returns>
        [LibraryClassMethodAttribute(Alias = "СтрСравнить", Name = "StrCompare")]
        public int StrCompare(string first, string second)
        {
            return string.Compare(first, second, true);
        }


        /// <summary>
        /// Находит вхождение искомой строки как подстроки в исходной строке
        /// </summary>
        /// <param name="haystack">Строка, в которой ищем</param>
        /// <param name="needle">Строка, которую надо найти</param>
        /// <param name="direction">значение перечисления НаправлениеПоиска (с конца/с начала)</param>
        /// <param name="startPos">Начальная позиция, с которой начинать поиск</param>
        /// <param name="occurance">Указывает номер вхождения искомой подстроки в исходной строке</param>
        /// <returns>Позицию искомой строки в исходной строке. Возвращает 0, если подстрока не найдена.</returns>
        [LibraryClassMethodAttribute(Alias = "СтрНайти", Name = "StrFind")]
        public int StrFind(string haystack, string needle, ScriptSearchDirectionInner direction = ScriptSearchDirectionInner.FromBegin, int startPos = 0, int occurance = 0)
        {
            int len = haystack.Length;
            if (len == 0 || needle.Length == 0)
                return 0;

            bool fromBegin = direction == ScriptSearchDirectionInner.FromBegin;

            if (startPos == 0)
            {
                startPos = fromBegin ? 1 : len;
            }

            if (startPos < 1 || startPos > len)
                throw new Exception("Неверное значение аргумента");

            if (occurance == 0)
                occurance = 1;

            int startIndex = startPos - 1;
            int foundTimes = 0;
            int index = len + 1;

            if (fromBegin)
            {
                while (foundTimes < occurance && index >= 0)
                {
                    index = haystack.IndexOf(needle, startIndex, StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        startIndex = index + 1;
                        foundTimes++;
                    }
                    if (startIndex >= len)
                        break;
                }

            }
            else
            {
                while (foundTimes < occurance && index >= 0)
                {
                    index = haystack.LastIndexOf(needle, startIndex, StringComparison.Ordinal);
                    if (index >= 0)
                    {
                        startIndex = index - 1;
                        foundTimes++;
                    }
                    if (startIndex < 0)
                        break;
                }

            }

            if (foundTimes == occurance)
                return index + 1;
            else
                return 0;
        }


        /// <summary>
        /// Функция НСтр имеет ограниченную поддержку и может использоваться только для упрощения портирования кода из 1С.
        /// Возвращает только строку на первом языке из списка, если второй параметр не указан. (Игнорирует "язык по-умолчанию")
        /// </summary>
        /// <param name="src">Строка на нескольких языках</param>
        /// <param name="lang">Код языка (если не указан, возвращается первый возможный вариант)</param>
        [LibraryClassMethodAttribute(Alias = "НСтр", Name = "NStr")]
        public string NStr(string src, string lang = null)
        {
            var parser = new FormatParametersList(src);
            string str;
            if (lang == null)
                str = parser.EnumerateValues().FirstOrDefault();
            else
                str = parser.GetParamValue(lang);

            return str == null ? string.Empty : str;
        }
    }
}
