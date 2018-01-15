using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ObjectContext
    {
        public ScriptSimpleContext Context { get; set; }
        public ScriptModule Module { get; set; }

        public ObjectContext(ScriptModule module)
        {
            Module = module;
        }
    }
}
