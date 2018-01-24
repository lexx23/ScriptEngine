using ScriptEngine.EngineBase.Compiler.Programm;
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
        private IValue [] _vars;
        private int _count;

        public int Count { get => _count; }
        public string Name { get; set; }


        public ScriptSimpleContext(string name, int size)
        {
            Name = name;
            _count = size+1;
            _vars = new IValue[_count];

            while (size >= 0)
            {
                _vars[size] = new Value();
                size--;
            }
        }

        /// <summary>
        /// Очистить значение.
        /// </summary>
        /// <param name="index"></param>
        public void ClearValue(int index)
        {
           _vars[index] = new Value();
        }

        public void CopyValue(int index, IValue value)
        {
            _vars[index] = value;
        }


        /// <summary>
        /// Присвоить переменной контекста значение, если переменной нет в контексте, то добавить.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetValue(int index, IValue value)
        {
            _vars[index].SetValue(value);
        }

        //public void SetValue(int index, bool value)
        //{
        //    _vars[index].SetValue(value);
        //}

        /// <summary>
        /// Получить значение из контекста выполнения.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public IValue GetValue(int index)
        {
            return _vars[index];
        }

    }
}
