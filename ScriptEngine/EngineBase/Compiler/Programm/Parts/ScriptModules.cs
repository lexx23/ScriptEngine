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

            if (module.Alias == null || module.Alias == string.Empty)
                module.Alias = module.Name;
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
                if (_modules[i].Name == name || _modules[i].Alias == name)
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

        /// <summary>
        /// Получить индекс модуля в коллекции.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public int GetIndex(string name)
        {
            for (int i = 0; i < _modules.Count; i++)
            {
                if (_modules[i].Name == name || _modules[i].Alias == name)
                    return i;
            }

            return -1;
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
