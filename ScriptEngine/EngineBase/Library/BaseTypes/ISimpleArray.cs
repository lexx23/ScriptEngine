using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public interface ISimpleArray
    {
        IVariable[] Array { get; set; }
        int Count { get; }
        IValue GetByIndex(int index);
        IValue GetByName(string name);

    }
}
