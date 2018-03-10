using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System;

namespace UnitTests
{
    [TestClass]
    public class Compiler_Test
    {

        private Helper _helper;
        public Compiler_Test()
        {
            _helper = new Helper("Compiler");
        }

        #region Try
        [TestMethod]
        public void Compile_Try()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("try", "Exception\\try.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [Description("Проверка вызватьисключение без параметров.")]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_RaiseError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("raise", "Exception\\raise_error.scr");

            _helper.Compile(files);
        }
        #endregion


        [TestMethod]
        public void Compile_ArrayIndexer()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("new", "Array\\indexer.scr");

            _helper.Compile(files);
        }



        [TestMethod]
        public void Compile_NewSimpleCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("new", "New\\simple.scr");

            _helper.Compile(files);
        }


        #region Extension

        [TestMethod]
        public void Compile_ExtensionFunctionCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Extension\\function call.scr");

            _helper.Compile(files);
        }


        #endregion

        #region For

        [TestMethod]
        public void Compile_Goto()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error3.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error4.scr");

            _helper.Compile(files);
        }
        #endregion


        #region For

        [TestMethod]
        public void Compile_ForEach()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach_error3.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForEachError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("foreach", "For\\foreach_error4.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        public void Compile_For()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error3.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error4.scr");

            _helper.Compile(files);
        }

        #endregion

        #region While

        [TestMethod]
        public void Compile_While()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\loop_words_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\loop_words_error2.scr");

            _helper.Compile(files);
        }


        #endregion

        #region If


        [TestMethod]
        public void Compile_IfShort()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error3.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error4.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        public void Compile_If()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error3.scr");

            _helper.Compile(files);
        }
        #endregion


        #region Global

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Compile_Global_CommonWithCodeError()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("Global\\Common with code error\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.COMMON, true), _helper.OpenModule("Global\\Common with code error\\object_module.scr"));

            _helper.CompileModules(modules);
        }

        #endregion


        #region Objects

        [TestMethod]
        public void Сompile_Objects_CrossObjectCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", "global", ModuleTypeEnum.STARTUP), _helper.OpenModule("Objects\\Cross object call\\global_module.scr"));
            modules.Add(new ScriptModule("object", "object", ModuleTypeEnum.OBJECT,true,true), _helper.OpenModule("Objects\\Cross object call\\object_module.scr"));

            _helper.CompileModules(modules);
        }

        #endregion

        #region Function

        [TestMethod]
        public void Compile_Function()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\function.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        public void Compile_Function_AssignResult()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\assign_result.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Function_EmptyReturn()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\function_empty_return.scr");

            _helper.Compile(files);
        }
        #endregion

        #region Procedure

        [TestMethod]
        public void Compile_Procedure()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        public void Compile_Procedure_Return()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\return.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ReturnError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\return_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_CallNotFoundError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_call_not_found.scr");

            _helper.Compile(files);
        }
        

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error3.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error4.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError5()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error5.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count2.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        public void Compile_Procedure_VarOrder()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_VarOrderError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order_error.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_RepeatError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_repeat_error.scr");

            _helper.Compile(files);
        }

        #endregion


        #region Vars

        [TestMethod]
        public void Compile_Var_Assign()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_assign.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_RepeatError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("repeat", "Var\\var_repeat_error.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error.scr");

            _helper.Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error2.scr");

            _helper.Compile(files);
        }

        [TestMethod]
        public void Compile_Var_AllTypes()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_all_types.scr");

            _helper.Compile(files);
        }

        #endregion

        #region Other
        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Other_ThrowError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("Проверка деления на 0", "Other\\throw_error.scr");

            _helper.Compile(files);
        }
        #endregion
    }
}
