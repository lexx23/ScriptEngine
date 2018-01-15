using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Praser.Token;


namespace ScriptEngine.EngineBase.Compiler.Programm
{
    public class ScriptStatement
    {
        public OP_CODES OP_CODE { get; set; }
        public IVariable Variable1 { get; set; }
        public IVariable Variable2 { get; set; }
        public IVariable Variable3 { get; set; }
        public int Line { get; set; }

        public override string ToString()
        {
            return OP_CODE.ToString() + " [" + Variable1?.Name + " " + Variable1?.Value?.ToString() + " ]" + " [" + Variable2?.Name + " " + Variable2?.Value?.ToString() + " ]" + " [" + Variable3?.Name + " " + Variable3?.Value?.ToString() + " ]";
        }
    }
}
