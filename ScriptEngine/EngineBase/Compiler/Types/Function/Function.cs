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
        public IList<IVariable> Param { get; set; }
        public int EntryPoint { get; set; }

        public Func<IVariable[], Value> Method { get; set; }

        public CodeInformation CodeInformation { get; set; }


    }
}
