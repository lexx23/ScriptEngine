using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraryClassPropertyAttribute : Attribute, IScriptName
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
