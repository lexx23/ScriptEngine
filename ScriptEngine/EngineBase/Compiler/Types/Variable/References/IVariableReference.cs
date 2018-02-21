using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public interface IVariableReference
    {
        IValue Get();
        void Set(IValue value);
        IVariableReference Clone(object instance);

    }
}
