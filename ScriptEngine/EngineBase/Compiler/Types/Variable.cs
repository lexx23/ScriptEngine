using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types
{
    public class Variable
    {
        public String Name { get; set; }
        public ScriptScope Scope { get; set; }
        public VariableValue Value { get; set; }
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
