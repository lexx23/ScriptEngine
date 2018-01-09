using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Parser.Precompiler.Directives
{
    /// <summary>
    /// Базовый класс обработчика директив прекомпилятора.
    /// </summary>
    public abstract class DirectiveBase
    {
        protected IDictionary<string, bool> _defines;
        protected PrecompilerStack _stack;
        protected TokenIteratorBase _iterator;

        public DirectiveBase()
        {
        }

        public DirectiveBase(TokenIteratorBase iterator, PrecompilerStack stack, IDictionary<string, bool> defines)
        {
            _iterator = iterator;
            _stack = stack;

            _defines = defines ?? new Dictionary<string, bool>();
        }



        /// <summary>
        /// Обработка директивы. Возвращает true если это директива этого обработчика.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public abstract bool ProcessDirective();

    }
}
