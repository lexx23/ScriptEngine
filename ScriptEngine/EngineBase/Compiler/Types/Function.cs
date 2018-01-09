using ScriptEngine.EngineBase.Praser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types
{
    public class Function
    {
        public string Name { get; set; }
        public FunctionTypeEnum Type { get; set; }
        public bool Public { get; set; }
        public ScriptScope Scope { get; set; }
        public IList<Variable> Param { get; set; }
        public int EntryPoint { get; set; }

        public CodeInformation CodeInformation { get; set; }

        public Variable GetParamByIndex(int index)
        {
            return Param[index];
        }
    }
}
