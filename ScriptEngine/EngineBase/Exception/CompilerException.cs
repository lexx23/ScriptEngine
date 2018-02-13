using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Exceptions
{
    /// <summary>
    /// Базовый класс исключений.
    /// </summary>
    public class CompilerException : ApplicationException
    {

        private readonly CodeInformation _code_information;

        internal CompilerException()
        {
            _code_information = new CodeInformation();
            _code_information.LineNumber = -1;
        }

        internal CompilerException(string message) : base(message)
        {

        }

        internal CompilerException(CodeInformation codeInfo, string message) : base (message)
        {
            _code_information = codeInfo;
        }

        /// <summary>
        /// Информация о коде.
        /// </summary>
        public CodeInformation CodeInformation
        {
            get { return _code_information; }
        }

        /// <summary>
        /// Описание ошибки.
        /// </summary>
        public string ErrorDescription
        {
            get
            {
                return base.Message;
            }
        }

        /// <summary>
        /// Описание ошибки с информацией о линиях кода.
        /// </summary>
        public string MessageWithCodeInfo
        {
            get
            {
                if(_code_information != null)
                    return $"Модуль [{_code_information.ModuleName}] | Ошибка в строке {_code_information.LineNumber}:{_code_information.ColumnNumber} | " + base.Message;
                else
                    return base.Message;
            }
        }

        /// <summary>
        /// Текст ошибки.
        /// </summary>
        public override string Message
        {
            get
            {
                return MessageWithCodeInfo;
            }
        }
    }

}
