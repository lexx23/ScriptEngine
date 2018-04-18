/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Parser.TokenParser.Parsers;
using ScriptEngine.EngineBase.Parser.TokenParser;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Parser
{
    /// <summary>
    /// Парсер токенов.
    /// </summary>
    public class ParserClass
    {
        private SourceIterator _iterator;
        private TokenParserFactory _factory;


        public ParserClass(string module_name,string module_src)
        {
            _iterator = new SourceIterator(module_name, module_src);
            _factory = new TokenParserFactory();
            _iterator.MoveNext();


            _factory.Add(TokenTypeEnum.IDENTIFIER, new IdentifierTokenParser());
            _factory.Add(TokenTypeEnum.LITERAL, new LiteralTokenParser());
            _factory.Add(TokenTypeEnum.PUNCTUATION, new PunctuationTokenParser());
            _factory.Add(TokenTypeEnum.NUMBER, new NumberTokenParser());
        }

        /// <summary>
        /// Итератор токенов.
        /// </summary>
        /// <returns></returns>
        public TokenIteratorBase GetEnumerator()
        {
            return new ParserTokenIterator(this);
        }


        /// <summary>
        /// Получить следующий токен.
        /// </summary>
        /// <returns></returns>
        internal bool NextToken(out IToken token)
        {
            token = null;
            return _factory.Parse(_iterator,out token);
        }


        /// <summary>
        /// Список найденных токенов.
        /// </summary>
        /// <returns>Список найденных лексем и их токенов</returns>
        public IList<IToken> GetAllTokens()
        {
            IToken token = null;
            IList<IToken> token_list = new List<IToken>();

            do
            {
                _factory.Parse(_iterator, out token);
                token_list.Add(token);

                Console.Write($"{token.Content} ( {token.Type} - {token.SubType} )\n");
            }
            while (token.Content != "\0");

            return token_list;
        }
    }
}
