using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptBaseFunctionsLibrary.BuildInTypes;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System;
using ScriptEngine.EngineBase.Library.BaseTypes;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptInterpreter
    {
        private int _instruction;
        private ScriptProgramm _programm;
        private InterpreterContext _context;

        private Queue<IVariableReference> _stack;
        private IList<ScriptStatement> _code;
        private ErrorInfo _error_info;
        private ScriptDebugger _debugger;
        private IValue _return_value;
        private int _current_line;

        /// <summary>
        /// Номер линии кода.
        /// </summary>
        public int CurrentLine { get => _current_line; }

        /// <summary>
        /// Номер инструкции.
        /// </summary>
        public int IstructionIndex { get => _instruction; }

        /// <summary>
        /// Отладчик.
        /// </summary>
        public ScriptDebugger Debugger { get => _debugger; }

        /// <summary>
        /// Программа.
        /// </summary>
        public ScriptProgramm Programm { get => _programm; }

        /// <summary>
        /// Текущая функция.
        /// </summary>
        public IFunction CurrentFunction { get => _context.CurrentFunction; }

        /// <summary>
        /// Текущий модуль.
        /// </summary>
        public ScriptModule CurrentModule { get => _context.CurrentModule; }

        /// <summary>
        /// Информация о текущем исключении.
        /// </summary>
        public ErrorInfo ErrorInfo { get => _error_info; }


        private static ScriptInterpreter _interpreter;

        /// <summary>
        /// Текущий интерпретатор.
        /// </summary>
        public static ScriptInterpreter Interpreter { get => _interpreter; }


        public ScriptInterpreter(ScriptProgramm programm)
        {
            _programm = programm;
            _instruction = int.MaxValue;

            _debugger = new ScriptDebugger(this);
            _stack = new Queue<IVariableReference>();
            _context = new Context.InterpreterContext(_programm);

            _interpreter = this;
            _error_info = new ErrorInfo();
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

            foreach (ScriptModule module in _programm.Modules)
            {
                if (module.Type == ModuleTypeEnum.STARTUP)
                    startup_module = module.Name;


                if (module.AsGlobal && module.AsObject)
                {
                    IValue value = CreateObject(module);
                    IVariable object_var = _programm.GlobalVariables.Get(module.Name);
                    object_var.Value = value;
                    if (module.Name != module.Alias)
                    {
                        object_var = null;
                        object_var = _programm.GlobalVariables.Get(module.Alias);
                        if (object_var != null)
                            object_var.Value = value;
                    }

                    continue;
                }

                if (module.AsGlobal && !module.AsObject)
                {
                    IVariable object_var = _programm.GlobalVariables.Get(module.Name);
                    object_var.Value = CreateObject(module);
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
        private IValue CreateObject(ScriptModule type)
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
            function = _programm.Modules.Get(module_name).Functions.Get(name);

            if (function == null)
                throw new Exception($"Функция [{name}] не найдена.");

            IVariable var = null;
            var = _programm.GlobalVariables.Get(function.Scope.Module.Name);
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
                InternalFunctionParams(function);
            }
            else
            {
                IValue result = function.Method.Run(LibraryFunctionParams(function));
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
        private IVariable[] LibraryFunctionParams(IFunction function)
        {
            IVariableReference stack_param;
            IVariable[] function_params;
            int i = 0;

            if (function.DefinedParameters.AnyCount)
            {
                function_params = new IVariable[_stack.Count];

                while (_stack.Count > 0)
                {
                    stack_param = _stack.Dequeue();
                    if (stack_param != null)
                        function_params[i] = Variable.Create(stack_param);
                    else
                        // Пустой параметр, тот который не указан между запятых. Функция Тест(,123) 
                        function_params[i] = Variable.Create(ValueFactory.Create());
                    i++;
                }
            }
            else
            {
                function_params = new IVariable[function.DefinedParameters.Count];

                FunctionParameter function_param;

                if (function.DefinedParameters != null)
                    while (i < function_params.Length)
                    {
                        function_param = function.DefinedParameters[i];
                        if (_stack.Count > 0)
                        {
                            stack_param = _stack.Dequeue();
                            if (stack_param != null)
                                function_params[i] = Variable.Create(stack_param);
                            else
                                // Пустой параметр, тот который не указан между запятых. Функция Тест(,123) 
                                function_params[i] = Variable.Create(ValueFactory.Create());
                        }
                        else
                            // Параметры по умолчанию.
                            function_params[i] = Variable.Create(function_param.DefaultValue);
                        i++;
                    }
            }

            return function_params;
        }


        /// <summary>
        /// Добавить в контекст параметры функции.
        /// </summary>
        /// <param name="function"></param>
        private void InternalFunctionParams(IFunction function)
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
                try
                {
                    if (_instruction < 0 ||  _instruction >= _code.Count)
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
                                var = _programm.GlobalVariables.Get(statement.Variable3.Value.AsString());
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

                            if (_instruction == -1)
                                return;

                            if (_return_value == null)
                                _instruction++;

                            _debugger.OnFunctionReturn();
                            break;

                        case OP_CODES.OP_TRY:
                            _context.TryAdd(statement.Variable2.Value.AsInt());
                            break;
                        case OP_CODES.OP_ENDTRY:
                            _context.TryRemove();
                            _error_info = new ErrorInfo();
                            break;
                        case OP_CODES.OP_RAISE:
                            throw new RuntimeException(this,statement.Variable2.Value.AsString());


                        case OP_CODES.OP_OBJ_CALL:
                            IValue object_call = statement.Variable2.Value;
                            if (object_call == null || object_call.Type != ValueTypeEnum.SCRIPT_OBJECT)
                                throw new RuntimeException(this, $"Значение не является значением объектного типа [{statement.Variable2.Name}]");
                            else
                            {
                                ScriptObjectContext script_object = object_call.AsScriptObject();
                                IFunction function = script_object.CheckFunction(CurrentModule.ObjectCallGet(statement.Variable3.Value.AsInt()));
                                FunctionCall(function, script_object);
                            }
                            continue;

                        case OP_CODES.OP_OBJ_GET_VAR:
                            IValue var_object = statement.Variable2.Value;
                            if (var_object == null || var_object.Type != ValueTypeEnum.SCRIPT_OBJECT)
                                throw new RuntimeException(this, $"Значение не является значением объектного типа [{statement.Variable2.Name}]");
                            else
                            {
                                ScriptObjectContext script_object = var_object.AsScriptObject();

                                IVariableReference var_ref = script_object.GetPublicVariable(statement.Variable3.Value.AsString());
                                _context.Update(statement.Variable1, var_ref);
                            }
                            break;

                        case OP_CODES.OP_ARRAY_GET:
                            IValue array_object = statement.Variable2.Value;
                            if (array_object == null || array_object.Type != ValueTypeEnum.SCRIPT_OBJECT)
                                throw new RuntimeException(this, $"Значение не является значением объектного типа [{statement.Variable2.Name}]");
                            else
                            {
                                ScriptObjectContext script_object = array_object.AsScriptObject();

                                if (statement.Variable3.Value.Type == ValueTypeEnum.NUMBER)
                                {
                                    if (!typeof(IScriptArray).IsAssignableFrom(script_object.Instance.GetType()))
                                        throw new RuntimeException(this, "Получение элемента по индексу для значения не определено.");
                                    else
                                    {
                                        IVariableReference var_ref = new ArrayReference((IScriptArray)script_object.Instance, statement.Variable3.Value.AsInt());
                                        _context.Update(statement.Variable1, var_ref);
                                    }
                                }

                                if (statement.Variable3.Value.Type == ValueTypeEnum.STRING)
                                {
                                    IVariableReference var_ref = script_object.GetPublicVariable(statement.Variable3.Value.AsString());
                                    if (var_ref == null)
                                        throw new RuntimeException(this, $"Поле объекта не обнаружено [{statement.Variable3.Value.AsString()}]");

                                    _context.Update(statement.Variable1, var_ref);
                                }
                            }
                            break;

                        case OP_CODES.OP_ITR_GET:
                            IValue foreach_object = statement.Variable2.Value;
                            if (foreach_object == null || foreach_object.Type != ValueTypeEnum.SCRIPT_OBJECT)
                                throw new RuntimeException(this, $"Значение не является значением объектного типа [{statement.Variable2.Name}]");
                            else
                            {
                                ScriptObjectContext script_object = foreach_object.AsScriptObject();
                                if (!typeof(IEnumerable<IValue>).IsAssignableFrom(script_object.Instance.GetType()))
                                    throw new RuntimeException(this, $"Итератор для значения не определен [{statement.Variable2.Name}]");

                                statement.Variable1.Value = ValueFactory.Create((script_object.Instance as IEnumerable<IValue>).GetEnumerator());
                            }
                            break;
                        case OP_CODES.OP_ITR_NEXT:
                            IEnumerator<IValue> iterator = statement.Variable3.Value.AsObject() as IEnumerator<IValue>;
                            if (iterator.MoveNext())
                            {
                                statement.Variable2.Value = iterator.Current;
                                _instruction += 2;
                            }
                            break;

                        case OP_CODES.OP_ITR_STOP:
                            IEnumerator<IValue> iterator_stop = statement.Variable2.Value.AsObject() as IEnumerator<IValue>;
                            iterator_stop.Dispose();
                            statement.Variable2.Value = ValueFactory.Create();
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
                catch (Exception ex)
                {
                    int instruction = _context.Exception();
                    if (instruction >= 0)
                    {
                        _error_info = new ErrorInfo(ex);
                        _instruction = instruction;
                        _code = _context.CurrentModule.Code;
                    }
                    else
                        throw new RuntimeException(this, ex.Message);
                }
            }
        }
    }
}
