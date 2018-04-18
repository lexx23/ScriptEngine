/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
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
            {
                function.Name = function.Name;
                function.Alias = function.Alias;
                _functions.Add(function);
            }
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
                if (String.Equals(_functions[i].Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_functions[i].Alias,name, StringComparison.OrdinalIgnoreCase))
                    return _functions[i];
            }

            return null;
        }

        /// <summary>
        /// Удалить функцию из модуля.
        /// </summary>
        /// <param name="name"></param>
        public void Delete(string name)
        {
            for (int i = 0; i < _functions.Count; i++)
            {
                if (String.Equals(_functions[i].Name, name, StringComparison.OrdinalIgnoreCase) || String.Equals(_functions[i].Alias, name, StringComparison.OrdinalIgnoreCase))
                    _functions.RemoveAt(i);
            }
        }

        public void Delete(int index)
        {
            _functions.RemoveAt(index);
        }

        public IFunction[] ToArray()
        {
            return _functions.ToArray();
        }

    }
}
