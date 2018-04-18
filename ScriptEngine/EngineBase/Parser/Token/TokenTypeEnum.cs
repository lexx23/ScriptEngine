/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Library.Attributes;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public enum TokenTypeEnum
    {
        [EnumStringAttribute("Число")]
        NUMBER = 0,
        [EnumStringAttribute("Литерал")]
        LITERAL = 1,
        [EnumStringAttribute("Идентификатор")]
        IDENTIFIER = 2,
        [EnumStringAttribute("Символ")]
        PUNCTUATION = 5
    }
}
