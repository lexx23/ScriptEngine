using ScriptEngine.EngineBase.Compiler.Programm.ModuleLoader;
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
        private IDictionary<string, ScriptModule> _modules;
        private GlobalFunctions _global_functions;
        private GlobalVariables _global_variables;
        private StaticVariables _static_variables;
        private ScriptScope _global_scope;

        public GlobalFunctions GlobalFunctions { get => _global_functions; }
        public GlobalVariables GlobalVariables { get => _global_variables; }
        public StaticVariables StaticVariables { get => _static_variables; }
        public IDictionary<string, ScriptModule> Modules { get => _modules; }

        public ScriptScope GlobalScope { get => _global_scope; }

        public ScriptModule this[string name]
        {
            get => _modules[name];
        }


        public ScriptProgramm()
        {
            _modules = new Dictionary<string, ScriptModule>();
            _global_scope = new ScriptScope() { Name = "global", Type = ScopeTypeEnum.GLOBAL, StackIndex = 0 };

            _global_functions = new GlobalFunctions();
            _global_variables = new GlobalVariables(_global_scope);
            _static_variables = new StaticVariables();
        }

        /// <summary>
        /// Добавить модуль в программу.
        /// </summary>
        /// <param name="module"></param>
        public void ModuleAdd(ScriptModule module)
        {
            _modules.Add(module.Name, module);

            string object_name, object_alias;
            object_name = module.Name;
            object_alias = "";
            if (module.Alias == null)
                module.Alias = string.Empty;
            else
                object_alias = module.Alias;


            if (!module.AsObject)
            {
                object_name = "<<" + module.Name + ">>";
                object_alias = "<<" + module.Alias + ">>";
            }


            IValue module_object = ValueFactory.Create();
            GlobalVariables.Create(object_name, module_object);
            if (object_name != object_alias && object_alias != string.Empty)
            {
                GlobalVariables.Create(object_alias, module_object);
                _modules.Add(object_alias, module);
            }
        }

        /// <summary>
        /// Проверка существования модуля.
        /// </summary>
        /// <param name="module_name"></param>
        /// <returns></returns>
        public bool ModuleExist(string module_name)
        {
            return _modules.ContainsKey(module_name);
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
