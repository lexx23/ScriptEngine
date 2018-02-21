using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ScriptModule:IModuleName,IModulePlace
    {
        private ModuleVariables _vars;
        private ModuleFunctions _functions;
        private IList<IFunction> _object_functions_call;
        private IList<ScriptStatement> _code;
        private ScriptScope _module_scope;

        public string Name { get; set; }
        public string Alias { get; set; }

        public bool AsGlobal { get; set; }
        public bool AsObject { get; set; }

        public string FileName { get; set; }

        public ModuleVariables Variables { get => _vars; }
        public ModuleFunctions Functions { get => _functions; }
        public ModuleTypeEnum Type { get; set; }
        public IList<ScriptStatement> Code { get => _code; }
        public ScriptScope ModuleScope { get => _module_scope; }

        public Type InstanceType { get; set; }
        public object CurrentInstance { get; set; }

        /// <summary>
        /// Номер линии программы.
        /// </summary>
        public int ProgrammLine
        {
            get => _code.Count;
        }

        public ScriptModule(string name,string alias, ModuleTypeEnum type, bool as_global = false, bool as_object = false)
        {
            Name = name;
            Alias = alias;
            Type = type;
            AsGlobal = as_global;
            AsObject = as_object;

            InstanceType = null;

            if (Alias == string.Empty)
                Alias = Name;

            // Стартовый модуль компилирую как глобальный.
            if (type == ModuleTypeEnum.STARTUP)
                AsGlobal = true;

            _module_scope = new ScriptScope() { Type = ScopeTypeEnum.MODULE, Name = name, Module = this, StackIndex = 1 };

            _object_functions_call = new List<IFunction>();

            _vars = new ModuleVariables(this);
            _code = new List<ScriptStatement>();
            _functions = new ModuleFunctions(this);
        }

        /// <summary>
        /// Добавить обращение к объекту.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public int ObjectCallAdd(IFunction function)
        {
            _object_functions_call.Add(function);
            return _object_functions_call.Count -1;
        }

        /// <summary>
        /// Получить обращение к обьекту.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
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
            ScriptStatement statement = new ScriptStatement
            {
                Line = -1
            };
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
