/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

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
