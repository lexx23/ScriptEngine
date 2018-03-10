using ScriptEngine.EngineBase.Compiler.Types.Function;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class GlobalFunctions
    {
        private IList<IFunction> _global_functions;

        public GlobalFunctions()
        {
            _global_functions = new List<IFunction>();
        }

        /// <summary>
        /// Создание глобальной функции. Если такая функция существует, то вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFunction Create(string name)
        {
            IFunction function;

            if (Get(name) == null)
            {
                function = new Function() { Name = name };
                _global_functions.Add(function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Добавление глобальной функции.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="function"></param>
        public void Add(IFunction function)
        {
            if (Get(function.Name) == null)
                _global_functions.Add(function);
            else
                throw new Exception($"Функция с именем {function.Name} уже существует.");
        }

        /// <summary>
        /// Получить глобальную функцию по имени.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFunction Get(string name)
        {
            for(int i=0;i<_global_functions.Count;i++)
            {
                if (String.Equals(_global_functions[i].Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_global_functions[i].Alias,name,StringComparison.OrdinalIgnoreCase))
                    return _global_functions[i];
            }

            return null;
        }

    }
}
