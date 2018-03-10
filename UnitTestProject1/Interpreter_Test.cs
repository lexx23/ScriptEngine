﻿using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System.Diagnostics;

namespace UnitTests
{
    [TestClass]
    public class Interpreter_Test
    {
        private Helper _helper;
        public Interpreter_Test()
        {
            _helper = new Helper("Interpreter");
        }


        [TestMethod]
        [Description("Проверка работы доступа к массиву через []")]
        public void Interpreter_ArrayIndexer()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("Array\\indexer.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7);
            interpreter.Debugger.AddBreakpoint("global", 9);

            interpreter.Debug();

            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual(123, interpreter.Debugger.RegisterGetValue("значение").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(9, interpreter.CurrentLine);
            Assert.AreEqual(124, interpreter.Debugger.RegisterGetValue("значение").AsNumber());
            interpreter.Debugger.Continue();
        }

        #region Exception
        [TestMethod]
        [Description("Проверка работы вызватьисключение(raise)")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Raise()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("raise", "raise", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("Exception\\raise.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }


        [TestMethod]
        [Description("Проверка функции ИнформацияОбОшибке(), без ошибки класс должен быть пустой.")]
        public void Interpreter_ErrorInfo()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("try", "try", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("Exception\\error_info.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("try", 6);

            interpreter.Debug();

            Assert.AreEqual(6, interpreter.CurrentLine);
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("инфо", "Описание").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "имяМодуля").AsString());
            Assert.AreEqual("0", interpreter.Debugger.ObjectGetValue("Инфо", "НомерСтроки").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "ИсходнаяСтрока").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "Причина").AsString());
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) блока и функции ИнформацияОбОшибке()")]
        public void Interpreter_Try()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("try", "try", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("Exception\\try.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("try", 13);

            interpreter.Debug();

            Assert.AreEqual(13, interpreter.CurrentLine);
            Assert.AreEqual("Деление на 0.", interpreter.Debugger.ObjectGetValue("инфо", "Описание").AsString());
            Assert.AreEqual("try", interpreter.Debugger.ObjectGetValue("Инфо", "имяМодуля").AsString());
            Assert.AreEqual("4", interpreter.Debugger.ObjectGetValue("Инфо", "НомерСтроки").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "ИсходнаяСтрока").AsString());
            Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "Причина").AsString());
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) внутри объекта.")]
        public void Interpreter_TryInObject()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP, true), _helper.OpenModule("Exception\\In object\\global.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, true), _helper.OpenModule("Exception\\In object\\object.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 5);
            interpreter.Debug();

            Assert.AreEqual(5, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) вне объекта.")]
        public void Interpreter_InObjectException()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP, true), _helper.OpenModule("Exception\\In object exception\\global.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, true), _helper.OpenModule("Exception\\In object exception\\object.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 10);
            interpreter.Debugger.AddBreakpoint("global", 19);
            interpreter.Debug();

            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            interpreter.Debugger.Continue();

            Assert.AreEqual(19, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            interpreter.Debugger.Continue();
        }


        [TestMethod]
        [Description("Проверка работы Попытка(Try) блока вложенного друг в друга.")]
        public void Interpreter_TryNasted()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("try", "try", ModuleTypeEnum.STARTUP, true, false), _helper.OpenModule("Exception\\try_nasted.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("try", 23);

            interpreter.Debug();

            Assert.AreEqual(23, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("f").AsNumber());
            interpreter.Debugger.Continue();

        }

        #endregion

        #region Extension

        [TestMethod]
        public void Interpreter_ExtensionFunctionCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,false), _helper.OpenModule("Extension\\function call.scr"));


            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 12);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();
            //interpreter.Debug();
            sw.Stop();

            Assert.AreEqual(1800, sw.ElapsedMilliseconds,150);
            //Assert.AreEqual(1000000, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
        }

        #endregion

        #region Other

        [TestMethod]
        [Description("Проверка преобразования ошибки из CompilerError в RuntimeError")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_OtherThrowError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "Other\\throw_error_runtime.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }


        [TestMethod]
        [Description("Ошибка при доступе к свойству только для чтения.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_OtherReadOnlyPropertyError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("other", "Other\\readonly_property_error.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }

        #endregion

        #region Goto

        [TestMethod]
        [Description("Проверка переходов.")]
        public void Interpreter_Goto()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("goto", 14);
            interpreter.Debugger.AddBreakpoint("goto", 28);
            interpreter.Debug();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsInt());
            interpreter.Debugger.Continue();

            Assert.AreEqual(28, interpreter.CurrentLine);
            Assert.AreEqual(20, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();
        }
        #endregion

        #region For

        [TestMethod]
        [Description("Проверка оператора для каждого.")]
        public void Interpreter_ForEach()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("foreach", 29);
            interpreter.Debugger.AddBreakpoint("foreach", 45);
            interpreter.Debugger.AddBreakpoint("foreach", 66);
            interpreter.Debug();

            Assert.AreEqual(29, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(45, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(7, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(66, interpreter.CurrentLine);
            Assert.AreEqual(8, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(7, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            interpreter.Debugger.Continue();

        }


        [TestMethod]
        [Description("Проверка работы оператора Для.")]
        public void Interpreter_For()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("for", 7);
            interpreter.Debugger.AddBreakpoint("for", 14);
            interpreter.Debugger.AddBreakpoint("for", 25);
            interpreter.Debugger.AddBreakpoint("for", 36);
            interpreter.Debugger.AddBreakpoint("for", 49);
            interpreter.Debug();

            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(8, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();


            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(25, interpreter.CurrentLine);
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(36, interpreter.CurrentLine);
            Assert.AreEqual(51, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(49, interpreter.CurrentLine);
            Assert.AreEqual(36, interpreter.Debugger.RegisterGetValue("в").AsNumber());
            Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

        }
        #endregion

        #region While

        [TestMethod]
        [Description("Проверка работы оператора Пока.")]
        public void Interpreter_While()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("while", 4);
            interpreter.Debugger.AddBreakpoint("while", 9);
            interpreter.Debugger.AddBreakpoint("while", 20);
            interpreter.Debugger.AddBreakpoint("while", 37);
            interpreter.Debugger.AddBreakpoint("while", 51);
            interpreter.Debug();

            Assert.AreEqual(4, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(9, interpreter.CurrentLine);
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(20, interpreter.CurrentLine);
            Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();


            Assert.AreEqual(37, interpreter.CurrentLine);
            Assert.AreEqual(400, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(51, interpreter.CurrentLine);
            Assert.AreEqual(401, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            Assert.AreEqual(199, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.Continue();
        }
        #endregion

        #region If

        [TestMethod]
        [Description("Проверка работы оператора 'короткий' Если.")]
        public void Interpreter_IfShort()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if.scr");

            ScriptProgramm programm = _helper.Compile(files);
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

            Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(14, interpreter.CurrentLine);
            Assert.AreEqual(-100, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(18, interpreter.CurrentLine);
            Assert.AreEqual(-200, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(26, interpreter.CurrentLine);
            Assert.AreEqual(30, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();
        }

        [TestMethod]
        [Description("Проверка работы оператора Если.")]
        public void Interpreter_If()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if.scr");

            ScriptProgramm programm = _helper.Compile(files);
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

            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(34, interpreter.CurrentLine);
            Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(46, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(54, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(62, interpreter.CurrentLine);
            Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(70, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
        }


        [TestMethod]
        [Description("Проверка работы оператора Если, вложенный друг в друга.")]
        public void Interpreter_IfNasted()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_nasted.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("if", 11);
            interpreter.Debugger.AddBreakpoint("if", 25);
            interpreter.Debugger.AddBreakpoint("if", 41);
            interpreter.Debugger.AddBreakpoint("if", 58);
            interpreter.Debug();

            Assert.AreEqual(11, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debug();

            Assert.AreEqual(25, interpreter.CurrentLine);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debug();

            Assert.AreEqual(41, interpreter.CurrentLine);
            Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debug();

            Assert.AreEqual(58, interpreter.CurrentLine);
            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debug();
        }

        #endregion

        #region Object

        [TestMethod]
        [Description("Проверка вызова методов и свойств обьекта.")]
        public void Interpreter_Objects_CrossObjectCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true), _helper.OpenModule("Objects\\Cross object call\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true,true), _helper.OpenModule("Objects\\Cross object call\\object_module.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 7);
            interpreter.Debugger.AddBreakpoint("global", 30);
            interpreter.Debugger.AddBreakpoint("global", 38);
            interpreter.Debugger.AddBreakpoint("global", 46);
            interpreter.Debugger.AddBreakpoint("global", 54);
            interpreter.Debug();
            Assert.AreEqual(7, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);

            Assert.AreEqual(100, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
            interpreter.Debugger.StepOver();

            Assert.AreEqual(9, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());

            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.CurrentLine);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            Assert.AreEqual(101, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());

            interpreter.Debugger.AddBreakpoint("object", 12);
            interpreter.Debugger.StepOver();
            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());

            interpreter.Debugger.AddBreakpoint("global", 12);
            interpreter.Debugger.Continue();


            Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
            Assert.AreEqual(102, interpreter.Debugger.RegisterGetValue("колво").AsNumber());

            interpreter.Debugger.Continue();
            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            interpreter.Debugger.RemoveBreakpoint("object", 12);
            interpreter.Debugger.AddBreakpoint("global", 22);
            interpreter.Debugger.Continue();

            Assert.AreEqual(22, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);

            Assert.AreEqual(105, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(30, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(38, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(46, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(54, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
        }


        [TestMethod]
        [Description("Проверка вызова процедур обьекта как функций.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_ProcedureAsFunction_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true), _helper.OpenModule("Objects\\Object procedure as function call\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true,true), _helper.OpenModule("Objects\\Object procedure as function call\\object_module.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [Description("Проверка вызова, не публичных, методов обьекта.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_NotPublicFunctionCall_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true), _helper.OpenModule("Objects\\Object not public call error\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true,true), _helper.OpenModule("Objects\\Object not public call error\\object_module.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [Description("Проверка вызова, не публичных, свойств обьекта.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_NotPublicPropertyCall_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true), _helper.OpenModule("Objects\\Not public property call\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true,true), _helper.OpenModule("Objects\\Not public property call\\object_module.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }

        #endregion

        #region Global
        [TestMethod]
        [Description("Проверка вызова глобальных методов обьекта.")]
        public void Interpreter_GlobalFunctionCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global","global", ModuleTypeEnum.STARTUP,true), _helper.OpenModule("Global\\Function call\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.COMMON, true,false), _helper.OpenModule("Global\\Function call\\object_module.scr"));

            ScriptProgramm programm = _helper.CompileModules(modules);
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

            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            interpreter.Debugger.StepInto();

            Assert.AreEqual(2, interpreter.CurrentLine);
            Assert.AreEqual("object", interpreter.CurrentModule.Name);
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());

            interpreter.Debugger.Continue();

            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(20, interpreter.CurrentLine);
            Assert.AreEqual(115, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
            interpreter.Debugger.Continue();


            Assert.AreEqual(28, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(36, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(44, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            interpreter.Debugger.Continue();

            Assert.AreEqual(52, interpreter.CurrentLine);
            Assert.AreEqual("global", interpreter.CurrentModule.Name);
            Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
        }

        #endregion



        #region Function

        [TestMethod]
        [Description("Проверка присвоения результата выполнения функции переменной.")]
        public void Interpreter_FunctionAssignResult()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\assign_result.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 10);
            interpreter.Debug();

            interpreter.Debugger.StepOver();
            Assert.AreEqual(11, interpreter.CurrentLine);
            Assert.AreEqual(1080, interpreter.Debugger.RegisterGetValue("а").AsNumber());

            interpreter.Debugger.StepOver();
            Assert.AreEqual(12, interpreter.CurrentLine);
            Assert.AreEqual(1060, interpreter.Debugger.RegisterGetValue("а").AsNumber());
        }

        [TestMethod]
        [Description("Проверка рекурсивного выполнения функции.")]
        public void Interpreter_FunctionRecursiveCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\recursive_call.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 12);
            interpreter.Debug();

            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
        }
        #endregion

        #region Procedure

        [TestMethod]
        [Description("Проверка выполнения процедур.")]
        public void Interpreter_Procedure()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("procedure", 30);
            interpreter.Debug();

            // Тест1
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            interpreter.Debugger.StepInto();
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(12, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").AsNumber());

            // Тест2
            interpreter.Debugger.StepInto();
            Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());


            // Тест3
            Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.StepInto();
            Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").AsNumber());

            // Тест
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            interpreter.Debugger.StepInto();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(600, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
            Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
            interpreter.Debugger.StepOver();
            Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("б").AsNumber());

        }

        #endregion

        #region Var


        [TestMethod]
        [Description("Проверка присвоения значений переменной.")]
        public void Interpreter_Var_Assign()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_assign.scr");

            ScriptProgramm programm = _helper.Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("var", 14);
            interpreter.Debugger.AddBreakpoint("var", 15);
            interpreter.Debugger.AddBreakpoint("var", 16);
            interpreter.Debug();


            Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("в1").AsNumber());
            Assert.AreEqual(298.6875m, interpreter.Debugger.RegisterGetValue("а1").AsNumber());
            Assert.AreEqual(498.6875m, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            Assert.AreEqual(2496.125m, interpreter.Debugger.RegisterGetValue("а2").AsNumber());
            Assert.AreEqual(2396.125m, interpreter.Debugger.RegisterGetValue("а3").AsNumber());
            Assert.AreEqual(494.6875m, interpreter.Debugger.RegisterGetValue("а4").AsNumber());
            Assert.AreEqual(-494.6875m, interpreter.Debugger.RegisterGetValue("а5").AsNumber());
            interpreter.Debug();

            Assert.AreEqual(0.6m, interpreter.Debugger.RegisterGetValue("а6").AsNumber());
            interpreter.Debug();
            Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("а7").AsNumber());
        }

        #endregion
    }
}
