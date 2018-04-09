using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class SimpleReference : IVariableReference
    {
        private IValue _value;

        public SimpleReference()
        {
            _value = ValueFactory.Create();
        }

        public IValue Get()
        {
            return _value;
        }

        public void Set(IValue value)
        {
            _value = value;
        }

        public IVariableReference Clone(object instance)
        {
            return new SimpleReference();
        }
    }
}
