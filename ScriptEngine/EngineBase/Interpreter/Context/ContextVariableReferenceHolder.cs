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
