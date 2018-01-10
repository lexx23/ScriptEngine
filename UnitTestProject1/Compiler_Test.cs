using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Exceptions;
using System;
using System.Collections.Generic;
using System.IO;

namespace UnitTests
{
    [TestClass]
    public class Compiler_Test
    {
        #region If

        [TestMethod]
        public void Compile_If()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_If_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_If_Error2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("if", "If\\if_error2.scr");

            Compile(files);
        }
        #endregion


        #region Global

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Compile_Global_Common_With_Code_Error()
        {
            IDictionary<ScriptModule, string> modules = new Dictionary<ScriptModule, string>();
            modules.Add(new ScriptModule("global", ModuleTypeEnum.STARTUP), OpenModule("Global\\Common with code error\\global_module.scr"));
            modules.Add(new ScriptModule("object", ModuleTypeEnum.COMMON, true), OpenModule("Global\\Common with code error\\object_module.scr"));

            CompileObjects(modules);
        }

        #endregion


        #region Objects

        [TestMethod]
        public void Compile_Cross_Object_Call()
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
        public void Compile_Assign_Function_Result()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Function\\assign_result.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Function_Empty_Return()
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
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Return_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\return_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_As_Function_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Call_Not_Found()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_call_not_found.scr");

            Compile(files);
        }
        

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_As_Function_Error2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error2.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_As_Function_Error3()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error3.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_As_Function_Error4()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error4.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_As_Function_Error5()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_as_function_error5.scr");

            Compile(files);
        }

        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Error_Param_Count()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Error_Param_Count2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_error_param_count2.scr");

            Compile(files);
        }


        [TestMethod]
        public void Compile_Procedure_Var_Order()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Var_Order_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("procedure", "Procedure\\procedure_var_order_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Procedure_Repeat_Error()
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
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Var_Repeat_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("repeat", "Var\\var_repeat_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Var_Order_Error()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error.scr");

            Compile(files);
        }


        [TestMethod]
        [ExpectedException(typeof(ExceptionBase))]
        public void Compile_Var_Order_Error2()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("order", "Var\\var_order_error2.scr");

            Compile(files);
        }

        [TestMethod]
        public void Compile_Var_All_Types()
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("var", "Var\\var_all_types.scr");

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
