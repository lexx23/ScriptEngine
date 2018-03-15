using System;


namespace ScriptEngine.EngineBase.Parser.Token
{
    public class CodeInformation : Object
    {
        /// <summary>
        /// Наименование модуля.
        /// </summary>
        public string ModuleName { get; set; }

        /// <summary>
        /// Строка в документе.
        /// </summary>
        public int LineNumber { get; set; }

        /// <summary>
        /// Позиция в строке.
        /// </summary>
        public int ColumnNumber { get; set; }

        /// <summary>
        /// Копия обьекта.
        /// </summary>
        /// <returns></returns>
        public CodeInformation Clone()
        {
            return (CodeInformation) this.MemberwiseClone();
        }
    }
}
