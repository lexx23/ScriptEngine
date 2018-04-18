/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Parser.Token;
using System;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсер лексем чисел.
    /// </summary>
    public class NumberTokenParser : ITokenParser
    {
        public bool Parse(SourceIterator iterator, out IToken token)
        {
            token = null;
            string content = string.Empty;

            if (Char.IsNumber(iterator.Current))
            {
                CodeInformation information = iterator.CodeInformation.Clone();
                content = iterator.GetDigits();
                token = new TokenClass()
                {
                    Content = content,
                    CodeInformation = information,
                    Type = TokenTypeEnum.NUMBER
                };
                return true;
            }
            return false;
        }
    }
}
