using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class Compiler_Test
    {
        #region Extension

        [TestMethod]
        public void Compile_ExtensionFunctionCall()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Extension\\function call.scr");

            Compile(files);
        }


        #endregion

        #region For

        [TestMethod]
        public void Compile_Goto()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error3.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_GotoError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("goto", "Goto\\goto_error4.scr");

            Compile(files);
        }
        #endregion


        #region For

        [TestMethod]
        public void Compile_For()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error3.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_ForError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("for", "For\\for_error4.scr");

            Compile(files);
        }

        #endregion

        #region While

        [TestMethod]
        public void Compile_While()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_WhileError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\while_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\loop_words_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_LoopWordsError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("while", "While\\loop_words_error2.scr");

            Compile(files);
        }


        #endregion

        #region If


        [TestMethod]
        public void Compile_IfShort()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error3.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfShortError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\short_if_error4.scr");

            Compile(files);
        }


        [TestMethod]
        public void Compile_If()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_IfError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error3.scr");

            Compile(files);
        }
        #endregion


        #region Global

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Compile_Global_CommonWithCodeError()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Global\\Common with code error\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.COMMON, true), OpenModule("Global\\Common with code error\\object_module.scr"));

            CompileObjects(modules);
        }

        #endregion


        #region Objects

        [TestMethod]
        public void Сompile_Objects_CrossObjectCall()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Objects\\Cross object call\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.OBJECT,false), OpenModule("Objects\\Cross object call\\object_module.scr"));

            CompileObjects(modules);
        }

        #endregion

        #region Function

        [TestMethod]
        public void Compile_Function()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\function.scr");

            Compile(files);
        }

        [TestMethod]
        public void Compile_Function_AssignResult()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\assign_result.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Function_EmptyReturn()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\function_empty_return.scr");

            Compile(files);
        }
        #endregion

        #region Procedure

        [TestMethod]
        public void Compile_Procedure()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure.scr");

            Compile(files);
        }

        [TestMethod]
        public void Compile_Procedure_Return()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\return.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ReturnError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\return_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_CallNotFoundError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_call_not_found.scr");

            Compile(files);
        }
        

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error3.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error4.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_AsFunctionError5()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error5.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_ParamCountError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count2.scr");

            Compile(files);
        }


        [TestMethod]
        public void Compile_Procedure_VarOrder()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_VarOrderError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Procedure_RepeatError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_repeat_error.scr");

            Compile(files);
        }

        #endregion


        #region Vars

        [TestMethod]
        public void Compile_Var_Assign()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_assign.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_RepeatError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("repeat", "Var\\var_repeat_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Var_OrderError2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error2.scr");

            Compile(files);
        }

        [TestMethod]
        public void Compile_Var_AllTypes()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_all_types.scr");

            Compile(files);
        }

        #endregion

        #region Other
        [TestMethod]
        [ExpectedException(typeof(CompilerException))]
        public void Compile_Other_ThrowError()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("Проверка деления на 0", "Other\\throw_error.scr");

            Compile(files);
        }
        #endregion

        private ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Compiler\\";

            foreach (KeyValuePair<string, string> file in file_names)
            {
                if (File.Exists(path + file.Value))
                    files.Add(new ScriptModule(file.Key, ModuleTypeEnum.STARTUP) { FileName = file.Value }, File.ReadAllText(path + file.Value));
                else
                    throw new Exception($"Файл {path} не найден.");
            }

            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(files);
        }

        private string OpenModule(string file_name)
        {
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Compiler\\";

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
