using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class StaticVariables
    {
        private IDictionary<Value, IVariable> _static_vars;

        public StaticVariables()
        {
            _static_vars = new Dictionary<Value, IVariable>();
        }


        /// <summary>
        /// Добавить статическую переменную. Если переменная с таким значением существует, то вернуть увеличить счетчик ее использования.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IVariable Add(Value value)
        {
            IVariable tmp_var;
            if (Exist(value))
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
        public bool Exist(Value value)
        {
            return _static_vars.ContainsKey(value);

        }

        /// <summary>
        /// Удалить статическую переменную.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="scope"></param>
        public void Delete(IVariable variable)
        {
            IVariable tmp_var;

            if (Exist(variable.Value))
            {
                tmp_var = _static_vars[variable.Value];
                tmp_var.Users--;
                if (tmp_var.Users <= 0)
                    _static_vars.Remove(variable.Value);
            }

            return;
        }
    }
}
