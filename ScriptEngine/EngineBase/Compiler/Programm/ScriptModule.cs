using ScriptEngine.EngineBase.Compiler.Types;
using System;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Programm
{
    public class ScriptModule
    {
        private IDictionary<string, Variable> _vars;
        private IDictionary<string, Function> _functions;
        private IList<Function> _object_functions_call;
        private IList<ScriptStatement> _code;
        private ScriptScope _module_scope;

        public string Name { get; set; }
        public string FileName { get; set; }
        public bool AsGlobal { get; set; }
        public ModuleTypeEnum Type { get; set; }
        public IList<ScriptStatement> Code { get => _code; }
        public int FunctionsCount { get => _functions.Count; }
        public ScriptScope ModuleScope { get => _module_scope; }

        /// <summary>
        /// Номер линии программы.
        /// </summary>
        public int ProgrammLine
        {
            get => _code.Count;
        }

        public ScriptModule(string name, ModuleTypeEnum type, bool compile_as_global = true)
        {
            Name = name;
            Type = type;
            AsGlobal = compile_as_global;
            _module_scope = new ScriptScope() { Type = ScopeTypeEnum.MODULE, Name = name, Module = this };

            _vars = new Dictionary<string, Variable>();
            _functions = new Dictionary<string, Function>();
            _object_functions_call = new List<Function>();

            _code = new List<ScriptStatement>();
        }


        public int ObjectCallAdd(Function function)
        {
            _object_functions_call.Add(function);
            return _object_functions_call.Count -1;
        }


        public Function ObjectCallGet(int index)
        {
            return _object_functions_call[index];
        }

        /// <summary>
        /// Добавить функцию в модуль.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module_name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public Function FunctionAdd(string name, bool as_public = false, ScriptScope scope = null)
        {
            Function function;

            if (scope == null)
                scope = _module_scope;

            if (!_functions.ContainsKey(name + "-" + scope.Name))
            {
                function = new Function() { Name = name, Scope = scope, Type = FunctionTypeEnum.PROCEDURE, Public = as_public };
                _functions.Add(name + "-" + scope.Name, function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Получить функцию, по имени, из модуля. Если такое имя есть, тогда вернуть null. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public Function FunctionGet(string name, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module_scope;

            if (_functions.ContainsKey(name + "-" + scope.Name))
                return _functions[name + "-" + scope.Name];

            return null;
        }


        /// <summary>
        /// Добавить инструкцию.
        /// </summary>
        /// <returns></returns>
        public ScriptStatement StatementAdd()
        {
            ScriptStatement statement = new ScriptStatement();
            _code.Add(statement);
            return statement;
        }

        /// <summary>
        /// Получть инструкцию.
        /// </summary>
        public ScriptStatement StatementGet(int index)
        {
            return _code[index];
        }


        /// <summary>
        /// Удалить переменную из модуля.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="scope"></param>
        public void VariableDelete(Variable variable, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module_scope;

            if (_vars.ContainsKey(variable.Name + "-" + scope.Name))
            {
                _vars.Remove(variable.Name + "-" + scope.Name);
                return;
            }

            if (_vars.ContainsKey(variable.Name + "-public-" + scope.Name))
                _vars.Remove(variable.Name + "-public-" + scope.Name);
        }

        /// <summary>
        /// Попытка переиспользовать переменную.
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private Variable VariableReUse(ScriptScope scope)
        {
            foreach (KeyValuePair<string, Variable> var in _vars)
            {
                if (var.Key[0] == '<' && var.Key[1] == '<')
                {
                    if (var.Value.Users <= 1)
                        continue;

                    if (var.Value.Scope != scope)
                        continue;

                    var.Value.Users = 1;
                    ScriptStatement statement = StatementAdd();
                    statement.OP_CODE = Interpreter.OP_CODES.OP_VAR_CLR;
                    statement.Variable2 = var.Value;

                    scope.VarCount++;
                    return var.Value;
                }
            }

            return null;
        }

        /// <summary>
        /// Добавить переменную в модуль. Если такая переменная уже существует вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Variable VariableAdd(string name, bool as_public, ScriptScope scope, VariableValue value = null)
        {
            Variable var;

            if (scope == null)
                scope = _module_scope;

            if (name == string.Empty)
            {
                name = "<<var_" + _vars.Count.ToString() + ">>";
                var = VariableReUse(scope);
                if (var != null)
                    return var;
            }
            else
            {
                if (as_public)
                    scope = _module_scope;

                if (_vars.ContainsKey(name + "-" + scope.Name))
                    return null;
            }

            var = new Variable()
            {
                Name = name,
                Scope = scope,
                Value = value,
                Public = as_public,
                Status = VariableStatusEnum.STACKVARIABLE,
                Users = 1,
                StackNumber = scope.VarCount
            };

            scope.VarCount++;

            _vars.Add(name + "-" + scope.Name, var);
            return var;
        }

        /// <summary>
        /// Получить переменную из модуля.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="scope"></param>
        /// <returns></returns>
        public Variable VariableGet(string name, ScriptScope scope = null)
        {
            if (scope == null)
                scope = _module_scope;

            if (_vars.ContainsKey(name + "-" + scope.Name))
                return _vars[name + "-" + scope.Name];

            if (_vars.ContainsKey(name + "-public-" + _module_scope.Name))
                return _vars[name + "-public-" + _module_scope.Name];

            return null;
        }

        public Variable VariableGet(string name, string scope_name)
        {
            if (scope_name == null)
                scope_name = _module_scope.Name;

            if (_vars.ContainsKey(name + "-" + scope_name))
                return _vars[name + "-" + scope_name];

            if (_vars.ContainsKey(name + "-public-" + _module_scope.Name))
                return _vars[name + "-public-" + _module_scope.Name];

            return null;
        }


    }
}
