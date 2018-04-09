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
