using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.Parameters
{
    public class FunctionParameter
    {
        private IVariable _var;

        public string Name { get => _var.Name; }
        public VariableTypeEnum Type { get => _var.Type; }
        public IValue DefaultValue { get => _var.Value; }
        public IVariable InternalVariable {get;set;}

        public FunctionParameter(IVariable var)
        {
            _var = var;
        }
    }
}
