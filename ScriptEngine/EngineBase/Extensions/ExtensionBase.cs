using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Extensions
{
    public abstract class ExtensionBase
    {
        protected ScriptProgrammContext _programm_context;

        public void SetContext(ScriptProgrammContext scriptProgrammContext)
        {
            _programm_context = scriptProgrammContext;
        }
    }
}
