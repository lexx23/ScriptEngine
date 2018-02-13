using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;

namespace ScriptEngine.EngineBase.Interpreter
{
    public class ScriptInterpreter
    {
        private int _instruction;
        private ScriptProgramm _programm;
        private Context.InterpreterContext _context;


        private ScriptDebugger _debugger;
        private Queue<IVariableReference> _stack;
        private IValue _return_value;
        private int _current_line;

        public ScriptDebugger Debugger { get => _debugger; }
        public int IstructionIndex { get => _instruction; }
        public int CurrentLine { get => _current_line; }
        public ScriptProgramm Programm { get => _programm; }
        //public ScriptProgrammContext Context { get => _context; }
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
                    IVariable object_var = _programm.GlobalVariables.Get(module_kv.Key);
                    object_var.Value = CreateObject(module_kv.Key, module_kv.Value);
                    continue;
                }

                if (module_kv.Value.AsGlobal && !module_kv.Value.AsObject)
                {
                    IVariable object_var = _programm.GlobalVariables.Get("<<" + module_kv.Key + ">>");
                    object_var.Value = CreateObject(module_kv.Key, module_kv.Value);
                    continue;
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
        private IValue CreateObject(string name, ScriptModule type)
        {
            IFunction function;
            ScriptObjectContext object_context = _context.CreateObject(type);

            if (type.AsObject)
            {
                function = type.Functions.Get("<<entry_point>>");

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
            if(var == null)
                throw new Exception($"Не найден контекст модуля [{module_name}].");

            FunctionCall(function, var.Value.AsScriptObject());
        }

        /// <summary>
        /// Вызов функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private IVariable FunctionCall(IFunction function,ScriptObjectContext context)
        {
            if (function.Scope != null)
            {
                _context.Set(context,function,_instruction);
                // добавить из стека в текущий (новый) контекст, переменные функции
                SetFunctionParams(function);
            }
            else
            {
                IValue result = function.Method(InternalFunctionParams(function));
                _return_value = result;
                _instruction++;
                return null;
            }

            //создать новый контекст функции
            _instruction = function.EntryPoint;
            _debugger.OnFunctionCall();

            return null;
        }


        /// <summary>
        /// Подготовка параметров для внешней функции.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        private IValue[] InternalFunctionParams(IFunction function)
        {
            return null;
            //IList<Value> function_params = new List<Value>();
            //IVariable stack_param;
            //if (function.Param != null)
            //    foreach (IVariable function_param in function.Param)
            //    {
            //        if (_stack.Count > 0)
            //        {
            //            stack_param = _stack.Dequeue();
            //            if (stack_param?.Value.Type != ValueTypeEnum.NULL)
            //            {
            //                function_param.Scope = function.Scope;
            //                // Если есть значение, значит это параметр по значению.
            //                if (function_param.Type == VariableTypeEnum.CONSTANTVARIABLE)
            //                    function_params.Add((Value)stack_param);
            //                else
            //                    // Параметр который передается как адрес. Изменение внутри функции, приведет к изменению и вне.
            //                    function_params.Add((Value)stack_param);
            //            }
            //            else
            //                // Пустой параметр, тот который не указан между запятых. Функция Тест(,123) 
            //                function_params.Add(new Value());
            //        }
            //        else
            //            // Параметры по умолчанию.
            //            function_params.Add((Value)function_param.Value);
            //    }

            //return function_params.ToArray();
        }


        /// <summary>
        /// Добавить в контекст параметры функции.
        /// </summary>
        /// <param name="function"></param>
        private void SetFunctionParams(IFunction function)
        {
            IVariableReference stack_param;
            IList<string> function_params = new List<string>();

            if (function.DefinedParameters != null)
                foreach (FunctionParameter function_param in function.DefinedParameters)
                {
                    if (_stack.Count > 0)
                    {
                        stack_param = _stack.Dequeue();
                        if (stack_param != null)
                        {
                            // Если есть значение, значит это параметр по значению.
                            if (function_param.Type == VariableTypeEnum.CONSTANTVARIABLE)
                            {
                                function_param.InternalVariable.Value = stack_param.Get();
                                if (_debugger.Debug)
                                    function_params.Add(function_param.Name + " = " + stack_param.ToString());
                            }
                            else
                            {
                                // Параметр который передается как адрес. Изменение внутри функции, приведет к изменению и вне.
                                IVariableReference reference = new ScriptReference(stack_param);
                                _context.Update(function_param.InternalVariable, reference);
                                if (_debugger.Debug)
                                    function_params.Add(function_param.Name + " = " + stack_param.ToString());
                            }
                        }
                        else
                        {
                            // Пустой параметр, тот который не указан между запятых. Тест(,123) 
                            function_param.InternalVariable.Value = ValueFactory.Create();
                            if (_debugger.Debug)
                                function_params.Add(function_param.Name + " = Null");
                        }
                    }
                    else
                    {
                        // Параметры по умолчанию.
                        function_param.InternalVariable.Value = function_param.DefaultValue;
                        if (_debugger.Debug)
                            function_params.Add(function_param.Name + " = " + function_param.DefaultValue?.ToString());
                    }
                }

            //Для дебага, сохраняем параметры функции. Используется при развороте вызовов стека.
           // if (_debugger.Debug)
            //    _context.FunctionContextsHolder.SetFunctionParams(function_params);
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
                throw new RuntimeException(this, $"Процедура или функция с именем [{function.Name}] не определена, у обьекта [{module.Name}].");

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
        /// Вызов метода у обьекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectCall(ScriptStatement statement)
        {
            IValue object_call = statement.Variable2.Value;
            IValue function_index = statement.Variable3.Value;

            IFunction function;
            function = _context.CurrentModule.ObjectCallGet(function_index.AsInt());

            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.AsScriptObject() == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{function.Name}]");

            IFunction work_function = CheckObjectFunctionCall(object_call.AsScriptObject().Module, function);

            if (!work_function.Public)
                throw new RuntimeException(this, $"Функция [{function.Name}] не имеет оператора Экспорт, и не доступна.");

            FunctionCall(work_function, object_call.AsScriptObject());
        }

        /// <summary>
        /// Получить значение свойства обьекта.
        /// </summary>
        /// <param name="statement"></param>
        private void ObjectResolveVariable(ScriptStatement statement)
        {
            IValue var_name;
            var_name = statement.Variable3.Value;

            IValue object_call = statement.Variable2.Value;
            if (object_call == null || object_call.Type != ValueTypeEnum.OBJECT || object_call.AsScriptObject() == null)
                throw new RuntimeException(this, $"Значение не является значением объектного типа [{var_name.ToString()}]");

            IVariable var = object_call.AsScriptObject().Module.Variables.Get(var_name.AsString(), object_call.AsScriptObject().Module.ModuleScope);
            if (var == null)
                throw new RuntimeException(this, $"У обьекта [{object_call.AsScriptObject().Module.Name}] нет свойства [{var_name.ToString()}].");
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
            throw new RuntimeException(this, $"Невозможно расчитать {EnumStringAttribute.GetStringValue(code)}  {left} и {right}.");
        }

        /// <summary>
        /// Выполнить код модуля.
        /// </summary>
        internal void Execute()
        {
            ScriptStatement statement;
            //IList<ScriptStatement> code = CurrentModule.Code;

            while (true)
            {
                if (_instruction == int.MaxValue || _instruction < 0 || _instruction >= _context.CurrentModule.Code.Count)
                    return;

                statement = CurrentModule.Code[_instruction];
                _current_line = statement.Line;

                if (_debugger.Debug && _debugger.OnExecute(statement))
                    return;

                switch (statement.OP_CODE)
                {
                    case OP_CODES.OP_PUSH:
                        _stack.Enqueue(statement.Variable2.Reference);
                        break;
                    case OP_CODES.OP_POP:
                        if (_return_value != null)
                        {
                            statement.Variable1.Value = _return_value;
                            _return_value = null;
                        }
                        break;

                    case OP_CODES.OP_CALL:
                        ScriptObjectContext context;
                        if (_context.CurrentModule != statement.Function.Scope.Module)
                        {
                            IVariable var = null;
                            var = _programm.GlobalVariables.Get("<<" + statement.Function.Scope.Module.Name + ">>");
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
                        _context.Update(statement.Variable2,new SimpleReference());
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
