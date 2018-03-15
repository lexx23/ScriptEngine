using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Parser.TokenParser
{
    /// <summary>
    /// Фабрика лексем, токенов и их парсеры.
    /// </summary>
    public class TokenParserFactory
    {
        private Dictionary<TokenTypeEnum, ITokenParser> _tokens;

        /// <summary>
        /// Лексемы
        /// </summary>
        public Dictionary<TokenTypeEnum, ITokenParser> Lexem { get => _tokens; }



        public TokenParserFactory()
        {
            _tokens = new Dictionary<TokenTypeEnum, ITokenParser>();
        }


        /// <summary>
        /// Добавить токен и его парсер. 
        /// </summary>
        public ITokenParser Add(TokenTypeEnum token_type, ITokenParser parser)
        {
            _tokens.Add(token_type, parser);
            return parser;
        }

        /// <summary>
        /// Создать токен используя парсер исходного кода.
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        public bool Parse(SourceIterator iterator,out IToken token) 
        {
            token = null;
            if (_tokens.Count == 0)
                throw new Exception("Фабрика не содержит парсеров.");

            foreach (KeyValuePair<TokenTypeEnum, ITokenParser> current_lexem_parser in _tokens)
            {
                if (current_lexem_parser.Value.Parse(iterator, out token))
                    return true;
            }

            if (iterator.Current != '\0')
                throw new CompilerException(iterator.CodeInformation, $"Не удалось распознать символ: {iterator.Current}");
            else
            {
                token = new TokenClass()
                {
                    Content = "\0",
                    Type = TokenTypeEnum.PUNCTUATION,
                    CodeInformation = iterator.CodeInformation.Clone(),
                    SubType = TokenSubTypeEnum.EOF
                };
                token.CodeInformation.LineNumber++;
            }

            return false;
        }

    }
}
