using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System;

namespace UnitTests
{

    [TestClass]
    public class SpeedTests
    {
        // Скорость для OneScript указана без оптимизации сборки и без отладки.
        
        /// <summary>
        /// OneScript: 2900, 1C: 2066
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        public void SpeedTest_LibraryFunctionCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP, true, false), OpenModule("speed_test_library_call.scr"));


            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(1800, sw.ElapsedMilliseconds, 250);
        }

        /// <summary>
        /// OneScript: 4700, 1C: 6000
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        public void SpeedTest_FunctionCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "speed_test_function_call.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(3100, sw.ElapsedMilliseconds, 350);
        }


        /// <summary>
        /// OneScript: 8500-9500, 1C: 4500
        /// </summary>
        [TestMethod]
        [TestCategory("Speed")]
        public void SpeedTest_Array()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("array_test", "array.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            sw.Stop();

            Assert.AreEqual(4800, sw.ElapsedMilliseconds, 500);
        }

        [TestMethod]
        [TestCategory("Speed")]
        public void SpeedTest_ArrayDebug()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("array_test", "array.scr");

            ScriptProgramm programm = Compile(files);
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
        public void SpeedTest_Precalc()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "speed_test.scr");

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            ScriptProgramm programm = Compile(files);
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
        public void SpeedTest_Precalc_Debug()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "speed_test.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("other", 107);
            interpreter.Debug();

            Assert.AreEqual(107, interpreter.CurrentLine);
            Assert.AreEqual(1000000, interpreter.Debugger.RegisterGetValue("ф").AsInt());
        }



        private ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\SpeedTest\\";

            foreach (KeyValuePair<string, string> file in file_names)
            {
                if (File.Exists(path + file.Value))
                    files.Add(new ScriptModule(file.Key, file.Key, ModuleTypeEnum.STARTUP) { FileName = file.Value }, File.ReadAllText(path + file.Value));
                else
                    throw new Exception($"Файл {path} не найден.");
            }

            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(files);
        }

        private string OpenModule(string file_name)
        {
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\SpeedTest\\";

            if (File.Exists(path + file_name))
                return File.ReadAllText(path + file_name);
            else
                throw new Exception($"Файл {path + file_name} не найден.");

        }

        private ScriptProgramm CompileObjects(IDictionary<ScriptModule, string> modules)
        {
            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(modules);
        }

    }
}
