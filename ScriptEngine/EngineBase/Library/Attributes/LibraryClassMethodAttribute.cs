using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;

namespace ScriptEngine.EngineBase.Library.Attributes
{
    public class LibraryClassMethodAttribute : Attribute, IScriptName
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}