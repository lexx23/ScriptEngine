using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public interface IScriptDynamicProperties
    {
        bool Exist(string name);
        IValue Get(string name);
        void Set(string name,IValue value);
    }
}
