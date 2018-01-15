using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class FunctionHistoryData
    {
        public int Position { get; set; }
        public IFunction PreviousFunction { get; set; }
        public IFunction CurrentFunction { get; set; }
        public ScriptModule Module { get; set; }
        public IList<string> FunctionParams { get; set; }
        public ScriptSimpleContext Context { get; set; }


        public string FunctionParamsAsString()
        {
            string result = string.Empty;

            foreach (string part in FunctionParams)
            {
                if (result == string.Empty)
                    result += part;
                else
                    result += ", " + part;
            }

            return result;
        }

        public override string ToString()
        {
            if (CurrentFunction.Name[0] == '<')
                return Module.Name;
            else
                return Module.Name + "." + CurrentFunction.Name + $"({FunctionParamsAsString()})";
        }
    }
}
