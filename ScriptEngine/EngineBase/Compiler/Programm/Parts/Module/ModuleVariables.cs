using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    public class ModuleVariables
    {
        private IDictionary<string, IVariable> _vars;
        private ScriptModule _module;

        public ModuleVariables(ScriptModule module)
        {
            _module = module;
            _vars = new Dictionary<string, IVariable>();
        }

        /// <summary>
        /// Удалить переменную из модуля.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="scope"></param>
        public void Delete(IVariable variable, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module.ModuleScope;

            if (_vars.ContainsKey(variable.Name + "-" + scope.Name))
            {
                _vars.Remove(variable.Name + "-" + scope.Name);
                return;
            }

            if (_vars.ContainsKey(variable.Name + "-public-" + scope.Name))
                _vars.Remove(variable.Name + "-public-" + scope.Name);
        }

        // TODO Перенести в глобальный модуль.
        /// <summary>
        /// Попытка переиспользовать переменную.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private IVariable ReUse(ScriptScope scope)
        {
            foreach (KeyValuePair<string, IVariable> var in _vars)
            {
                if (var.Key[0] == '<' && var.Key[1] == '<')
                {
                    if (var.Value.Users <= 1)
                        continue;

                    if (var.Value.Scope != scope)
                        continue;

                    var.Value.Users = 1;
                    if (var.Value.Type == VariableTypeEnum.REFERENCE)
                    {
                        ScriptStatement statement = _module.StatementAdd();
                        statement.OP_CODE = Interpreter.OP_CODES.OP_VAR_CLR;
                        statement.Variable2 = var.Value;
                    }

                    if (!scope.Vars.Contains(var.Value))
                    {
                        scope.Vars.Add(var.Value);
                        scope.VarCount++;
                    }
                    return var.Value;
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
                if (_vars.ContainsKey(name + "-" + scope.Name))
                return null;


            var = new Variable()
            {
                Name = name,
                Scope = scope,
                Value = value,
                Public = as_public,
                Type = VariableTypeEnum.STACKVARIABLE,
                Users = 1,
                StackNumber = scope.VarCount
            };

            scope.Vars.Add(var);
            scope.VarCount++;

            _vars.Add(name + "-" + scope.Name, var);
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

            if (_vars.ContainsKey(name + "-" + variable.Scope.Name))
                return false;

            _vars.Add(name + "-" + variable.Scope.Name, variable);
            _module.ModuleScope.Vars.Add(variable);
            _module.ModuleScope.VarCount++;
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

            if (_vars.ContainsKey(name + "-" + scope.Name))
                return _vars[name + "-" + scope.Name];

            if (_vars.ContainsKey(name + "-public-" + _module.ModuleScope.Name))
                return _vars[name + "-public-" + _module.ModuleScope.Name];

            return null;
        }

        public IVariable Get(string name, string scope_name)
        {
            if (scope_name == null)
                scope_name = _module.ModuleScope.Name;

            if (_vars.ContainsKey(name + "-" + scope_name))
                return _vars[name + "-" + scope_name];

            if (_vars.ContainsKey(name + "-public-" + _module.ModuleScope.Name))
                return _vars[name + "-public-" + _module.ModuleScope.Name];

            return null;
        }
    }
}
