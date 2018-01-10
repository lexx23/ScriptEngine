using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptInterpreter
    {
        internal ScriptProgramm _programm;
        internal int _instruction;
        internal ScriptGlobalContext _context;

        private ScriptDebugger _debugger;
        private Queue<VariableValue> _stack;
        private int _current_line;

        public ScriptDebugger Debugger { get => _debugger; }
        public int CurrentLine { get => _current_line; }
        public ScriptModule CurrentModule { get => _context.ModuleContexts.Current.Module; }
        public string CurrentModuleName { get => _context.ModuleContexts.Current.Module.Name; }

        public ScriptInterpreter(ScriptProgramm programm)
        {
            _programm = programm;
            _instruction = int.MaxValue;

            _debugger = new ScriptDebugger(this);
            _stack = new Queue<VariableValue>();
            _context = new ScriptGlobalContext(_programm.GlobalScope.VarCount);
        }


        /// <summary>
        /// Запустить отладку.
        /// </summary>
        /// <param name="module_name"></param>
        public void Debug()
        {
            if (_instruction == int.MaxValue)
            {
                _debugger.Debug = true;
                Run();
            }
            else
                _debugger.Continue();
        }

        /// <summary>
        /// Запуск программы.
        /// </summary>
        /// <param name="module_name"></param>
        public void Run()
        {
            string startup_module = string.Empty;

            foreach (KeyValuePair<string, ScriptModule> module_kv in _programm.Modules)
            {
                if (module_kv.Value.Type == ModuleTypeEnum.COMMON && !module_kv.Value.AsGlobal)
                {
                    Variable object_var = _programm.GlobalVariableGet(module_kv.Key);
                    _context.Global.SetValue(object_var, CreateObject(module_kv.Key, module_kv.Value));
                }

                if (module_kv.Value.Type == ModuleTypeEnum.OBJECT)
                {
                    Variable object_var = _programm.GlobalVariableGet(module_kv.Key);
                    _context.Global.SetValue(object_var, CreateObject(module_kv.Key, module_kv.Value));
                }

                if (module_kv.Value.Type == ModuleTypeEnum.STARTUP)
                    startup_module = module_kv.Value.Name;
            }


            FunctionCall("<<entry_point>>", startup_module);
            Execute();
        }

        /// <summary>
        /// Инициализация обьектов по умолчанию.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private VariableValue CreateObject(string name, ScriptModule type)
        {
            ScriptModuleContext object_context = _context.ModuleContexts.CreateModuleContext(name, type);

            FunctionCall("<<entry_point>>", type.Name);

            VariableValue var = new VariableValue
            {
                Type = ValueTypeEnum.OBJECT,
                Object = new VariableValueObject(type, object_context)
            };

            Execute();

            _context.ModuleContexts.DeleteModuleContext(name);
            return var;
        }

        /// <summary>
        /// Вызов функции по ее имени. Поиск названия в программе.
        /// </summary>
        /// <param name="name"></param>
        public void FunctionCall(string name, string module_name)
        {
            Function function;
            bool change_context = false;

            if (module_name != string.Empty)
            {
                ScriptModule module;
                module = _programm[module_name];
                change_context = true;

                if (!_context.ModuleContexts.ExistModuleContext(module_name))
                    _context.ModuleContexts.CreateModuleContext(module_name, module);
                _context.ModuleContexts.SetModuleContext(module_name,_instruction);
            }

            function = _context.ModuleContexts.Current.Module.FunctionGet(name);

            if (function == null)
                function = _programm.GlobalFunctionGet(name);

            if (function == null)
                throw new Exception($"Функция [{name}] не найдена.");

            FunctionCall(function, change_context);
        }

        /// <summary>
        /// Вызов функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private Variable FunctionCall(Function function, bool change_context)
        {
            int tmp_instruction;

            tmp_instruction = _instruction;
            // сохранить позицию кода, и создать новый контекст функции
            if (change_context)
                tmp_instruction = _instruction * -1;

            if(_context.ModuleContexts.Current.Function != null)
                _context.ModuleContexts.Current.Function.CreateFunctionContext(function, tmp_instruction);
            _instruction = function.EntryPoint;

            // добавить из стека в текущий (новый) контекст, переменные функции
            VariableValue stack_param;
            if (function.Param != null)
                foreach (Variable function_param in function.Param)
                {
                    if (_stack.Count > 0)
                    {
                        stack_param = _stack.Dequeue();
                        if (stack_param?.Type != ValueTypeEnum.NULL)
                        {
                            if (function_param.Status == VariableStatusEnum.CONSTANTVARIABLE)
                                _context.ModuleContexts.Current.Function.Context.SetValue(function_param, stack_param.Clone());
                            else
                                _context.ModuleContexts.Current.Function.Context.SetValue(function_param, stack_param);
                        }
                        else
                            _context.ModuleContexts.Current.Function.Context.SetValue(function_param, new VariableValue(ValueTypeEnum.NULL, ""));
                    }
                    else
                        _context.ModuleContexts.Current.Function.Context.SetValue(function_param, function_param.Value?.Clone());
                }

            _debugger.OnFunctionCall();

            return null;
        }

        /// <summary>
        /// Выход из функции return
        /// </summary>
        public void FunctionReturn(Variable return_var)
        {
            VariableValue var = null;

            // забираем возвращаемое значение из текущего контекста
            if (return_var != null)
            {
                var = GetValue(return_var).Clone();
                _stack.Enqueue(var);
            }

            // установка предыдущего контекста, и предыдущей функции.
            int position = _context.ModuleContexts.Current.Function.RestoreFunctionContext();
            if (position < 0)
                _instruction = _context.ModuleContexts.RestoreModuleContext();
            else
                _instruction = position;

            _debugger.OnFunctionReturn();
        }

        /// <summary>
        /// Проверка параметров вызываемой функции, на соответствие прототипу.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="param"></param>
        /// <param name="module"></param>
        private Function CheckObjectFunctionCall(ScriptModule module, Function function)
        {
            Function work_function;

            work_function = module.FunctionGet(function.Name);

            if (work_function == null)
                throw new ExceptionBase(function.CodeInformation, $"Процедура или функция с именем [{function.Name}] не определена, у обьекта [{module.Name}].");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new ExceptionBase(function.CodeInformation, $"Обращение к процедуре [{function.Name}] как к функции.");

            function.EntryPoint = work_function.EntryPoint;

            if (function.Param.Count == work_function.Param.Count)
                return work_function;

            // Блок проверки параметров.
            if (function.Param.Count > work_function.Param.Count)
                throw new ExceptionBase(work_function.CodeInformation, $"Много фактических параметров [{work_function.Name}].");

            int i, param_count;
            param_count = i = function.Param.Count;

            while (i < work_function.Param.Count)
            {
                if (work_function.Param[i].Status == VariableStatusEnum.CONSTANTVARIABLE && work_function.Param[i].Value != null)
                    param_count++;
                i++;
            }

            if (param_count < work_function.Param.Count)
                throw new ExceptionBase(work_function.CodeInformation, $"Недостаточно фактических параметров [{work_function.Name}].");

            return work_function;
        }


        /// <summary>
        /// Вызов метода у обьекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectCall(ScriptStatement statement)
        {
            VariableValue object_call = GetValue(statement.Variable2);
            VariableValue function_index = GetValue(statement.Variable3);

            Function function = _context.ModuleContexts.Current.Module.ObjectCallGet(function_index.ToInt());

            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.Object == null)
                throw new ExceptionBase(statement.CodeInformation, $"Значение не является значением объектного типа [{function.Name}]");

            Function work_function = CheckObjectFunctionCall(object_call.Object.Type, function);

            if (!work_function.Public)
                throw new ExceptionBase(statement.CodeInformation, $"Функция [{function.Name}] не имеет ключевого слова Экспорт, и не доступна.");

            _context.ModuleContexts.SetModuleContext(object_call.Object.Context,_instruction);
            FunctionCall(work_function,true);
        }


        private void ObjectResoleVariable(ScriptStatement statement)
        {
            VariableValue var_name;
            var_name = GetValue(statement.Variable3);

            VariableValue object_call = GetValue(statement.Variable2);
            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.Object == null)
                throw new ExceptionBase(statement.CodeInformation, $"Значение не является значением объектного типа [{var_name.Content}]");

            Variable var = object_call.Object.Type.VariableGet(var_name.Content, object_call.Object.Type.ModuleScope);
            if(!var.Public)
                throw new ExceptionBase(statement.CodeInformation, $"[{var_name.Content}] не имеет ключевого слова Экспорт, и не доступна.");

            VariableValue value = object_call.Object.Context.Context.GetValue(var);
            SetValue(statement.Variable1,value);
        }

        /// <summary>
        /// Очистка значения, которе используется еще раз.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        private void ClearValue(Variable variable)
        {
            if (variable.Scope.Type == ScopeTypeEnum.MODULE)
            {
                _context.ModuleContexts.Current.Context.ClearValue(variable);
                return;
            }

            if (variable.Scope.Type == ScopeTypeEnum.FUNCTION || variable.Scope.Type == ScopeTypeEnum.PROCEDURE)
            {
                _context.ModuleContexts.Current.Function.Context.ClearValue(variable);
                return;
            }
        }


        /// <summary>
        /// Присвоить переменной контекста значение, если переменной нет в контексте, то добавить.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        private void SetValue(Variable variable, VariableValue value)
        {
            if (variable.Scope.Type == ScopeTypeEnum.GLOBAL)
            {
                _context.Global.SetValue(variable, value);
                return;
            }

            if (variable.Scope.Type == ScopeTypeEnum.MODULE)
            {
                _context.ModuleContexts.Current.Context.SetValue(variable, value);
                return;
            }

            if (variable.Scope.Type == ScopeTypeEnum.FUNCTION || variable.Scope.Type == ScopeTypeEnum.PROCEDURE)
            {
                _context.ModuleContexts.Current.Function.Context.SetValue(variable, value);
                return;
            }
        }

        /// <summary>
        /// Получить значение из контекста выполнения.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        private VariableValue GetValue(Variable variable)
        {
            if (variable.Status == VariableStatusEnum.CONSTANTVARIABLE)
                return variable.Value;

            // глобальный контекст
            if (variable.Scope.Type == ScopeTypeEnum.GLOBAL)
                return _context.Global.GetValue(variable);

            // контекст модуля
            if (variable.Scope.Type == ScopeTypeEnum.MODULE)
                return _context.ModuleContexts.Current.Context.GetValue(variable);

            // контекст функции
            if (variable.Scope.Type == ScopeTypeEnum.FUNCTION || variable.Scope.Type == ScopeTypeEnum.PROCEDURE)
                return _context.ModuleContexts.Current.Function.Context.GetValue(variable);

            return null;
        }

        /// <summary>
        /// Выполнить код модуля.
        /// </summary>
        internal void Execute()
        {
            ScriptStatement statement;
            VariableValue v2, v3,result;

            while (_instruction < _context.ModuleContexts.Current.Module.Code.Count)
            {
                if (_instruction == int.MaxValue || _instruction < 0)
                    return;

                statement = _context.ModuleContexts.Current.Module.Code[_instruction];
                _current_line = statement.CodeInformation != null ? statement.CodeInformation.LineNumber : int.MinValue;

                if (_debugger.OnExecute(statement))
                    return;

                switch (statement.OP_CODE)
                {
                    case OP_CODES.OP_PUSH:
                        v2 = GetValue(statement.Variable2);
                        _stack.Enqueue(v2);
                        break;
                    case OP_CODES.OP_POP:
                        if (_stack.Count > 0)
                            v2 = _stack.Dequeue();
                        else
                            v2 = new VariableValue(ValueTypeEnum.NULL, "");
                        SetValue(statement.Variable1, v2);
                        break;

                    case OP_CODES.OP_CALL:
                        string module_name = string.Empty;

                        v2 = GetValue(statement.Variable2);
                        if (statement.Variable3 != null)
                        {
                            v3 = GetValue(statement.Variable3);
                            module_name = v3.Content;
                        }
                        FunctionCall(v2.Content, module_name);
                        continue;

                    case OP_CODES.OP_RETURN:
                        FunctionReturn(statement.Variable2);
                        if (_context.ModuleContexts.Current == null)
                            return;
                        break;


                    case OP_CODES.OP_OBJECT_CALL:
                        ObjectCall(statement);
                        continue;
                    case OP_CODES.OP_OBJECT_RESOLVE_VAR:
                        ObjectResoleVariable(statement);
                        break;


                    case OP_CODES.OP_IFNOT:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        if (!v2.Boolean)
                        {
                            _instruction = v3.Integer;
                            continue;
                        }
                        break;
                    case OP_CODES.OP_JMP:
                        v2 = GetValue(statement.Variable2);
                        _instruction = v2.Integer;
                        continue;


                    case OP_CODES.OP_GT:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 > v3);
                        break;
                    case OP_CODES.OP_LT:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 < v3);
                        break;
                    case OP_CODES.OP_GE:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 >= v3);
                        break;
                    case OP_CODES.OP_LE:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 <= v3);
                        break;
                    case OP_CODES.OP_NOT:
                        v2 = GetValue(statement.Variable2);
                        result = new VariableValue();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = !v2.ToBoolean();
                        SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_OR:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        result = new VariableValue();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2.ToBoolean() || v3.ToBoolean();
                        SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_AND:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        result = new VariableValue();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2.ToBoolean() && v3.ToBoolean();
                        SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_EQ:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        result = new VariableValue();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2 == v3;
                        SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_UNEQ:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        result = new VariableValue();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2 != v3;
                        SetValue(statement.Variable1, result);
                        break;

                    case OP_CODES.OP_VAR_CLR:
                        ClearValue(statement.Variable2);
                        break;
                    case OP_CODES.OP_STORE:
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable2, v3.Clone());
                        break;
                    case OP_CODES.OP_ADD:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 + v3);
                        break;
                    case OP_CODES.OP_SUB:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 - v3);
                        break;
                    case OP_CODES.OP_MUL:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 * v3);
                        break;
                    case OP_CODES.OP_DIV:
                        v2 = GetValue(statement.Variable2);
                        v3 = GetValue(statement.Variable3);
                        SetValue(statement.Variable1, v2 / v3);
                        break;


                }
                _instruction++;
            }
        }
    }
}
