/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Interpreter;
using System;

namespace ScriptEngine.EngineBase.Exceptions
{
    public class RuntimeException: ApplicationException
    {
        private int _line;
        private string _module;
        public  string _message;

        internal RuntimeException(Exception ex)
        {
            if (ex.Data.Count != 0)
            {
                _line = Convert.ToInt32(ex.Data["line"]);
                _module = Convert.ToString(ex.Data["module"]);
                _message = Convert.ToString(ex.Data["message"]);
                base.Data["line"] = ex.Data["line"];
                base.Data["module"] = ex.Data["module"];
                base.Data["message"] = ex.Data["message"];
            }
            else
                _message = ex.Message;
        }


        internal RuntimeException(int line,string module, string message) : base(message)
        {
            _line = line;
            _module = module;
            _message = message;
            base.Data["line"] = _line;
            base.Data["module"] = _module;
            base.Data["message"] = message;
        }

        internal RuntimeException(ScriptInterpreter interpreter, string message) : base(message)
        {
            _module = interpreter.CurrentModule.Name;
            _line = interpreter.CurrentLine;
            _message = message;
            base.Data["line"] = _line;
            base.Data["module"] = _module;
            base.Data["message"] = message;
        }


        /// <summary>
        /// Текст ошибки.
        /// </summary>
        public override string Message
        {
            get
            {
                return $"{{{_module}({_line})}} : " + _message;
            }
        }
    }
}
