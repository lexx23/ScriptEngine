using ScriptEngine.EngineBase.Compiler.Types;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Compiler.Programm
{
    /// <summary>
    /// Программа. Содержит в себе модули программы, статические переменные, а так же глобальный модуль.
    /// </summary>
    public class ScriptProgramm
    {
        private IDictionary<string, ScriptModule> _modules;
        private IDictionary<string, Variable> _global_vars;
        private IDictionary<string, Function> _global_functions;
        private IDictionary<VariableValue, Variable> _static_vars;
        private ScriptScope _global_scope;


        public IDictionary<string, ScriptModule> Modules { get => _modules; }

        public ScriptScope GlobalScope { get => _global_scope; }
        public ScriptModule this[string name]
        {
            get => _modules[name];
        }


        public ScriptProgramm()
        {
            _modules = new Dictionary<string, ScriptModule>();
            _global_scope = new ScriptScope() { Name = "global", Type = ScopeTypeEnum.GLOBAL };

            _static_vars = new Dictionary<VariableValue, Variable>();

            _global_vars = new Dictionary<string, Variable>();
            _global_functions = new Dictionary<string, Function>();

        }

        /// <summary>
        /// Добавить модуль в программу.
        /// </summary>
        /// <param name="module"></param>
        public void ModuleAdd(ScriptModule module)
        {
            _modules.Add(module.Name, module);

            if (!module.AsGlobal && module.Type == ModuleTypeEnum.COMMON)
                GlobalVariableAdd(module.Name, new VariableValue(ValueTypeEnum.OBJECT, module.Name));

            if (module.Type == ModuleTypeEnum.OBJECT)
                GlobalVariableAdd(module.Name, new VariableValue(ValueTypeEnum.OBJECT, module.Name));
        }

        /// <summary>
        /// Проверка существования модуля.
        /// </summary>
        /// <param name="module_name"></param>
        /// <returns></returns>
        public bool ModuleExist(string module_name)
        {
            return _modules.ContainsKey(module_name);
        }

        #region GlobalFunctions
        /// <summary>
        /// Добавление глобальной функции. Если такая функция существует, то вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="module"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public Function GlobalFunctionAdd(string name)
        {
            Function function;

            if (!_global_functions.ContainsKey(name + "-" + _global_scope.Name))
            {
                function = new Function() { Name = name };
                _global_functions.Add(name + "-" + _global_scope.Name, function);
                return function;
            }

            return null;
        }

        /// <summary>
        /// Получить глобальную функцию.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Function GlobalFunctionGet(string name)
        {
            if (_global_functions.ContainsKey(name + "-" + _global_scope.Name))
                return _global_functions[name + "-" + _global_scope.Name];

            return null;
        }
        #endregion

        #region GlobalVariable
        /// <summary>
        /// Добавить глобальную переменную. Если переменная с таким именем существует, то вернуть null.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public Variable GlobalVariableAdd(string name, VariableValue value = null)
        {
            if (name == string.Empty)
                name = "<<var_" + _global_vars.Count.ToString() + ">>";

            if (_global_vars.ContainsKey(name + "-" + _global_scope.Name))
                return null;

            Variable var = new Variable()
            {
                Name = name,
                Value = value,
                Status = VariableStatusEnum.STACKVARIABLE,
                Users = 1,
                Scope = _global_scope,
                StackNumber = _global_scope.VarCount
            };

            _global_scope.VarCount++;
            _global_vars.Add(name + "-" + _global_scope.Name, var);
            return var;
        }


        /// <summary>
        /// Получить глобальную переменную.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Variable GlobalVariableGet(string name)
        {
            if (_global_vars.ContainsKey(name + "-" + _global_scope.Name))
                return _global_vars[name + "-" + _global_scope.Name];
            return null;
        }
        #endregion

        #region StaticVariable

        /// <summary>
        /// Добавить статическую переменную. Если переменная с таким значением существует, то вернуть увеличить счетчик ее использования.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Variable StaticVariableAdd(VariableValue value)
        {
            Variable tmp_var;
            if (StaticVariableExist(value))
            {
                tmp_var = _static_vars[value];
                tmp_var.Users++;
                return tmp_var;
            }

            tmp_var = new Variable();
            tmp_var.Name = "<<static>>";
            tmp_var.Status = VariableStatusEnum.CONSTANTVARIABLE;
            tmp_var.Value = value;

            _static_vars.Add(value, tmp_var);

            return tmp_var;
        }

        /// <summary>
        /// Проверка существования статической переменной.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool StaticVariableExist(VariableValue value)
        {
            return _static_vars.ContainsKey(value);

        }

        /// <summary>
        /// Удалить статическую переменную.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="scope"></param>
        public void StaticVariableDelete(Variable variable, Variable scope = null)
        {
            Variable tmp_var;

            if (StaticVariableExist(variable.Value))
            {
                tmp_var = _static_vars[variable.Value];
                tmp_var.Users--;
                if (tmp_var.Users <= 0)
                    _static_vars.Remove(variable.Value);
            }

            return;
        }
        #endregion
    }


}
