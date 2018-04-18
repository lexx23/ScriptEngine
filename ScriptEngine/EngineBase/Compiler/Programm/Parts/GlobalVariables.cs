/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class GlobalVariables
    {
        private ScriptScope _global_scope;
        private IList<IVariable> _vars;


        public GlobalVariables(ScriptScope scope)
        {
            _global_scope = scope;

            _vars = new List<IVariable>();
        }

        /// <summary>
        /// Добавить переменную в глобальный контекст.
        /// </summary>
        /// <param name="variable"></param>
        public void Add(IVariable variable)
        {
            if (Get(variable.Name) != null)
                return;

            _global_scope.Vars.Add(variable);
            _vars.Add(variable);
        }


        /// <summary>
        /// Создать глобальную переменную. Если переменная с таким именем существует, то вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IVariable Create(string name, IValue value = null)
        {
            if (name == string.Empty)
                name = "<<var_" + _vars.Count.ToString() + ">>";

            if (Get(name) != null)
                return null;

            IVariable var = new Variable()
            {
                Name = name,
                Value = value,
                Type = VariableTypeEnum.STACKVARIABLE,
                Users = 1,
                Scope = _global_scope,
            };

            _global_scope.Vars.Add(var);
            _vars.Add(var);
            return var;
        }


        /// <summary>
        /// Получить глобальную переменную.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IVariable Get(string name)
        {
            for (int i = 0; i < _vars.Count; i++)
            {
                if (String.Equals(_vars[i].Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_vars[i].Alias,name, StringComparison.OrdinalIgnoreCase))
                    return _vars[i];
            }

            return null;
        }
    }
}
