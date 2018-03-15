using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using System;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсер строковых токенов "" и даты.
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
                throw new CompilerException(information, "Дата не может быть длинной более 14 символов.");

            foreach (char symbol in date)
            {
                if (!Char.IsNumber(symbol))
                    throw new CompilerException(information, String.Format("Не верный формат даты, символ '{0}' .", symbol));
            }
        }

        /// <summary>
        /// Парсинг даты.
        /// </summary>
        /// <param name="iterator"></param>
        /// <param name="date"></param>
        /// <returns></returns>
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
                    throw new CompilerException(iterator.CodeInformation, "Ожидается символ \'.");

                date = date.Remove(0, 1);
                date = date.Remove(date.Length - 1, 1);

                IsValidDate(date, inforamtion);

                return true;
            }
            return false;
        }

        /// <summary>
        /// Парсинг текстовой строки в кавычках ""
        /// </summary>
        /// <param name="iterator"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        private bool ParseString(SourceIterator iterator, out string str)
        {
            str = string.Empty;

            if (iterator.Current == '"')
            {
                str = iterator.GetString();
                return true;
            }
            return false;
        }


        /// <summary>
        /// Парсер даты и строк.
        /// </summary>
        /// <param name="iterator"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public bool Parse(SourceIterator iterator, out IToken token)
        {
            token = null;
            string content = string.Empty;

            CodeInformation information = iterator.CodeInformation.Clone();

            if (ParseDate(iterator, out content))
            {
                token = new TokenClass()
                {
                    Content = content,
                    CodeInformation = information,
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
                    CodeInformation = information,
                    Type = TokenTypeEnum.LITERAL,
                    SubType = TokenSubTypeEnum.L_STRING
                };
                return true;
            }

            return false;
        }
    }
}
