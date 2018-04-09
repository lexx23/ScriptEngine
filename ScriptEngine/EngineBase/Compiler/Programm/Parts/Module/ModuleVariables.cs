using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types;
using System.Collections.Generic;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ModuleVariables
    {
        private IList<IVariable> _vars;
        private ScriptModule _module;

        public ModuleVariables(ScriptModule module)
        {
            _module = module;
            _vars = new List<IVariable>();
        }

        /// <summary>
        /// Попытка переиспользовать переменную.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private IVariable ReUse(ScriptScope scope)
        {
            foreach (IVariable var in _vars)
            {
                if (var.Name[0] == '<' && var.Name[1] == '<')
                {
                    if (var.Users <= 1)
                        continue;

                    if (var.Scope != scope)
                        continue;

                    var.Users = 1;
                    if (var.Type == VariableTypeEnum.REFERENCE)
                    {
                        ScriptStatement statement = _module.StatementAdd();
                        statement.OP_CODE = Interpreter.OP_CODES.OP_VAR_CLR;
                        statement.Variable2 = var;
                    }

                    if (!scope.Vars.Contains(var))
                        scope.Vars.Add(var);

                    return var;
                }
            }

            return null;
        }

        /// <summary>
        /// Создать переменную в модуле. Если такая переменная уже существует вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public IVariable Create(string name, bool as_public, ScriptScope scope, IValue value = null)
        {
            IVariable var;

            if (scope == null || as_public)
                scope = _module.ModuleScope;

            if (name == string.Empty)
            {
                name = "<<var_" + _vars.Count.ToString() + ">>";
                var = ReUse(scope);
                if (var != null)
                    return var;
            }
            else
                if (Get(name,scope) != null)
                return null;


            var = new Variable()
            {
                Name = name,
                Scope = scope,
                Value = value,
                Public = as_public,
                Type = VariableTypeEnum.STACKVARIABLE,
                Users = 1
            };

            scope.Vars.Add(var);
            _vars.Add(var);
            return var;
        }


        /// <summary>
        /// Добавить переменную в модуль.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="variable"></param>
        /// <returns></returns>
        public bool Add(string name, IVariable variable)
        {

            if (variable.Scope == null)
                variable.Scope = _module.ModuleScope;

            if (Get(name,variable.Scope) != null)
                return false;

            variable.Name = variable.Name;
            variable.Alias = variable.Alias;

            _vars.Add(variable);
            _module.ModuleScope.Vars.Add(variable);
            return true;
        }

        /// <summary>
        /// Получить переменную из модуля.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public IVariable Get(string name, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module.ModuleScope;

            for (int i = 0; i < _vars.Count; i++)
            {
                if (String.Equals(_vars[i].Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_vars[i].Alias,name, StringComparison.OrdinalIgnoreCase))
                    if (_vars[i].Scope == scope)
                        return _vars[i];
            }

            return null;
        }

        /// <summary>
        /// Удаление переменной.
        /// </summary>
        public void Remove(IVariable variable)
        {
            _vars.Remove(variable);
        }
    }
}
