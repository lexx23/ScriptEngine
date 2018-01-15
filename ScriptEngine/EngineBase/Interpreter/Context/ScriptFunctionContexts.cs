using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptFunctionContexts
    {
        private ScriptProgrammContext _main_context;
        private IList<FunctionHistoryData> _history;

        public ScriptFunctionContexts(ScriptProgrammContext main_context)
        {
            _main_context = main_context;
            _history = new List<FunctionHistoryData>();
        }

        // Создать новый контекст функции.
        public void CreateFunctionContext(IFunction function, int position)
        {
            // Сохранить контекст функции.
            _history.Add(new FunctionHistoryData()
            {
                Position = position,
                Module = _main_context.CurrentModule,
                PreviousFunction = _main_context._current_function,
                CurrentFunction = function,
                Context = _main_context._contexts[2]
            });

            // Установить новый контекст функции.
            _main_context._contexts[2] = new ScriptSimpleContext(function.Name, function.Scope.VarCount);
            _main_context._current_function = function;
        }

        /// <summary>
        /// Установить параметры с коромыми функция была вызвана.
        /// </summary>
        /// <param name="function_params"></param>
        public void SetFunctionParams(IList<string> function_params)
        {
            _history[_history.Count - 1].FunctionParams = function_params;
        }

        /// <summary>
        /// Список вызовов функций.
        /// </summary>
        /// <returns></returns>
        public IList<FunctionHistoryData> StackCall()
        {
            return _history;
        }

        /// <summary>
        /// Восстановить предыдущий контекст функции.
        /// </summary>
        public int RestoreFunctionContext()
        {
            if (_history.Count > 0)
            {
                FunctionHistoryData history_data;
                history_data = _history[_history.Count - 1];
                _history.RemoveAt(_history.Count - 1);
                _main_context._contexts[2] = history_data.Context;
                _main_context._current_function = history_data.PreviousFunction;
                return history_data.Position;
            }
            return int.MaxValue;
        }
    }
}
