/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System.Collections.Generic;


namespace UnitTests
{
    [TestClass]
    public class OneScript_Test
    {
        private Helper _helper;
        public OneScript_Test()
        {
            _helper = new Helper("OneScript");
        }

        [TestMethod]
        public void OneScript_MainTest()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            //modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("main_module.scr"));
            modules.Add(new ScriptModule("Утверждения", "Approval", ModuleTypeEnum.OBJECT, true, true), _helper.OpenModule("xunit.scr"));
            modules.Add(new ScriptModule("Ожидаем", "Expect", ModuleTypeEnum.OBJECT, true, true), _helper.OpenModule("bdd.scr"));

            _helper.CompileModules(modules);
        }
    }
}
