using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Exceptions
{
    /// <summary>
    /// Базовый класс исключений.
    /// </summary>
    public class ExceptionBase : ApplicationException
    {

        private readonly CodeInformation _code_information;

        internal ExceptionBase()
        {
            _code_information = new CodeInformation();
            _code_information.LineNumber = -1;
        }

        internal ExceptionBase(string message) : this(new CodeInformation(), message, null)
        {

        }

        internal ExceptionBase(CodeInformation codeInfo, string message) : this(codeInfo, message, null)
        {

        }

        internal ExceptionBase(CodeInformation codeInfo, string message, Exception innerException) : base(message, innerException)
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
                return $"Модуль [{_code_information.ModuleName}] | Ошибка в строке {_code_information.LineNumber}:{_code_information.ColumnNumber} | " + base.Message;
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
