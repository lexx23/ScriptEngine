using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using System;


namespace ScriptEngine.EngineBase.Compiler.Types
{
    public class ScriptScope
    {
        public String Name { get; set; }
        public ScriptModule Module { get; set; }
        public ScopeTypeEnum Type { get; set; }
        public int VarCount { get; set; }

        public ScriptScope()
        {
            VarCount = 0;
        }
    }
}
