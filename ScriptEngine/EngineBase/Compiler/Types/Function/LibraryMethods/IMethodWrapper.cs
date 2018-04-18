/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    /// <summary>
    /// Обертка для вызова внешних процедур/функций.
    /// </summary>
    public interface IMethodWrapper
    {

        /// <summary>
        /// Выполнить функцию/процедуру.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IValue Run(IVariable[] param);

        /// <summary>
        /// Копия функции с указанием нового объекта.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        IMethodWrapper Clone(object instance);
    }
}
