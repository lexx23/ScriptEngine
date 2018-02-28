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
    public class Library_Test
    {
        [TestMethod]
        public void Library_MessageStatus()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), OpenModule("message_status.scr"));


            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 5);
            interpreter.Debugger.AddBreakpoint("global", 8);
            interpreter.Debugger.AddBreakpoint("global", 11);
            interpreter.Debugger.AddBreakpoint("global", 14);

            interpreter.Debug();

            Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
            Assert.AreEqual(5, interpreter.CurrentLine);
            interpreter.Debugger.Continue();

            Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
            Assert.AreEqual(8, interpreter.CurrentLine);
            interpreter.Debugger.Continue();


            Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
            Assert.AreEqual(11, interpreter.CurrentLine);
            interpreter.Debugger.Continue();

            Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
            Assert.AreEqual(14, interpreter.CurrentLine);
            interpreter.Debugger.Continue();
        }



        private ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Library\\";

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
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Library\\";

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
