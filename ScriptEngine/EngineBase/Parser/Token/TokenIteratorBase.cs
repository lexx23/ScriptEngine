/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System.Collections;
using System;

namespace ScriptEngine.EngineBase.Parser.Token
{
    /// <summary>
    /// Итератор токенов исходного года.
    /// </summary>
    public abstract class TokenIteratorBase : IEnumerator<IToken>
    {
        protected IToken _token;
        protected ParserClass _parser;


        /// <summary>
        /// Текущий токен.
        /// </summary>
        public IToken Current { get => _token; }

        object IEnumerator.Current { get => (object)Current; }

        public void Dispose()
        {
            _parser = null;
        }

        /// <summary>
        /// Получить следующующий токен.
        /// </summary>
        /// <returns></returns>
        public abstract bool MoveNext();


        /// <summary>
        /// Не реализован.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Проверить тип токена, если токен не того типа ошибка. Итератор не делает шаг.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public void IsTokenType(TokenTypeEnum type, TokenSubTypeEnum subtype = TokenSubTypeEnum.ANY)
        {
            if (subtype != TokenSubTypeEnum.ANY)
            {
                if (Current.Type != type || Current.SubType != subtype)
                    throw new CompilerException(Current.CodeInformation, $"Ожидается [{EnumStringAttribute.GetStringValue(type) + (subtype != TokenSubTypeEnum.NA ? "-" + EnumStringAttribute.GetStringValue(subtype) : "")}] а найден [{Current.ToString()}]");
            }
            else
                if (Current.Type != type)
                throw new CompilerException(Current.CodeInformation, $"Ожидается [{EnumStringAttribute.GetStringValue(type)}] а найден [{EnumStringAttribute.GetStringValue(Current.SubType)}]");
        }

        /// <summary>
        /// Проверить содержимое токена и в случае совпадения перейти на следующий токен.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public bool CheckToken(string content)
        {
            if (!String.Equals(Current.Content,content,StringComparison.OrdinalIgnoreCase))
                return false;

            MoveNext();

            return true;
        }

        /// <summary>
        /// Проверить тип токена и в случае совпадения перейти на следующий токен.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        /// <returns></returns>
        public bool CheckToken(TokenTypeEnum type, TokenSubTypeEnum subtype = TokenSubTypeEnum.ANY)
        {
            if (subtype != TokenSubTypeEnum.ANY)
            {
                if (Current.Type != type || Current.SubType != subtype)
                    return false;
            }
            else
                if (Current.Type != type)
                return false;

            MoveNext();

            return true;
        }


        /// <summary>
        /// Проверка текущего содержимого токена. Если содержимое не такое как ожидалось, то ошибка. Иначе переход на следующий токен.
        /// </summary>
        /// <param name="content"></param>
        public void ExpectToken(string content)
        {
            if (!String.Equals(Current.Content,content,StringComparison.OrdinalIgnoreCase))
                throw new CompilerException($"Ожидается токен {content} , а получен {Current.Content}");

            MoveNext();
        }

        /// <summary>
        /// Проверка типа токена. Если тип не такой как ожидалось, то ошибка. Иначе переход на следующий токен.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="subtype"></param>
        public void ExpectToken(TokenTypeEnum type, TokenSubTypeEnum subtype = TokenSubTypeEnum.NA)
        {
            if (Current.Type != type || Current.SubType != subtype)
                throw new CompilerException(Current.CodeInformation, $"Ожидается [{EnumStringAttribute.GetStringValue(type) + (subtype != TokenSubTypeEnum.NA ? "-" + EnumStringAttribute.GetStringValue(subtype) : "")}] а найден [{Current.ToString()}]");

            MoveNext();
        }

    }
}
