using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Function
{
    public class Function: IFunction
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public FunctionTypeEnum Type { get; set; }
        public bool Public { get; set; }
        public ScriptScope Scope { get; set; }

        public IList<IVariable> CallParameters { get; set; }
        public FunctionParameters DefinedParameters { get; set; }

        public int EntryPoint { get; set; }

        public IMethodWrapper Method { get; set; }

        public CodeInformation CodeInformation { get; set; }


    }
}
