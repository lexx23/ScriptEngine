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
