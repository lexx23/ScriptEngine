/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

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
        /// Парсинг даты.
        /// </summary>
        /// <param name="iterator"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private bool ParseDate(SourceIterator iterator, out string date)
        {
            CodeInformation information;
            date = string.Empty;

            if (iterator.Current == '\'')
            {
                information = iterator.CodeInformation.Clone();
                do
                {
                    if (Char.IsNumber(iterator.Current))
                        date += iterator.Current;
                    if (iterator.Current == '\'' && iterator.CodeInformation.ColumnNumber != information.ColumnNumber)
                    {
                        iterator.MoveNext();
                        break;
                    }

                }
                while (iterator.MoveNext());

                if (date.Length > 14)
                    throw new CompilerException(information, "Дата не может быть длинной более 14 символов.");

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
