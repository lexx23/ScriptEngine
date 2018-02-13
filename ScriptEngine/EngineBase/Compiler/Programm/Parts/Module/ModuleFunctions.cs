using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ModuleFunctions
    {
        private IDictionary<string,IFunction> _functions;


        private ScriptModule _module;

        public ModuleFunctions(ScriptModule module)
        {
            _module = module;
            _functions = new Dictionary<string, IFunction>();
        }

        /// <summary>
        /// Создать функцию в модуле.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module_name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IFunction Create(string name, bool as_public = false)
        {
            Function function;

            if (!_functions.ContainsKey(name))
            {
                function = new Function() { Name = name, Scope = _module.ModuleScope, Type = FunctionTypeEnum.PROCEDURE, Public = as_public };
                _functions.Add(name,function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Добавить функцию в модуль.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="as_public"></param>
        public void Add(string name,IFunction function)
        {
            if (!_functions.ContainsKey(name))
                _functions.Add(name,function);
        }

        /// <summary>
        /// Получить функцию, по имени, из модуля. Если такое имя есть, тогда вернуть null. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IFunction Get(string name)
        {
            if (_functions.TryGetValue(name, out IFunction function))
                return function;
            return null;
        }
    }
}
