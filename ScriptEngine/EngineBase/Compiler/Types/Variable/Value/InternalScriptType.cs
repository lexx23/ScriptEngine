using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public class InternalScriptType : IScriptName
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string Description { get; set; }
        public ScriptModule Module { get; set; }
        public Type Type { get; set; }

        public bool IsBaseType()
        {
            if (Module == null)
                return true;
            return Type.Namespace == "ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values";
        }
    }
}
