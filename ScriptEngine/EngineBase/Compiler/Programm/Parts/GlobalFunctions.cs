using ScriptEngine.EngineBase.Compiler.Types.Function;
using System.Linq;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class GlobalFunctions
    {
        private IDictionary<string,IFunction> _global_functions;

        public GlobalFunctions()
        {
            _global_functions = new Dictionary<string, IFunction>();
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

            if (!_global_functions.ContainsKey(name))
            {
                function = new Function() { Name = name };
                _global_functions.Add(name,function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Добавление глобальной функции.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="function"></param>
        public void Add(string name,IFunction function)
        {
            if (!_global_functions.ContainsKey(name))
                _global_functions.Add(name, function);
            else
                throw new Exception($"Функция с именем {name} уже существует.");
        }

        /// <summary>
        /// Получить глобальную функцию по имени.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFunction Get(string name)
        {
            IFunction function;
            if (_global_functions.TryGetValue(name,out function))
                return function;
            return null;
        }

    }
}
