/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;

namespace ScriptEngine.EngineBase.Library.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LibraryClassAttribute : Attribute,IModulePlace,IScriptName
    {
        public bool AsGlobal { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public bool RegisterType { get; set; }
    }
}
