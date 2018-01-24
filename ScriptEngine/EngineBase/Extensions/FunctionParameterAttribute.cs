using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Extensions
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public class FunctionParameterAttribute : Attribute
    {
        public bool ByVal { get; set; }
        public Value Default { get; set; }

        public FunctionParameterAttribute(string value) => Default = new Value(value);
        public FunctionParameterAttribute(ObjectContext value) => Default = new Value();
    }
}
