/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Extensions
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LibraryClassPropertyAttribute : Attribute, IScriptName
    {
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}
