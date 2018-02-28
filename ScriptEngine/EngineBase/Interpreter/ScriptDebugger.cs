using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptDebugger
    {
        private ScriptInterpreter _interpreter;
        private IDictionary<string, IList<int>> _break_points;
        private string _current_break_point;
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
            _break_points = new Dictionary<string, IList<int>>();
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
                _break_points[module_name] = new List<int>();

            if (_break_points[module_name].IndexOf(line) == -1)
                _break_points[module_name].Add(line);
        }

        /// <summary>
        /// Удалить точку останова.
        /// </summary>
        /// <param name="module_name"></param>
        /// <param name="line"></param>
        public void RemoveBreakpoint(string module_name, int line)
        {
            int line_index;
            if (!_break_points.ContainsKey(module_name))
            {
                line_index = _break_points[module_name].IndexOf(line);
                if (line_index != -1)
                    _break_points[module_name].RemoveAt(line_index);
            }

        }

        /// <summary>
        /// Шаг с заходом в функции и процедуры.
        /// </summary>
        public void StepInto()
        {
            _step_break_point = (int)DEBUG_COMMANDS.STEP_INTO_START;
            _interpreter.Execute();
        }

        /// <summary>
        /// Шаг без захода в функции и процедуры.
        /// </summary>
        public void StepOver()
        {
            GetNextStep();
            _interpreter.Execute();
        }

        /// <summary>
        /// Определить следующий шаг интерпритатора. Используется в stepover
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
            _interpreter.Execute();
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

            if (object_value != null && object_value.AsScriptObject() != null)// && object_value.AsScriptObject().Context != null)
                return object_value.AsScriptObject().GetReference(var_name).Get();

            return null;
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


        internal bool OnExecute(ScriptStatement statement)
        {
            if (Debug)
            {
                if (_interpreter.CurrentLine != -1)
                {
                    if (_break_points.Count > 0)
                    {
                        if (_break_points.ContainsKey(_interpreter.CurrentModule.Name) && _break_points[_interpreter.CurrentModule.Name].Contains(_interpreter.CurrentLine))
                        {
                            if (_current_break_point != _interpreter.CurrentModule.Name + "_" + _interpreter.CurrentLine)
                            {
                                _current_break_point = _interpreter.CurrentModule.Name + "_" + _interpreter.CurrentLine;
                                _on_break_delegate?.Invoke(_interpreter);
                                return true;
                            }
                        }
                    }

                    if (_step_break_point == (int)DEBUG_COMMANDS.STEP_INTO_BREAK || _step_break_point == _interpreter.CurrentLine)
                    {
                        _on_break_delegate?.Invoke(_interpreter);
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
