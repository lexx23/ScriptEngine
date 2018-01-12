using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using System;
using System.Collections.Generic;
using System.IO;

namespace ConsoleApp1
{
    class Program
    {

        static void Main(string[] args)
        {
            IList<string> files = new List<string>();
            files.Add("namespace.scr");

            Compile(files);
        }

        private static void Compile(IList<string> file_names)
        {
            Dictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\Compiler\\";

            foreach (string file_name in file_names)
            {
                if (File.Exists(path + file_name))
                    files.Add(new ScriptModule(file_name,ModuleTypeEnum.STARTUP), File.ReadAllText(path + file_name));
                else
                    throw new Exception($"Файл {path} не найден.");
            }

            ScriptCompiler compiler = new ScriptCompiler();
            compiler.Compile(files);
        }
    }
}
