/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public class Function: IFunction
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public FunctionTypeEnum Type { get; set; }
        public bool Public { get; set; }
        public ScriptScope Scope { get; set; }

        public IList<IVariable> CallParameters { get; set; }
        public FunctionParameters DefinedParameters { get; set; }

        public int EntryPoint { get; set; }

        public IMethodWrapper Method { get; set; }

        public CodeInformation CodeInformation { get; set; }


    }
}
