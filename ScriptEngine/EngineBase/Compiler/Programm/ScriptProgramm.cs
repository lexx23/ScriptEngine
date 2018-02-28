﻿using ScriptEngine.EngineBase.Compiler.Programm.ModuleLoader;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace ScriptEngine.EngineBase.Compiler.Programm
{
    /// <summary>
    /// Программа. Содержит в себе модули программы, статические переменные, а так же глобальный модуль.
    /// </summary>
    public class ScriptProgramm
    {
        private ScriptModules _modules;
        private GlobalFunctions _global_functions;
        private GlobalVariables _global_variables;
        private StaticVariables _static_variables;
        private ScriptScope _global_scope;

        public GlobalFunctions GlobalFunctions { get => _global_functions; }
        public GlobalVariables GlobalVariables { get => _global_variables; }
        public StaticVariables StaticVariables { get => _static_variables; }
        public ScriptModules Modules { get => _modules; }

        public ScriptScope GlobalScope { get => _global_scope; }

        public ScriptProgramm()
        {
            _modules = new ScriptModules();
            _global_scope = new ScriptScope() { Name = "global", Type = ScopeTypeEnum.GLOBAL, StackIndex = 0 };

            _global_functions = new GlobalFunctions();
            _global_variables = new GlobalVariables(_global_scope);
            _static_variables = new StaticVariables();
        }

        public void LoadLibraries()
        {
            string path = Directory.GetCurrentDirectory() + "\\LanguageExtensions\\";
            Loader loader = new Loader(this);

            foreach (string file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
            {
                Assembly assembly = Assembly.LoadFile(file);
                loader.LoadFromAssembly(assembly);
            }
        }
    }

}
