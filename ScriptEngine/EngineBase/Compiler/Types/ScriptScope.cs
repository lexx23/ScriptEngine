/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types
{
    public class ScriptScope
    {
        public String Name { get; set; }
        public ScriptModule Module { get; set; }
        public ScopeTypeEnum Type { get; set; }

        public IList<IVariable> Vars { get; set; }

        public ScriptScope()
        {
            Vars = new List<IVariable>();
        }
    }
}
