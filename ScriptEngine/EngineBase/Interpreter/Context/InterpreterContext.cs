﻿/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Programm;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    class InterpreterContext
    {
        private IFunction _function;
        private ScriptProgramm _programm;
        private IList<int> _catch_blocks;
        private IScriptObjectContext _current;
        private IVariableReference[] _current_function_context { get; set; }

        private Stack<(int, IScriptObjectContext, IVariableReference[], IFunction, IList<int>)> _history;


        public ScriptModule CurrentModule { get => _current?.Module; }
        public IScriptObjectContext Current { get => _current; }
        public IFunction CurrentFunction { get => _function; }


        public InterpreterContext(ScriptProgramm programm)
        {
            _catch_blocks = new List<int>();
            _programm = programm;
            _history = new Stack<(int, IScriptObjectContext, IVariableReference[], IFunction, IList<int>)>();
        }

        /// <summary>
        /// Очистка истории.
        /// </summary>
        public void Reset()
        {
            _history.Clear();
            _function = null;
            _current = null;
            _current_function_context = null;
        }

        /// <summary>
        /// Установить текущий контекст.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="function"></param>
        /// <param name="position"></param>
        public void Set(IScriptObjectContext context, IFunction function, int position)
        {
            IVariableReference[] _function_context = _current_function_context;

            _current_function_context = new IVariableReference[function.Scope.Vars.Count];

            for (int i = 0; i < function.Scope.Vars.Count; i++)
            {
                IVariableReference reference = new SimpleReference();
                function.Scope.Vars[i].Reference = reference;
                _current_function_context[i] = reference;
            }

            if (_current != context)
            {
                _history.Push((position, _current, _function_context, _function, _catch_blocks));
                if(!context.Module.AsGlobal)
                    context.Set();
                _current = context;
            }
            else
                _history.Push((position, null, _function_context, _function, _catch_blocks));

            _catch_blocks = new List<int>();
            _function = function;
        }

        /// <summary>
        /// Установить новую ссылку для переменной контекста.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="reference"></param>
        public void Update(IVariable variable, IVariableReference reference)
        {
            for (int i = 0; i < _function.Scope.Vars.Count; i++)
            {
                if (_function.Scope.Vars[i] == variable)
                {
                    _current_function_context[i] = reference;
                    break;
                }
            }
            variable.Reference = reference;
        }

        /// <summary>
        /// Установить предыдущий контекст. Используется в операторе return.
        /// </summary>
        /// <returns></returns>
        public int Restore()
        {
            (int, IScriptObjectContext, IVariableReference[], IFunction, IList<int>) data = _history.Pop();

            if (data.Item2 != null)
                _current = data.Item2;

            _current_function_context = data.Item3;
            _catch_blocks = data.Item5;
            _function = data.Item4;

            if (_function != null)
            {
                for (int i = 0; i < _current_function_context.Length; i++)
                    _function.Scope.Vars[i].Reference = _current_function_context[i];
            }

            return data.Item1;
        }

        /// <summary>
        /// Добавить номер инструкции Catch блока.
        /// </summary>
        /// <param name="instruction"></param>
        public void TryBlockAdd(int instruction)
        {
            _catch_blocks.Add(instruction);
        }

        /// <summary>
        /// Удалить блок Catch.
        /// </summary>
        public void TryBlockRemove()
        {
            if(_catch_blocks.Count > 0)
                _catch_blocks.RemoveAt(_catch_blocks.Count - 1);
        }

        /// <summary>
        /// Поиск обработчика исключений.
        /// </summary>
        /// <returns></returns>
        public int Exception()
        {
            if (_catch_blocks.Count != 0)
            {
                int instruction = _catch_blocks[_catch_blocks.Count - 1];
                return instruction;
            }
            else
            {
                if (_history.Count > 0)
                {
                    Restore();
                    return Exception();
                }
                else
                    return -1;
            }
        }
    }
}
