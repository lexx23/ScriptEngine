using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class GlobalFunctions
    {
        private IDictionary<string, IFunction> _global_functions;

        public GlobalFunctions()
        {
            _global_functions = new Dictionary<string, IFunction>();
        }

        /// <summary>
        /// Добавление глобальной функции. Если такая функция существует, то вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IFunction Add(string name)
        {
            IFunction function;

            if (!_global_functions.ContainsKey(name))
            {
                function = new Function() { Name = name };
                _global_functions.Add(name , function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Получить глобальную функцию.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IFunction Get(string name)
        {
            if (_global_functions.ContainsKey(name))
                return _global_functions[name];

            return null;
        }
    }
}
