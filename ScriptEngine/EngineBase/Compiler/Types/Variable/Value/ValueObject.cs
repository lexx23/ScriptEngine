using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public class ValueObject
    {
        public ScriptModule Type { get; set; }
        public ScriptModuleContext Context { get; set; }

        public ValueObject(ScriptModule type, ScriptModuleContext context)
        {
            Type = type;
            Context = context;
        }
    }
}
