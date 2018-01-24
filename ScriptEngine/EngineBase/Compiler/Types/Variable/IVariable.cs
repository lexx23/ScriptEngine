using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariable
    {
        String Name { get; set; }
        ScriptScope Scope { get; set; }
        IValue Value { get; set; }
        VariableStatusEnum Status { get; set; }
        bool Ref { get; set; }
        bool Public { get; set; }
        int StackNumber { get; set; }
        int Users { get; set; }
    }
}
