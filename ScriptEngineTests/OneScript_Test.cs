/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Compiler;
using System.Collections.Generic;
using System.IO;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace UnitTests
{
    [TestClass]
    public class OneScript_Test
    {
        private readonly string _path;
        public OneScript_Test()
        {
            _path = Directory.GetCurrentDirectory() + "\\Scripts\\OneScript\\";
        }

        [TestMethod]
        public void OneScript_MainTest()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "main_module.scr"),
                new ScriptModule("testrunner", "testrunner", ModuleTypeEnum.OBJECT,true, _path + "testrunner.scr"),
                new ScriptModule("Утверждения", "Approval", ModuleTypeEnum.OBJECT, true, _path + "xunit.scr"),
                new ScriptModule("Ожидаем", "Expect", ModuleTypeEnum.OBJECT, true, _path + "bdd.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("testrunner", 523, (interpreter1) =>
              {
                 IValue val1 =  interpreter1.Debugger.Eval("ПервоеЗначение");
                 IValue val2 = interpreter1.Debugger.Eval("ВтороеЗначение");
              });

            interpreter.Debug();
        }
    }
}
