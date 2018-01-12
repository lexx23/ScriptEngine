using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm
{
    public class ModuleFunctions
    {
        private IDictionary<string, IFunction> _functions;


        private ScriptModule _module;

        public ModuleFunctions(ScriptModule module)
        {
            _module = module;
            _functions = new Dictionary<string, IFunction>();
        }

        /// <summary>
        /// Добавить функцию в модуль.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module_name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IFunction Add(string name, bool as_public = false, ScriptScope scope = null)
        {
            Function function;

            if (scope == null)
                scope = _module.ModuleScope;

            if (!_functions.ContainsKey(name + "-" + scope.Name))
            {
                function = new Function() { Name = name, Scope = scope, Type = FunctionTypeEnum.PROCEDURE, Public = as_public };
                _functions.Add(name + "-" + scope.Name, function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Получить функцию, по имени, из модуля. Если такое имя есть, тогда вернуть null. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IFunction Get(string name, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module.ModuleScope;

            if (_functions.ContainsKey(name + "-" + scope.Name))
                return _functions[name + "-" + scope.Name];

            return null;
        }
    }
}
