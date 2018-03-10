using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    /// <summary>
    /// Предназначен для представления структурированной информации об ошибке (исключении).
    /// </summary>
    [LibraryClassAttribute(Name = "ErrorInfo", Alias = "ИнформацияОбОшибке", AsGlobal = false, AsObject = true)]
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
            ModuleName = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.CurrentModule.Name;
            LineNumber = ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.CurrentLine;

            Description = exception.Message;
            Cause = null;
        }

    }
}
