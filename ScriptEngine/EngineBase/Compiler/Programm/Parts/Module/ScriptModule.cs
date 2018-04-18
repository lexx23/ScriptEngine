/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types;
using System.Collections.Generic;
using System.IO;
using System;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ScriptModule : IScriptName
    {
        private ModuleVariables _vars;
        private ModuleFunctions _functions;
        private IList<IFunction> _object_functions_call;
        private IList<ScriptStatement> _code;
        private ScriptScope _module_scope;

        public string Name { get; set; }
        public string Alias { get; set; }
        public bool AsGlobal { get; set; }
        public string FileName { get; set; }
        public string Source { get; set; }

        public ModuleVariables Variables { get => _vars; set => _vars = value; }
        public ModuleFunctions Functions { get => _functions; }
        public ModuleTypeEnum Type { get; set; }
        public IList<ScriptStatement> Code { get => _code; }
        public ScriptScope ModuleScope { get => _module_scope; }

        public Type InstanceType { get; set; }

        /// <summary>
        /// Номер линии программы.
        /// </summary>
        public int ProgrammLine
        {
            get => _code.Count;
        }

        public ScriptModule(string name, string alias, ModuleTypeEnum type, bool as_global = false, string file_name = null)
        {
            Name = name;
            Alias = alias;
            Type = type;
            AsGlobal = as_global;
            FileName = file_name;

            InstanceType = null;

            // Стартовый модуль компилирую как глобальный.
            if (type == ModuleTypeEnum.STARTUP)
                AsGlobal = true;

            // Область видимости модуля. Сюда попадают переменные и функции объявленные в теле модуля.
            _module_scope = new ScriptScope() { Type = ScopeTypeEnum.MODULE, Name = name, Module = this };

            _object_functions_call = new List<IFunction>();

            _vars = new ModuleVariables(this);
            _code = new List<ScriptStatement>();
            _functions = new ModuleFunctions(this);

            OpenFile();
        }

        /// <summary>
        /// Открыть файл и считать его содержимое.
        /// </summary>
        private void OpenFile()
        {
            if (FileName != string.Empty && FileName != null)
            {
                string full_name;
                if (Path.DirectorySeparatorChar != '\\')
                    full_name = FileName.Replace('\\', Path.DirectorySeparatorChar);
                else
                    full_name = FileName;

                if (File.Exists(full_name))
                    Source = File.ReadAllText(full_name, System.Text.Encoding.UTF8);
                else
                    throw new Exception($"Файл {full_name} не найден.");
            }
        }

        /// <summary>
        /// Получить линию кода по номеру строки.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public string GetCodeLine(int index)
        {
            char prev_symbol = '\0';
            string buffer = string.Empty;
            string ignore_chars = "\n\r\t";

            if (Source == string.Empty && FileName != string.Empty)
                OpenFile();

            int counter = 1;
            for (int i = 0; i < Source.Length; i++)
            {
                if (Source[i] == '\n')
                    counter++;
                if (counter >= index)
                {
                    if (ignore_chars.IndexOf(Source[i]) == -1)
                        buffer += Source[i];
                }

                if (Source[i] == '\n' && counter == index + 1)
                    break;

                prev_symbol = Source[i];
            }


            Source = string.Empty;
            return buffer;
        }

        /// <summary>
        /// Добавить обращение к объекту.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public int ObjectCallAdd(IFunction function)
        {
            _object_functions_call.Add(function);
            return _object_functions_call.Count - 1;
        }

        /// <summary>
        /// Получить обращение к обьекту.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public IFunction ObjectCallGet(int index)
        {
            return _object_functions_call[index];
        }


        /// <summary>
        /// Добавить инструкцию.
        /// </summary>
        /// <returns></returns>
        public ScriptStatement StatementAdd()
        {
            ScriptStatement statement = new ScriptStatement
            {
                Line = -1
            };
            _code.Add(statement);
            return statement;
        }

        /// <summary>
        /// Получить инструкцию.
        /// </summary>
        public ScriptStatement StatementGet(int index)
        {
            return _code[index];
        }
    }
}
