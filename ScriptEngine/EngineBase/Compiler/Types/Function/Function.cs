using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public class Function: IFunction
    {
        public string Name { get; set; }
        public FunctionTypeEnum Type { get; set; }
        public bool Public { get; set; }
        public ScriptScope Scope { get; set; }

        public IList<IVariable> CallParameters { get; set; }
        public FunctionParameters DefinedParameters { get; set; }

        public int EntryPoint { get; set; }

        public Func<IValue[], IValue> Method { get; set; }

        public CodeInformation CodeInformation { get; set; }


    }
}
