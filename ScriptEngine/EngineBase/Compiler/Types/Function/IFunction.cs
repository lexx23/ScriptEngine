using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public interface IFunction
    {
        string Name { get; set; }
        FunctionTypeEnum Type { get; set; }
        bool Public { get; set; }
        ScriptScope Scope { get; set; }
        IList<IVariable> Param { get; set; }
        int EntryPoint { get; set; }

        CodeInformation CodeInformation { get; set; }

        IVariable GetParamByIndex(int index);
    }
}
