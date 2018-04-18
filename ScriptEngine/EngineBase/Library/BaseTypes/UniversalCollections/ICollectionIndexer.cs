/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections
{
    public interface ICollectionIndexer
    {
        IValue Get(IValue index);
        void Set(IValue index, IValue value);
    }
}
