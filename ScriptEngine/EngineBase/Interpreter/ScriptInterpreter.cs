using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
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
        private ScriptProgrammContext _context;

        private ScriptDebugger _debugger;
        private Queue<Value> _stack;
        private int _current_line;

        public ScriptDebugger Debugger { get => _debugger; }
        public int CurrentLine { get => _current_line; }
        public ScriptProgrammContext Context { get => _context; }
        public IFunction CurrentFunction { get => _context.CurrentFunction; }
        public ScriptModule CurrentModule { get => _context.CurrentModule; }
        public string CurrentModuleName { get => _context.CurrentModule.Name; }

        public ScriptInterpreter(ScriptProgramm programm)
        {
            _programm = programm;
            _instruction = int.MaxValue;

            _debugger = new ScriptDebugger(this);
            _stack = new Queue<Value>();
            _context = new ScriptProgrammContext(_programm.GlobalScope.VarCount);
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
                    IVariable object_var = _programm.GlobalVariables.Get(module_kv.Key);
                    _context.SetValue(object_var, CreateObject(module_kv.Key, module_kv.Value));
                }

                if (module_kv.Value.Type == ModuleTypeEnum.OBJECT)
                {
                    IVariable object_var = _programm.GlobalVariables.Get(module_kv.Key);
                    _context.SetValue(object_var, CreateObject(module_kv.Key, module_kv.Value));
                }

                if (module_kv.Value.Type == ModuleTypeEnum.STARTUP)
                {
                    startup_module = module_kv.Value.Name;
                    _context.SetStartModule(module_kv.Value);
                }
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
        private Value CreateObject(string name, ScriptModule type)
        {
            ObjectContext object_context = _context.ModuleContextsHolder.CreateModuleContext(name, type);

            FunctionCall("<<entry_point>>", type.Name);

            Value var = new Value
            {
                Type = ValueTypeEnum.OBJECT,
                Object = object_context
            };

            Execute();

            _context.ModuleContextsHolder.DeleteModuleContext(name);
            return var;
        }

        /// <summary>
        /// Вызов функции по ее имени. Поиск названия в программе.
        /// </summary>
        /// <param name="name"></param>
        public void FunctionCall(string name, string module_name)
        {
            IFunction function;
            bool change_context = false;

            if (module_name != string.Empty)
            {
                ScriptModule module;
                module = _programm[module_name];
                change_context = true;

                if (!_context.ModuleContextsHolder.ExistModuleContext(module_name))
                    _context.ModuleContextsHolder.CreateModuleContext(module_name, module);
                _context.ModuleContextsHolder.SetModuleContext(module_name,_instruction);
            }

            function = _context.CurrentModule.Functions.Get(name);

            if (function == null)
                function = _programm.GlobalFunctions.Get(name);

            if (function == null)
                throw new Exception($"Функция [{name}] не найдена.");

            FunctionCall(function, change_context);
        }

        /// <summary>
        /// Вызов функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private IVariable FunctionCall(IFunction function, bool change_context)
        {
            int tmp_instruction;

            tmp_instruction = _instruction;
            // сохранить позицию кода, и создать новый контекст функции
            if (change_context)
                tmp_instruction = _instruction * -1;

            _context.FunctionContextsHolder.CreateFunctionContext(function, tmp_instruction);
            _instruction = function.EntryPoint;

            // добавить из стека в текущий (новый) контекст, переменные функции
            List<string> function_params = new List<string>();
            Value stack_param;
            if (function.Param != null)
                foreach (IVariable function_param in function.Param)
                {
                    if (_stack.Count > 0)
                    {
                        stack_param = _stack.Dequeue();
                        if (stack_param?.Type != ValueTypeEnum.NULL)
                        {
                            function_param.Scope = function.Scope;
                            if (function_param.Status == VariableStatusEnum.CONSTANTVARIABLE)
                            {
                                _context.SetValue(function_param, stack_param.Clone());
                                function_params.Add(function_param.Name + " = " + stack_param.ToString());
                            }
                            else
                            {
                                _context.CopyValue(function_param, stack_param);
                                function_params.Add(function_param.Name + " = " + stack_param.ToString());
                            }
                        }
                        else
                        {
                            _context.SetValue(function_param, new Value(ValueTypeEnum.NULL, ""));
                            function_params.Add(function_param.Name + " = Null");
                        }
                    }
                    else
                    {
                        _context.SetValue(function_param, function_param.Value?.Clone());
                        function_params.Add(function_param.Name + " = " + function_param.Value?.ToString());
                    }
                }
            _context.FunctionContextsHolder.SetFunctionParams(function_params);

            _debugger.OnFunctionCall();

            return null;
        }

        /// <summary>
        /// Выход из функции return
        /// </summary>
        public void FunctionReturn(IVariable return_var)
        {
            Value var = null;

            // забираем возвращаемое значение из текущего контекста
            if (return_var != null)
            {
                var = _context.GetValue(return_var).Clone();
                _stack.Enqueue(var);
            }

            // установка предыдущего контекста, и предыдущей функции.
            int position = _context.FunctionContextsHolder.RestoreFunctionContext();
            if (position < 0)
                _instruction = _context.ModuleContextsHolder.RestoreModuleContext();
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
        private IFunction CheckObjectFunctionCall(ScriptModule module, IFunction function)
        {
            IFunction work_function;

            work_function = module.Functions.Get(function.Name);

            if (work_function == null)
                throw new CompilerException(function.CodeInformation, $"Процедура или функция с именем [{function.Name}] не определена, у обьекта [{module.Name}].");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new CompilerException(function.CodeInformation, $"Обращение к процедуре [{function.Name}] как к функции.");

            function.EntryPoint = work_function.EntryPoint;

            if (function.Param.Count == work_function.Param.Count)
                return work_function;

            // Блок проверки параметров.
            if (function.Param.Count > work_function.Param.Count)
                throw new CompilerException(work_function.CodeInformation, $"Много фактических параметров [{work_function.Name}].");

            int i, param_count;
            param_count = i = function.Param.Count;

            while (i < work_function.Param.Count)
            {
                if (work_function.Param[i].Status == VariableStatusEnum.CONSTANTVARIABLE && work_function.Param[i].Value != null)
                    param_count++;
                i++;
            }

            if (param_count < work_function.Param.Count)
                throw new CompilerException(work_function.CodeInformation, $"Недостаточно фактических параметров [{work_function.Name}].");

            return work_function;
        }


        /// <summary>
        /// Вызов метода у обьекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectCall(ScriptStatement statement)
        {
            Value object_call = _context.GetValue(statement.Variable2);
            Value function_index = _context.GetValue(statement.Variable3);

            IFunction function = _context.CurrentModule.ObjectCallGet(function_index.ToInt());

            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.Object == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{function.Name}]");

            IFunction work_function = CheckObjectFunctionCall(object_call.Object.Module, function);

            if (!work_function.Public)
                throw new RuntimeException(this, $"Функция [{function.Name}] не имеет оператора Экспорт, и не доступна.");

            _context.ModuleContextsHolder.SetModuleContext(object_call.Object,_instruction);
            FunctionCall(work_function,true);
        }

        /// <summary>
        /// Получить значение свойства обьекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectResoleVariable(ScriptStatement statement)
        {
           Value var_name;
            var_name = _context.GetValue(statement.Variable3);

            Value object_call = _context.GetValue(statement.Variable2);
            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.Object == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{var_name.Content}]");

            IVariable var = object_call.Object.Module.Variables.Get(var_name.Content, object_call.Object.Module.ModuleScope);
            if(!var.Public)
                throw new RuntimeException(this, $"Переменная [{var_name.Content}] не имеет оператора Экспорт, и не доступна.");

            Value value = object_call.Object.Context.GetValue(var.StackNumber);
            _context.CopyValue(statement.Variable1,value);
        }


        /// <summary>
        /// Выполнить код модуля.
        /// </summary>
        internal void Execute()
        {
            ScriptStatement statement;
            Value v2, v3,result;

            while (_instruction < _context.CurrentModule.Code.Count)
            {
                if (_instruction == int.MaxValue || _instruction < 0)
                    return;

                statement = _context.CurrentModule.Code[_instruction];
                _current_line = statement.Line;

                if (_debugger.OnExecute(statement))
                    return;

                switch (statement.OP_CODE)
                {
                    case OP_CODES.OP_PUSH:
                        v2 = _context.GetValue(statement.Variable2);
                        _stack.Enqueue(v2);
                        break;
                    case OP_CODES.OP_POP:
                        if (_stack.Count > 0)
                            v2 = _stack.Dequeue();
                        else
                            v2 = new Value(ValueTypeEnum.NULL, "");
                        _context.SetValue(statement.Variable1, v2);
                        break;

                    case OP_CODES.OP_CALL:
                        string module_name = string.Empty;

                        v2 = _context.GetValue(statement.Variable2);
                        if (statement.Variable3 != null)
                        {
                            v3 = _context.GetValue(statement.Variable3);
                            module_name = v3.Content;
                        }
                        FunctionCall(v2.Content, module_name);
                        continue;

                    case OP_CODES.OP_RETURN:
                        FunctionReturn(statement.Variable2);
                        break;


                    case OP_CODES.OP_OBJECT_CALL:
                        ObjectCall(statement);
                        continue;
                    case OP_CODES.OP_OBJECT_RESOLVE_VAR:
                        ObjectResoleVariable(statement);
                        break;

                    case OP_CODES.OP_IFNOT:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        if (!v2.Boolean)
                        {
                            _instruction = v3.Integer;
                            continue;
                        }
                        break;
                    case OP_CODES.OP_JMP:
                        v2 = _context.GetValue(statement.Variable2);
                        _instruction = v2.Integer;
                        continue;


                    case OP_CODES.OP_GT:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 > v3);
                        break;
                    case OP_CODES.OP_LT:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 < v3);
                        break;
                    case OP_CODES.OP_GE:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 >= v3);
                        break;
                    case OP_CODES.OP_LE:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 <= v3);
                        break;
                    case OP_CODES.OP_NOT:
                        v2 = _context.GetValue(statement.Variable2);
                        result = new Value();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = !v2.ToBoolean();
                        _context.SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_OR:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        result = new Value();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2.ToBoolean() || v3.ToBoolean();
                        _context.SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_AND:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        result = new Value();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2.ToBoolean() && v3.ToBoolean();
                        _context.SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_EQ:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        result = new Value();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2 == v3;
                        _context.SetValue(statement.Variable1, result);
                        break;
                    case OP_CODES.OP_UNEQ:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        result = new Value();
                        result.Type = ValueTypeEnum.BOOLEAN;
                        result.Boolean = v2 != v3;
                        _context.SetValue(statement.Variable1, result);
                        break;

                    case OP_CODES.OP_VAR_CLR:
                        _context.ClearValue(statement.Variable2);
                        break;
                    case OP_CODES.OP_STORE:
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable2, v3.Clone());
                        break;
                    case OP_CODES.OP_ADD:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 + v3);
                        break;
                    case OP_CODES.OP_SUB:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 - v3);
                        break;
                    case OP_CODES.OP_MUL:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 * v3);
                        break;
                    case OP_CODES.OP_DIV:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 / v3);
                        break;
                    case OP_CODES.OP_MOD:
                        v2 = _context.GetValue(statement.Variable2);
                        v3 = _context.GetValue(statement.Variable3);
                        _context.SetValue(statement.Variable1, v2 % v3);
                        break;

                }
                _instruction++;
            }
        }
    }
}
