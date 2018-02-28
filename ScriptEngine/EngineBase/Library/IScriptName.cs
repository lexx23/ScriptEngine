using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public interface IScriptName
    {
        string Name { get; set; }
        string Alias { get; set; }
    }
}
