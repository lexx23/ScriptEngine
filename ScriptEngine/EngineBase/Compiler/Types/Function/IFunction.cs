/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public interface IFunction: IScriptName
    {
        FunctionTypeEnum Type { get; set; }
        bool Public { get; set; }
        ScriptScope Scope { get; set; }

        IList<IVariable> CallParameters { get; set; }
        FunctionParameters DefinedParameters { get; set; }

        int EntryPoint { get; set; }

        IMethodWrapper Method { get; set; }

        CodeInformation CodeInformation { get; set; }

    }
}
