using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ContextReferenceHolder
    {
        public IVariable Variable;
        public IVariableReference Reference;

        public ContextReferenceHolder(IVariable variable, IVariableReference reference)
        {
            Variable = variable;
            Reference = reference;
        }


        public void Set()
        {
            Variable.Reference = Reference;
        }
    }

}
