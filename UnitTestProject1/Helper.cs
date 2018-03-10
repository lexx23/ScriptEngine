using ScriptEngine.EngineBase.Compiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace UnitTests
{
    class Helper
    {
        private string _path;

        public Helper(string path)
        {
            _path = path;
        }

        private string CheckPath(string path)
        {
            return path.Replace('\\',Path.DirectorySeparatorChar);
        }

        public ScriptProgramm Compile(IDictionary<string, string> file_names)
        {
            IDictionary<ScriptModule, string> files = new Dictionary<ScriptModule, string>();
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\" + _path + "\\";

            foreach (KeyValuePair<string, string> file in file_names)
            {
                string full_name = CheckPath(path + file.Value);
                if (File.Exists(full_name))
                    files.Add(new ScriptModule(file.Key, file.Key, ModuleTypeEnum.STARTUP) { FileName = file.Value }, File.ReadAllText(full_name));
                else
                    throw new Exception($"Файл {path} не найден.");
            }

            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(files);
        }

        public string OpenModule(string file_name)
        {
            string path = Directory.GetCurrentDirectory() + "\\Scripts\\" + _path + "\\";
            string full_name = CheckPath(path + file_name);

            if (File.Exists(full_name))
                return File.ReadAllText(full_name).Replace("\r",string.Empty);
            else
                throw new Exception($"Файл {full_name} не найден.");

        }

        public ScriptProgramm CompileModules(IDictionary<ScriptModule, string> modules)
        {
            ScriptCompiler compiler = new ScriptCompiler();
            return compiler.Compile(modules);
        }
    }
}
