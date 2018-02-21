using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptInterpreter
    {
        private int _instruction;
        private ScriptProgramm _programm;
        private InterpreterContext _context;

        private Queue<IVariableReference> _stack;
        private IList<ScriptStatement> _code;
        private ScriptDebugger _debugger;
        private IValue _return_value;
        private int _current_line;

        public int CurrentLine { get => _current_line; }
        public int IstructionIndex { get => _instruction; }
        public ScriptDebugger Debugger { get => _debugger; }
        public ScriptProgramm Programm { get => _programm; }
        public IFunction CurrentFunction { get => _context.CurrentFunction; }
        public ScriptModule CurrentModule { get => _context.CurrentModule; }

        public ScriptInterpreter(ScriptProgramm programm)
        {
            _programm = programm;
            _instruction = int.MaxValue;

            _debugger = new ScriptDebugger(this);
            _stack = new Queue<IVariableReference>();
            _context = new Context.InterpreterContext(_programm);

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
                if (module_kv.Value.Type == ModuleTypeEnum.STARTUP)
                    startup_module = module_kv.Value.Name;


                if (module_kv.Value.AsGlobal && module_kv.Value.AsObject)
                {
                    IValue value = CreateObject(module_kv.Key, module_kv.Value);
                    IVariable object_var = _programm.GlobalVariables.Get(module_kv.Value.Name);
                    object_var.Value = value;
                    if (module_kv.Value.Name != module_kv.Value.Alias && module_kv.Value.Alias != string.Empty)
                    {
                        object_var = null;
                        object_var = _programm.GlobalVariables.Get(module_kv.Value.Alias);
                        if (object_var != null)
                            object_var.Value = value;
                    }

                    continue;
                }

                if (module_kv.Value.AsGlobal && !module_kv.Value.AsObject)
                {
                    IVariable object_var = _programm.GlobalVariables.Get("<<" + module_kv.Value.Name + ">>");
                    object_var.Value = CreateObject(module_kv.Value.Name, module_kv.Value);
                    continue;
                }
            }


            FunctionCall("<<entry_point>>", startup_module);
            Execute();
        }

        /// <summary>
        /// Инициализация объектов по умолчанию.
        /// </summary>
        /// <param name="name"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private IValue CreateObject(string name, ScriptModule type)
        {
            IFunction function;
            ScriptObjectContext object_context = _context.CreateObject(type);

            if (type.Type != ModuleTypeEnum.STARTUP)
            {

                if (!type.AsObject)
                    function = type.Functions.Get("<<entry_point>>");
                else
                    function = type.Functions.Get("<<constructor>>");

                if (function != null)
                {
                    FunctionCall(function, object_context);
                    Execute();
                    _context.Reset();
                }

            }
            IValue value = ValueFactory.Create(object_context);
            return value;
        }

        /// <summary>
        /// Вызов функции по ее имени. Поиск названия в программе.
        /// </summary>
        /// <param name="name"></param>
        public void FunctionCall(string name, string module_name)
        {
            IFunction function;
            function = _programm[module_name].Functions.Get(name);

            if (function == null)
                throw new Exception($"Функция [{name}] не найдена.");

            IVariable var = null;
            var = _programm.GlobalVariables.Get("<<" + function.Scope.Module.Name + ">>");
            if (var == null)
                throw new Exception($"Не найден контекст модуля [{module_name}].");

            FunctionCall(function, var.Value.AsScriptObject());
        }

        /// <summary>
        /// Вызов функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private void FunctionCall(IFunction function, ScriptObjectContext context)
        {
            if (function.Method == null)
            {
                _context.Set(context, function, _instruction);
                _code = _context.CurrentModule.Code;
                // добавить из стека в текущий (новый) контекст, переменные функции
                SetFunctionParams(function);
            }
            else
            {
                IValue result = function.Method.Run(InternalFunctionParams(function));
                _return_value = result;
                _instruction++;
                return;
            }

            _instruction = function.EntryPoint;

            _debugger.OnFunctionCall();
        }


        /// <summary>
        /// Подготовка параметров для внешней функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private IValue[] InternalFunctionParams(IFunction function)
        {
            IVariableReference stack_param;
            IValue[] function_params = new IValue[function.DefinedParameters.Count];

            int i = 0;
            FunctionParameter function_param;

            if (function.DefinedParameters != null)
                while (i < function_params.Length)
                {
                    function_param = function.DefinedParameters[i];
                    if (_stack.Count > 0)
                    {
                        stack_param = _stack.Dequeue();
                        if (stack_param != null)
                        {
                            // Если есть значение, значит это параметр по значению.
                            if (function_param.Type == VariableTypeEnum.CONSTANTVARIABLE)
                                function_params[i] = stack_param.Get();
                            else
                                // Параметр который передается как адрес. Изменение внутри функции, приведет к изменению и вне.
                                function_params[i] = stack_param.Get();
                        }
                        else
                            // Пустой параметр, тот который не указан между запятых. Функция Тест(,123) 
                            function_params[i] = ValueFactory.Create();
                    }
                    else
                        // Параметры по умолчанию.
                        function_params[i] = function_param.DefaultValue;
                    i++;
                }

            return function_params;
        }


        /// <summary>
        /// Добавить в контекст параметры функции.
        /// </summary>
        /// <param name="function"></param>
        private void SetFunctionParams(IFunction function)
        {
            int i = 0;
            FunctionParameter function_param;
            IVariableReference stack_param;

            if (function.DefinedParameters != null)
                while (i < function.DefinedParameters.Count)
                {
                    function_param = function.DefinedParameters[i];

                    if (_stack.Count > 0)
                    {
                        stack_param = _stack.Dequeue();
                        if (stack_param != null)
                        {
                            // Если есть значение, значит это параметр по значению.
                            if (function_param.Type == VariableTypeEnum.CONSTANTVARIABLE)
                                function_param.InternalVariable.Value = stack_param.Get();
                            else
                            {
                                // Параметр который передается как адрес. Изменение внутри функции, приведет к изменению и вне.
                                IVariableReference reference = new ScriptReference(stack_param);
                                _context.Update(function_param.InternalVariable, reference);
                            }
                        }
                        else
                        {
                            // Пустой параметр, тот который не указан между запятых. Тест(,123) 
                            function_param.InternalVariable.Value = ValueFactory.Create();
                        }
                    }
                    else
                        // Параметры по умолчанию.
                        function_param.InternalVariable.Value = function_param.DefaultValue;

                    i++;
                }
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
                throw new RuntimeException(this, $"Процедура или функция с именем [{function.Name}] не определена, у объекта [{module.Name}].");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new RuntimeException(this, $"Обращение к процедуре [{function.Name}] как к функции.");

            function.EntryPoint = work_function.EntryPoint;

            if (function.CallParameters.Count == work_function.DefinedParameters.Count)
                return work_function;

            // Блок проверки параметров.
            if (function.CallParameters.Count > work_function.DefinedParameters.Count)
                throw new RuntimeException(this, $"Много фактических параметров [{work_function.Name}].");

            int i, param_count;
            param_count = i = function.CallParameters.Count;

            while (i < work_function.DefinedParameters.Count)
            {
                if (work_function.DefinedParameters[i].DefaultValue != null)
                    param_count++;
                i++;
            }

            if (param_count < work_function.DefinedParameters.Count)
                throw new RuntimeException(this, $"Недостаточно фактических параметров [{work_function.Name}].");

            return work_function;
        }


        /// <summary>
        /// Вызов метода у объекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectCall(ScriptStatement statement)
        {
            IValue object_call = statement.Variable2.Value;
            IValue function_index = statement.Variable3.Value;

            IFunction function;
            function = _context.CurrentModule.ObjectCallGet(function_index.AsInt());

            if (object_call == null || object_call.Type != ValueTypeEnum.SCRIPT_OBJECT || object_call.AsScriptObject() == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{function.Name}]");

            IFunction work_function = CheckObjectFunctionCall(object_call.AsScriptObject().Module, function);

            if (!work_function.Public)
                throw new RuntimeException(this, $"Функция [{function.Name}] не имеет оператора Экспорт, и не доступна.");

            object_call.AsScriptObject().Set();
            FunctionCall(work_function, object_call.AsScriptObject());
        }

        /// <summary>
        /// Получить значение свойства объекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectResolveVariable(ScriptStatement statement)
        {
            IValue var_name;
            var_name = statement.Variable3.Value;

            IValue object_call = statement.Variable2.Value;
            if (object_call == null || object_call.Type != ValueTypeEnum.SCRIPT_OBJECT || object_call.AsScriptObject() == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{var_name.ToString()}]");

            IVariable var = object_call.AsScriptObject().Module.Variables.Get(var_name.AsString(), object_call.AsScriptObject().Module.ModuleScope);
            if (var == null)
                throw new RuntimeException(this, $"У объекта [{object_call.AsScriptObject().Module.Name}] нет свойства [{var_name.ToString()}].");
            if (!var.Public)
                throw new RuntimeException(this, $"Свойство [{var_name.ToString()}] не имеет оператора Экспорт, и не доступно.");

            IVariableReference var_ref = object_call.AsScriptObject().GetReference(var);
            _context.Update(statement.Variable1, var_ref);
        }

        /// <summary>
        /// Проверить результат вычислений.
        /// </summary>
        /// <param name="result"></param>
        private void ErrorResult(string left, string right, OP_CODES code)
        {
            throw new RuntimeException(this, $"Невозможно рассчитать {EnumStringAttribute.GetStringValue(code)}  {left} и {right}.");
        }

        /// <summary>
        /// Выполнить код модуля.
        /// </summary>
        internal void Execute()
        {
            ScriptStatement statement;

            while (true)
            {
                if (_instruction == int.MaxValue || _instruction < 0 || _instruction >= _code.Count)
                    return;

                statement = _code[_instruction];
                _current_line = statement.Line;

                if (_debugger.Debug && _debugger.OnExecute(statement))
                    return;

                switch (statement.OP_CODE)
                {
                    case OP_CODES.OP_PUSH:
                        _stack.Enqueue(statement.Variable2.Reference);
                        break;
                    case OP_CODES.OP_POP:
                        statement.Variable1.Value = _return_value;
                        _return_value = null;
                        break;

                    case OP_CODES.OP_NEW:
                        FunctionCall(statement.Function, _context.Current);
                        statement.Variable1.Value = _return_value;
                        continue;

                    case OP_CODES.OP_CALL:
                        ScriptObjectContext context;
                        if (statement.Variable3 != null)
                        {
                            IVariable var = null;
                            var = _programm.GlobalVariables.Get("<<" + statement.Variable3.Value.AsString() + ">>");
                            context = var.Value.AsScriptObject();
                        }
                        else
                            context = _context.Current;

                        FunctionCall(statement.Function, context);
                        continue;

                    case OP_CODES.OP_RETURN:
                        // забираем возвращаемое значение из текущего контекста
                        if (statement.Variable2 != null)
                            _return_value = statement.Variable2.Value;

                        // установка предыдущего контекста, и предыдущей функции.
                        _instruction = _context.Restore();
                        _code = _context.CurrentModule.Code;

                        if (_return_value == null)
                            _instruction++;

                        _debugger.OnFunctionReturn();
                        break;


                    case OP_CODES.OP_OBJECT_CALL:
                        ObjectCall(statement);
                        continue;
                    case OP_CODES.OP_OBJECT_RESOLVE_VAR:
                        ObjectResolveVariable(statement);
                        break;

                    case OP_CODES.OP_IFNOT:
                        if (!statement.Variable2.Value.AsBoolean())
                        {
                            _instruction = statement.Variable3.Value.AsInt();
                            continue;
                        }
                        break;
                    case OP_CODES.OP_JMP:
                        _instruction = statement.Variable2.Value.AsInt();
                        continue;


                    case OP_CODES.OP_GT:
                        statement.Variable1.Value = ValueFactory.GT(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_LT:
                        statement.Variable1.Value = ValueFactory.LT(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_GE:
                        statement.Variable1.Value = ValueFactory.GE(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_LE:
                        statement.Variable1.Value = ValueFactory.LE(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_NOT:
                        statement.Variable1.Value = ValueFactory.Create(!statement.Variable2.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_OR:
                        statement.Variable1.Value = ValueFactory.Create(statement.Variable2.Value.AsBoolean() || statement.Variable3.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_AND:
                        statement.Variable1.Value = ValueFactory.Create(statement.Variable2.Value.AsBoolean() && statement.Variable3.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_EQ:
                        statement.Variable1.Value = ValueFactory.EQ(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_UNEQ:
                        statement.Variable1.Value = ValueFactory.UNEQ(statement.Variable2.Value, statement.Variable3.Value);
                        break;

                    case OP_CODES.OP_VAR_CLR:
                        _context.Update(statement.Variable2, new SimpleReference());
                        break;
                    case OP_CODES.OP_STORE:
                        statement.Variable2.Value = statement.Variable3.Value;
                        break;


                    case OP_CODES.OP_ADD:
                        statement.Variable1.Value = ValueFactory.ADD(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_SUB:
                        statement.Variable1.Value = ValueFactory.SUB(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_MUL:
                        statement.Variable1.Value = ValueFactory.MUL(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_DIV:
                        statement.Variable1.Value = ValueFactory.DIV(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                    case OP_CODES.OP_MOD:
                        statement.Variable1.Value = ValueFactory.MOD(statement.Variable2.Value, statement.Variable3.Value);
                        break;
                }
                _instruction++;
            }
        }
    }
}
