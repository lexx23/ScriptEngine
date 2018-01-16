using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class Interpreter_Test
    {
        #region Goto

        [TestMethod]
        public void Interpreter_Goto()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("goto", 14);
            interpreter.Debugger.AddBreakpoint("goto", 28);
            interpreter.Debug();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(28, interpreter.CurrentLine);
            Assert.AreEqual(20, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();
        }
        #endregion


        #region For

        [TestMethod]
        public void Interpreter_For()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("for", 7);
            interpreter.Debugger.AddBreakpoint("for", 14);
            interpreter.Debugger.AddBreakpoint("for", 25);
            interpreter.Debugger.AddBreakpoint("for", 36);
            interpreter.Debug();

            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(8, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();


            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(25, interpreter.CurrentLine);
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(36, interpreter.CurrentLine);
            Assert.AreEqual(51, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();

        }
        #endregion



        #region While

        [TestMethod]
        public void Interpreter_While()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("while", 4);
            interpreter.Debugger.AddBreakpoint("while", 9);
            interpreter.Debugger.AddBreakpoint("while", 20);
            interpreter.Debugger.AddBreakpoint("while", 37);
            interpreter.Debugger.AddBreakpoint("while", 51);
            interpreter.Debug();

            Assert.AreEqual(4, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(9, interpreter.CurrentLine);
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(20, interpreter.CurrentLine);
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();


            Assert.AreEqual(37, interpreter.CurrentLine);
            Assert.AreEqual(400, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(51, interpreter.CurrentLine);
            Assert.AreEqual(401, interpreter.Debugger.RegisterGetValue("ф").Integer);
            Assert.AreEqual(199, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.Continue();
        }
        #endregion


        #region If

        [TestMethod]
        public void Interpreter_IfShort()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("if", 6);
            interpreter.Debugger.AddBreakpoint("if", 10);
            interpreter.Debugger.AddBreakpoint("if", 14);
            interpreter.Debugger.AddBreakpoint("if", 18);
            interpreter.Debugger.AddBreakpoint("if", 22);
            interpreter.Debugger.AddBreakpoint("if", 26);
            interpreter.Debug();

            Assert.AreEqual(6, interpreter.CurrentLine);
            Assert.AreEqual("if", interpreter.CurrentModule.Name);

            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(-100, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(18, interpreter.CurrentLine);
            Assert.AreEqual(-200, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(26, interpreter.CurrentLine);
            Assert.AreEqual(30, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        public void Interpreter_If()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("if", 12);
            interpreter.Debugger.AddBreakpoint("if", 22);
            interpreter.Debugger.AddBreakpoint("if", 34);
            interpreter.Debugger.AddBreakpoint("if", 46);
            interpreter.Debugger.AddBreakpoint("if", 54);
            interpreter.Debugger.AddBreakpoint("if", 62);
            interpreter.Debugger.AddBreakpoint("if", 70);
            interpreter.Debug();

            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual("if", interpreter.CurrentModule.Name);

            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(34, interpreter.CurrentLine);
            Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(46, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(54, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(62, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(70, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
        }


        [TestMethod]
        public void Interpreter_IfNasted()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_nasted.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("if", 11);
            interpreter.Debugger.AddBreakpoint("if", 25);
            interpreter.Debugger.AddBreakpoint("if", 41);
            interpreter.Debugger.AddBreakpoint("if", 58);
            interpreter.Debug();

            Assert.AreEqual(11, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debug();

            Assert.AreEqual(25, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debug();

            Assert.AreEqual(41, interpreter.CurrentLine);
            Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debug();

            Assert.AreEqual(58, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debug();
        }

        #endregion

        #region Object

        [TestMethod]
        public void Interpreter_Objects_CrossObjectCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Objects\\Cross object call\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.OBJECT, false), OpenModule("Objects\\Cross object call\\object_module.scr"));

            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7);
            interpreter.Debugger.AddBreakpoint("global", 30);
            interpreter.Debugger.AddBreakpoint("global", 38);
            interpreter.Debugger.AddBreakpoint("global", 46);
            interpreter.Debugger.AddBreakpoint("global", 54);
            interpreter.Debug();
            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);

            Assert.AreEqual(100, interpreter.Debugger.ObjectGetValue("object", "Количество").Integer);
            interpreter.Debugger.StepOver();

            Assert.AreEqual(9, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").Integer);

            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").Integer);
            Assert.AreEqual(101, interpreter.Debugger.ObjectGetValue("object", "Количество").Integer);

            interpreter.Debugger.AddBreakpoint("object", 12);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").Integer);

            interpreter.Debugger.AddBreakpoint("global", 12);
            interpreter.Debugger.Continue();


            Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").Integer);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").Integer);
            Assert.AreEqual(102, interpreter.Debugger.RegisterGetValue("колво").Integer);

            interpreter.Debugger.Continue();
            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            interpreter.Debugger.RemoveBreakpoint("object", 12);
            interpreter.Debugger.AddBreakpoint("global", 22);
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);

            Assert.AreEqual(105, interpreter.Debugger.ObjectGetValue("object", "Количество").Integer);
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("колво").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(30, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(38, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(46, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(54, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").Integer);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Interpreter_Objects_ProcedureAsFunction_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Objects\\Object procedure as function call\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.OBJECT, false), OpenModule("Objects\\Object procedure as function call\\object_module.scr"));

            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Interpreter_Objects_NotPublicFunctionCall_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Objects\\Object not public call error\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.OBJECT, false), OpenModule("Objects\\Object not public call error\\object_module.scr"));

            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_NotPublicMethodCall_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Objects\\Not public method call\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.OBJECT, false), OpenModule("Objects\\Not public method call\\object_module.scr"));

            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }

        #endregion

        #region Global
        [TestMethod]
        public void Interpreter_GlobalFunctionCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Global\\Function call\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.COMMON, true), OpenModule("Global\\Function call\\object_module.scr"));

            ScriptProgramm programm = CompileObjects(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 6);
            interpreter.Debugger.AddBreakpoint("global", 20);

            interpreter.Debugger.AddBreakpoint("global", 28);
            interpreter.Debugger.AddBreakpoint("global", 36);
            interpreter.Debugger.AddBreakpoint("global", 44);
            interpreter.Debugger.AddBreakpoint("global", 52);

            interpreter.Debug();
            Assert.AreEqual(6, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);

            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").Integer);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").Integer);
            interpreter.Debugger.StepInto();

            Assert.AreEqual(2, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").Integer);

            interpreter.Debugger.Continue();

            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(20, interpreter.CurrentLine);
            Assert.AreEqual(115, interpreter.Debugger.RegisterGetValue("колво").Integer);
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("Количество").Integer);
            interpreter.Debugger.Continue();


            Assert.AreEqual(28, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(36, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(44, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").Integer);
            interpreter.Debugger.Continue();

            Assert.AreEqual(52, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").Integer);
        }

        #endregion



        #region Function

        [TestMethod]
        public void Interpreter_FunctionAssignResult()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\assign_result.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 10);
            interpreter.Debug();

            interpreter.Debugger.StepOver();
            Assert.AreEqual(1080, interpreter.Debugger.RegisterGetValue("а").Integer);
            Assert.AreEqual(11, interpreter.CurrentLine);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(1060, interpreter.Debugger.RegisterGetValue("а").Integer);
            Assert.AreEqual(12, interpreter.CurrentLine);
        }

        [TestMethod]
        public void Interpreter_FunctionRecursiveCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\recursive_call.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 12);
            interpreter.Debug();

            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").Integer);
        }
        #endregion

        #region Procedure

        [TestMethod]
        public void Interpreter_Procedure()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("procedure", 30);
            interpreter.Debug();

            // Тест1
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").Integer);
            interpreter.Debugger.StepInto();
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(12, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").Integer);

            // Тест2
            interpreter.Debugger.StepInto();
            IList<FunctionHistoryData> stack_list = interpreter.Debugger.GetStackCall();
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").Integer);


            // Тест3
            Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.StepInto();
            Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").Integer);

            // Тест
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").Integer);
            interpreter.Debugger.StepInto();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(600, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").Integer);
            Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("парам1").Integer);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").Integer);
            Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("б").Integer);

        }

        #endregion

        #region Var


        [TestMethod]
        public void Interpreter_Var_Assign()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_assign.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("var", 14);
            interpreter.Debugger.AddBreakpoint("var", 15);
            interpreter.Debugger.AddBreakpoint("var", 16);
            interpreter.Debug();


            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("в1").Integer);
            Assert.AreEqual(298.6875, interpreter.Debugger.RegisterGetValue("а1").Float);
            Assert.AreEqual(498.6875, interpreter.Debugger.RegisterGetValue("а").Float);
            Assert.AreEqual(2496.125, interpreter.Debugger.RegisterGetValue("а2").Float);
            Assert.AreEqual(2396.125, interpreter.Debugger.RegisterGetValue("а3").Float);
            Assert.AreEqual(494.6875, interpreter.Debugger.RegisterGetValue("а4").Float);
            Assert.AreEqual(-494.6875, interpreter.Debugger.RegisterGetValue("а5").Float);
            interpreter.Debug();

            Assert.AreEqual(0.6, interpreter.Debugger.RegisterGetValue("а6").Float,3);
            interpreter.Debug();
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("а7").Float,3);
        }

        #endregion

        private ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Interpreter\\";

            foreach (KeyValuePair<string, string> file in file_names)
            {
                if (File.Exists(path + file.Value))
                    files.Add(new ScriptModule(file.Key,ModuleTypeEnum.STARTUP) { FileName = file.Value }, File.ReadAllText(path + file.Value));
                else
                    throw new Exception($"Файл {path} не найден.");
            }

            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(files);
        }

        private string OpenModule(string file_name)
        {
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Interpreter\\";

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
