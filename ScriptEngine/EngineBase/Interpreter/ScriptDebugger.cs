/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptDebugger
    {
        private ScriptInterpreter _interpreter;
        private IDictionary<string, IDictionary<int, OnBreakHandler>> _break_points;
        private string _current_break_point;
        private string _eval_break_point;
        private int _step_break_point;

        public bool Debug { get; set; }

        private OnBreakHandler _on_break_delegate;
        public OnBreakHandler OnBreak { get => _on_break_delegate; set => _on_break_delegate = value; }
        public delegate void OnBreakHandler(ScriptInterpreter interpreter);


        private enum DEBUG_COMMANDS
        {
            STEP_INTO_START = -1,
            STEP_INTO_BREAK = -2,
            STEP_OVER_START = -3
        }


        public ScriptDebugger(ScriptInterpreter interpreter)
        {
            Debug = false;
            _interpreter = interpreter;
            _step_break_point = int.MaxValue;
            _eval_break_point = string.Empty;
            _break_points = new Dictionary<string, IDictionary<int, OnBreakHandler>>();
        }

        /// <summary>
        /// Добавить точку останова.
        /// </summary>
        /// <param name="module_name"></param>
        /// <param name="line"></param>
        public void AddBreakpoint(string module_name, int line)
        {
            if (!_interpreter.Programm.Modules.Exist(module_name))
                throw new Exception($"В программе нет модуля с именем [{module_name}].");

            if (!_break_points.ContainsKey(module_name))
                _break_points[module_name] = new Dictionary<int, OnBreakHandler>();

            if (!_break_points[module_name].ContainsKey(line))
                _break_points[module_name].Add(line, null);
        }

        /// <summary>
        /// Добавить событие для конкретного модуля и строки.
        /// </summary>
        /// <param name="module_name"></param>
        /// <param name="line"></param>
        public void AddBreakpoint(string module_name, int line, OnBreakHandler handler)
        {
            if (!_interpreter.Programm.Modules.Exist(module_name))
                throw new Exception($"В программе нет модуля с именем [{module_name}].");

            if (!_break_points.ContainsKey(module_name))
                _break_points[module_name] = new Dictionary<int, OnBreakHandler>();

            if (_break_points[module_name].ContainsKey(line))
                _break_points[module_name][line] = handler;
            else
                _break_points[module_name].Add(line, handler);
        }

        /// <summary>
        /// Удалить точку останова.
        /// </summary>
        /// <param name="module_name"></param>
        /// <param name="line"></param>
        public void RemoveBreakpoint(string module_name, int line)
        {
            if (_break_points.ContainsKey(module_name))
            {
                if (_break_points[module_name].ContainsKey(line))
                    _break_points[module_name].Remove(line);
            }

        }

        /// <summary>
        /// Шаг с заходом в функции и процедуры.
        /// </summary>
        public void StepInto()
        {
            _step_break_point = (int)DEBUG_COMMANDS.STEP_INTO_START;
            //_interpreter.Execute();
        }

        /// <summary>
        /// Шаг без захода в функции и процедуры.
        /// </summary>
        public void StepOver()
        {
            GetNextStep();
            //_interpreter.Execute();
        }

        /// <summary>
        /// Определить следующий шаг интерпретатора. Используется в stepover
        /// </summary>
        private void GetNextStep()
        {
            if (_interpreter.IstructionIndex >= _interpreter.CurrentModule.Code.Count)
                return;

            _step_break_point = int.MinValue;
            int i = _interpreter.IstructionIndex;

            if (_interpreter.CurrentModule.Code[i].OP_CODE == OP_CODES.OP_RETURN)
            {
                _step_break_point = (int)DEBUG_COMMANDS.STEP_OVER_START;
                return;
            }

            while (i < _interpreter.CurrentModule.Code.Count - 1)
            {
                i++;
                if (_interpreter.CurrentModule.Code[i].Line != -1)
                {
                    _step_break_point = _interpreter.CurrentModule.Code[i].Line;
                    if (_step_break_point == _interpreter.CurrentLine)
                        _step_break_point = int.MaxValue;
                    else
                        return;
                }
            }
        }

        /// <summary>
        /// Продолжить выполнение.
        /// </summary>
        public void Continue()
        {
            _step_break_point = int.MaxValue;
        }

        /// <summary>
        /// Получить значение переменной.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue RegisterGetValue(string name)
        {
            IVariable var = null;

            var = _interpreter.CurrentModule.Variables.Get(name, _interpreter.CurrentModule.ModuleScope);

            if (var == null)
                var = _interpreter.CurrentModule.Variables.Get(name, _interpreter.CurrentFunction.Scope);

            if (var == null)
                var = _interpreter.Programm.GlobalVariables.Get(name);

            if (var == null)
                throw new Exception($"Невозможно найти переменную с именем [{name}].");
            if (var.Value == null)
                throw new Exception($"Переменной [{name}] не присвоено значение.");

            return var.Value;
        }

        /// <summary>
        /// Получить значение переменной обьекта. Используется при отладке.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public IValue ObjectGetValue(string object_name, string var_name)
        {
            IValue object_value = null;

            object_value = RegisterGetValue(object_name);

            if (object_value != null && object_value.AsScriptObject() != null)
                return object_value.AsScriptObject().GetAnyVaribale(var_name).Get();

            return null;
        }

        /// <summary>
        /// Рассчитать выражение.
        /// </summary>
        /// <param name="expression"></param>
        /// <returns></returns>
        public IValue Eval(string expression)
        {
            if (_interpreter.CurrentLine != -1 && _interpreter.CurrentFunction != null)
            {
                IVariable result = Variable.Create(ValueFactory.Create("Не удалось рассчитать значение."));
                try
                {
                    _eval_break_point = _interpreter.CurrentModule.Name + "_" + _interpreter.IstructionIndex;
                    _interpreter.Eval(expression, result);
                }
                catch (Exception ex)
                {
                    _eval_break_point = string.Empty;
                    return ValueFactory.Create(ex.Message);
                }

                _interpreter.Execute();
                return result.Value;
            }
            else
                return ValueFactory.Create("Достигнут конец кода.");
        }

        /// <summary>
        /// Определяет выход в случае вычисления выражения в дебагере.
        /// </summary>
        /// <returns></returns>
        internal bool OnEvalExit()
        {
            if (_eval_break_point == _interpreter.CurrentModule.Name + "_" + _interpreter.IstructionIndex)
            {
                _eval_break_point = string.Empty;
                return true;
            }
            return false;
        }


        internal void OnFunctionCall()
        {
            if (Debug && _step_break_point == (int)DEBUG_COMMANDS.STEP_INTO_START)
                _step_break_point = (int)DEBUG_COMMANDS.STEP_INTO_BREAK;
        }

        internal void OnFunctionReturn()
        {
            if (Debug && _step_break_point == (int)DEBUG_COMMANDS.STEP_OVER_START)
                GetNextStep();
        }

        private void ExecuteEvent(string module, int line)
        {
            if (_break_points.ContainsKey(module) && _break_points[module].ContainsKey(line))
                _break_points[_interpreter.CurrentModule.Name][line]?.Invoke(_interpreter);
            _on_break_delegate?.Invoke(_interpreter);
        }


        internal void OnExecute()
        {
            if (Debug)
            {
                if (_interpreter.CurrentLine != -1)
                {
                    if (_current_break_point == _interpreter.CurrentModule.Name + "_" + _interpreter.CurrentLine)
                        return;

                    if (_break_points.Count > 0)
                    {
                        if (_break_points.ContainsKey(_interpreter.CurrentModule.Name) && _break_points[_interpreter.CurrentModule.Name].ContainsKey(_interpreter.CurrentLine))
                        {
                            _current_break_point = _interpreter.CurrentModule.Name + "_" + _interpreter.CurrentLine;
                            ExecuteEvent(_interpreter.CurrentModule.Name, _interpreter.CurrentLine);
                            return;
                        }
                    }

                    if (_step_break_point == (int)DEBUG_COMMANDS.STEP_INTO_BREAK || _step_break_point == _interpreter.CurrentLine)
                        ExecuteEvent(_interpreter.CurrentModule.Name, _interpreter.CurrentLine);
                }
            }
        }
    }
}
