using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public interface ILibraryModule
    {
        IList<IVariable> Properties { get; set; }

        //ScriptObjectContext Constructor(params IVariable[] parameters);
    }
}
