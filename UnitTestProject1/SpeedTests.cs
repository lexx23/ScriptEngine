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

namespace UnitTests
{

    [TestClass]
    public class SpeedTests
    {
        private Helper _helper;
        public SpeedTests()
        {
            _helper = new Helper("SpeedTest");
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
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("struct", "structure.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(1700, sw.ElapsedMilliseconds, 200);
        }

        /// <summary>
        /// OneScript: 2900, 1C: 2066
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы вызова функции из внешней библиотеки.")]
        public void SpeedTest_LibraryFunctionCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("speed_test_library_call.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
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
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "speed_test_function_call.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(3300, sw.ElapsedMilliseconds, 350);
        }

        /// <summary>
        /// OneScript: 4300, 1C: 2325
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы оператора для каждого.")]
        public void SpeedTest_ForEach()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach_test", "foreach.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(2100, sw.ElapsedMilliseconds, 500);
        }

        /// <summary>
        /// OneScript: 85000, 1C: 18000
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы массива.")]
        public void SpeedTest_Eval()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("eval_test", "eval.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(50000, sw.ElapsedMilliseconds, 3000);
        }

        /// <summary>
        /// OneScript: 8500-9500, 1C: 4500
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы массива.")]
        public void SpeedTest_Array()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("array_test", "array.scr");

            ScriptProgramm programm = _helper.Compile(files);
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
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("array_test", "array.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("array_test", 28);

            interpreter.Debug();
            Assert.AreEqual(499999500000, interpreter.Debugger.RegisterGetValue("result").AsNumber());
        }


        /// <summary>
        /// OneScript: 2100, 1C: 1900
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка скорости работы вычислений при компиляции.")]
        public void SpeedTest_Precalc()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "speed_test.scr");

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            ScriptProgramm programm = _helper.Compile(files);
            sw.Stop();
            Assert.AreEqual(60, sw.ElapsedMilliseconds, 80);

            sw.Reset();
            sw.Start();
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
            sw.Stop();
            Assert.AreEqual(20, sw.ElapsedMilliseconds, 20);
        }

        [TestMethod]
        [TestCategory("Speed")]
        [Description("Проверка результатов вычислений при компиляции.")]
        public void SpeedTest_Precalc_Debug()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "speed_test.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("other", 107);
            interpreter.Debug();

            Assert.AreEqual(107, interpreter.CurrentLine);
            Assert.AreEqual(1000000, interpreter.Debugger.RegisterGetValue("ф").AsInt());
        }
    }
}
