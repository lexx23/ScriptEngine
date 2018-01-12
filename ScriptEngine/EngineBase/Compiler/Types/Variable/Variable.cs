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
        public ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Value Value { get; set; }
        public VariableStatusEnum Status { get; set; }
        public bool Public { get; set; }

        public int StackNumber { get; set; }
        public int Users { get; set; }


        public Variable()
        {
            Users = 1;
            StackNumber  = -1;
        }
    }
}
