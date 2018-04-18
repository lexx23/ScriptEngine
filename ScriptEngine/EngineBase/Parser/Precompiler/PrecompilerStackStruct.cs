/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Parser.Token;

namespace ScriptEngine.EngineBase.Parser.Precompiler
{
    /// <summary>
    /// Информационная структура для стека прекомпилятора.
    /// </summary>
    public struct PrecompilerStackStruct
    {
        public IToken Token;
        public bool Skip;
        public bool Run;
    }
}
