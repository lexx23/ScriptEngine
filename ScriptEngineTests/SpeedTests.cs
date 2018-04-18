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
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ScriptEngine.EngineBase.Compiler;

namespace UnitTests
{

    [TestClass]
    public class SpeedTests
    {
        private readonly string _path;
        public SpeedTests()
        {
            _path = Directory.GetCurrentDirectory() + "\\Scripts\\SpeedTest\\";
        }

        // Скорость для OneScript указана без оптимизации сборки и без отладки.


        /// <summary>
        /// OneScript: 2400, 1C: 1400
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы структуры и динамического доступа к свойствам.")]
        public void SpeedTest_Structure()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("struct","struct", ModuleTypeEnum.STARTUP,false,_path+"structure.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(2000, sw.ElapsedMilliseconds, 200);
        }

        /// <summary>
        /// OneScript: 2900, 1C: 2066
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы вызова функции из внешней библиотеки.")]
        public void SpeedTest_LibraryFunctionCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("struct","struct", ModuleTypeEnum.STARTUP,false,_path+"speed_test_library_call.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(1800, sw.ElapsedMilliseconds, 350);
        }

        /// <summary>
        /// OneScript: 4700, 1C: 6000
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы вызова функции.")]
        public void SpeedTest_FunctionCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function","function", ModuleTypeEnum.STARTUP,false,_path+"speed_test_function_call.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(3400, sw.ElapsedMilliseconds, 350);
        }

        /// <summary>
        /// OneScript: 4300, 1C: 2325
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы оператора для каждого.")]
        public void SpeedTest_ForEach()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach_test","foreach_test", ModuleTypeEnum.STARTUP,false,_path+"foreach.scr")
            };


            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(2100, sw.ElapsedMilliseconds, 500);
        }

        /// <summary>
        /// OneScript: 8500, 1C: 1800
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы массива.")]
        public void SpeedTest_Eval()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("eval_test","foreach_test", ModuleTypeEnum.STARTUP,false,_path+"eval.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(5000, sw.ElapsedMilliseconds, 500);
        }

        /// <summary>
        /// OneScript: 8500-9500, 1C: 4500
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы массива.")]
        public void SpeedTest_Array()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("array_test","foreach_test", ModuleTypeEnum.STARTUP,false,_path+"array.scr")
            };


            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(4800, sw.ElapsedMilliseconds, 500);
        }

        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка результатов работы с массивом.")]
        public void SpeedTest_ArrayDebug()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("array_test","foreach_test", ModuleTypeEnum.STARTUP,false,_path+"array.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("array_test", 28, (interpreter_int) =>
            {
                Assert.AreEqual(499999500000, interpreter_int.Debugger.RegisterGetValue("result").AsNumber());
            });

            interpreter.Debug();

        }


        /// <summary>
        /// OneScript: 2100, 1C: 1900
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы вычислений при компиляции.")]
        public void SpeedTest_Precalc()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("other","other", ModuleTypeEnum.STARTUP,false,_path+"speed_test.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            sw.Stop();
            Assert.AreEqual(250, sw.ElapsedMilliseconds, 100);

            sw.Reset();
            sw.Start();
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
            sw.Stop();
            Assert.AreEqual(200, sw.ElapsedMilliseconds, 20);
        }

        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка результатов вычислений при компиляции.")]
        public void SpeedTest_Precalc_Debug()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("other","other", ModuleTypeEnum.STARTUP,false,_path+"speed_test.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("other", 107, (interpreter_int) =>
            {
                Assert.AreEqual(107, interpreter.CurrentLine, 10);
                Assert.AreEqual(1000000, interpreter.Debugger.RegisterGetValue("ф").AsInt());
            });
            interpreter.Debug();

        }
    }
}
