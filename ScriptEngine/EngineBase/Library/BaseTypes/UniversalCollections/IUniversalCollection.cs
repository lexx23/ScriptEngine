using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections
{
    public interface IUniversalCollection
    {
        int Count();
        void Clear();
        void Insert(IValue index, IValue value);
        void Delete(IValue value);

    }
}
