using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptSimpleContext
    {
        private IList<Value> _vars;

        public int Count { get => _vars.Count; }
        public string Name { get; set; }


        public ScriptSimpleContext(string name, int size)
        {
            Name = name;
            _vars = new List<Value>();

            while (size > 0)
            {
                _vars.Add(new Value(ValueTypeEnum.NULL, ""));
                size--;
            }
        }

        /// <summary>
        /// Очистить значение.
        /// </summary>
        /// <param name="index"></param>
        public void ClearValue(int index)
        {
            _vars[index] = new Value(ValueTypeEnum.NULL, "");
        }

        public void CopyValue(int index, Value value)
        {
            _vars[index] = value;
        }


        /// <summary>
        /// Присвоить переменной контекста значение, если переменной нет в контексте, то добавить.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetValue(int index, Value value)
        {
            _vars[index].SetValue(value);
        }

        /// <summary>
        /// Получить значение из контекста выполнения.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Value GetValue(int index)
        {
            return _vars[index];
        }

    }
}
