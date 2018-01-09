using ScriptEngine.EngineBase.Compiler.Programm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptGlobalContext
    {

        public ScriptSimpleContext Global { get; set; }
        public ScriptModuleContextsHolder ModuleContexts { get; set; }

        public ScriptGlobalContext(int size)
        {
            Global = new ScriptSimpleContext("<<global>>",size);
            ModuleContexts = new ScriptModuleContextsHolder();
        }

    }
}
