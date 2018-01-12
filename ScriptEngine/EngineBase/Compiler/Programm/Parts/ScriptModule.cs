using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class ScriptModule
    {
        private ModuleVariables _vars;
        private ModuleFunctions _functions;
        private IList<IFunction> _object_functions_call;
        private IList<ScriptStatement> _code;
        private ScriptScope _module_scope;

        public string Name { get; set; }
        public string FileName { get; set; }
        public bool AsGlobal { get; set; }
        public ModuleVariables Variables { get => _vars; }
        public ModuleFunctions Functions { get => _functions; }
        public ModuleTypeEnum Type { get; set; }
        public IList<ScriptStatement> Code { get => _code; }
        public ScriptScope ModuleScope { get => _module_scope; }

        /// <summary>
        /// Номер линии программы.
        /// </summary>
        public int ProgrammLine
        {
            get => _code.Count;
        }

        public ScriptModule(string name, ModuleTypeEnum type, bool compile_as_global = false)
        {
            Name = name;
            Type = type;
            AsGlobal = compile_as_global;

            // Стартовый модуль компилирую как глобальный.
            if (type == ModuleTypeEnum.STARTUP)
                AsGlobal = true;

            _module_scope = new ScriptScope() { Type = ScopeTypeEnum.MODULE, Name = name, Module = this };

            _object_functions_call = new List<IFunction>();

            _vars = new ModuleVariables(this);
            _code = new List<ScriptStatement>();
            _functions = new ModuleFunctions(this);
        }


        public int ObjectCallAdd(IFunction function)
        {
            _object_functions_call.Add(function);
            return _object_functions_call.Count -1;
        }


        public IFunction ObjectCallGet(int index)
        {
            return _object_functions_call[index];
        }


        /// <summary>
        /// Добавить инструкцию.
        /// </summary>
        /// <returns></returns>
        public ScriptStatement StatementAdd()
        {
            ScriptStatement statement = new ScriptStatement();
            _code.Add(statement);
            return statement;
        }

        /// <summary>
        /// Получть инструкцию.
        /// </summary>
        public ScriptStatement StatementGet(int index)
        {
            return _code[index];
        }
    }
}
