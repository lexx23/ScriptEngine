using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Collections.Generic;


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

            if (!module.AsGlobal && module.Type == ModuleTypeEnum.COMMON)
                GlobalVariables.Add(module.Name, new Value(ValueTypeEnum.OBJECT, module.Name));

            if (module.Type == ModuleTypeEnum.OBJECT)
                GlobalVariables.Add(module.Name, new Value(ValueTypeEnum.OBJECT, module.Name));
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

    }


}
