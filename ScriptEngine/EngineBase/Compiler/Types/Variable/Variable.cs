using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public class Variable : IVariable
    {
        public String Name { get; set; }
        public ScriptScope Scope { get; set; }
        public IValue Value { get; set; }
        public VariableStatusEnum Status { get; set; }
        public bool Ref { get; set; }
        public bool Public { get; set; }

        public int StackNumber { get; set; }
        public int Users { get; set; }


        public Variable()
        {
            Users = 1;
            StackNumber  = -1;
        }

        public static Variable CreateConstant(int value) => new Variable() { Status = VariableStatusEnum.CONSTANTVARIABLE,  Value = new Value.Value(value) };
        public static Variable CreateConstant(string value) => new Variable() { Status = VariableStatusEnum.CONSTANTVARIABLE, Value = new Value.Value(value) };
    }
}
