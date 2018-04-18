/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Interpreter;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public class LibraryModule<T> : ScriptObjectValue
    {
        public LibraryModule()
        {
            LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(LibraryClassAttribute), false);

            InternalScriptType type = ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(attribute.Name);
            _value = new ScriptObjectContext(type.Module, this);
        }

    }
}
