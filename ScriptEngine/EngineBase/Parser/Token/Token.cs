/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Parser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public class TokenClass : IToken
    {
        /// <summary>
        /// Тип токена.
        /// </summary>
        public TokenTypeEnum Type{ get; set; }

        public TokenSubTypeEnum SubType { get; set; }

        /// <summary>
        /// Текст токена из модуля. 
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// Информация о коде где был получен токен.
        /// </summary>
        public CodeInformation CodeInformation { get; set; }


        public TokenClass()
        {
            SubType = TokenSubTypeEnum.NA;
        }

        public IToken Clone()
        {
            return (IToken)this.MemberwiseClone();
        }

        public override string ToString()
        {
            return EnumStringAttribute.GetStringValue(Type) + (SubType != TokenSubTypeEnum.NA ? " " + EnumStringAttribute.GetStringValue(SubType) : "");
        }

    }
}
