/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

namespace ScriptEngine.EngineBase.Parser.Token
{
    public interface IToken
    {
        TokenTypeEnum Type { get; set; }

        TokenSubTypeEnum SubType { get; set; }

        /// <summary>
        /// Текст токена из модуля. 
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Информация о коде где был получен токен.
        /// </summary>
        CodeInformation CodeInformation { get; set; }


        IToken Clone();
        string ToString();
    }
}
