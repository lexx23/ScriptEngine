/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

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
