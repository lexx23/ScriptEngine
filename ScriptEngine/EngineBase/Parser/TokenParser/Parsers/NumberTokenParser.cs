using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсел лексем чисел.
    /// </summary>
    public class NumberTokenParser : ITokenParser
    {
        public bool Parse(SourceIterator iterator, out IToken token)
        {
            token = null;
            string content = string.Empty;
            if (Char.IsNumber(iterator.Current))
            {
                content = iterator.GetDigits();
                token = new TokenClass()
                {
                    Content = content,
                    CodeInformation = iterator.CodeInformation.Clone(),
                    Type = TokenTypeEnum.NUMBER
                };
                return true;
            }
            return false;
        }
    }
}
