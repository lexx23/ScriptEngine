using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    class ContextMethodReferenceHolder
    {
        private IFunction _function;
        private IMethodWrapper _wrapper;

        public IFunction Function { get => _function; }
        public IMethodWrapper Wrapper { get => _wrapper; }

        public ContextMethodReferenceHolder(IFunction function, IMethodWrapper wrapper)
        {
            _function = function;
            _wrapper = wrapper;
        }

        public void Set()
        {
            _function.Method = _wrapper;
        }
    }
}
