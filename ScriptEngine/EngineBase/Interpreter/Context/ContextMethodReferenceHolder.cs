﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ContextMethodReferenceHolder
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
