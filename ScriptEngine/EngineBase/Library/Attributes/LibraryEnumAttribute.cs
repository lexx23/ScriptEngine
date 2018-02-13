using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;


namespace ScriptEngine.EngineBase.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Enum)]
    public class LibraryEnumAttribute : Attribute, IModuleName
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
