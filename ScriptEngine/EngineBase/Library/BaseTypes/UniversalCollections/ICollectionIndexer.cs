using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections
{
    public interface ICollectionIndexer
    {
        IValue Get(IValue index);
        void Set(IValue index, IValue value);
    }
}
