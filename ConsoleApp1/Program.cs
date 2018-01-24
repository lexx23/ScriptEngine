using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Interpreter;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            IDictionary<string, string> files = new Dictionary<string, string>();
            files.Add("function", "Extension\\function call.scr");

            ScriptProgramm programm = Compile(files);
            ScriptInterpreter interpreter = new ScriptInterpreter(programm);
            interpreter.Debugger.AddBreakpoint("function", 12);

            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();
            interpreter.Run();// Debug();
            sw.Stop();

            Console.Write(sw.ElapsedMilliseconds);
        }

        private static ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Interpreter\\";

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
