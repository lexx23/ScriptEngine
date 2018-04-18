/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;


namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ContextVariableReferenceHolder
    {
        public IVariable Variable;
        public IVariableReference Reference;

        public ContextVariableReferenceHolder(IVariable variable, IVariableReference reference)
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
