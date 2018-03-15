using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class StaticVariables
    {
        private IDictionary<IValue, IVariable> _static_vars;

        public StaticVariables()
        {
            _static_vars = new Dictionary<IValue, IVariable>(new IValueComparer());
        }


        /// <summary>
        /// Добавить статическую переменную. Если переменная с таким значением существует, то вернуть увеличить счетчик ее использования.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IVariable Create(IValue value)
        {
            IVariable tmp_var;
            tmp_var = Exist(value);
            if (tmp_var != null)
            {
                tmp_var.Users++;
                return tmp_var;
            }

            tmp_var = new Variable();
            tmp_var.Name = "<<static>>";
            tmp_var.Type = VariableTypeEnum.CONSTANTVARIABLE;
            tmp_var.Value = value;

            _static_vars.Add(value, tmp_var);

            return tmp_var;
        }

        /// <summary>
        /// Проверка существования статической переменной.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public IVariable Exist(IValue value)
        {
            if (_static_vars.TryGetValue(value, out IVariable out_value))
                return out_value;
            else
                return null;
        }

        /// <summary>
        /// Удалить статическую переменную.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="scope"></param>
        public void Delete(IVariable variable)
        {
            IVariable tmp_var;

            if (Exist(variable.Value) != null)
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
