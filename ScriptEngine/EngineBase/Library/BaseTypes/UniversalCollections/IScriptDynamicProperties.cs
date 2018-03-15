using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections
{
    public interface IScriptDynamicProperties
    {
        bool Exist(string name);
        IValue Get(string name);
        void Set(string name,IValue value);
    }
}
