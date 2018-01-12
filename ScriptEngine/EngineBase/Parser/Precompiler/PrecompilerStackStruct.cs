using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;


namespace ScriptEngine.EngineBase.Parser.Precompiler
{
    /// <summary>
    /// Информационная структура для стека прекомпилятора.
    /// </summary>
    public struct PrecompilerStackStruct
    {
        public IToken Token;
        public bool Skip;
        public bool Run;
    }
}
