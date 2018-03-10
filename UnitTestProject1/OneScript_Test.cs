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
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("main_module.scr"));
            //modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, true), _helper.OpenModule("Objects\\Cross object call\\object_module.scr"));

            _helper.CompileModules(modules);
        }
    }
}
