using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    class ContextMethodReferenceHolder
    {
        private IFunction _function;
        private IMethodWrapper _wrapper;

        public ContextMethodReferenceHolder(IFunction function)
        {
            _function = function;
            _wrapper = _function.Method;
        }

        public void Set()
        {
            _function.Method = _wrapper;
        }
    }
}
