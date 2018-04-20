using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptBaseFunctionsLibrary.BuildInTypes;
using ScriptEngine.EngineBase.Interpreter;

namespace ScriptBaseFunctionsLibrary.BaseFunctions
{
    [LibraryClassAttribute(AsGlobal = true, Name = "script_error")]
    public class ScriptError
    {
        [LibraryClassMethodAttribute(Alias = "ИнформацияОбОшибке", Name = "ErrorInfo")]
        public IValue ErrorInfo()
        {
            return ScriptInterpreter.Interpreter.ErrorInfo;
        }

        [LibraryClassMethodAttribute(Alias = "ОписаниеОшибки", Name = "ErrorDescription")]
        public IValue ErrorDescription()
        {
            return ValueFactory.Create(ScriptInterpreter.Interpreter.ErrorInfo.Description);
        }

        [LibraryClassMethodAttribute(Alias = "ПодробноеПредставлениеОшибки", Name = "DetailErrorDescription")]
        public IValue DetailErrorDescription(ErrorInfo error_info)
        {
            return ValueFactory.Create($"{{{error_info.ModuleName}({error_info.LineNumber})}} : " + error_info.Description + "\n" + error_info.SourceLine);
        }
    }
}
