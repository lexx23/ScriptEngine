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
        private IDictionary<string, IVariable> _vars;


        public GlobalVariables(ScriptScope scope)
        {
            _global_scope = scope;

            _vars = new Dictionary<string, IVariable>();
        }

        /// <summary>
        /// Добавить переменную в глобальный контекст.
        /// </summary>
        /// <param name="variable"></param>
        public void Add(IVariable variable)
        {
            if (_vars.ContainsKey(variable.Name))
                return;

            _global_scope.Vars.Add(variable);
            _global_scope.VarCount++;

            _vars.Add(variable.Name, variable);
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

            if (_vars.ContainsKey(name))
                return null;

            IVariable var = new Variable()
            {
                Name = name,
                Value = value,
                Type = VariableTypeEnum.STACKVARIABLE,
                Users = 1,
                Scope = _global_scope,
                StackNumber = _global_scope.VarCount
            };

            _global_scope.Vars.Add(var);
            _global_scope.VarCount++;

            _vars.Add(name, var);
            return var;
        }


        /// <summary>
        /// Получить глобальную переменную.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IVariable Get(string name)
        {
            if (_vars.ContainsKey(name))
                return _vars[name];
            return null;
        }
    }
}
