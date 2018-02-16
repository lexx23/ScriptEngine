using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public interface ILibraryModule
    {
        IList<IVariable> Properties { get; set; }

        void Constructor(params IVariable[] parameters);
    }
}
