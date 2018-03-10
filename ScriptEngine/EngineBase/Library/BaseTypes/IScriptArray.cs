using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public interface IScriptArray
    {
        IValue Get(int index);
        void Set(int index, IValue value);
    }
}
