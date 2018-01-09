using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
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
            _punctuation_table = new Dictionary<string, TokenSubTypeEnum>();

            _punctuation_table.Add(">=", TokenSubTypeEnum.P_LOGIC_GEQ);
            _punctuation_table.Add("<=", TokenSubTypeEnum.P_LOGIC_LEQ);
            _punctuation_table.Add("<>", TokenSubTypeEnum.P_LOGIC_UNEQ);
            _punctuation_table.Add(">", TokenSubTypeEnum.P_LOGIC_GREATER);
            _punctuation_table.Add("<", TokenSubTypeEnum.P_LOGIC_LESS);

            _punctuation_table.Add("*", TokenSubTypeEnum.P_MUL);
            _punctuation_table.Add("/", TokenSubTypeEnum.P_DIV);
            _punctuation_table.Add("%", TokenSubTypeEnum.P_MOD);
            _punctuation_table.Add("+", TokenSubTypeEnum.P_ADD);
            _punctuation_table.Add("-", TokenSubTypeEnum.P_SUB);
            _punctuation_table.Add("=", TokenSubTypeEnum.P_ASSIGN);

            _punctuation_table.Add(".", TokenSubTypeEnum.P_DOT);

            _punctuation_table.Add(",", TokenSubTypeEnum.P_COMMA);
            _punctuation_table.Add(";", TokenSubTypeEnum.P_SEMICOLON);

            _punctuation_table.Add("?", TokenSubTypeEnum.QUESTIONMARK);

            _punctuation_table.Add("(", TokenSubTypeEnum.P_PARENTHESESOPEN);
            _punctuation_table.Add(")", TokenSubTypeEnum.P_PARENTHESESCLOSE);
            _punctuation_table.Add("[", TokenSubTypeEnum.SQBRACKETOPEN);
            _punctuation_table.Add("]", TokenSubTypeEnum.SQBRACKETCLOSE);

            _punctuation_table.Add("#", TokenSubTypeEnum.PRECOMP);
            _punctuation_table.Add("&", TokenSubTypeEnum.COMP);
        }

        public bool Parse(SourceIterator iterator, out TokenClass token)
        {
            token = null;
            string content = string.Empty;
            TokenSubTypeEnum subtype;

            if (_punctuation_table.TryGetValue(iterator.Current.ToString(),out subtype))
            {
                char forward_symbol;

                forward_symbol = iterator.GetForwardSymbol();
                content += iterator.Current;

                if (forward_symbol == '>' || forward_symbol == '=')
                {
                    content = iterator.Current + forward_symbol.ToString();
                    _punctuation_table.TryGetValue(content, out subtype);

                    iterator.MoveNext();
                }

                iterator.MoveNext();

                token = new TokenClass()
                {
                    Content = content,
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
