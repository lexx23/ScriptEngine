using ScriptEngine.EngineBase.Compiler.Types.Function;
using System.Collections.Generic;
using System.Linq;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ModuleFunctions
    {
        private IList<IFunction> _functions;
        private ScriptModule _module;

        public int Count { get => _functions.Count; }

        public ModuleFunctions(ScriptModule module)
        {
            _module = module;
            _functions = new List<IFunction>();
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

            if (Get(name) == null)
            {
                function = new Function() { Name = name, Scope = _module.ModuleScope, Type = FunctionTypeEnum.PROCEDURE, Public = as_public };
                _functions.Add(function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Добавить функцию в модуль.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="as_public"></param>
        public void Add(IFunction function)
        {
            if (Get(function.Name) == null)
                _functions.Add(function);
        }

        /// <summary>
        /// Получить функцию, по имени, из модуля. Если такое имя есть, тогда вернуть null. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IFunction Get(string name)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                if (_functions[i].Name == name || _functions[i].Alias == name)
                    return _functions[i];
            }

            return null;
        }


        public IFunction[] ToArray()
        {
            return _functions.ToArray();
        }

    }
}
