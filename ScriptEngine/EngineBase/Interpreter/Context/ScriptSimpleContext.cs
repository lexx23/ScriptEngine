using ScriptEngine.EngineBase.Compiler.Types;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptSimpleContext
    {
        private IList<VariableValue> _vars;

        public int Count { get => _vars.Count; }
        public string Name { get; set; }


        public ScriptSimpleContext(string name,int size)
        {
            Name = name;
            _vars = new List<VariableValue>();

            while (size > 0)
            {
                _vars.Add(null);
                size--;
            }
        }

        public void ClearValue(Variable variable)
        {
             _vars[variable.StackNumber] = new VariableValue(ValueTypeEnum.NULL, "");
        }


        /// <summary>
        /// Присвоить переменной контекста значение, если переменной нет в контексте, то добавить.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetValue(Variable variable, VariableValue value)
        {
            if(_vars[variable.StackNumber] != null)
                _vars[variable.StackNumber].SetValue(value);
            else
                _vars[variable.StackNumber] = value;
        }

        /// <summary>
        /// Получить значение из контекста выполнения.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public VariableValue GetValue(Variable variable)
        {
            return _vars[variable.StackNumber];
        }

    }
}
