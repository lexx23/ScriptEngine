using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Parser;
using ScriptEngine.EngineBase.Parser.Precompiler;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Compiler
{
    public class ScriptCompiler
    {
        private TokenIteratorBase _iterator;
        private ScriptProgramm _programm;
        private ScriptScope _scope;
        private ScriptModule _current_module;
        private IDictionary<string, Variable> _deferred_var;
        private IDictionary<string, Function> _deferred_function;
        private int _module_entry_point;
        private int _function_entry_point;

        private IDictionary<TokenSubTypeEnum, op_code> _expression_op_codes;
        private IDictionary<OP_CODES, op_code> _op_codes;

        private enum OP_TYPE
        {
            RESULT_OPTIMIZATION,
            RESULT,
            WO_RESULT
        }

        private struct op_code
        {
            public OP_CODES code;
            public OP_TYPE type;
            public int level;

        }

        private enum Priority
        {
            LOW = 0,
            WO_LOGIC = 3,
            TOP = 4
        }

        public ScriptCompiler()
        {
            _programm = new ScriptProgramm();
            _deferred_var = new Dictionary<string, Variable>();
            _deferred_function = new Dictionary<string, Function>();

            _expression_op_codes = new Dictionary<TokenSubTypeEnum, op_code>();
            _expression_op_codes.Add(TokenSubTypeEnum.P_MUL, new op_code { code = OP_CODES.OP_MUL, type = OP_TYPE.RESULT_OPTIMIZATION, level = 1 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_DIV, new op_code { code = OP_CODES.OP_DIV, type = OP_TYPE.RESULT_OPTIMIZATION, level = 1 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_ADD, new op_code { code = OP_CODES.OP_ADD, type = OP_TYPE.RESULT_OPTIMIZATION, level = 2 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_SUB, new op_code { code = OP_CODES.OP_SUB, type = OP_TYPE.RESULT_OPTIMIZATION, level = 2 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_LOGIC_GREATER, new op_code { code = OP_CODES.OP_GT, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_LOGIC_LESS, new op_code { code = OP_CODES.OP_LT, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_LOGIC_GEQ, new op_code { code = OP_CODES.OP_GE, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_LOGIC_LEQ, new op_code { code = OP_CODES.OP_LE, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_LOGIC_UNEQ, new op_code { code = OP_CODES.OP_UNEQ, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.P_ASSIGN, new op_code { code = OP_CODES.OP_EQ, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 });
            _expression_op_codes.Add(TokenSubTypeEnum.I_LOGIC_AND, new op_code { code = OP_CODES.OP_AND, type = OP_TYPE.RESULT_OPTIMIZATION, level = 4 });
            _expression_op_codes.Add(TokenSubTypeEnum.I_LOGIC_OR, new op_code { code = OP_CODES.OP_OR, type = OP_TYPE.RESULT_OPTIMIZATION, level = 4 });

            _op_codes = new Dictionary<OP_CODES, op_code>();
            _op_codes.Add(OP_CODES.OP_JMP, new op_code { code = OP_CODES.OP_JMP, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_IFNOT, new op_code { code = OP_CODES.OP_IFNOT, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_NOT, new op_code { code = OP_CODES.OP_NOT, type = OP_TYPE.RESULT_OPTIMIZATION });
            //_op_codes.Add(OP_CODES.OP_NEG, new op_code { code = OP_CODES.OP_NEG, type = OP_TYPE.RESULT });
            _op_codes.Add(OP_CODES.OP_PUSH, new op_code { code = OP_CODES.OP_PUSH, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_POP, new op_code { code = OP_CODES.OP_POP, type = OP_TYPE.RESULT });
            _op_codes.Add(OP_CODES.OP_RETURN, new op_code { code = OP_CODES.OP_RETURN, type = OP_TYPE.RESULT });
            _op_codes.Add(OP_CODES.OP_STORE, new op_code { code = OP_CODES.OP_STORE, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_CALL, new op_code { code = OP_CODES.OP_CALL, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_OBJECT_CALL, new op_code { code = OP_CODES.OP_OBJECT_CALL, type = OP_TYPE.WO_RESULT });
            _op_codes.Add(OP_CODES.OP_OBJECT_RESOLVE_VAR, new op_code { code = OP_CODES.OP_OBJECT_RESOLVE_VAR, type = OP_TYPE.RESULT });
        }


        /// <summary>
        /// Установка точки входа модуля.
        /// </summary>
        private void SetModuleEntryPoint()
        {
            if (_module_entry_point == -1 && _scope.Type == ScopeTypeEnum.MODULE)
            {
                _module_entry_point = _current_module.ProgrammLine;
                _scope = new ScriptScope() { Type = ScopeTypeEnum.PROCEDURE, Module = _current_module, Name = "<<entry_point>>" };
                return;
            }

            if (_function_entry_point == -1 && (_scope.Type == ScopeTypeEnum.PROCEDURE || _scope.Type == ScopeTypeEnum.FUNCTION))
                _function_entry_point = _current_module.ProgrammLine;
        }


        /// <summary>
        /// Оптимизация кода, предварительный расчет где это возможно.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private Variable OptimizeCodes(op_code code, Variable left, Variable right)
        {
            VariableValue result = null;
            if (left.Value == null)
                return null;
            else
                if (left.Status != VariableStatusEnum.CONSTANTVARIABLE)
                return null;

            if (right != null)
            {
                if (right.Status != VariableStatusEnum.CONSTANTVARIABLE)
                    return null;

                if (right.Value == null && code.code != OP_CODES.OP_NOT)
                    return null;
            }

            switch (code.code)
            {
                case OP_CODES.OP_GT:
                    result = new VariableValue();
                    result = left.Value > right.Value;
                    break;
                case OP_CODES.OP_LT:
                    result = new VariableValue();
                    result = left.Value < right.Value;
                    break;
                case OP_CODES.OP_GE:
                    result = new VariableValue();
                    result = left.Value >= right.Value;
                    break;
                case OP_CODES.OP_LE:
                    result = new VariableValue();
                    result = left.Value <= right.Value;
                    break;
                case OP_CODES.OP_NOT:
                    result = new VariableValue();
                    result.Type = ValueTypeEnum.BOOLEAN;
                    result.Boolean = !left.Value.ToBoolean();
                    break;
                case OP_CODES.OP_OR:
                    result = new VariableValue();
                    result.Type = ValueTypeEnum.BOOLEAN;
                    result.Boolean = left.Value.ToBoolean() || right.Value.ToBoolean();
                    break;
                case OP_CODES.OP_AND:
                    result = new VariableValue();
                    result.Type = ValueTypeEnum.BOOLEAN;
                    result.Boolean = left.Value.ToBoolean() && right.Value.ToBoolean();
                    break;
                case OP_CODES.OP_EQ:
                    result = new VariableValue();
                    result.Type = ValueTypeEnum.BOOLEAN;
                    result.Boolean = left.Value == right.Value;
                    break;
                case OP_CODES.OP_UNEQ:
                    result = new VariableValue();
                    result.Type = ValueTypeEnum.BOOLEAN;
                    result.Boolean = left.Value != right.Value;
                    break;


                case OP_CODES.OP_MUL:
                    result = left.Value * right.Value;
                    break;
                case OP_CODES.OP_ADD:
                    result = left.Value + right.Value;
                    break;
                case OP_CODES.OP_SUB:
                    result = left.Value - right.Value;
                    break;
                case OP_CODES.OP_DIV:
                    result = left.Value / right.Value;
                    break;
            }

            if (result != null)
            {
                _programm.StaticVariableDelete(left);

                if (right != null)
                    _programm.StaticVariableDelete(right);

                return _programm.StaticVariableAdd(result);
            }

            return null;
        }

        /// <summary>
        /// Добавить команду к программе
        /// </summary>
        /// <param name="code"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private Variable EmitCode(OP_CODES code, Variable left, Variable right)
        {
            return EmitCode(_op_codes[code], left, right);
        }

        /// <summary>
        /// Добавить команду к программе
        /// </summary>
        /// <param name="code"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private Variable EmitCode(op_code code, Variable left, Variable right)
        {
            Variable result = null;

            if (code.type == OP_TYPE.RESULT_OPTIMIZATION)
            {
                result = OptimizeCodes(code, left, right);

                if (result != null)
                    return result;
                else
                    result = _current_module.VariableAdd("", false, _scope);
            }
            else
            if (code.type == OP_TYPE.RESULT)
                result = _current_module.VariableAdd("", false, _scope);

            if (left != null)
                left.Users++;
            if (right != null)
                right.Users++;

            ScriptStatement statement;
            statement = _current_module.StatementAdd();
            if (code.code != OP_CODES.OP_PUSH)
                statement.CodeInformation = _iterator.Current.CodeInformation.Clone();
            statement.OP_CODE = code.code;
            statement.Variable1 = result;
            statement.Variable2 = left;
            statement.Variable3 = right;


            return result;
        }

        #region Выражения 
        /// <summary>
        /// Парсер части выражения
        /// </summary>
        /// <returns></returns>
        private Variable ParseExpressionPart()
        {
            TokenClass sign;
            TokenClass not;

            Variable var = new Variable();

            // Проверка что есть знак перед переменной
            sign = _iterator.Current;
            if (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SUB) && !_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ADD))
                sign = null;

            // Проверка логическое НЕ
            not = _iterator.Current;
            if (!_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_NOT))
                not = null;


            TokenClass token = _iterator.Current;

            // Парсер значений в скобках.
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                var = ParseExpression((int)Priority.TOP);
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);
            }
            else
            {
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.NA))
                    var = GetVariable(token, FunctionTypeEnum.FUNCTION);
                else
                    var = ParseConstantVariable(token);
            }

            // Если есть знак то
            if (sign != null)
            {
                // Меняем знак у константы, если константа уже используется, создаем новую.
                if (var.Status == VariableStatusEnum.CONSTANTVARIABLE)
                {
                    if (var.Value.Type != ValueTypeEnum.NUMBER && var.Value.Type != ValueTypeEnum.FLOAT)
                        throw new ExceptionBase(_iterator.Current.CodeInformation, $"Неожиданный символ ({sign.Content}).");

                    if (sign.SubType == TokenSubTypeEnum.P_SUB)
                    {
                        if (var.Users > 1)
                            var = _programm.StaticVariableAdd(var.Value.Clone() * -1);
                        else
                            var.Value = var.Value * -1;
                    }
                }
                else
                    // В остальных случаях добавляем в код команду изменения знака.
                    var = EmitCode(_expression_op_codes[TokenSubTypeEnum.P_MUL], var, _programm.StaticVariableAdd(new VariableValue(-1)));
            }

            // Обработка логическое НЕ
            if (not != null)
                var = EmitCode(OP_CODES.OP_NOT, var, null);

            return var;

        }


        /// <summary>
        /// Парсер выражений
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private Variable ParseExpression(int level)
        {
            op_code work_code = new op_code();
            Variable left;
            Variable right;

            if (level == (int)Priority.LOW)
                return ParseExpressionPart();

            left = ParseExpression(level - 1);

            if (_iterator.Current.SubType == TokenSubTypeEnum.P_SEMICOLON)
                return left;

            if (_expression_op_codes.ContainsKey(_iterator.Current.SubType))
            {
                work_code = _expression_op_codes[_iterator.Current.SubType];
                if (work_code.level == level)
                    _iterator.MoveNext();
                else
                    return left;
            }
            else
                return left;

            right = ParseExpression(level);

            Console.Write($"{left.Value?.ToString()} - {work_code.code.ToString()} - {right.Value?.ToString()}\n");

            return EmitCode(work_code, left, right);
        }

        #endregion

        #region Переменные 

        /// <summary>
        /// Парсер статических данных указанных в коде.
        /// </summary>
        /// <returns></returns>
        private Variable ParseConstantVariable(TokenClass token)
        {
            ValueTypeEnum type = ValueTypeEnum.NULL;

            // Это число.
            if (_iterator.CheckToken(TokenTypeEnum.NUMBER))
                type = token.SubType == TokenSubTypeEnum.N_INTEGER ? ValueTypeEnum.NUMBER : ValueTypeEnum.FLOAT;

            // Это строка.
            else if (_iterator.CheckToken(TokenTypeEnum.LITERAL, TokenSubTypeEnum.L_STRING))
                type = ValueTypeEnum.STRING;

            // Это дата.
            else if (_iterator.CheckToken(TokenTypeEnum.LITERAL, TokenSubTypeEnum.L_DATE))
                type = ValueTypeEnum.DATE;

            // Это логические константы.
            else if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_TRUE) || _iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_FALSE))
                type = ValueTypeEnum.BOOLEAN;

            // Это Null
            else if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.N_NULL))
                type = ValueTypeEnum.NULL;

            VariableValue value = new VariableValue(type, token.Content);

            return _programm.StaticVariableAdd(value);
        }

        /// <summary>
        /// Получить переменную из программы.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private Variable GetVariable(TokenClass token, FunctionTypeEnum function_type, bool create = false)
        {
            Variable var;

            // Проверка что это переменная в функции.
            var = _current_module.VariableGet(token.Content, _scope);
            if (var != null)
                return var;

            // Проверка что это переменная в модуле.
            var = _current_module.VariableGet(token.Content);
            if (var != null)
                return var;

            // Проверка что это глобальная переменная.
            var = _programm.GlobalVariableGet(token.Content);
            if (var != null)
                return var;

            // Проверка что это вызов функции или процедуры, в зависимости от контекста.
            Function function = null;
            if (ParseFunctionCall(token, function_type, ref var, ref function))
                if (_iterator.Current.Type != TokenTypeEnum.PUNCTUATION || _iterator.Current.SubType != TokenSubTypeEnum.P_DOT)
                    return var;

            // Проверка что это обращение к обьекту, в зависимости от контекста.
            if (ParseObjectCall(token, function_type, var, ref var))
            {
                // Проверить если процедура используется как функция.
                if (function != null)
                    function.Type = FunctionTypeEnum.FUNCTION;
                return var;
            }

            // Если не найдена, то создать новую для последующей проверки. 
            // Переменная может не существовать только в том случае, если она глобальная и обьявлена в модуле, который еще не обработан.
            if (var == null && !create)
            {
                if (!_deferred_var.ContainsKey(token.Content))
                {
                    var = new Variable
                    {
                        Name = token.Content,
                        Scope = _programm.GlobalScope
                    };
                    _deferred_var[var.Name] = var;
                }
                else
                    var = _deferred_var[token.Content];
            }

            if (!create && var == null)
                throw new ExceptionBase(token.CodeInformation, $"Переменная не определена ({token.Content})");

            if (create)
                return _current_module.VariableAdd(token.Content, false, _scope);
            else
                return var;
        }

        /// <summary>
        /// Добавить имя переменной в список для последующей обработки. Обработка конструкций вида: Перем а1,а2 экспорт;
        /// </summary>
        /// <param name="vars"></param>
        private void AddVariableDefineToList(IList<TokenClass> vars)
        {
            TokenClass var_name = _iterator.Current;
            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            vars.Add(var_name);

            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                AddVariableDefineToList(vars);
        }

        /// <summary>
        /// Парсер переменных Перем (Var)
        /// </summary>
        /// <param name="vars"></param>
        private bool ParseVariableDefine()
        {
            bool export = false;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_VARDEF))
            {
                // Проверка порядка расположения.
                if (_function_entry_point == -1 && _module_entry_point != -1)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Объявления переменных должны быть расположены в начале модуля, процедуры или функции.");
                else
                {
                    if (_function_entry_point != -1)
                        throw new ExceptionBase(_iterator.Current.CodeInformation, "Объявления переменных должны быть расположены в начале модуля, процедуры или функции.");
                }


                // Забрать имена переменных
                IList<TokenClass> vars = new List<TokenClass>();
                AddVariableDefineToList(vars);

                // Проверка ключевого слова Экспорт.
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EXPORT))
                {
                    // Обьвление находится в функции.
                    if (_scope.Type != ScopeTypeEnum.MODULE)
                        throw new ExceptionBase("Локальные переменные не могут быть экспортированы.");

                    export = true;
                }

                // Если есть ключевое слово Экспорт, тогда в зависимости от типа модуля и его параметров, добовляем переменную в глобальный модуль или делаем ее "публичной", доступной для обращения через обьект этого модуля.
                foreach (TokenClass var in vars)
                {
                    if (export)
                    {
                        if (_current_module.VariableGet(var.Content) != null)
                            throw new ExceptionBase(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");

                        if (_current_module.AsGlobal)
                        {
                            if (_programm.GlobalVariableAdd(var.Content) == null)
                                throw new ExceptionBase(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
                        }
                        else
                         if (_current_module.VariableAdd(var.Content, true, _scope) == null)
                            throw new ExceptionBase(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
                    }
                    else
                    {
                        if (_programm.GlobalVariableGet(var.Content) != null)
                            throw new ExceptionBase(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");

                        if (_current_module.VariableAdd(var.Content, false, _scope) == null)
                            throw new ExceptionBase(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
                    }
                }

                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
                vars.Clear();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Парсер присвоения значений переменным =, а также вызовов функций и методов.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private void Parse(TokenClass token)
        {
            Variable right, left;

            _iterator.MoveNext();
            // Установить точку входа модуля.
            SetModuleEntryPoint();
            // Забрать левую часть выражения. Возможные варианты: переменная, вызов функции, вызов обьекта.
            left = GetVariable(token, FunctionTypeEnum.PROCEDURE, true);
            // Если есть присвоение
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ASSIGN))
            {
                // Забрать правую часть выражения. Возможные варианты: переменная, вызов функции, вызов обьекта, выражение.
                right = ParseExpression((int)Priority.TOP);
                // Добавить в код модуля присвоение значения.
                EmitCode(OP_CODES.OP_STORE, left, right);
            }
            else
                // Увеличить количество использований, для того что бы переменная могла быть использована в другом месте.
                if (left.Users == 1)
                left.Users++;


            _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
        }


        /// <summary>
        /// Обработка отложенных переменных. Переменные которые были вызваны но еще не объявлены.
        /// </summary>
        public void ProcessDifferedVars()
        {
            Variable var = null;

            foreach (KeyValuePair<string, Variable> var_kp in _deferred_var)
            {
                // глобальный контекст
                if (var_kp.Value.Scope.Type == ScopeTypeEnum.GLOBAL)
                {
                    var = _programm.GlobalVariableGet(var_kp.Value.Name);
                    var_kp.Value.StackNumber = var.StackNumber;
                }
                else
                    throw new ExceptionBase($"Переменная не определена [{var_kp.Value.Name}].");
            }

            _deferred_var.Clear();
        }

        #endregion

        #region Процедуры и функции

        /// <summary>
        /// Парсер идентификатора Возврат ( return ).
        /// </summary>
        /// <returns></returns>
        private bool ParseReturn()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_RETURN))
            {
                Variable return_var;

                // Проверка что Возврат внутри функции/процедуры.
                if (_scope.Type == ScopeTypeEnum.MODULE || _scope.Type == ScopeTypeEnum.GLOBAL)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Оператор Возврат (Return) не может употребляться вне процедуры или функции.");

                if (_iterator.Current.Type != TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType != TokenSubTypeEnum.P_SEMICOLON)
                {
                    // Возврат содержит выражение.
                    if (_scope.Type == ScopeTypeEnum.PROCEDURE)
                        throw new ExceptionBase(_iterator.Current.CodeInformation, "Процедура не может возвращать значение.");

                    return_var = ParseExpression((int)Priority.TOP);

                    EmitCode(OP_CODES.OP_RETURN, return_var, null);
                }
                else
                {
                    // Слово Возврат не содержит выражения.
                    if (_scope.Type == ScopeTypeEnum.FUNCTION)
                        throw new ExceptionBase(_iterator.Current.CodeInformation, "Функция должна возвращать результат.");
                    EmitCode(OP_CODES.OP_RETURN, null, null);
                }

                return true;
            }
            return false;
        }



        /// <summary>
        /// Парсер параметров процедуры или функции.
        /// </summary>
        /// <param name="param_list"></param>
        private void ParseFunctionDefineParam(IList<Variable> param_list)
        {
            Variable var;
            VariableStatusEnum var_status = VariableStatusEnum.STACKVARIABLE;

            // Парсер ключевого слова Знач, передача параметра по значению.
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_VAL))
                var_status = VariableStatusEnum.CONSTANTVARIABLE;

            TokenClass param_name = _iterator.Current;
            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            // Парсер параметра с значением по умолчанию.
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ASSIGN))
            {
                var = ParseConstantVariable(_iterator.Current);
                if (var == null)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, $"Ожидается константа типа Число, Строка, Дата или Булево.");
                var.Name = param_name.Content;
            }
            else
            {
                var = new Variable()
                {
                    Name = param_name.Content,
                    Scope = _scope,
                    Status = var_status,
                    Users = 1
                };
            }

            param_list.Add(var);


            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                ParseFunctionDefineParam(param_list);
        }

        /// <summary>
        /// Парсер объявлений процедуры и функции.
        /// </summary>
        /// <returns></returns>
        private bool ParseFunctionDefine()
        {
            TokenClass type;
            type = _iterator.Current;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_PROCEDURE) || _iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_FUNCTION))
            {
                TokenClass function_name;
                ScriptScope old_scope = _scope;

                // Проверка где обьявили.
                if (_scope.Type != ScopeTypeEnum.MODULE)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Процедуры и функциии могут распологаться только в теле модуля.");

                // Проверка порядка расположения вызовов и обьявлений в модуле.
                if (_module_entry_point != -1)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Определения процедур и функций должны размещаться перед операторами тела модуля.");

                _function_entry_point = -1;

                // Информация о функции, имя и расположение в коде.
                function_name = _iterator.Current;
                function_name.CodeInformation = _iterator.Current.CodeInformation.Clone();

                _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);
                _iterator.MoveNext();

                _scope = new ScriptScope()
                {
                    Type = type.SubType == TokenSubTypeEnum.I_PROCEDURE ? ScopeTypeEnum.PROCEDURE : ScopeTypeEnum.FUNCTION,
                    Module = _current_module,
                    Name = function_name.Content
                };

                // Блок обработки параметров.
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN);
                IList<Variable> param_list = new List<Variable>();
                if (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                {
                    ParseFunctionDefineParam(param_list);
                    _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);
                }

                Function function;

                // Проверка дубляжа.
                if (_programm.GlobalFunctionGet(function_name.Content) != null || _current_module.FunctionGet(function_name.Content) != null)
                    throw new ExceptionBase(function_name.CodeInformation, $"Процедура или функция с указанным именем уже определена ({function_name.Content})");

                // Если есть ключевое слово Экспорт. Тогда в зависимости от типа модуля и его параметров, добовляем функцию в глобальный модуль или делаем ее "публичной", доступной для обращения через обьект этого модуля.
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EXPORT))
                {
                    if (_current_module.AsGlobal)
                        function = _programm.GlobalFunctionAdd(function_name.Content);
                    else
                        function = _current_module.FunctionAdd(function_name.Content, true, old_scope);
                }
                else
                    function = _current_module.FunctionAdd(function_name.Content, false, old_scope);

                // Добавляю параметры функции в ее контекст.
                foreach (Variable var in param_list)
                {
                    Variable tmp_var = _current_module.VariableAdd(var.Name, false, _scope, var.Value);
                    var.StackNumber = tmp_var.StackNumber;
                }

                function.EntryPoint = _current_module.ProgrammLine;
                function.Param = param_list;
                function.Type = type.SubType == TokenSubTypeEnum.I_PROCEDURE ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                function.CodeInformation = function_name.CodeInformation;
                function.Scope = _scope;

                // Парсим все содержимое функции/процедуры, до ключевого слова КонецФункции/Процедуры.
                if (function.Type == FunctionTypeEnum.PROCEDURE)
                    ParseModuleBody(TokenSubTypeEnum.I_ENDPROCEDURE);
                else
                    ParseModuleBody(TokenSubTypeEnum.I_ENDFUNCTION);

                // Добавляю в код модуля выход из функции.
                EmitCode(OP_CODES.OP_RETURN, null, null);

                // Проверка ключевого слова КонецФункции/Процедуры.
                if (function.Type == FunctionTypeEnum.PROCEDURE)
                    _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDPROCEDURE);
                else
                    _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDFUNCTION);

                _scope = old_scope;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Проверка параметров вызываемой функции, на соответствие прототипу.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="param"></param>
        /// <param name="module"></param>
        private void CheckFunctionCall(Function function)
        {
            Function work_function;

            // Ищем в модуле
            work_function = function.Scope.Module.FunctionGet(function.Name);

            // Ищем в глобальном модуле
            if (work_function == null)
                work_function = _programm.GlobalFunctionGet(function.Name);

            if (work_function == null)
                throw new ExceptionBase(function.CodeInformation, $"Процедура или функция с именем [{function.Name}] не определена.");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new ExceptionBase(function.CodeInformation, $"Обращение к процедуре [{function.Name}] как к функции.");

            if (function.Param.Count == work_function.Param.Count)
                return;

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

            // Патч вызова функции. Указываю правильный модуль.
            if (work_function.Scope.Module != function.Scope.Module)
            {
                ScriptStatement statement = _current_module.StatementGet(function.EntryPoint);
                statement.Variable3 = _programm.StaticVariableAdd(new VariableValue(ValueTypeEnum.STRING, work_function.Scope.Name));
            }
        }

        /// <summary>
        /// Парсер параметров у вызываемой функции.
        /// </summary>
        /// <param name="param"></param>
        private void ParseFunctionCallParam(Function function)
        {
            TokenClass token;
            token = _iterator.Current;

            Variable var = null;

            do
            {
                // Обработка пустого параметра. Кода стоит просто запятая.
                if (_iterator.Current.Type == TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType == TokenSubTypeEnum.P_COMMA)
                    var = new Variable() { Scope = _scope, Name = "<<null>>", Status = VariableStatusEnum.CONSTANTVARIABLE, Value = new VariableValue(ValueTypeEnum.NULL, "") };
                else
                    // Получить параметр функции, может быть любое выражение или вызов.
                    var = ParseExpression((int)Priority.TOP);

                function.Param.Add(var);
            } while (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA));

        }

        /// <summary>
        /// Парсер вызовов функций.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool ParseFunctionCall(TokenClass token, FunctionTypeEnum function_type, ref Variable result, ref Function function)
        {
            result = null;

            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                function = new Function()
                {
                    Type = function_type,
                    Name = token.Content,
                    Param = new List<Variable>(),
                    Scope = _scope,
                    CodeInformation = token.CodeInformation.Clone()
                };

                while (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                    ParseFunctionCallParam(function);

                // Добавить в отложенные функции для последующей проверки.
                _deferred_function[token.Content + "_" + _scope.Name + "_" + function.Param?.Count] = function;

                // Добавить в код передачу параметров через стек.
                foreach (Variable var in function.Param)
                    EmitCode(OP_CODES.OP_PUSH, var, null);

                Variable function_name = _programm.StaticVariableAdd(new VariableValue(ValueTypeEnum.STRING, function.Name));

                // Добавить в код вызов функции из модуля по ее имени. 
                function.EntryPoint = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_CALL, function_name, null);
                result = EmitCode(OP_CODES.OP_POP, null, null);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Обработка отложенных функций.
        /// </summary>
        public void ProcessDifferedFunctions()
        {
            foreach (KeyValuePair<string, Function> function_vp in _deferred_function)
            {
                CheckFunctionCall(function_vp.Value);
            }

            _deferred_function.Clear();
        }

        #endregion

        #region Обьекты

        /// <summary>
        /// Парсер вызовов функций обьекта.
        /// </summary>
        private bool ParseObjectFunctionCall(TokenClass token, Variable object_call, FunctionTypeEnum function_type, ref Variable result, ref Function function)
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                // Прототип функции, для последующего сравнения с оригиналом.
                function = new Function()
                {
                    Type = function_type,
                    Name = token.Content,
                    Param = new List<Variable>(),
                    Scope = _scope,
                    CodeInformation = token.CodeInformation.Clone()
                };

                // Обработка передачи параметров.
                while (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                    ParseFunctionCallParam(function);

                // Добавляю прототип в хранилище.
                int function_number = _current_module.ObjectCallAdd(function);

                foreach (Variable var in function.Param)
                    EmitCode(OP_CODES.OP_PUSH, var, null);

                Variable function_name_var = _programm.StaticVariableAdd(new VariableValue(function_number));

                // Вызов функции обьекта.
                EmitCode(OP_CODES.OP_OBJECT_CALL, object_call, function_name_var);
                result = EmitCode(OP_CODES.OP_POP, null, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Парсинг частей, разделенных точкой, вызова обьекта. 
        /// </summary>
        /// <param name="object_call"></param>
        private Variable ParseObjectCallParts(Variable object_call, FunctionTypeEnum function_type)
        {
            TokenClass token;
            Variable var = null;
            Function function = null;

            do
            {
                token = _iterator.Current;
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER))
                {
                    // Обращение к методу или свойству.
                    if (!ParseObjectFunctionCall(token, object_call, function_type, ref object_call, ref function))
                    {
                        var = _programm.StaticVariableAdd(new VariableValue(ValueTypeEnum.STRING, token.Content));
                        object_call = EmitCode(OP_CODES.OP_OBJECT_RESOLVE_VAR, object_call, var);

                        // Проверка и изменение типа вызова обьекта, с процедуры на функцию.
                        if (function != null)
                        {
                            function.Type = FunctionTypeEnum.FUNCTION;
                            function = null;
                        }
                    }
                }
            } while (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_DOT));

            return object_call;
        }


        /// <summary>
        /// Парсинг обращения к обьекту.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="function_type"></param>
        /// <param name="var"></param>
        /// <param name="return_var"></param>
        /// <returns></returns>
        private bool ParseObjectCall(TokenClass token, FunctionTypeEnum function_type, Variable var, ref Variable return_var)
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_DOT))
            {
                // Если переменной еще нет, то отложить ее проверку и создать временную переменную.
                if (var == null)
                {
                    if (!_deferred_var.ContainsKey(token.Content))
                    {
                        var = new Variable
                        {
                            Status = VariableStatusEnum.STACKVARIABLE,
                            Name = token.Content,
                            Scope = _programm.GlobalScope
                        };
                        _deferred_var[var.Name] = var;
                    }
                    else
                        var = _deferred_var[token.Content];
                }

                // Парсинг частей вызова, разделенных точкой.
                return_var = ParseObjectCallParts(var, function_type);
                return true;
            }
            return false;
        }

        #endregion

        #region Ключивые слова Если,Для

        /// <summary>
        /// Парсин ключевого слова Если, ИначеЕсли.
        /// </summary>
        /// <returns></returns>
        private bool ParseIf()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_IF))
            {
                int line;
                ScriptStatement statement;
                Variable expression = null;
                IList<int> jmp = new List<int>();

                line = int.MinValue;

                // Установить точку входа модуля.
                SetModuleEntryPoint();

                do
                {
                    if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_THEN))
                        throw new ExceptionBase(_iterator.Current.CodeInformation, "Ожидается выражение.");

                    expression = ParseExpression((int)Priority.TOP);

                    // Установка перехода для предыдущего условия.
                    if (line != int.MinValue)
                    {
                        statement = _current_module.StatementGet(line);
                        statement.Variable3 = _programm.StaticVariableAdd(new VariableValue(_current_module.ProgrammLine));
                    }

                    line = _current_module.ProgrammLine;
                    EmitCode(OP_CODES.OP_IFNOT, expression, null);


                    _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_THEN);

                    // Идентификаторы для остановки парсинга.
                    HashSet<TokenSubTypeEnum> stop = new HashSet<TokenSubTypeEnum>()
                    {
                        TokenSubTypeEnum.I_ELSE,
                        TokenSubTypeEnum.I_ELSEIF,
                        TokenSubTypeEnum.I_ENDIF
                    };

                    // Если КонецЕсли или конец файла, то выход из цикла.
                    if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDIF) || _iterator.Current.SubType == TokenSubTypeEnum.EOF)
                        break;

                    ParseModuleBody(stop);

                    // Выход после выполнения кода условия.
                    jmp.Add(_current_module.ProgrammLine);
                    EmitCode(OP_CODES.OP_JMP, null, null);

                    // Блок Если
                    if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ELSE))
                    {
                        // Фикс перехода с предыдущего Если или ИначеЕсли
                        statement = _current_module.StatementGet(line);
                        statement.Variable3 = _programm.StaticVariableAdd(new VariableValue(_current_module.ProgrammLine));
                        line = int.MinValue;

                        ParseModuleBody(TokenSubTypeEnum.I_ENDIF);
                        _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDIF);
                        break;
                    }

                } while (_iterator.MoveNext());

                // Для последнего или первого (единственного) Если, установить переход.
                if (line != int.MinValue)
                {
                    statement = _current_module.StatementGet(line);
                    statement.Variable3 = _programm.StaticVariableAdd(new VariableValue(_current_module.ProgrammLine));
                }

                // Установить Jmp актуальную строку кода для выхода.
                foreach (int index in jmp)
                {
                    statement = _current_module.StatementGet(index);
                    statement.Variable2 = _programm.StaticVariableAdd(new VariableValue(_current_module.ProgrammLine));
                }

                return true;
            }
            return false;
        }

        #endregion


        /// <summary>
        /// Парсинг тела функции, модуля.
        /// </summary>
        private void ParseModuleBody(TokenSubTypeEnum stop_type = TokenSubTypeEnum.EOF)
        {
            HashSet<TokenSubTypeEnum> stop_hash_set = new HashSet<TokenSubTypeEnum>() { stop_type };
            ParseModuleBody(stop_hash_set);
        }


        /// <summary>
        /// Парсинг тела функции, модуля.
        /// </summary>
        private void ParseModuleBody(HashSet<TokenSubTypeEnum> stop_type)
        {
            TokenClass token;
            do
            {
                if (stop_type.Contains(_iterator.Current.SubType))
                    return;

                _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);
                token = _iterator.Current;

                // Парсим обьявление переменных, ключевое слово Перем.
                if (ParseVariableDefine())
                    continue;

                // Парсим обьявление функций и процедур.
                if (ParseFunctionDefine())
                    continue;

                // Парсим ключевое слово Возврат (return).
                if (ParseReturn())
                {
                    _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
                    continue;
                }

                if (ParseIf())
                    continue;

                // Парсим вызов методов, свойств, а так же присвоение значений.
                Parse(token);

            }
            while (_iterator.MoveNext() && !stop_type.Contains(_iterator.Current.SubType));
        }


        /// <summary>
        /// Компилирует группу модулей в программу.
        /// </summary>
        /// <param name="modules"></param>
        /// <returns></returns>
        public ScriptProgramm Compile(IDictionary<ScriptModule, string> modules)
        {
            ParserClass parser;
            Dictionary<string, bool> defines = new Dictionary<string, bool>();
            defines.Add("НаКлиенте", true);

            foreach (KeyValuePair<ScriptModule, string> module in modules)
            {
                parser = new ParserClass(module.Key.Name, module.Value);
                PrecompilerClass precompiler = new PrecompilerClass(parser.GetEnumerator(), defines);
                _iterator = precompiler.GetEnumerator();

                _module_entry_point = -1;
                _function_entry_point = -1;
                _current_module = module.Key;
                _scope = _current_module.ModuleScope;

                _iterator.MoveNext();
                ParseModuleBody();

                // Виртуальная функция, код модуля.
                Function entry_point = _current_module.FunctionAdd("<<entry_point>>");
                entry_point.Type = FunctionTypeEnum.PROCEDURE;
                entry_point.EntryPoint = _module_entry_point;
                entry_point.Scope = _scope;

                EmitCode(OP_CODES.OP_RETURN, null, null);

                _programm.ModuleAdd(_current_module);

                if (_iterator.Current.Type != TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType != TokenSubTypeEnum.EOF)
                    throw new ExceptionBase("Есть не разобранный участок кода.");
            }

            ProcessDifferedVars();
            ProcessDifferedFunctions();
            return _programm;
        }

    }
}
