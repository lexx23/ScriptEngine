using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariable: IScriptName
    {
        ScriptScope Scope { get; set; }

        VariableTypeEnum Type { get; set; }
        IVariableReference Reference { get; set; }
        IValue Value { get; set; }

        bool Public { get; set; }
        int Users { get; set; }
          
    }
}
