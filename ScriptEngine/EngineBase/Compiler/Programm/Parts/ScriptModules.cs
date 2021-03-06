﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class ScriptModules:IEnumerable<ScriptModule>
    {
        private IList<ScriptModule> _modules;


        public ScriptModules()
        {
            _modules = new List<ScriptModule>();
        }

        /// <summary>
        /// Добавить модуль в программу.
        /// </summary>
        /// <param name="module"></param>
        public void Add(ScriptModule module)
        {
            _modules.Add(module);
        }

        /// <summary>
        /// Проверка существования модуля.
        /// </summary>
        /// <param name="module_name"></param>
        /// <returns></returns>
        public bool Exist(string module_name)
        {
            return Get(module_name) != null;
        }

        /// <summary>
        /// Получить модуль по имени.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScriptModule Get(string name)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                if (String.Equals(_modules[i].Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_modules[i].Alias, name, StringComparison.OrdinalIgnoreCase))
                    return _modules[i];
            }

            return null;
        }

        /// <summary>
        /// Получить модуль по его индексу в коллекции.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public ScriptModule Get(int index)
        {
            return _modules[index];
        }


        public IEnumerator<ScriptModule> GetEnumerator()
        {
            return _modules.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
