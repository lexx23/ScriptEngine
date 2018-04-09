/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Compiler;
using System.Collections.Generic;
using System.IO;
using System;
using System.Globalization;

namespace UnitTests
{
    [TestClass]
    public class Library_Test
    {

        private readonly string _path;
        public Library_Test()
        {
            _path = Directory.GetCurrentDirectory() + "\\Scripts\\Library\\";
        }

        [TestMethod]
        [Description("Тест СистемнаяИнформация(SystemInfo)")]
        public void Library_SystemInfo()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "systeminfo.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debug();
        }

        [TestMethod]
        [Description("Тест ТипЗнч(typeof)")]
        public void Library_FunctionTypeOf()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("object", "object", ModuleTypeEnum.COMMON,false,  _path + "Functions\\typeof_object.scr"),
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,  _path + "Functions\\typeof.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            foreach (InternalScriptType type in interpreter.Programm.InternalTypes)
                Console.WriteLine(type.Description);
            Console.WriteLine("--");

            interpreter.Debugger.AddBreakpoint("global", 68, (interpreater) =>
            {
                Assert.AreEqual(68, interpreter.CurrentLine);
                //Assert.AreEqual(interpreter.Programm.InternalTypes.Count, interpreter.Debugger.Eval("счетчик").AsInt());
            });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Тест функции Формат(Format)")]
        public void Library_FunctionFormat()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,  _path + "Functions\\format.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("global", 3, (interpreater) =>
            {
                Assert.AreEqual(3, interpreter.CurrentLine);
                Assert.AreEqual($"123 456{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}79", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 5, (interpreater) =>
            {
                Assert.AreEqual(5, interpreter.CurrentLine);
                Assert.AreEqual("123 456-789", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
            {
                Assert.AreEqual(7, interpreter.CurrentLine);
                Assert.AreEqual($"(123 456{System.Globalization.CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator}789)", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 11, (interpreater) =>
            {
                Assert.AreEqual(11, interpreter.CurrentLine);
                //Assert.AreEqual($"20 {CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames[7]} 2002 г. 15:33:09", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 13, (interpreater) =>
            {
                Assert.AreEqual(13, interpreter.CurrentLine);
                Assert.AreEqual("20/08-2002", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 15, (interpreater) =>
            {
                Assert.AreEqual(15, interpreter.CurrentLine);
                //Assert.AreEqual($"20 {CultureInfo.CurrentCulture.DateTimeFormat.MonthGenitiveNames[7]} 2002 г.", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 17, (interpreater) =>
            {
                Assert.AreEqual(17, interpreter.CurrentLine);
            Assert.AreEqual($"20{CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator}08{CultureInfo.CurrentCulture.DateTimeFormat.DateSeparator}2002", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 19, (interpreater) =>
            {
                Assert.AreEqual(19, interpreter.CurrentLine);
                Assert.AreEqual("15:33:09", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 21, (interpreater) =>
            {
                Assert.AreEqual(21, interpreter.CurrentLine);
                Assert.AreEqual("7-2-14", interpreter.Debugger.RegisterGetValue("А").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 25, (interpreater) =>
            {
                Assert.AreEqual(25, interpreter.CurrentLine);
                Assert.AreEqual("Доступен", interpreter.Debugger.RegisterGetValue("А").AsString());
            });


            interpreter.Debug();
        }

        [TestMethod]
        [Description("Тест Соответствие(Map)")]
        public void Library_MapCollection()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,  _path + "map.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
            {
                Assert.AreEqual(7, interpreter.CurrentLine);
                Assert.AreEqual("Олимпиада в Москве", interpreter.Debugger.RegisterGetValue("значение").AsString());
                Assert.AreEqual("Первый полет человека в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            });


            interpreter.Debugger.AddBreakpoint("global", 10, (interpreater) =>
            {
                Assert.AreEqual(10, interpreter.CurrentLine);
            });


            interpreter.Debugger.AddBreakpoint("global", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual("Олимпиада в Москве", interpreter.Debugger.RegisterGetValue("значение").AsString());
                Assert.AreEqual("Первый полет человека в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 17, (interpreater) =>
            {
                Assert.AreEqual(17, interpreter.CurrentLine);
                Assert.AreEqual("Первый полет Гагарина в космос.", interpreter.Debugger.RegisterGetValue("значение2").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 24, (interpreater) =>
            {
                Assert.AreEqual(24, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("counter").AsInt());
            });

            interpreter.Debugger.AddBreakpoint("global", 26, (interpreater) =>
            {
                Assert.AreEqual(26, interpreter.CurrentLine);
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("значение").AsInt());
            });


            interpreter.Debug();
        }


        [TestMethod]
        [Description("Тест перечисления СтатусСообщения(MessageStatus)")]
        public void Library_MessageStatus()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,  _path + "message_status.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
            {
                Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
                Assert.AreEqual(7, interpreter.CurrentLine);
            });

            interpreter.Debugger.AddBreakpoint("global", 9, (interpreater) =>
            {
                Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
                Assert.AreEqual(9, interpreter.CurrentLine);
            });

            interpreter.Debugger.AddBreakpoint("global", 13, (interpreater) =>
            {
                Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
                Assert.AreEqual(13, interpreter.CurrentLine);
            });

            interpreter.Debugger.AddBreakpoint("global", 16, (interpreater) =>
            {
                Assert.AreEqual("Важное", interpreter.Debugger.RegisterGetValue("статус").AsString());
                Assert.AreEqual(16, interpreter.CurrentLine);
            });


            interpreter.Debug();
        }


        [TestMethod]
        [Description("Тест Структура(Structure)")]
        public void Library_Structure()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,  _path + "structure.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 10, (interpreater) =>
            {
                Assert.AreEqual(10, interpreter.CurrentLine);
                Assert.AreEqual(true, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
            });

            interpreter.Debugger.AddBreakpoint("global", 11, (interpreater) =>
            {
                Assert.AreEqual(11, interpreter.CurrentLine);
                Assert.AreEqual(false, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
            });

            interpreter.Debugger.AddBreakpoint("global", 12, (interpreater) =>
            {
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual(true, interpreter.Debugger.RegisterGetValue("результат").AsBoolean());
                Assert.AreEqual("Основные", interpreter.Debugger.RegisterGetValue("настройки").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual("Основные", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 15, (interpreater) =>
            {
                Assert.AreEqual(15, interpreter.CurrentLine);
                Assert.AreEqual("Основные2", interpreter.Debugger.RegisterGetValue("настройки").AsString());
                Assert.AreEqual("Основные", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 26, (interpreater) =>
            {
                Assert.AreEqual(26, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("количество").AsInt());
                Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
                Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 27, (interpreater) =>
            {
                Assert.AreEqual(27, interpreter.CurrentLine);
                Assert.AreEqual("124", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
                Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            });

            interpreter.Debugger.AddBreakpoint("global", 28, (interpreater) =>
            {
                Assert.AreEqual(28, interpreter.CurrentLine);
                Assert.AreEqual("124", interpreter.Debugger.ObjectGetValue("Запись", "настройки").AsString());
                Assert.AreEqual("Отчет", interpreter.Debugger.ObjectGetValue("Запись", "Отчет").AsString());
            });


            interpreter.Debug();
        }
    }
}
