/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    /// <summary>
    /// Предназначен для представления структурированной информации об ошибке (исключении).
    /// </summary>
    [LibraryClassAttribute(Name = "ErrorInfo", Alias = "ИнформацияОбОшибке", RegisterType = true, AsGlobal = false)]
    public class ErrorInfo : LibraryModule<ErrorInfo>
    {
        [LibraryClassProperty(Alias = "ИмяМодуля", Name = "ModuleName")]
        public string ModuleName
        {
            get;
        }

        [LibraryClassProperty(Alias = "ИсходнаяСтрока", Name = "SourceLine")]
        public string SourceLine
        {
            get;
        }

        [LibraryClassProperty(Alias = "НомерСтроки", Name = "LineNumber")]
        public int LineNumber
        {
            get;
        }

        [LibraryClassProperty(Alias = "Описание", Name = "Description")]
        public string Description
        {
            get;
        }

        [LibraryClassProperty(Alias = "Причина", Name = "Cause")]
        public ErrorInfo Cause
        {
            get;
        }

        public ErrorInfo()
        {
            ModuleName = "";
            SourceLine = "";
            LineNumber = 0;
            Description = "";
            Cause = null;
        }


        public ErrorInfo(Exception exception)
        {
            if (exception.Data.Count == 0)
            {
                ModuleName = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.CurrentModule.Name;
                LineNumber = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.CurrentLine;
                SourceLine = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.CurrentModule.GetCodeLine(LineNumber);
                Description = exception.Message;
                Cause = null;
            }
            else
            {
                ModuleName = Convert.ToString(exception.Data["module"]);
                LineNumber = Convert.ToInt32(exception.Data["line"]);
                ScriptModule module = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.Modules.Get(ModuleName);
                if(module != null)
                    SourceLine = module.GetCodeLine(LineNumber);
                Description = Convert.ToString(exception.Data["message"]); ;
                Cause = null;
            }
        }

    }
}
