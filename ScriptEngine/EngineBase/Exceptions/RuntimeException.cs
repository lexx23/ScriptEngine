using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Exceptions
{
    public class RuntimeException: ApplicationException
    {
        private ScriptInterpreter _script_interpreter;


        internal RuntimeException(ScriptInterpreter interpreter, string message) : base(message)
        {
            _script_interpreter = interpreter;
        }


        public string MessageWithLine
        {
            get
            {
                return $"{{{_script_interpreter.CurrentModule.Name}({_script_interpreter.CurrentLine})}} : " + base.Message;
            }
        }

        /// <summary>
        /// Текст ошибки.
        /// </summary>
        public override string Message
        {
            get
            {
                return MessageWithLine;
            }
        }
    }
}
