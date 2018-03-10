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

        private Helper _helper;
        public Library_Test()
        {
            _helper = new Helper("Library");
        }

        [TestMethod]
        [Description("Тест СистемнаяИнформация(SystemInfo)")]
        public void Library_SystemInfo()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("systeminfo.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debug();
        }


        [TestMethod]
        [Description("Тест перечисления СтатусСообщения(MessageStatus)")]
        public void Library_MessageStatus()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("message_status.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
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


        [TestMethod]
        [Description("Тест Структура(Structure)")]
        public void Library_Structure()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("structure.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 10);
            interpreter.Debugger.AddBreakpoint("global", 11);
            interpreter.Debugger.AddBreakpoint("global", 12);
            interpreter.Debugger.AddBreakpoint("global", 14);
            interpreter.Debugger.AddBreakpoint("global", 15);
            interpreter.Debugger.AddBreakpoint("global", 26);
            interpreter.Debugger.AddBreakpoint("global", 27);
            interpreter.Debugger.AddBreakpoint("global", 28);

            interpreter.Debug();

            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(true, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
            interpreter.Debugger.Continue();

            Assert.AreEqual(11, interpreter.CurrentLine);
            Assert.AreEqual(false, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
            interpreter.Debugger.Continue();

            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual(true, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
            Assert.AreEqual("Основные", interpreter.Debugger.RegisterGetValue("настройки").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual("Основные", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(15, interpreter.CurrentLine);
            Assert.AreEqual("Основные2", interpreter.Debugger.RegisterGetValue("настройки").AsString());
            Assert.AreEqual("Основные", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            interpreter.Debugger.Continue();


            Assert.AreEqual(26, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("количество").AsInt());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(27, interpreter.CurrentLine);
            Assert.AreEqual("124", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(28, interpreter.CurrentLine);
            Assert.AreEqual("124", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            Assert.AreEqual("Отчет", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            interpreter.Debugger.Continue();

        }
    }
}
