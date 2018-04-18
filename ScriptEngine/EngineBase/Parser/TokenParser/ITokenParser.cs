/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/


using ScriptEngine.EngineBase.Parser.Token;

namespace ScriptEngine.EngineBase.Parser.TokenParser
{
    /// <summary>
    /// Интерфейс парсера лексем.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Проверка текущего символа из итератора на соответствие данному токену.
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        bool Parse(SourceIterator iterator, out IToken token);

    }
}
