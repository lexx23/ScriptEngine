using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FunctionDefaultParameterAttribute : Attribute
    {
        public bool ByVal { get; set; }
        public IValue Default { get; set; }

        //public FunctionDefaultParameterAttribute(string value) => Default = new Value(value);
        //public FunctionDefaultParameterAttribute(decimal value) => Default = new Value(value);
    }
}
