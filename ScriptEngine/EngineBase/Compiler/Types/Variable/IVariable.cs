using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariable
    {
        String Name { get; set; }
        ScriptScope Scope { get; set; }
        ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Value Value { get; set; }
        VariableStatusEnum Status { get; set; }
        bool Public { get; set; }
        int StackNumber { get; set; }
        int Users { get; set; }
    }
}
