using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods
{
    public interface IMethodWrapper
    {
        IValue Run(IValue[] param);
        IMethodWrapper Clone(object instance);
    }
}
