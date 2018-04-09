/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Compiler;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class Compiler_Test
    {

        private string _path;
        public Compiler_Test()
        {
            _path = Directory.GetCurrentDirectory() + "\\Scripts\\Compiler\\";
        }

        #region Вычислить(Eval) Выполнить(Execute)
        [TestMethod]
        [Description("Проверка Вычислить.")]
        public void Compile_Eval()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("eval","eval", ModuleTypeEnum.STARTUP,false,_path+"Eval\\eval.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка Вычислить без параметров.")]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_EvalError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("eval","eval", ModuleTypeEnum.STARTUP,false,_path+"Eval\\eval_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }
        #endregion

        #region Try
        [TestMethod]
        [Description("Проверка блока Попытка-Исключение.")]
        public void Compile_Try()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("try","try", ModuleTypeEnum.STARTUP,false,_path+"Exception\\try.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка ВызватьИсключение без параметров.")]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_RaiseError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("raise","raise", ModuleTypeEnum.STARTUP,false,_path + "Exception\\raise_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка ВызватьИсключение без параметров внутри блока Исключение.")]
        public void Compile_RaiseWoError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("raise","raise", ModuleTypeEnum.STARTUP,false,_path + "Exception\\raise_wo_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }
        #endregion


        [TestMethod]
        public void Compile_ArrayIndexer()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global","global", ModuleTypeEnum.STARTUP,false,_path + "Array\\indexer.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }


        #region Новый(New)
        [TestMethod]
        [Description("Проверка Новый(Вариант 1: Новый <Идентификатор типа>[(<Парам1>, <Парам2>, …)] ).")]
        public void Compile_NewType1()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("new","new", ModuleTypeEnum.STARTUP,false,_path + "New\\simple.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка Новый(Вариант 1), примитивные типы")]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_NewType1Erorr()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("new","new", ModuleTypeEnum.STARTUP,false,_path + "New\\type1_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка Новый(Вариант 2: Новый(<Тип>[, <ПараметрыКонструктора>])")]
        public void Compile_NewType2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("new","new", ModuleTypeEnum.STARTUP,false,_path + "New\\type2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка Новый(Вариант 2: Новый(<Тип>[, <ПараметрыКонструктора>]), более 2х параметров")]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_NewType2Erorr()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("new","new", ModuleTypeEnum.STARTUP,false,_path + "New\\type2_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        #endregion


        #region Extension

        [TestMethod]
        public void Compile_ExtensionFunctionCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function","function", ModuleTypeEnum.STARTUP,false,_path + "Extension\\function call.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }


        #endregion

        #region For

        [TestMethod]
        public void Compile_Goto()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto","goto", ModuleTypeEnum.STARTUP,false,_path + "Goto\\goto.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto","goto", ModuleTypeEnum.STARTUP,false,_path + "Goto\\goto_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto","goto", ModuleTypeEnum.STARTUP,false,_path + "Goto\\goto_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto","goto", ModuleTypeEnum.STARTUP,false,_path + "Goto\\goto_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError4()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("goto","goto", ModuleTypeEnum.STARTUP,false,_path + "Goto\\goto_error4.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }
        #endregion


        #region For

        [TestMethod]
        public void Compile_ForEach()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\foreach.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\foreach_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\foreach_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\foreach_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError4()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("foreach","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\foreach_error4.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        public void Compile_For()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for","foreach", ModuleTypeEnum.STARTUP,false,_path + "For\\for.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for","for", ModuleTypeEnum.STARTUP,false,_path + "For\\for_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError2()
        {

            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for","for", ModuleTypeEnum.STARTUP,false,_path + "For\\for_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for","for", ModuleTypeEnum.STARTUP,false,_path + "For\\for_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError4()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("for","for", ModuleTypeEnum.STARTUP,false,_path + "For\\for_error4.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        #endregion

        #region While

        [TestMethod]
        public void Compile_While()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\while.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [Description("Проверка вложенного цикла пока, без ; у вложенного цикла.")]
        public void Compile_WhileWoSemicolon()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\while_wo_semicolon.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\while_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError2()
        {

            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\while_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\loop_words_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("while","while", ModuleTypeEnum.STARTUP,false,_path + "While\\loop_words_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        #endregion

        #region If


        [TestMethod]
        public void Compile_IfShort()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\short_if.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\short_if_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\short_if_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\short_if_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError4()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\short_if_error4.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        public void Compile_If()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\if.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\if_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "if\\if_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("if","if", ModuleTypeEnum.STARTUP,false,_path + "If\\if_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }
        #endregion


        #region Global

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Global_CommonWithCodeError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,_path +  "Global\\Common with code error\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.COMMON, true, _path + "Global\\Common with code error\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        #endregion


        #region Objects

        [TestMethod]
        public void Сompile_Objects_CrossObjectCall()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true,_path +  "Objects\\Cross object call\\global_module.scr"),
                new ScriptModule("object", "object", ModuleTypeEnum.OBJECT, true, _path + "Objects\\Cross object call\\object_module.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        #endregion

        #region Function

        [TestMethod]
        public void Compile_Function()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function", "function", ModuleTypeEnum.STARTUP,true,_path +  "Function\\function.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        public void Compile_Function_AssignResult()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function", "function", ModuleTypeEnum.STARTUP,true,_path +  "Function\\assign_result.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Function_EmptyReturn()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("function", "function", ModuleTypeEnum.STARTUP,true,_path +  "Function\\function_empty_return.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }
        #endregion

        #region Procedure

        [TestMethod]
        public void Compile_Procedure()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        public void Compile_Procedure_Return()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\return.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ReturnError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\return_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_as_function_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_CallNotFoundError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_call_not_found.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_as_function_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError3()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_as_function_error3.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError4()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_as_function_error4.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError5()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_as_function_error5.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_error_param_count.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_error_param_count2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        public void Compile_Procedure_VarOrder()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_var_order.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_VarOrderError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_var_order_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_RepeatError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("procedure", "procedure", ModuleTypeEnum.STARTUP,true,_path +  "Procedure\\procedure_var_repeat_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        #endregion


        #region Vars

        [TestMethod]
        public void Compile_Var_Assign()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true,_path +  "Var\\var_assign.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_RepeatError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true,_path +  "Var\\var_repeat_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true,_path +  "Var\\var_order_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError2()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true,_path +  "Var\\var_order_error2.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        [TestMethod]
        public void Compile_Var_AllTypes()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("var", "var", ModuleTypeEnum.STARTUP,true,_path +  "Var\\var_all_types.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        #endregion

        #region Other
        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Other_ThrowError()
        {
            IList<ScriptModule> modules = new List<ScriptModule>()
            {
                new ScriptModule("other", "other", ModuleTypeEnum.STARTUP,true,_path +  "Other\\throw_error.scr")
            };

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.CompileProgramm(modules);

        }

        #endregion
    }
}
