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
        [Description("Тест функции Формат(Format)")]
        public void Library_FunctionFormat()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("Functions\\format.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 3);
            interpreter.Debugger.AddBreakpoint("global", 5);
            interpreter.Debugger.AddBreakpoint("global", 7);
            interpreter.Debugger.AddBreakpoint("global", 11);
            interpreter.Debugger.AddBreakpoint("global", 13);
            interpreter.Debugger.AddBreakpoint("global", 15);
            interpreter.Debugger.AddBreakpoint("global", 17);
            interpreter.Debugger.AddBreakpoint("global", 19);
            interpreter.Debugger.AddBreakpoint("global", 21);
            interpreter.Debugger.AddBreakpoint("global", 25);

            interpreter.Debug();

            Assert.AreEqual(3, interpreter.CurrentLine);
            Assert.AreEqual("123 456,79", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(5, interpreter.CurrentLine);
            Assert.AreEqual("123 456-789", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual($"(123 456{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}789)", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(11, interpreter.CurrentLine);
            Assert.AreEqual("20 августа 2002 г. 15:33:09", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(13, interpreter.CurrentLine);
            Assert.AreEqual("20/08-2002", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(15, interpreter.CurrentLine);
            Assert.AreEqual("20 августа 2002 г.", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(17, interpreter.CurrentLine);
            Assert.AreEqual("20.08.2002", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(19, interpreter.CurrentLine);
            Assert.AreEqual("15:33:09", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(21, interpreter.CurrentLine);
            Assert.AreEqual("7-2-14", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(25, interpreter.CurrentLine);
            Assert.AreEqual("Доступен", interpreter.Debugger.RegisterGetValue("А").AsString());
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        [Description("Тест Соответствие(Map)")]
        public void Library_MapCollection()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("map.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7);
            interpreter.Debugger.AddBreakpoint("global", 10);
            interpreter.Debugger.AddBreakpoint("global", 14);
            interpreter.Debugger.AddBreakpoint("global", 17);
            interpreter.Debugger.AddBreakpoint("global", 24);
            interpreter.Debugger.AddBreakpoint("global", 26);

            interpreter.Debug();

            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual("Олимпиада в Москве", interpreter.Debugger.RegisterGetValue("значение").AsString());
            Assert.AreEqual("Первый полет человека в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(10, interpreter.CurrentLine);
            interpreter.Debugger.Continue();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual("Олимпиада в Москве", interpreter.Debugger.RegisterGetValue("значение").AsString());
            Assert.AreEqual("Первый полет человека в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            interpreter.Debugger.Continue();


            Assert.AreEqual(17, interpreter.CurrentLine);
            Assert.AreEqual("Первый полет Гагарина в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            interpreter.Debugger.Continue();

            Assert.AreEqual(24, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("counter").AsInt());
            interpreter.Debugger.Continue();

            Assert.AreEqual(26, interpreter.CurrentLine);
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("значение").AsInt());
            interpreter.Debugger.Continue();
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
