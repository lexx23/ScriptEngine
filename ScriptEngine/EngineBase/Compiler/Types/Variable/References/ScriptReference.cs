using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class ScriptReference : IVariableReference
    {
        private IVariableReference _value;

        public ScriptReference(IVariableReference reference)
        {
            _value = reference;
        }

        public IValue Get()
        {
            return _value.Get();
        }
        public void Set(IValue value)
        {
            _value.Set(value);
        }

        public IVariableReference Clone(object instance)
        {
            return new ScriptReference(null);
        }
    }
}
