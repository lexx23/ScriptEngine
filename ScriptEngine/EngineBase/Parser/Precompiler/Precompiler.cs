/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Parser.Precompiler.Directives;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Parser.Precompiler
{
    /// <summary>
    /// Обработчик директив прекомпилятора.
    /// </summary>
    public class PrecompilerClass
    {
        private TokenIteratorBase _iterator;
        private PrecompilerStack _stack;
        private IDictionary<string, bool> _defines;
        private IList<DirectiveBase> _directives;

        public PrecompilerClass(TokenIteratorBase iterator, IDictionary<string, bool> defines)
        {
            _iterator = iterator ?? throw new Exception("Обработчик инструкций препроцессора, не получил итератор.");
            _defines = defines;

            _stack = new PrecompilerStack();
            _directives = new List<DirectiveBase>();

            _directives.Add(new IfDirective(_iterator, _stack, defines));
            _directives.Add(new RegionDirective(_iterator, _stack, defines));
        }

        /// <summary>
        /// Проверить, текущий токен дериктива препроцессора?
        /// </summary>
        /// <returns></returns>
        public bool IsDirective()
        {
            if (_iterator.Current.Type == TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType == TokenSubTypeEnum.PRECOMP)
                return true;
            return false;
        }

        /// <summary>
        /// Выбор обработчика для токена
        /// </summary>
        private void ProcessDirective()
        {
            _iterator.MoveNext();
            _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);


            foreach (DirectiveBase directive in _directives)
            {
                if (directive.ProcessDirective())
                    return;
            }

            throw new CompilerException(_iterator.Current.CodeInformation, $"Оператор препроцессора #{_iterator.Current.Content} не распознан.");
        }


        /// <summary>
        /// Обработка текущего токена.
        /// </summary>
        public void Process()
        {
            do
            {
                while (IsDirective())
                    ProcessDirective();

                if (_stack.Skip)
                    if (!_iterator.MoveNext())
                        break;

            } while (_stack.Skip);
        }

        /// <summary>
        /// Сообщить о ошибке, если такая имеется (незакрытые токены).
        /// </summary>
        public void Error()
        {
            if (_stack.Count == 0)
                return;
            PrecompilerStackStruct str = _stack.Peek();
            throw new CompilerException(str.Token.CodeInformation, $"Ожидается завершение оператора препроцессора #{str.Token.Content}");

        }


        /// <summary>
        /// Итератор токенов.
        /// </summary>
        /// <returns></returns>
        public TokenIteratorBase GetEnumerator()
        {
            return new PrecompilerTokenIterator(_iterator,this);
        }


        /// <summary>
        /// Список найденых токенов.
        /// </summary>
        /// <returns>Список найденых лексем и их токенов</returns>
        public IList<IToken> GetAllTokens()
        {
            IToken token = null;
            IList<IToken> token_list = new List<IToken>();
            TokenIteratorBase iterator = GetEnumerator();
            do
            {
                iterator.MoveNext();
                token = iterator.Current;
                token_list.Add(token);
            }
            while (token.Content != "\0");

            return token_list;
        }
    }
}
