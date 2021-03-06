﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using System;


namespace ScriptEngine.EngineBase.Parser.Precompiler
{
    class PrecompilerTokenIterator : TokenIteratorBase
    {
        private TokenIteratorBase _iterator;
        private PrecompilerClass _precompiler;

        public PrecompilerTokenIterator(TokenIteratorBase iterator, PrecompilerClass precompiler)
        {
            _iterator = iterator;
            _precompiler = precompiler;
        }

        public override bool MoveNext()
        {
            _iterator.MoveNext();

            _precompiler.Process();

            _token = _iterator.Current;
            if (_token.Content == "\0")
            {
                _precompiler.Error();
                return false;
            }

            if (_token == null)
                throw new CompilerException(_iterator.Current.CodeInformation, "Не удалось получить токен.");

            if (_precompiler.IsDirective())
                throw new Exception("Есть не обработанная директива.");

            return true;
        }
    }
}
