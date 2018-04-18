/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections
{
    public interface IUniversalCollection
    {
        int Count();
        void Clear();
        void Insert(IValue index, IValue value);
        void Delete(IValue value);

    }
}
