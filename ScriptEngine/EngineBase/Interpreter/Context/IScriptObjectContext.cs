using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public interface IScriptObjectContext
    {
        ScriptModule Module { get; }
        object Instance { get; set; }

        void Set();
        int VariablesCount();
        IFunction CheckFunction(IFunction function);
        IVariableReference GetAnyVaribale(string name);
        IVariableReference GetPublicVariable(string name);
        ContextVariableReferenceHolder GetPublicVariable(int index);
        ContextMethodReferenceHolder GetContextFunction(string name);
    }
}
