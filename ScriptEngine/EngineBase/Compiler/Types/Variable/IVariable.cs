/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariable: IScriptName
    {
        ScriptScope Scope { get; set; }

        VariableTypeEnum Type { get; set; }
        IVariableReference Reference { get; set; }
        IValue Value { get; set; }

        bool Public { get; set; }
        int Users { get; set; }
          
    }
}
