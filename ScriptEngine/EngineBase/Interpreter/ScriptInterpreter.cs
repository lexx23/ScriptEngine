using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptBaseFunctionsLibrary.BuildInTypes;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System;
using ScriptEngine.EngineBase.Compiler;
using System.Diagnostics;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptInterpreter
    {
        private int _instruction;
        private ScriptProgramm _programm;
        private InterpreterContext _context;

        private Queue<IVariableReference> _stack;
        private IList<ScriptStatement> _code;
        private IList<(int, int, int)> _eval;
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

            _eval = new List<(int, int, int)>();
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
            ScriptModule start_module = null;

            foreach (ScriptModule module in _programm.Modules)
            {
                if (module.AsGlobal || module.Type == ModuleTypeEnum.COMMON)
                {
                    if (module.Type != ModuleTypeEnum.STARTUP)
                    {
                        IValue value = ValueFactory.Create(new ScriptObjectContext(module));
                        IVariable object_var = _programm.GlobalVariables.Get(module.Name);
                        object_var.Value = value;
                        if (module.Name != module.Alias)
                        {
                            object_var = null;
                            object_var = _programm.GlobalVariables.Get(module.Alias);
                            if (object_var != null)
                                object_var.Value = value;
                        }
                    }
                    else
                        start_module = module;
                }
            }

            if (start_module != null)
                new ScriptObjectContext(start_module);
        }

        /// <summary>
        /// Вызов функции.
        /// </summary>
        /// <param name="name"></param>
        public IValue FunctionCall(ScriptObjectContext context, IFunction function, IVariable[] param = null)
        {
            int tmp_instruction;
            InterpreterContext tmp_context;

            if (param != null)
                for (int i = 0; i < param.Length; i++)
                    _stack.Enqueue(param[i].Reference);

            tmp_context = _context;
            _context = new InterpreterContext(_programm);

            try
            {
                tmp_instruction = _instruction;
                _instruction = _instruction * -1;
                FunctionCall(function, context);
                if (function.Method == null)
                    Execute();
            }
            catch (Exception ex)
            {
                _context = tmp_context;
                _code = tmp_context.CurrentModule?.Code;
                throw new RuntimeException(ex);
            }

            _instruction = tmp_instruction;
            _context = tmp_context;
            _code = tmp_context.CurrentModule?.Code;
            return _return_value;
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
        /// Оператор Новый(new) вариант 2: Новый(<Тип>[, <ПараметрыКонструктора>])
        /// </summary>
        /// <returns></returns>
        private IValue NewType2()
        {
            IVariableReference[] stack_param = new IVariableReference[_stack.Count];

            int i = 0;
            while (_stack.Count > 0)
            {
                stack_param[i] = _stack.Dequeue();
                i++;
            }

            string type_name = stack_param[0].Get().AsString();

            // Проверяем что такой тип существует.
            InternalScriptType type = _programm.InternalTypes.Get(type_name);
            if (type == null)
                throw new RuntimeException(this, $"Тип не определен ({type_name})");

            // Находим его конструктор.
            IFunction constructor = type.Module.Functions.Get("Constructor");
            if (constructor == null)
                constructor = type.Module.Functions.Get("<<entry_point>>");
            if (constructor == null)
                throw new RuntimeException(this, "Конструктор не найден.");

            IValue result;

            // Обрабатываем параметры.
            if (stack_param.Length > 1)
            {
                IValue value = stack_param[1].Get();
                if (value.BaseType == ValueTypeEnum.SCRIPT_OBJECT)
                {
                    // Если это массив то забираем параметры из массива.
                    ScriptObjectContext script_object = value.AsScriptObject();
                    if (typeof(ICollectionIndexer).IsAssignableFrom(script_object.Instance.GetType()) && typeof(IEnumerable<IValue>).IsAssignableFrom(script_object.Instance.GetType()))
                        foreach (IValue array_value in script_object.Instance as IEnumerable<IValue>)
                            _stack.Enqueue(ReferenceFactory.Create(array_value));
                }
            }


            if (constructor.Name == "<<entry_point>>")
            {
                result = ValueFactory.Create(new ScriptObjectContext(type.Module));
                _instruction++;
            }
            else
            {
                FunctionCall(constructor, _context.Current);
                result = _return_value;
            }

            return result;
        }

        /// <summary>
        /// Оператор Eval(Вычислить)
        /// </summary>
        /// <param name="expression"></param>
        /// <param name="result"></param>
        public void Eval(string expression, IVariable result)
        {
            _eval.Add((_context.CurrentFunction.Scope.Vars.Count, _code.Count, _instruction));
            ScriptCompiler compiler = new ScriptCompiler(_programm);
            _instruction = compiler.CompileEval(expression, result);
            compiler = null;
        }


        /// <summary>
        /// Выполнить код модуля.
        /// </summary>
        internal void Execute()
        {
            ScriptStatement statement;
            while (true)
            {
                if (_instruction < 0 || _instruction >= _code.Count)
                    return;

                try
                {
                    statement = _code[_instruction];
                    _current_line = statement.Line;

                    if (_debugger.Debug)
                        _debugger.OnExecute();

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
                            if (statement.Function != null)
                            {
                                // Вариант 1: Новый <Идентификатор типа>[(<Парам1>, <Парам2>, …)] 
                                FunctionCall(statement.Function, _context.Current);
                                statement.Variable1.Value = _return_value;
                            }
                            else
                            {
                                // Вариант 2: Новый(<Тип>[, <ПараметрыКонструктора>])
                                statement.Variable1.Value = NewType2();
                            }
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

                            if (_instruction < 0)
                                return;

                            _debugger.OnFunctionReturn();
                            break;

                        case OP_CODES.OP_TRY:
                            _context.TryBlockAdd(statement.Variable2.Value.AsInt());
                            break;
                        case OP_CODES.OP_ENDTRY:
                            _context.TryBlockRemove();
                            _error_info = new ErrorInfo();
                            break;
                        case OP_CODES.OP_RAISE:
                            // Если нет сообщения значить это исключение в блоке обработки исключений. Удаляем этот обработчик.
                            if (statement.Variable2 == null)
                                _context.TryBlockRemove();
                            throw new RuntimeException(this, statement.Variable2.Value.AsString());


                        case OP_CODES.OP_OBJ_CALL:
                            IValue object_call = statement.Variable2.Value;
                            if (object_call == null || object_call.BaseType != ValueTypeEnum.SCRIPT_OBJECT)
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
                            if (var_object == null || var_object.BaseType != ValueTypeEnum.SCRIPT_OBJECT)
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
                            if (array_object == null || array_object.BaseType != ValueTypeEnum.SCRIPT_OBJECT)
                                throw new RuntimeException(this, $"Значение не является значением объектного типа [{statement.Variable2.Name}]");
                            else
                            {
                                ScriptObjectContext script_object = array_object.AsScriptObject();

                                bool indexed = false;
                                if (typeof(ICollectionIndexer).IsAssignableFrom(script_object.Instance.GetType()))
                                    indexed = true;

                                // Если значение число то обращение к номеру массива.
                                if (statement.Variable3.Value.BaseType == ValueTypeEnum.NUMBER)
                                {
                                    if (!indexed)
                                        throw new RuntimeException(this, "Получение элемента по индексу для значения не определено.");
                                    else
                                    {
                                        IVariableReference var_ref = new CollectionReference((ICollectionIndexer)script_object.Instance, statement.Variable3.Value);
                                        _context.Update(statement.Variable1, var_ref);
                                    }

                                    break;
                                }

                                // Если значение строка то обращение к свойству.
                                if (statement.Variable3.Value.BaseType == ValueTypeEnum.STRING)
                                {
                                    IVariableReference var_ref;
                                    if (indexed)
                                        var_ref = new CollectionReference((ICollectionIndexer)script_object.Instance, statement.Variable3.Value);
                                    else
                                        var_ref = script_object.GetPublicVariable(statement.Variable3.Value.AsString());
                                    _context.Update(statement.Variable1, var_ref);
                                }
                            }
                            break;

                        case OP_CODES.OP_ITR_GET:
                            IValue foreach_object = statement.Variable2.Value;
                            if (foreach_object == null || foreach_object.BaseType != ValueTypeEnum.SCRIPT_OBJECT)
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

                        case OP_CODES.OP_EVAL:
                            Eval(statement.Variable2.Value.AsString(), statement.Variable1);
                            continue;

                        case OP_CODES.OP_EVAL_EXIT:
                            (int, int, int) eval_exit = _eval[_eval.Count - 1];
                            _eval.RemoveAt(_eval.Count - 1);
                            int i;
                            for (i = eval_exit.Item1; i < _context.CurrentFunction.Scope.Vars.Count; i++)
                            {
                                _context.CurrentModule.Variables.Remove(_context.CurrentFunction.Scope.Vars[i]);
                                _context.CurrentFunction.Scope.Vars.RemoveAt(i);
                            }

                            i = eval_exit.Item2;
                            while (i < _code.Count)
                                _code.RemoveAt(i);

                            statement.Variable2.Value = statement.Variable3.Value;

                            _instruction = eval_exit.Item3;
                            if (_debugger.Debug && _debugger.OnEvalExit())
                                return;
                            break;


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
                    int line;
                    string module;
                    string message;

                    if (ex.Data.Count == 0)
                    {
                        line = _current_line;
                        module = _context.CurrentModule.Name;
                        message = ex.Message;
                    }
                    else
                    {
                        line = Convert.ToInt32(ex.Data["line"]);
                        module = Convert.ToString(ex.Data["module"]);
                        message = Convert.ToString(ex.Data["message"]);
                    }

                    _stack.Clear();
                    _error_info = new ErrorInfo(ex);
                    int instruction = _context.Exception();
                    if (instruction != -1)
                    {
                        _instruction = instruction;
                        _code = _context.CurrentModule.Code;
                    }
                    else
                        throw new RuntimeException(line, module, message);
                }
            }
        }
    }
}
