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
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System.IO;
using ScriptEngine.EngineBase.Compiler;
using System;

namespace UnitTests
{
    [TestClass]
    public class Interpreter_Test
    {
        private readonly string _path;
        public Interpreter_Test()
        {
            _path = Directory.GetCurrentDirectory() + "\\Scripts\\Interpreter\\";
        }

        [TestMethod]
        [Ignore]
        public void Interpreter_Other()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Other\\tmp_test.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debug();
        }

        #region Вычислить(Eval) Выполнить(Execute)
        [TestMethod]
        [Description("Проверка работы Вычислить(Eval)")]
        public void Interpreter_Eval()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Eval\\eval.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Eval\\object_eval.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 3, (interpreater) =>
            {
                Assert.AreEqual(3, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 5, (interpreater) =>
            {
                Assert.AreEqual(5, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 8, (interpreater) =>
            {
                Assert.AreEqual(8, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });

            interpreter.Debug();
        }

        #endregion


        [TestMethod]
        [Description("Проверка работы доступа к массиву через []")]
        public void Interpreter_ArrayIndexer()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Array\\indexer.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
             {
                 Assert.AreEqual(7, interpreter.CurrentLine);
                 Assert.AreEqual(123, interpreter.Debugger.RegisterGetValue("значение").AsNumber());
             });

            interpreter.Debugger.AddBreakpoint("global", 9, (interpreater) =>
            {
                Assert.AreEqual(9, interpreter.CurrentLine);
                Assert.AreEqual(124, interpreter.Debugger.RegisterGetValue("значение").AsNumber());
            });
            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка работы оперратор Новый(New)")]
        public void Interpreter_New()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "New\\new.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 13, (interpreater) =>
            {
                Assert.AreEqual(13, interpreter.CurrentLine);
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("счетчик").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 32, (interpreater) =>
            {
                Assert.AreEqual(32, interpreter.CurrentLine);
                Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("счетчик").AsNumber());
            });

            interpreter.Debug();
        }

        #region Exception
        [TestMethod]
        [Description("Проверка работы вызватьисключение(raise)")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Raise()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Exception\\raise.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }


        [TestMethod]
        [Description("Проверка функции ИнформацияОбОшибке(), без ошибки класс должен быть пустой.")]
        public void Interpreter_ErrorInfo()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("info", "info", ModuleTypeEnum.STARTUP,true, _path + "Exception\\error_info.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("info", 6, (interpreater) =>
             {
                 Assert.AreEqual(6, interpreter.CurrentLine);
                 Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("инфо", "Описание").AsString());
                 Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "имяМодуля").AsString());
                 Assert.AreEqual("0", interpreter.Debugger.ObjectGetValue("Инфо", "НомерСтроки").AsString());
                 Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "ИсходнаяСтрока").AsString());
                 Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "Причина").AsString());
             });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) блока и функции ИнформацияОбОшибке()")]
        public void Interpreter_Try()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("try", "try", ModuleTypeEnum.STARTUP,true, _path + "Exception\\try.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("try", 13, (interpreater) =>
            {
                Assert.AreEqual(13, interpreter.CurrentLine);
                Assert.AreEqual("Деление на 0.", interpreter.Debugger.ObjectGetValue("инфо", "Описание").AsString());
                Assert.AreEqual("try", interpreter.Debugger.ObjectGetValue("Инфо", "имяМодуля").AsString());
                Assert.AreEqual("4", interpreter.Debugger.ObjectGetValue("Инфо", "НомерСтроки").AsString());
                Assert.AreEqual("f = f/0;", interpreter.Debugger.ObjectGetValue("Инфо", "ИсходнаяСтрока").AsString());
                Assert.AreEqual("", interpreter.Debugger.ObjectGetValue("Инфо", "Причина").AsString());
            });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) внутри объекта.")]
        public void Interpreter_TryInObject()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Exception\\In object\\global.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Exception\\In object\\object.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 5, (interpreater) =>
            {
                Assert.AreEqual(5, interpreter.CurrentLine);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка работы Попытка(Try) вне объекта.")]
        public void Interpreter_InObjectException()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Exception\\In object exception\\global.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Exception\\In object exception\\object.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 10, (interpreater) =>
            {
                Assert.AreEqual(10, interpreter.CurrentLine);
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            });

            interpreter.Debugger.AddBreakpoint("global", 19, (interpreater) =>
            {
                Assert.AreEqual(19, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ТестЗначение").AsInt());
            });
            interpreter.Debug();
        }


        [TestMethod]
        [Description("Проверка работы Попытка(Try) блока вложенного друг в друга.")]
        public void Interpreter_TryNasted()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("try", "try", ModuleTypeEnum.STARTUP,true, _path + "Exception\\try_nasted.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("try", 23, (interpreater) =>
            {
                Assert.AreEqual(23, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("f").AsNumber());
            });

            interpreter.Debug();
        }

        #endregion

        #region Other

        [TestMethod]
        [Description("Проверка преобразования ошибки из CompilerError в RuntimeError")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_OtherThrowError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("other", "other", ModuleTypeEnum.STARTUP,true, _path + "Other\\throw_error_runtime.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }

        [TestMethod]
        [Description("Проверка строки с переносом на несколько строк.")]
        public void Interpreter_MultilineString()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("string","string", ModuleTypeEnum.STARTUP,false,_path + "Other\\multiline_string.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("string", 5, (interpreater) =>
            {
                Assert.AreEqual(5, interpreter.CurrentLine);
                Assert.AreEqual($"<xml>{Environment.NewLine}<data>hello</data>{Environment.NewLine}</xml>", interpreter.Debugger.RegisterGetValue("Текст").AsString());
            });


            interpreter.Debug();
        }

        [TestMethod]
        [Description("Ошибка при доступе к свойству только для чтения.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_OtherReadOnlyPropertyError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("other", "other", ModuleTypeEnum.STARTUP,true, _path + "Other\\readonly_property_error.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();
        }

        #endregion

        #region Goto

        [TestMethod]
        [Description("Проверка переходов.")]
        public void Interpreter_Goto()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto", "goto", ModuleTypeEnum.STARTUP,true, _path + "Goto\\goto.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("goto", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsInt());
            });

            interpreter.Debugger.AddBreakpoint("goto", 28, (interpreater) =>
            {
                Assert.AreEqual(28, interpreter.CurrentLine);
                Assert.AreEqual(20, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });
            interpreter.Debug();
        }
        #endregion

        #region For

        [TestMethod]
        [Description("Проверка оператора для каждого.")]
        public void Interpreter_ForEach()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach", "foreach", ModuleTypeEnum.STARTUP,true, _path + "For\\foreach.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("foreach", 29, (interpreater) =>
            {
                Assert.AreEqual(29, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("foreach", 45, (interpreater) =>
            {
                Assert.AreEqual(45, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(7, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("foreach", 66, (interpreater) =>
            {
                Assert.AreEqual(66, interpreter.CurrentLine);
                Assert.AreEqual(8, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(7, interpreter.Debugger.RegisterGetValue("результат").AsNumber());
            });

            interpreter.Debug();
        }


        [TestMethod]
        [Description("Проверка работы оператора Для.")]
        public void Interpreter_For()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for", "for", ModuleTypeEnum.STARTUP,true, _path + "For\\for.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("for", 7, (interpreater) =>
            {
                Assert.AreEqual(7, interpreter.CurrentLine);
                Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(8, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("for", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("for", 25, (interpreater) =>
            {
                Assert.AreEqual(25, interpreter.CurrentLine);
                Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(51, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("for", 36, (interpreater) =>
            {
                Assert.AreEqual(36, interpreter.CurrentLine);
                Assert.AreEqual(51, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(51, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("for", 49, (interpreater) =>
            {
                Assert.AreEqual(49, interpreter.CurrentLine);
                Assert.AreEqual(36, interpreter.Debugger.RegisterGetValue("в").AsNumber());
                Assert.AreEqual(6, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debug();
        }
        #endregion

        #region While

        [TestMethod]
        [Description("Проверка работы оператора Пока.")]
        public void Interpreter_While()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while", "while", ModuleTypeEnum.STARTUP,true, _path + "While\\while.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("while", 4, (interpreater) =>
            {
                Assert.AreEqual(4, interpreter.CurrentLine);
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("while", 9, (interpreater) =>
            {
                Assert.AreEqual(9, interpreter.CurrentLine);
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("while", 20, (interpreater) =>
            {
                Assert.AreEqual(20, interpreter.CurrentLine);
                Assert.AreEqual(50, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("while", 37, (interpreater) =>
            {
                Assert.AreEqual(37, interpreter.CurrentLine);
                Assert.AreEqual(400, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("while", 51, (interpreater) =>
            {
                Assert.AreEqual(51, interpreter.CurrentLine);
                Assert.AreEqual(401, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
                Assert.AreEqual(199, interpreter.Debugger.RegisterGetValue("б").AsNumber());
            });

            interpreter.Debug();
        }
        #endregion

        #region If

        [TestMethod]
        [Description("Проверка работы оператора 'короткий' Если.")]
        public void Interpreter_IfShort()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if", "if", ModuleTypeEnum.STARTUP,true, _path + "If\\short_if.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("if", 6, (interpreater) =>
            {
                Assert.AreEqual(6, interpreter.CurrentLine);
                Assert.AreEqual("if", interpreter.CurrentModule.Name);
                Assert.AreEqual(0, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 10, (interpreater) =>
            {
                Assert.AreEqual(10, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual(-100, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 18, (interpreater) =>
            {
                Assert.AreEqual(18, interpreter.CurrentLine);
                Assert.AreEqual(-200, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 22, (interpreater) =>
            {
                Assert.AreEqual(22, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 26, (interpreater) =>
            {
                Assert.AreEqual(26, interpreter.CurrentLine);
                Assert.AreEqual(30, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка работы оператора Если.")]
        public void Interpreter_If()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if", "if", ModuleTypeEnum.STARTUP,true, _path + "If\\if.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("if", 12, (interpreater) =>
            {
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual("if", interpreter.CurrentModule.Name);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 22, (interpreater) =>
            {
                Assert.AreEqual(22, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 34, (interpreater) =>
            {
                Assert.AreEqual(34, interpreter.CurrentLine);
                Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 46, (interpreater) =>
            {
                Assert.AreEqual(46, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());

            });

            interpreter.Debugger.AddBreakpoint("if", 54, (interpreater) =>
            {
                Assert.AreEqual(54, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());

            });

            interpreter.Debugger.AddBreakpoint("if", 62, (interpreater) =>
            {
                Assert.AreEqual(62, interpreter.CurrentLine);
                Assert.AreEqual(2, interpreter.Debugger.RegisterGetValue("ф").AsNumber());

            });

            interpreter.Debugger.AddBreakpoint("if", 70, (interpreater) =>
            {
                Assert.AreEqual(70, interpreter.CurrentLine);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debug();
        }


        [TestMethod]
        [Description("Проверка работы оператора Если, вложенный друг в друга.")]
        public void Interpreter_IfNasted()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if", "if", ModuleTypeEnum.STARTUP,true, _path + "If\\if_nasted.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("if", 11, (interpreater) =>
            {
                Assert.AreEqual(11, interpreter.CurrentLine);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 25, (interpreater) =>
            {
                Assert.AreEqual(25, interpreter.CurrentLine);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 41, (interpreater) =>
            {
                Assert.AreEqual(41, interpreter.CurrentLine);
                Assert.AreEqual(3, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("if", 58, (interpreater) =>
            {
                Assert.AreEqual(58, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debug();
        }

        #endregion

        #region Object

        [TestMethod]
        [Description("Проверка создания обьекта из файла скрипта.")]
        public void Interpreter_Objects_MultiCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Objects\\Object multi call\\global.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("global", 4, (interpreater) =>
            {
                Assert.AreEqual(4, interpreter.CurrentLine);
                Assert.AreEqual(5, interpreter.Debugger.Eval("обьект1.ПолучитьКоличество()").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 6, (interpreater) =>
            {
                Assert.AreEqual(6, interpreter.CurrentLine);
                Assert.AreEqual(5, interpreter.Debugger.Eval("обьект2.ПолучитьКоличество()").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 11, (interpreater) =>
            {
                Assert.AreEqual(11, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.Eval("обьект1.ПолучитьКоличество()").AsNumber());
                Assert.AreEqual(20, interpreter.Debugger.Eval("обьект2.ПолучитьКоличество()").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 15, (interpreater) =>
            {
                Assert.AreEqual(15, interpreter.CurrentLine);
                Assert.AreEqual(5, interpreter.Debugger.Eval("обьект2.ПолучитьКоличество()").AsNumber());
            });


            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка вызова методов и свойств обьекта.")]
        public void Interpreter_Objects_CrossObjectCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Objects\\Cross object call\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Objects\\Cross object call\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
            {
                Assert.AreEqual(7, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);

                Assert.AreEqual(100, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
                interpreter.Debugger.StepOver();
            });

            interpreter.Debugger.AddBreakpoint("global", 9, (interpreater) =>
            {
                Assert.AreEqual(9, interpreter.CurrentLine);
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
                interpreter.Debugger.StepOver();
            });

            interpreter.Debugger.AddBreakpoint("global",10, (interpreater) =>
            {
                Assert.AreEqual(10, interpreter.CurrentLine);
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
                Assert.AreEqual(101, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
                interpreter.Debugger.RemoveBreakpoint("global", 10);
            });


            interpreter.Debugger.AddBreakpoint("object", 13, (interpreater) =>
            {
                Assert.AreEqual(13, interpreter.CurrentLine);
                Assert.AreEqual("object", interpreter.CurrentModule.Name);
                Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
                interpreter.Debugger.RemoveBreakpoint("object", 13);
            });

            interpreter.Debugger.AddBreakpoint("global", 12, (interpreater) =>
            {
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
                interpreter.Debugger.StepOver();
            });

            interpreter.Debugger.AddBreakpoint("global", 13, (interpreater) =>
            {
                Assert.AreEqual(13, interpreter.CurrentLine);
                Assert.AreEqual(102, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
                Assert.AreEqual(102, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            });



            interpreter.Debugger.AddBreakpoint("global", 22, (interpreater) =>
            {
                Assert.AreEqual(22, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(105, interpreter.Debugger.ObjectGetValue("object", "Количество").AsNumber());
                Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 30, (interpreater) =>
            {
                Assert.AreEqual(30, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 38, (interpreater) =>
            {
                Assert.AreEqual(38, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("global", 46, (interpreater) =>
            {
                Assert.AreEqual(46, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 54, (interpreater) =>
            {
                Assert.AreEqual(54, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });
            interpreter.Debug();
        }


        [TestMethod]
        [Description("Проверка вызова процедур обьекта как функций.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_ProcedureAsFunction_Error()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Objects\\Object procedure as function call\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Objects\\Object procedure as function call\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [Description("Проверка вызова, не публичных, методов обьекта.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_NotPublicFunctionCall_Error()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Objects\\Object not public call error\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Objects\\Object not public call error\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }


        [TestMethod]
        [Description("Проверка вызова, не публичных, свойств обьекта.")]
        [ExpectedException(typeof(RuntimeException))]
        public void Interpreter_Objects_NotPublicPropertyCall_Error()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Objects\\Not public property call\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Objects\\Not public property call\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Run();

        }

        #endregion

        #region Global
        [TestMethod]
        [Description("Проверка вызова глобальных методов обьекта.")]
        public void Interpreter_GlobalFunctionCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "Global\\Function call\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.COMMON, true, _path + "Global\\Function call\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("global", 6, (interpreater) =>
            {
                Assert.AreEqual(6, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);

                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("колво").AsNumber());

                interpreter.Debugger.AddBreakpoint("object", 2, (interpreater2) =>
                {
                    Assert.AreEqual(2, interpreter.CurrentLine);
                    Assert.AreEqual("object", interpreter.CurrentModule.Name);
                    Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("object", 2);
                });
                interpreter.Debugger.RemoveBreakpoint("global", 6);
            });

            interpreter.Debugger.AddBreakpoint("global", 7, (interpreater) =>
            {
                Assert.AreEqual(7, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(101, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("global", 20, (interpreater) =>
            {
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(20, interpreter.CurrentLine);
                Assert.AreEqual(115, interpreter.Debugger.RegisterGetValue("колво").AsNumber());
                Assert.AreEqual(105, interpreter.Debugger.RegisterGetValue("Количество").AsNumber());
            });


            interpreter.Debugger.AddBreakpoint("global", 28, (interpreater) =>
            {
                Assert.AreEqual(28, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 36, (interpreater) =>
            {
                Assert.AreEqual(36, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 44, (interpreater) =>
            {
                Assert.AreEqual(44, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(-1, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("global", 52, (interpreater) =>
            {
                Assert.AreEqual(52, interpreter.CurrentLine);
                Assert.AreEqual("global", interpreter.CurrentModule.Name);
                Assert.AreEqual(106, interpreter.Debugger.RegisterGetValue("ф").AsNumber());
            });


            interpreter.Debug();
        }

        #endregion



        #region Function

        [TestMethod]
        [Description("Проверка присвоения результата выполнения функции переменной.")]
        public void Interpreter_FunctionAssignResult()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function", "function", ModuleTypeEnum.STARTUP,true, _path + "Function\\assign_result.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 11, (interpreater) =>
            {
                Assert.AreEqual(11, interpreter.CurrentLine);
                Assert.AreEqual(1080, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("function", 12, (interpreater) =>
            {
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual(1060, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            });

            interpreter.Debug();
        }

        [TestMethod]
        [Description("Проверка рекурсивного выполнения функции.")]
        public void Interpreter_FunctionRecursiveCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function", "function", ModuleTypeEnum.STARTUP,true, _path + "Function\\recursive_call.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 12, (interpreater2) =>
            {
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual(12, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
            });
            interpreter.Debug();
        }
        #endregion

        #region Procedure

        [TestMethod]
        [Description("Проверка выполнения процедур.")]
        public void Interpreter_Procedure()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true, _path + "Procedure\\procedure.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);

            interpreter.Debugger.AddBreakpoint("procedure", 30, (interpreater) =>
            {
                Assert.AreEqual(30, interpreter.CurrentLine);
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").AsNumber());

                interpreter.Debugger.AddBreakpoint("procedure", 13, (interpreater2) =>
                {
                    Assert.AreEqual(13, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(12, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 13);
                });

                interpreter.Debugger.RemoveBreakpoint("procedure", 30);
            });

            // Тест2
            interpreter.Debugger.AddBreakpoint("procedure", 33, (interpreater) =>
            {
                Assert.AreEqual(33, interpreter.CurrentLine);
                Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("а").AsNumber());

                interpreter.Debugger.AddBreakpoint("procedure", 16, (interpreater2) =>
                {
                    Assert.AreEqual(16, interpreter.CurrentLine);
                    Assert.AreEqual(100, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 16);
                });


                interpreter.Debugger.AddBreakpoint("procedure", 17, (interpreater2) =>
                {
                    Assert.AreEqual(17, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(5, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 17);
                });

                interpreter.Debugger.RemoveBreakpoint("procedure", 33);
            });


            // Тест3
            interpreter.Debugger.AddBreakpoint("procedure", 36, (interpreater) =>
            {
                Assert.AreEqual(36, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
                Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("б").AsNumber());

                interpreter.Debugger.AddBreakpoint("procedure", 21, (interpreater2) =>
                {
                    Assert.AreEqual(21, interpreter.CurrentLine);
                    Assert.AreEqual(200, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure",21);
                });


                interpreter.Debugger.AddBreakpoint("procedure", 23, (interpreater2) =>
                {
                    Assert.AreEqual(23, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 23);
                });

                interpreter.Debugger.RemoveBreakpoint("procedure", 36);
            });


            interpreter.Debugger.AddBreakpoint("procedure", 39, (interpreater) =>
            {
                Assert.AreEqual(39, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
                Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("б").AsNumber());

                interpreter.Debugger.AddBreakpoint("procedure", 6, (interpreater2) =>
                {
                    Assert.AreEqual(6, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure",6);
                });


                interpreter.Debugger.AddBreakpoint("procedure", 7, (interpreater2) =>
                {
                    Assert.AreEqual(7, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 7);
                });


                interpreter.Debugger.AddBreakpoint("procedure", 8, (interpreater2) =>
                {
                    Assert.AreEqual(8, interpreter.CurrentLine);
                    Assert.AreEqual(600, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(210, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 8);
                });

                interpreter.Debugger.AddBreakpoint("procedure", 9, (interpreater2) =>
                {
                    Assert.AreEqual(9, interpreter.CurrentLine);
                    Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("парам").AsNumber());
                    Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("парам1").AsNumber());
                    interpreter.Debugger.RemoveBreakpoint("procedure", 9);
                });

                interpreter.Debugger.RemoveBreakpoint("procedure", 39);
            });


            interpreter.Debugger.AddBreakpoint("procedure", 41, (interpreater) =>
            {
                Assert.AreEqual(41, interpreter.CurrentLine);
                Assert.AreEqual(10, interpreter.Debugger.RegisterGetValue("а").AsNumber());
                Assert.AreEqual(220, interpreter.Debugger.RegisterGetValue("б").AsNumber());

                interpreter.Debugger.RemoveBreakpoint("procedure", 41);
            });

            interpreter.Debug();
        }

        #endregion

        #region Var


        [TestMethod]
        [Description("Проверка присвоения значений переменной.")]
        public void Interpreter_Var_Assign()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true, _path + "Var\\var_assign.scr"),
            };

            ScriptCompiler compiler = new ScriptCompiler();
            ScriptProgramm programm = compiler.CompileProgramm(modules);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("var", 14, (interpreater) =>
            {
                Assert.AreEqual(14, interpreter.CurrentLine);
                Assert.AreEqual(4, interpreter.Debugger.RegisterGetValue("в1").AsNumber());
                Assert.AreEqual(298.6875m, interpreter.Debugger.RegisterGetValue("а1").AsNumber());
                Assert.AreEqual(498.6875m, interpreter.Debugger.RegisterGetValue("а").AsNumber());
                Assert.AreEqual(2496.125m, interpreter.Debugger.RegisterGetValue("а2").AsNumber());
                Assert.AreEqual(2396.125m, interpreter.Debugger.RegisterGetValue("а3").AsNumber());
                Assert.AreEqual(494.6875m, interpreter.Debugger.RegisterGetValue("а4").AsNumber());
                Assert.AreEqual(-494.6875m, interpreter.Debugger.RegisterGetValue("а5").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("var", 15, (interpreater) =>
            {
                Assert.AreEqual(15, interpreter.CurrentLine);
                Assert.AreEqual(0.6m, interpreter.Debugger.RegisterGetValue("а6").AsNumber());
            });

            interpreter.Debugger.AddBreakpoint("var", 16, (interpreater) =>
            {
                Assert.AreEqual(16, interpreter.CurrentLine);
                Assert.AreEqual(1, interpreter.Debugger.RegisterGetValue("а7").AsNumber());
            });
            interpreter.Debug();
        }

        #endregion
    }
}
