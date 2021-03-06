﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсер лексем операторов.
    /// </summary>
    public class PunctuationTokenParser : ITokenParser
    {
        private static IDictionary<string, TokenSubTypeEnum> _punctuation_table;


        public PunctuationTokenParser()
        {
            _punctuation_table = new Dictionary<string, TokenSubTypeEnum>
            {
                { ">=", TokenSubTypeEnum.P_LOGIC_GEQ },
                { "<=", TokenSubTypeEnum.P_LOGIC_LEQ },
                { "<>", TokenSubTypeEnum.P_LOGIC_UNEQ },
                { ">", TokenSubTypeEnum.P_LOGIC_GREATER },
                { "<", TokenSubTypeEnum.P_LOGIC_LESS },

                { "*", TokenSubTypeEnum.P_MUL },
                { "/", TokenSubTypeEnum.P_DIV },
                { "%", TokenSubTypeEnum.P_MOD },
                { "+", TokenSubTypeEnum.P_ADD },
                { "-", TokenSubTypeEnum.P_SUB },
                { "=", TokenSubTypeEnum.P_ASSIGN },

                { ".", TokenSubTypeEnum.P_DOT },

                { ",", TokenSubTypeEnum.P_COMMA },
                { ";", TokenSubTypeEnum.P_SEMICOLON },
                { ":", TokenSubTypeEnum.P_COLON },

                { "?", TokenSubTypeEnum.P_QUESTION },

                { "(", TokenSubTypeEnum.P_PARENTHESESOPEN },
                { ")", TokenSubTypeEnum.P_PARENTHESESCLOSE },
                { "[", TokenSubTypeEnum.P_SQBRACKETOPEN },
                { "]", TokenSubTypeEnum.P_SQBRACKETCLOSE },

                { "#", TokenSubTypeEnum.PRECOMP },
                { "&", TokenSubTypeEnum.COMP },
                { "~", TokenSubTypeEnum.P_TILDE }
            };
        }

        public bool Parse(SourceIterator iterator, out IToken token)
        {
            token = null;
            string content = string.Empty;
            TokenSubTypeEnum subtype;

            if (_punctuation_table.TryGetValue(iterator.Current.ToString(),out subtype))
            {
                char forward_symbol;

                CodeInformation information = iterator.CodeInformation.Clone();
                forward_symbol = iterator.GetForwardSymbol();
                content += iterator.Current;

                if ((forward_symbol == '>' || forward_symbol == '=') && (iterator.Current == '<' || iterator.Current == '>'))
                {
                    content = iterator.Current + forward_symbol.ToString();
                    _punctuation_table.TryGetValue(content, out subtype);

                    iterator.MoveNext();
                }

                iterator.MoveNext();

                token = new TokenClass()
                {
                    Content = content,
                    CodeInformation = information,
                    Type = TokenTypeEnum.PUNCTUATION,
                    SubType = subtype
                };
                return true;
            }
            else
                return false;
        }
    }
}
