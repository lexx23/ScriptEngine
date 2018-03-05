﻿using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;

namespace ScriptEngine.EngineBase.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LibraryClassAttribute : Attribute,IModulePlace,IScriptName
    {
        public bool AsGlobal { get; set; }
        public bool AsObject { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }

        public ScriptModule Module { get; set; }
    }
}