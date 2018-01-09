using ScriptEngine.EngineBase.Parser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser
{
    public class ParserTokenIterator : TokenIteratorBase
    {
        public ParserTokenIterator(ParserClass parser)
        {
            _parser = parser;
        }

        public override bool MoveNext()
        {
            return _parser.NextToken(out _token);
        }
    }
}
