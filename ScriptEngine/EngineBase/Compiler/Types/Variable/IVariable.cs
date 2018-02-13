using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariable
    {
        String Name { get; set; }
        ScriptScope Scope { get; set; }

        VariableTypeEnum Type { get; set; }
        IVariableReference Reference { get; set; }
        IValue Value { get; set; }

        bool Public { get; set; }
        int StackNumber { get; set; }
        int Users { get; set; }

        bool HaveValue();
    }
}
