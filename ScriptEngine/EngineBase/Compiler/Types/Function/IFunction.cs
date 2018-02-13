using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public interface IFunction
    {
        string Name { get; set; }
        FunctionTypeEnum Type { get; set; }
        bool Public { get; set; }
        ScriptScope Scope { get; set; }

        IList<IVariable> CallParameters { get; set; }
        FunctionParameters DefinedParameters { get; set; }

        int EntryPoint { get; set; }

        Func<IValue[], IValue> Method { get; set; }

        CodeInformation CodeInformation { get; set; }

    }
}
