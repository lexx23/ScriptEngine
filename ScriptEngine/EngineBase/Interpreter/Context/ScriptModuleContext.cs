using ScriptEngine.EngineBase.Compiler.Programm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptModuleContext
    {
        private ScriptModule _module;

        public ScriptModule Module { get => _module; }
        public ScriptSimpleContext Context { get; set; }
        public ScriptFunctionContext Function { get; set; }

        public ScriptModuleContext(string name,ScriptModule type)
        {
            _module = type;
            Context = new ScriptSimpleContext(name,type.ModuleScope.VarCount);
            Function = new ScriptFunctionContext();
        }

    }
}
