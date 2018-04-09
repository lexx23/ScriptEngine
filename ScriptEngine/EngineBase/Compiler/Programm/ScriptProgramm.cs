using ScriptEngine.EngineBase.Compiler.Programm.ModuleLoader;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptBaseFunctionsLibrary.BuildInTypes;
using ScriptEngine.EngineBase.Compiler.Types;
using System.Reflection;
using System.IO;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;

namespace ScriptEngine.EngineBase.Compiler.Programm
{
    /// <summary>
    /// Программа. Содержит в себе модули программы, статические переменные, а так же глобальный модуль.
    /// </summary>
    public class ScriptProgramm
    {
        private ScriptModules _modules;
        private InternalTypes _internal_types;
        private GlobalFunctions _global_functions;
        private GlobalVariables _global_variables;
        private StaticVariables _static_variables;
        private ScriptScope _global_scope;

        public InternalTypes InternalTypes { get => _internal_types; }
        public GlobalFunctions GlobalFunctions { get => _global_functions; }
        public GlobalVariables GlobalVariables { get => _global_variables; }
        public StaticVariables StaticVariables { get => _static_variables; }
        public ScriptModules Modules { get => _modules; }

        public ScriptScope GlobalScope { get => _global_scope; }

        public ScriptProgramm()
        {
            _modules = new ScriptModules();
            _global_scope = new ScriptScope() { Name = "global", Type = ScopeTypeEnum.GLOBAL};

            _internal_types = new InternalTypes();
            _global_functions = new GlobalFunctions();
            _global_variables = new GlobalVariables(_global_scope);
            _static_variables = new StaticVariables();
        }

        /// <summary>
        /// Загрузка библиотек по умолчанию. Основные функции платформы.
        /// </summary>
        public void LoadDefaultLibraries()
        {
            string path = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar +  "LanguageExtensions" + Path.DirectorySeparatorChar;
            Loader loader = new Loader(this);
            {
                // Обьект описывающий исключение.
                loader.AddObjectOfType(typeof(ErrorInfo));

                foreach (string file in Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories))
                {
                    Assembly assembly = Assembly.LoadFile(file);
                    loader.LoadAssembly(assembly);
                    assembly = null;
                }
            }
        }

        /// <summary>
        /// Добавить скрипт к существующей программе.
        /// </summary>
        public void AttachScript(ScriptModule module)
        {
            ScriptCompiler compiler = new ScriptCompiler(this);
            compiler.CompileProgramm(new ScriptModule[] { module },false);
        }
    }

}
