using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсер строковых токенов ""
    /// </summary>
    public class LiteralTokenParser : ITokenParser
    {
        /// <summary>
        /// Проверка формата даты.
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        private void IsValidDate(string date, CodeInformation information)
        {
            if (date.Length > 14)
                throw new ExceptionBase(information, "Дата не может быть длинной более 14 символов.");

            foreach (char symbol in date)
            {
                if (!Char.IsNumber(symbol))
                    throw new ExceptionBase(information, String.Format("Не верный формат даты, символ '{0}' .", symbol));
            }
        }


        private bool ParseDate(SourceIterator iterator, out string date)
        {
            CodeInformation inforamtion;
            date = string.Empty;

            if (iterator.Current == '\'')
            {
                inforamtion = iterator.CodeInformation.Clone();
                do
                {
                    date += iterator.Current;
                    if (iterator.Current == '\'' && iterator.CodeInformation.ColumnNumber != inforamtion.ColumnNumber)
                    {
                        iterator.MoveNext();
                        break;
                    }

                }
                while (iterator.MoveNext());


                if (date[date.Length - 1] != '\'')
                    throw new ExceptionBase(iterator.CodeInformation, "Ожидется символ \'.");

                date = date.Remove(0, 1);
                date = date.Remove(date.Length - 1, 1);

                IsValidDate(date, inforamtion);

                return true;
            }
            return false;
        }


        private bool ParseString(SourceIterator iterator, out string str)
        {
            int counter = 0;
            str = string.Empty;

            CodeInformation inforamtion;

            if (iterator.Current == '"')
            {
                inforamtion = iterator.CodeInformation.Clone();
                do
                {
                    str += iterator.Current;
                    if (iterator.Current == '"')
                    {
                        counter++;
                        if (counter % 2 == 0 && iterator.GetForwardSymbol() != '"')
                        {
                            iterator.MoveNext();
                            break;
                        }
                    }

                }
                while (iterator.MoveNext());

                if (str[str.Length - 1] != '"')
                    throw new ExceptionBase(iterator.CodeInformation, "Ожидется символ \"");

                str = str.Remove(0, 1);
                str = str.Remove(str.Length - 1, 1);

                return true;

            }
            return false;
        }



        public bool Parse(SourceIterator iterator, out TokenClass token)
        {
            token = null;
            string content = string.Empty;

            if (ParseDate(iterator, out content))
            {
                token = new TokenClass()
                {
                    Content = content,
                    Type = TokenTypeEnum.LITERAL,
                    SubType = TokenSubTypeEnum.L_DATE
                };
                return true;
            }

            if (ParseString(iterator, out content))
            {
                token = new TokenClass()
                {
                    Content = content,
                    Type = TokenTypeEnum.LITERAL,
                    SubType = TokenSubTypeEnum.L_STRING
                };
                return true;
            }

            return false;
        }
    }
}
