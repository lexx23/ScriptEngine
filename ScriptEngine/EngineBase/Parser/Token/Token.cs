using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Parser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Praser.Token
{
    public class TokenClass : IToken
    {
        /// <summary>
        /// Наименование лексемы.
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
