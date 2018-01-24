using System;

namespace ScriptEngine.EngineBase.Extensions
{
    public class ScriptExtensionMethodAttribute : Attribute
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}