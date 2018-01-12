using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptFunctionContext
    {
        private ScriptSimpleContext _function_context;
        private Stack<(int, ScriptSimpleContext)> _history;

        public ScriptSimpleContext Context { get => _function_context; }

        public ScriptFunctionContext()
        {
            _history = new Stack<(int, ScriptSimpleContext)>();
        }

        // Создать новый контекст функции.
        public void CreateFunctionContext(IFunction function, int position)
        {
            // Сохранить контекст функции.
            _history.Push((position, _function_context));

            // Установить новый контекст функции.
            _function_context = new ScriptSimpleContext(function.Name,function.Scope.VarCount);
        }

        /// <summary>
        /// Восстановить предыдущий контекст функции.
        /// </summary>
        public int RestoreFunctionContext()
        {
            if (_history.Count > 0)
            {
                (int, ScriptSimpleContext) history_data;
                history_data = _history.Pop();
                _function_context = history_data.Item2;
                return history_data.Item1;
            }
            return int.MaxValue;
        }
    }
}
