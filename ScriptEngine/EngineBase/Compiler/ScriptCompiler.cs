using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Parser.Precompiler;
using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Types;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser;
using System.Collections.Generic;
using System;

namespace ScriptEngine.EngineBase.Compiler
{
    public class ScriptCompiler
    {
        private TokenIteratorBase _iterator;
        private ScriptProgramm _programm;
        private ScriptScope _scope;
        private ScriptModule _current_module;
        private IDictionary<string, IVariable> _deferred_var;
        private IList<(ScriptModule, IFunction)> _deferred_function;
        private IList<IList<int>> _loop;

        private IList<(string, int)> _goto_jmp;
        private IDictionary<string, int> _goto_labels;

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
            _deferred_var = new Dictionary<string, IVariable>();
            _deferred_function = new List<(ScriptModule, IFunction)>();
            _loop = new List<IList<int>>();
            _goto_jmp = new List<(string, int)>();
            _goto_labels = new Dictionary<string, int>();

            _expression_op_codes = new Dictionary<TokenSubTypeEnum, op_code>
            {
                { TokenSubTypeEnum.P_MUL, new op_code { code = OP_CODES.OP_MUL, type = OP_TYPE.RESULT_OPTIMIZATION, level = 1 } },
                { TokenSubTypeEnum.P_MOD, new op_code { code = OP_CODES.OP_MOD, type = OP_TYPE.RESULT_OPTIMIZATION, level = 1 } },
                { TokenSubTypeEnum.P_DIV, new op_code { code = OP_CODES.OP_DIV, type = OP_TYPE.RESULT_OPTIMIZATION, level = 1 } },
                { TokenSubTypeEnum.P_ADD, new op_code { code = OP_CODES.OP_ADD, type = OP_TYPE.RESULT_OPTIMIZATION, level = 2 } },
                { TokenSubTypeEnum.P_SUB, new op_code { code = OP_CODES.OP_SUB, type = OP_TYPE.RESULT_OPTIMIZATION, level = 2 } },
                { TokenSubTypeEnum.P_LOGIC_GREATER, new op_code { code = OP_CODES.OP_GT, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.P_LOGIC_LESS, new op_code { code = OP_CODES.OP_LT, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.P_LOGIC_GEQ, new op_code { code = OP_CODES.OP_GE, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.P_LOGIC_LEQ, new op_code { code = OP_CODES.OP_LE, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.P_LOGIC_UNEQ, new op_code { code = OP_CODES.OP_UNEQ, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.P_ASSIGN, new op_code { code = OP_CODES.OP_EQ, type = OP_TYPE.RESULT_OPTIMIZATION, level = 3 } },
                { TokenSubTypeEnum.I_LOGIC_AND, new op_code { code = OP_CODES.OP_AND, type = OP_TYPE.RESULT_OPTIMIZATION, level = 4 } },
                { TokenSubTypeEnum.I_LOGIC_OR, new op_code { code = OP_CODES.OP_OR, type = OP_TYPE.RESULT_OPTIMIZATION, level = 4 } }
            };

            _op_codes = new Dictionary<OP_CODES, op_code>
            {
                { OP_CODES.OP_RAISE, new op_code { code = OP_CODES.OP_RAISE, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_ENDTRY, new op_code { code = OP_CODES.OP_ENDTRY, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_TRY, new op_code { code = OP_CODES.OP_TRY, type = OP_TYPE.WO_RESULT } },

                { OP_CODES.OP_ITR_STOP, new op_code { code = OP_CODES.OP_ITR_STOP, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_ITR_GET, new op_code { code = OP_CODES.OP_ITR_GET, type = OP_TYPE.RESULT } },
                { OP_CODES.OP_ITR_NEXT, new op_code { code = OP_CODES.OP_ITR_NEXT, type = OP_TYPE.WO_RESULT } },

                { OP_CODES.OP_ARRAY_GET, new op_code { code = OP_CODES.OP_ARRAY_GET, type = OP_TYPE.RESULT } },

                { OP_CODES.OP_NEW, new op_code { code = OP_CODES.OP_NEW, type = OP_TYPE.RESULT } },

                { OP_CODES.OP_ADD, new op_code { code = OP_CODES.OP_ADD, type = OP_TYPE.RESULT_OPTIMIZATION } },
                { OP_CODES.OP_GE, new op_code { code = OP_CODES.OP_GE, type = OP_TYPE.RESULT_OPTIMIZATION } },
                { OP_CODES.OP_JMP, new op_code { code = OP_CODES.OP_JMP, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_IFNOT, new op_code { code = OP_CODES.OP_IFNOT, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_NOT, new op_code { code = OP_CODES.OP_NOT, type = OP_TYPE.RESULT_OPTIMIZATION } },
                { OP_CODES.OP_PUSH, new op_code { code = OP_CODES.OP_PUSH, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_POP, new op_code { code = OP_CODES.OP_POP, type = OP_TYPE.RESULT } },
                { OP_CODES.OP_RETURN, new op_code { code = OP_CODES.OP_RETURN, type = OP_TYPE.RESULT } },
                { OP_CODES.OP_STORE, new op_code { code = OP_CODES.OP_STORE, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_CALL, new op_code { code = OP_CODES.OP_CALL, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_OBJ_CALL, new op_code { code = OP_CODES.OP_OBJ_CALL, type = OP_TYPE.WO_RESULT } },
                { OP_CODES.OP_OBJ_GET_VAR, new op_code { code = OP_CODES.OP_OBJ_GET_VAR, type = OP_TYPE.RESULT } }
            };
        }


        /// <summary>
        /// Установка точки входа модуля.
        /// </summary>
        private void SetModuleEntryPoint()
        {
            if (_module_entry_point == -1 && _scope.Type == ScopeTypeEnum.MODULE)
            {
                _module_entry_point = _current_module.ProgrammLine;
                _scope = new ScriptScope() { Type = ScopeTypeEnum.PROCEDURE, Module = _current_module, Name = "<<entry_point>>", StackIndex = 2 };

                if (_current_module.Type == ModuleTypeEnum.COMMON)
                    throw new Exception($"Данный [{_current_module.Name}] модуль может содержать только определения процедур и функций");

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
        private IVariable OptimizeCodes(op_code code, IVariable left, IVariable right)
        {
            IValue result = null;
            if (left.Value == null)
                return null;
            else
                if (left.Type != VariableTypeEnum.CONSTANTVARIABLE)
                return null;

            if (right != null)
            {
                if (right.Type != VariableTypeEnum.CONSTANTVARIABLE)
                    return null;

                if (right.Value == null && code.code != OP_CODES.OP_NOT)
                    return null;
            }

            try
            {
                switch (code.code)
                {
                    case OP_CODES.OP_GT:
                        result = ValueFactory.GT(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_LT:
                        result = ValueFactory.LT(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_GE:
                        result = ValueFactory.GE(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_LE:
                        result = ValueFactory.LE(left.Value, right.Value);
                        break;

                    case OP_CODES.OP_MUL:
                        result = ValueFactory.MUL(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_ADD:
                        result = ValueFactory.ADD(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_SUB:
                        result = ValueFactory.SUB(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_DIV:
                        result = ValueFactory.DIV(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_MOD:
                        result = ValueFactory.MOD(left.Value, right.Value);
                        break;


                    case OP_CODES.OP_NOT:
                        result = ValueFactory.Create(!left.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_OR:
                        result = ValueFactory.Create(left.Value.AsBoolean() || right.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_AND:
                        result = ValueFactory.Create(left.Value.AsBoolean() && right.Value.AsBoolean());
                        break;
                    case OP_CODES.OP_EQ:
                        result = ValueFactory.EQ(left.Value, right.Value);
                        break;
                    case OP_CODES.OP_UNEQ:
                        result = ValueFactory.UNEQ(left.Value, right.Value);
                        break;

                    default:
                        return null;
                }
            }
            catch (Exception ex)
            {
                throw new CompilerException(_iterator.Current.CodeInformation, ex.Message);
            }

            if (right != null)
            {
                right.Users--;
                if (right.Users <= 0)
                    _programm.StaticVariables.Delete(right);

            }

            left.Users--;
            if (left.Users <= 0)
                _programm.StaticVariables.Delete(left);

            return _programm.StaticVariables.Create(result);

        }

        /// <summary>
        /// Добавить команду к программе
        /// </summary>
        /// <param name="code"></param>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        private IVariable EmitCode(OP_CODES code, IVariable left, IVariable right)
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
        private IVariable EmitCode(op_code code, IVariable left, IVariable right)
        {
            IVariable result = null;

            if (code.type == OP_TYPE.RESULT_OPTIMIZATION)
            {
                result = OptimizeCodes(code, left, right);

                if (result != null)
                    return result;
                else
                    result = _current_module.Variables.Create("", false, _scope);
            }
            else
            if (code.type == OP_TYPE.RESULT)
                result = _current_module.Variables.Create("", false, _scope);

            if (left != null && left.Type != VariableTypeEnum.CONSTANTVARIABLE)
                left.Users++;
            if (right != null && right.Type != VariableTypeEnum.CONSTANTVARIABLE)
                right.Users++;

            ScriptStatement statement;
            statement = _current_module.StatementAdd();
            if (code.code != OP_CODES.OP_PUSH)
                statement.Line = _iterator.Current.CodeInformation.LineNumber;
            statement.OP_CODE = code.code;
            statement.Variable1 = result;
            statement.Variable2 = left;
            statement.Variable3 = right;




            return result;
        }

        #region Исключения
        private bool ParseRaise()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_RAISE))
            {
                // Установить точку входа модуля.
                SetModuleEntryPoint();

                bool parentheses = false;
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
                    parentheses = true;

                // Забрать выражение.
                IVariable exception = ParseExpression((int)Priority.TOP);
                if (exception == null || exception.Value?.Type == ValueTypeEnum.NULL)
                    throw new CompilerException(_iterator.Current.CodeInformation, "Оператор ВызватьИсключение (Raise) без аргументов может употребляться только при обработке исключения.");

                if (parentheses)
                    _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);

                // Добавить в код модуля вызов исключения.
                EmitCode(OP_CODES.OP_RAISE, exception, null);

                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Парсинг блок Попытка (try) 
        /// </summary>
        /// <returns></returns>
        private bool ParseTryCatch()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_TRY))
            {
                int try_index, exit_try;
                ScriptStatement statement;

                // Установить точку входа модуля.
                SetModuleEntryPoint();

                // Парсинг тела блока попытка.
                try_index = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_TRY, null, null);
                ParseModuleBody(TokenSubTypeEnum.I_EXCEPT);
                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EXCEPT);


                exit_try = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_JMP, null, null);
                // Патч перехода в случае исключения.
                statement = _current_module.StatementGet(try_index);
                statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));

                // Парсинг тела блока исключение.
                ParseModuleBody(TokenSubTypeEnum.I_ENDTRY);
                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDTRY);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

                // Патч перехода в случае выполнения без ошибки.
                statement = _current_module.StatementGet(exit_try);
                statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                EmitCode(OP_CODES.OP_ENDTRY, null, null);

                return true;
            }
            return false;
        }
        #endregion

        #region Выражения 
        /// <summary>
        /// Парсер части выражения
        /// </summary>
        /// <returns></returns>
        private IVariable ParseExpressionPart()
        {
            IToken sign;
            IToken not;

            IVariable var = new Variable();

            // Проверка что есть знак перед переменной
            sign = _iterator.Current;
            if (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SUB) && !_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ADD))
                sign = null;

            // Проверка логическое НЕ
            not = _iterator.Current;
            if (!_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_NOT))
                not = null;


            IToken token = _iterator.Current;

            // Парсер "короткий" Если.
            var = ParseIfShort();

            if (var == null)
                var = ParseNew();

            // Парсер значений в скобках.
            if (var == null)
            {
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
            }

            // Если есть знак то
            if (sign != null)
            {
                // Меняем знак у константы, если константа уже используется, создаем новую.
                if (var.Type == VariableTypeEnum.CONSTANTVARIABLE)
                {
                    if (var.Value.Type != ValueTypeEnum.NUMBER)
                        throw new CompilerException(_iterator.Current.CodeInformation, $"Неожиданный символ ({sign.Content}).");
                }

                if (sign.SubType == TokenSubTypeEnum.P_SUB)
                    var = EmitCode(_expression_op_codes[TokenSubTypeEnum.P_MUL], var, _programm.StaticVariables.Create(ValueFactory.Create(-1)));
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
        private IVariable ParseExpression(int level)
        {
            op_code work_code = new op_code();
            IVariable left;
            IVariable right;

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

            return EmitCode(work_code, left, right);
        }

        #endregion

        #region Переменные 

        /// <summary>
        /// Парсер статических данных указанных в коде.
        /// </summary>
        /// <returns></returns>
        private IVariable ParseConstantVariable(IToken token)
        {
            Nullable<ValueTypeEnum> type = null;

            // Это число.
            if (_iterator.CheckToken(TokenTypeEnum.NUMBER))
                type = ValueTypeEnum.NUMBER;

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

            if (type == null)
                throw new CompilerException(_iterator.Current.CodeInformation, $"Не удалось создать константу {_iterator.Current}.");

            return _programm.StaticVariables.Create(ValueFactory.Create(type.Value, token.Content));
        }

        /// <summary>
        /// Парсинг массивов и объектов. 
        /// </summary>
        /// <param name="token"></param>
        /// <param name="function_type"></param>
        /// <param name="var"></param>
        /// <param name="function"></param>
        /// <returns></returns>
        private IVariable ParseArrayAndObject(IToken token, FunctionTypeEnum function_type, IVariable var, IFunction function)
        {
            do
            {
                // Проверка что это обращение к объекту, в зависимости от контекста.
                if (ParseObjectCall(token, function_type, var, ref var))
                {
                    // Проверить если процедура используется как функция.
                    if (function != null)
                        function.Type = FunctionTypeEnum.FUNCTION;
                }

                // Доступ к массиву.
                if (_iterator.Current.SubType == TokenSubTypeEnum.P_SQBRACKETOPEN)
                {
                    // Проверить если процедура используется как функция.
                    if (function != null)
                        function.Type = FunctionTypeEnum.FUNCTION;

                    while (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SQBRACKETOPEN))
                    {
                        IVariable expression = ParseExpression((int)Priority.TOP);
                        if (expression == null)
                            throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                        var = EmitCode(OP_CODES.OP_ARRAY_GET, var, expression);
                        var.Type = VariableTypeEnum.REFERENCE;
                        _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SQBRACKETCLOSE);
                    }
                }
            } while (_iterator.Current.SubType == TokenSubTypeEnum.P_DOT);

            return var;
        }

        /// <summary>
        /// Получить переменную из контекста.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private IVariable GetVariableFromContext(string name)
        {
            IVariable var;

            // Проверка что это переменная в контексте функции.
            var = _current_module.Variables.Get(name, _scope);
            if (var != null)
                return var;

            // Проверка что это переменная в контексте модуля.
            var = _current_module.Variables.Get(name);
            if (var != null)
                return var;

            // Проверка что это глобальная переменная.
            var = _programm.GlobalVariables.Get(name);
            if (var != null)
                return var;

            return null;
        }


        /// <summary>
        /// Получить переменную из программы.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="create"></param>
        /// <returns></returns>
        private IVariable GetVariable(IToken token, FunctionTypeEnum function_type, bool create = false)
        {
            IVariable var;

            // Переменная есть в контексте.
            var = GetVariableFromContext(token.Content);
            if (var != null)
                return ParseArrayAndObject(token, function_type, var, null);

            // Проверка что это вызов функции или процедуры, в зависимости от контекста.
            IFunction function = null;

            if (ParseFunctionCall(token, function_type, ref var, ref function))
                return ParseArrayAndObject(token, function_type, var, function);

            if (_iterator.Current.SubType == TokenSubTypeEnum.P_DOT)
                create = false;

            // Если не найдена, то создать новую для последующей проверки. 
            // Переменная может не существовать только в том случае, если она глобальная и объявлена в модуле который еще не обработан.
            if (var == null && !create)
            {
                if (!_deferred_var.ContainsKey(token.Content + _scope.Name))
                {
                    var = new Variable
                    {
                        Name = token.Content,
                        Scope = _scope
                    };
                    _deferred_var[var.Name + _scope.Name] = var;
                }
                else
                    var = _deferred_var[token.Content + _scope.Name];
            }

            if (!create && var == null)
                throw new CompilerException(token.CodeInformation, $"Переменная не определена ({token.Content})");

            if (create)
                var = _current_module.Variables.Create(token.Content, false, _scope);
            return ParseArrayAndObject(token, function_type, var, function);
        }

        /// <summary>
        /// Добавить имя переменной в список для последующей обработки. Обработка конструкций вида: Перем а1,а2 экспорт;
        /// </summary>
        /// <param name="vars"></param>
        private void AddVariableDefineToList(IList<IToken> vars)
        {
            IToken var_name = _iterator.Current;
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
                    throw new CompilerException(_iterator.Current.CodeInformation, "Объявления переменных должны быть расположены в начале модуля, процедуры или функции.");
                else
                {
                    if (_function_entry_point != -1)
                        throw new CompilerException(_iterator.Current.CodeInformation, "Объявления переменных должны быть расположены в начале модуля, процедуры или функции.");
                }


                // Забрать имена переменных
                IList<IToken> vars = new List<IToken>();
                AddVariableDefineToList(vars);

                // Проверка оператора Экспорт.
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EXPORT))
                {
                    // Объявление находится в функции.
                    if (_scope.Type != ScopeTypeEnum.MODULE)
                        throw new CompilerException("Локальные переменные не могут быть экспортированы.");

                    export = true;
                }

                // Если есть оператор Экспорт, тогда в зависимости от типа модуля и его параметров, добавляем переменную в глобальный модуль или делаем ее "публичной", доступной для обращения через обьект этого модуля.
                foreach (IToken var in vars)
                {
                    if (export)
                    {
                        if (_current_module.Variables.Get(var.Content) != null)
                            throw new CompilerException(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");

                        if (_current_module.AsGlobal && !_current_module.AsObject)
                        {
                            if (_programm.GlobalVariables.Create(var.Content) == null)
                                throw new CompilerException(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
                        }
                        else
                         if (_current_module.Variables.Create(var.Content, true, _scope) == null)
                            throw new CompilerException(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
                    }
                    else
                    {
                        if (_programm.GlobalVariables.Get(var.Content) != null)
                            throw new CompilerException(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");

                        if (_current_module.Variables.Create(var.Content, false, _scope) == null)
                            throw new CompilerException(var.CodeInformation, $"Переменная с указанным именем уже определена ({var.Content})");
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
        private IVariable Parse(IToken token)
        {
            IVariable right, left;

            _iterator.MoveNext();
            // Установить точку входа модуля.
            SetModuleEntryPoint();
            // Забрать левую часть выражения. Возможные варианты: переменная, вызов функции, вызов объекта.
            left = GetVariable(token, FunctionTypeEnum.PROCEDURE, true);
            // Если есть присвоение
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ASSIGN))
            {
                // Забрать правую часть выражения. Возможные варианты: переменная, вызов функции, вызов объекта, выражение.
                right = ParseExpression((int)Priority.TOP);
                // Добавить в код модуля присвоение значения.
                EmitCode(OP_CODES.OP_STORE, left, right);
            }
            else
                // Увеличить количество пользователей, для того что бы переменная могла быть использована в другом месте.
                if (left.Users == 1)
                left.Users++;

            return left;
        }


        /// <summary>
        /// Обработка отложенных переменных. Переменные которые были вызваны но еще не объявлены.
        /// </summary>
        public void ProcessDifferedVars()
        {
            IVariable var = null;

            foreach (KeyValuePair<string, IVariable> var_kp in _deferred_var)
            {
                // глобальный контекст
                var = _programm.GlobalVariables.Get(var_kp.Value.Name);
                if (var != null)
                {
                    var_kp.Value.StackNumber = var.StackNumber;
                    var_kp.Value.Scope = _programm.GlobalScope;
                    var_kp.Value.Reference = var.Reference;
                }
                else
                    throw new CompilerException($"Переменная не определена [{var_kp.Value.Name}], модуль [{var_kp.Value.Scope.Module.Name}].");
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
                IVariable return_var;

                // Проверка что Возврат внутри функции/процедуры.
                if (_scope.Type == ScopeTypeEnum.MODULE || _scope.Type == ScopeTypeEnum.GLOBAL)
                    throw new CompilerException(_iterator.Current.CodeInformation, "Оператор Возврат (Return) не может употребляться вне процедуры или функции.");

                if (_iterator.Current.Type != TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType != TokenSubTypeEnum.P_SEMICOLON)
                {
                    // Возврат содержит выражение.
                    if (_scope.Type == ScopeTypeEnum.PROCEDURE)
                        throw new CompilerException(_iterator.Current.CodeInformation, "Процедура не может возвращать значение.");

                    return_var = ParseExpression((int)Priority.TOP);

                    EmitCode(OP_CODES.OP_RETURN, return_var, null);
                }
                else
                {
                    // Слово Возврат не содержит выражения.
                    if (_scope.Type == ScopeTypeEnum.FUNCTION)
                        throw new CompilerException(_iterator.Current.CodeInformation, "Функция должна возвращать результат.");
                    EmitCode(OP_CODES.OP_RETURN, null, null);
                }

                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

                return true;
            }
            return false;
        }



        /// <summary>
        /// Парсер параметров процедуры или функции.
        /// </summary>
        /// <param name="param_list"></param>
        private void ParseFunctionDefineParam(FunctionParameters param_list)
        {
            IVariable var = null;
            bool by_value = false;

            // Парсер оператора Знач, передача параметра по значению.
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_VAL))
                by_value = true;

            IToken param_name = _iterator.Current;
            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            // Парсер параметра с значением по умолчанию.
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_ASSIGN))
            {
                var = ParseConstantVariable(_iterator.Current);
                if (var == null)
                    throw new CompilerException(_iterator.Current.CodeInformation, $"Ожидается константа типа Число, Строка, Дата или Булево.");
            }

            if (by_value)
                param_list.CreateByVal(param_name.Content, _scope, var?.Value);
            else
                param_list.CreateByRef(param_name.Content, _scope, var?.Value);


            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                ParseFunctionDefineParam(param_list);
        }

        /// <summary>
        /// Парсер объявлений процедуры и функции.
        /// </summary>
        /// <returns></returns>
        private bool ParseFunctionDefine()
        {
            IToken type;
            type = _iterator.Current;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_PROCEDURE) || _iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_FUNCTION))
            {
                IToken function_name;
                ScriptScope old_scope = _scope;

                // Проверка где объявили.
                if (_scope.Type != ScopeTypeEnum.MODULE)
                    throw new CompilerException(_iterator.Current.CodeInformation, "Процедуры и функции могут располагаться только в теле модуля.");

                // Проверка порядка расположения вызовов и объявлений в модуле.
                if (_module_entry_point != -1)
                    throw new CompilerException(_iterator.Current.CodeInformation, "Определения процедур и функций должны размещаться перед операторами тела модуля.");

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
                    Name = function_name.Content,
                    StackIndex = 2
                };

                // Блок обработки параметров.
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN);
                FunctionParameters param_list = new FunctionParameters();
                if (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                {
                    ParseFunctionDefineParam(param_list);
                    _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);
                }

                IFunction function;

                // Проверка дубляжа.
                if (_programm.GlobalFunctions.Get(function_name.Content) != null || _current_module.Functions.Get(function_name.Content) != null)
                    throw new CompilerException(function_name.CodeInformation, $"Процедура или функция с указанным именем уже определена ({function_name.Content})");

                // Если есть оператор Экспорт. Тогда в зависимости от типа модуля и его параметров, добавляем функцию в глобальный модуль или делаем ее "публичной", доступной для обращения через обьект этого модуля.
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EXPORT))
                {
                    if (_current_module.AsGlobal && !_current_module.AsObject)
                        function = _programm.GlobalFunctions.Create(function_name.Content);
                    else
                        function = _current_module.Functions.Create(function_name.Content, true);
                }
                else
                    function = _current_module.Functions.Create(function_name.Content, false);

                // Добавляю параметры функции в ее контекст.
                foreach (FunctionParameter var in param_list)
                {
                    IVariable tmp_var = _current_module.Variables.Create(var.Name, false, _scope);
                    tmp_var.Type = var.Type;
                    var.InternalVariable = tmp_var;
                }

                function.EntryPoint = _current_module.ProgrammLine;
                function.DefinedParameters = param_list;
                function.Type = type.SubType == TokenSubTypeEnum.I_PROCEDURE ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                function.CodeInformation = function_name.CodeInformation;
                function.Scope = _scope;

                // Парсим все содержимое функции/процедуры, до оператора КонецФункции/Процедуры.
                if (function.Type == FunctionTypeEnum.PROCEDURE)
                    ParseModuleBody(TokenSubTypeEnum.I_ENDPROCEDURE);
                else
                    ParseModuleBody(TokenSubTypeEnum.I_ENDFUNCTION);

                // Добавляю в код модуля, выход из функции.
                EmitCode(OP_CODES.OP_RETURN, null, null);

                // Проверка оператора КонецФункции/Процедуры.
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
        private void CheckFunctionCall(ScriptModule module, IFunction function)
        {
            IFunction work_function;

            // Ищем в модуле
            work_function = function.Scope.Module.Functions.Get(function.Name);

            // Ищем в глобальном модуле
            if (work_function == null)
                work_function = _programm.GlobalFunctions.Get(function.Name);

            if (work_function == null)
                throw new CompilerException(function.CodeInformation, $"Процедура или функция с именем [{function.Name}] не определена.");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new CompilerException(function.CodeInformation, $"Обращение к процедуре [{function.Name}] как к функции.");

            ScriptStatement statement = module.StatementGet(function.EntryPoint);
            statement.Function = work_function;

            // Патч вызова функции. Указываю правильный модуль.
            if (work_function.Scope != null && work_function.Scope.Module != function.Scope.Module && work_function.Method == null)
                statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(work_function.Scope.Module.Name));


            function.CodeInformation = null;
            if (function.CallParameters.Count == work_function.DefinedParameters.Count || work_function.DefinedParameters.AnyCount)
                return;

            // Блок проверки параметров.
            if (function.CallParameters.Count > work_function.DefinedParameters.Count)
                throw new CompilerException(work_function.CodeInformation, $"Много фактических параметров [{work_function.Name}].");

            int i, param_count;
            param_count = i = function.CallParameters.Count;

            while (i < work_function.DefinedParameters.Count)
            {
                if (work_function.DefinedParameters[i].DefaultValue != null)
                    param_count++;
                i++;
            }

            if (param_count < work_function.DefinedParameters.Count)
                throw new CompilerException(work_function.CodeInformation, $"Недостаточно фактических параметров [{work_function.Name}].");

        }

        /// <summary>
        /// Парсер параметров у вызываемой функции.
        /// </summary>
        /// <param name="param"></param>
        private void ParseFunctionCallParam(IFunction function)
        {
            IToken token;
            token = _iterator.Current;

            IVariable var = null;

            do
            {
                // Обработка пустого параметра. Кода стоит просто запятая.
                if (_iterator.Current.Type == TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType == TokenSubTypeEnum.P_COMMA)
                    var = new Variable() { Scope = _scope, Name = "<<null>>", Type = VariableTypeEnum.CONSTANTVARIABLE, Value = ValueFactory.Create() };
                else
                    // Получить параметр функции, может быть любое выражение или вызов.
                    var = ParseExpression((int)Priority.TOP);


                function.CallParameters.Add(var);
            } while (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA));

        }

        /// <summary>
        /// Парсер вызовов функций.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private bool ParseFunctionCall(IToken token, FunctionTypeEnum function_type, ref IVariable result, ref IFunction function)
        {
            result = null;

            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                function = new Function()
                {
                    Type = function_type,
                    Name = token.Content,
                    CallParameters = new List<IVariable>(),
                    Scope = _scope,
                    CodeInformation = token.CodeInformation.Clone()
                };

                while (_iterator.Current.SubType != TokenSubTypeEnum.P_PARENTHESESCLOSE)
                    ParseFunctionCallParam(function);

                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);

                // Добавить в отложенные функции для последующей проверки.
                _deferred_function.Add((_current_module, function));

                // Добавить в код передачу параметров через стек.
                foreach (IVariable var in function.CallParameters)
                    EmitCode(OP_CODES.OP_PUSH, var, null);


                // Добавить в код вызов функции из модуля по ее имени. 
                function.EntryPoint = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_CALL, null, null);
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
            foreach ((ScriptModule, IFunction) data in _deferred_function)
            {
                CheckFunctionCall(data.Item1, data.Item2);
            }

            _deferred_function.Clear();
        }

        #endregion

        #region Объекты

        /// <summary>
        /// Парсинг оператора Новый (new).
        /// </summary>
        /// <returns></returns>
        private IVariable ParseNew()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_NEW))
            {
                IVariable result = null;

                // Вариант 2: Новый(<Тип>[, <ПараметрыКонструктора>])
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
                {

                }
                // Вариант 1: Новый <Идентификатор типа>[(<Парам1>, <Парам2>, …)]
                else
                {

                    IToken token = _iterator.Current;
                    if(!_iterator.CheckToken(TokenTypeEnum.IDENTIFIER))
                        throw new CompilerException(token.CodeInformation, "Ожидается имя обьекта.");

                    IFunction function;
                    ScriptModule module;

                    if (!_programm.Modules.Exist(token.Content))
                        throw new CompilerException(token.CodeInformation, $"Тип не определен ({token.Content})");
                    else
                        module = _programm.Modules.Get(token.Content);

                    IFunction constructor = module.Functions.Get("Constructor");
                    if (constructor == null)
                        throw new CompilerException(token.CodeInformation, "Конструктор не найден.");

                    function = new Function()
                    {
                        Type = FunctionTypeEnum.FUNCTION,
                        Name = "Constructor",
                        CallParameters = new List<IVariable>(),
                        Scope = module.ModuleScope,
                        CodeInformation = token.CodeInformation.Clone()
                    };

                    if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
                    {
                        while (_iterator.Current.SubType != TokenSubTypeEnum.P_PARENTHESESCLOSE)
                            ParseFunctionCallParam(function);

                        _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);


                        // Добавить в код передачу параметров через стек.
                        foreach (IVariable var in function.CallParameters)
                            EmitCode(OP_CODES.OP_PUSH, var, null);
                    }
                    //else
                    //    _iterator.MoveNext();

                    result = EmitCode(OP_CODES.OP_NEW, null, null);
                    function.EntryPoint = _current_module.ProgrammLine-1;

                    // Добавить в отложенные функции для последующей проверки.
                    _deferred_function.Add((_current_module, function));
                }

                return result;
            }

            return null;
        }


        /// <summary>
        /// Парсер вызовов функций объекта.
        /// </summary>
        private bool ParseObjectFunctionCall(IToken token, IVariable object_call, FunctionTypeEnum function_type, ref IVariable result, ref Function function)
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                // Прототип функции, для последующего сравнения с оригиналом.
                function = new Function()
                {
                    Type = function_type,
                    Name = token.Content,
                    CallParameters = new List<IVariable>(),
                    Scope = _scope,
                    CodeInformation = token.CodeInformation.Clone()
                };

                // Обработка передачи параметров.
                while (!_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                    ParseFunctionCallParam(function);

                // Добавляю прототип в хранилище.
                int function_number = _current_module.ObjectCallAdd(function);

                foreach (IVariable var in function.CallParameters)
                    EmitCode(OP_CODES.OP_PUSH, var, null);

                IVariable function_name_var = _programm.StaticVariables.Create(ValueFactory.Create(function_number));

                // Вызов функции объекта.
                EmitCode(OP_CODES.OP_OBJ_CALL, object_call, function_name_var);
                result = EmitCode(OP_CODES.OP_POP, null, null);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Парсинг частей, разделенных точкой, вызова объекта. 
        /// </summary>
        /// <param name="object_call"></param>
        private IVariable ParseObjectCallParts(IVariable object_call, FunctionTypeEnum function_type)
        {
            IToken token;
            IVariable var = null;
            Function function = null;

            do
            {
                token = _iterator.Current;
                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER))
                {
                    // Обращение к методу или свойству.
                    if (!ParseObjectFunctionCall(token, object_call, function_type, ref object_call, ref function))
                    {
                        var = _programm.StaticVariables.Create(ValueFactory.Create(token.Content));
                        object_call = EmitCode(OP_CODES.OP_OBJ_GET_VAR, object_call, var);
                        object_call.Type = VariableTypeEnum.REFERENCE;

                        // Проверка и изменение типа вызова объекта, с процедуры на функцию.
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
        /// Парсинг обращения к объекту.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="function_type"></param>
        /// <param name="var"></param>
        /// <param name="return_var"></param>
        /// <returns></returns>
        private bool ParseObjectCall(IToken token, FunctionTypeEnum function_type, IVariable var, ref IVariable return_var)
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_DOT))
            {
                // Если переменной еще нет, то отложить ее проверку и создать временную переменную.
                if (var == null)
                {
                    if (!_deferred_var.ContainsKey(token.Content + _scope.Name))
                    {
                        var = new Variable
                        {
                            Type = VariableTypeEnum.STACKVARIABLE,
                            Name = token.Content,
                            Scope = _scope
                        };
                        _deferred_var[var.Name + _scope.Name] = var;
                    }
                    else
                        var = _deferred_var[token.Content + _scope.Name];
                }

                // Парсинг частей вызова, разделенных точкой.
                return_var = ParseObjectCallParts(var, function_type);
                return true;
            }
            return false;
        }

        #endregion

        #region Операторы Если,Для,Пока

        /// <summary>
        /// Парсинг операторов Продолжить (Continue) и Прервать (Break).
        /// </summary>
        /// <returns></returns>
        private bool ParseLoopCommands()
        {
            IToken token;
            token = _iterator.Current;
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_CONTINUE) || _iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_BREAK))
            {
                if (_loop.Count == 0)
                    throw new CompilerException(_iterator.Current.CodeInformation, "Оператор Продолжить (Continue) и Прервать (Break) может употребляться только внутри цикла.");

                // Продолжить (Continue), делаем номер строки отрицательным, для того чтобы потом различить Продолжить и Прервать.
                if (token.SubType == TokenSubTypeEnum.I_CONTINUE)
                    _loop[_loop.Count - 1].Add(_current_module.ProgrammLine * -1);
                else
                    // Прервать(Break)
                    _loop[_loop.Count - 1].Add(_current_module.ProgrammLine);

                // Заглушка для перехода. Номер строки будет заполнен позже.
                EmitCode(OP_CODES.OP_JMP, null, null);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
                return true;
            }

            return false;
        }


        /// <summary>
        /// Парсинг оператора Для каждого ( For each ).
        /// </summary>
        private void ParseForEach()
        {
            int continue_line, exit_jmp;
            ScriptStatement statement;

            // Установить точку входа модуля.
            SetModuleEntryPoint();

            IToken token = _iterator.Current;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_IN))
                throw new CompilerException(token.CodeInformation, "Ожидается имя переменной.");

            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            // Забираем имя переменной "каждого". Если нет в контексте то создаем.
            IVariable each_var = GetVariableFromContext(token.Content);
            if (each_var == null)
                each_var = _current_module.Variables.Create(token.Content, false, _scope);

            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_IN);

            token = _iterator.Current;
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP))
                throw new CompilerException(token.CodeInformation, "Ожидается имя переменной.");

            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            // Забираем переменную Из
            IVariable in_var = GetVariable(token, FunctionTypeEnum.FUNCTION);

            // Начало цикла. Оператор Цикл.
            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP);

            IVariable iterator_var = EmitCode(OP_CODES.OP_ITR_GET, in_var, null);

            // Сохранить начало выполнения условия.
            continue_line = _current_module.ProgrammLine;
            IVariable has_next = EmitCode(OP_CODES.OP_ITR_NEXT, each_var, iterator_var);
            EmitCode(OP_CODES.OP_ITR_STOP, iterator_var, null);
            // Запрет повторного использования переменной до конца выполнения цикла.
            iterator_var.Users = 1;

            // Переход в конец блока.
            exit_jmp = _current_module.ProgrammLine;
            EmitCode(OP_CODES.OP_JMP, null, null);

            // Новый массив для добавления позиций Продолжить, Прервать.
            _loop.Add(new List<int>());


            // Парсинг тела цикла.
            ParseModuleBody(TokenSubTypeEnum.I_ENDLOOP);


            // Переход в начало цикла.
            EmitCode(OP_CODES.OP_JMP, _programm.StaticVariables.Create(ValueFactory.Create(continue_line)), null);

            // Обработка операторов Продолжить, Прервать.
            foreach (int patch_line in _loop[_loop.Count - 1])
            {
                if (patch_line < 0)
                {
                    // Продолжить.
                    statement = _current_module.StatementGet(patch_line * -1);
                    statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(continue_line));
                }
                else
                {
                    // Прервать.
                    statement = _current_module.StatementGet(patch_line);
                    statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                }
            }

            // Удалить массив.
            _loop.RemoveAt(_loop.Count - 1);

            // Патч выхода из цикла.
            statement = _current_module.StatementGet(exit_jmp);
            statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));

            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDLOOP);
            _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
            // Снять запрет.
            iterator_var.Users = 2;
        }

        /// <summary>
        /// Парсинг оператора Для ( For ).
        /// </summary>
        /// <returns></returns>
        private bool ParseFor()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_FOR))
            {

                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_EACH))
                {
                    ParseForEach();
                    return true;
                }

                ScriptStatement statement;
                IVariable expression, result = null;
                IList<int> break_list = new List<int>();
                int continue_line, patch_if_line;

                // Установить точку входа модуля.
                SetModuleEntryPoint();

                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_TO) || _iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                IToken token;
                token = _iterator.Current;
                IVariable var = Parse(token);

                if (var == null)
                    throw new CompilerException(token.CodeInformation, "Ожидается имя переменной.");

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_TO);

                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                expression = ParseExpression((int)Priority.TOP);

                // Сохранить начало выполнения условия.
                continue_line = _current_module.ProgrammLine;
                // Проверка условия переменная > условия.
                result = EmitCode(OP_CODES.OP_GE, expression, var);
                // Запрет повторного использования переменной до конца выполнения цикла.
                expression.Users = 1;
                // Сохранить позицию условия.
                patch_if_line = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_IFNOT, result, null);

                // Увеличить переменную на 1.
                result = EmitCode(OP_CODES.OP_ADD, var, _programm.StaticVariables.Create(ValueFactory.Create(1)));
                EmitCode(OP_CODES.OP_STORE, var, result);



                // Начало цикла. Оператор Цикл.
                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP);

                // Новый массив для добавления позиций Продолжить, Прервать.
                _loop.Add(new List<int>());

                // Парсинг тела цикла.
                ParseModuleBody(TokenSubTypeEnum.I_ENDLOOP);

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDLOOP);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

                // Переход в начало цикла.
                EmitCode(OP_CODES.OP_JMP, _programm.StaticVariables.Create(ValueFactory.Create(continue_line)), null);

                // Обработка операторов Продолжить, Прервать.
                foreach (int patch_line in _loop[_loop.Count - 1])
                {
                    if (patch_line < 0)
                    {
                        // Продолжить.
                        statement = _current_module.StatementGet(patch_line * -1);
                        statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(continue_line));
                    }
                    else
                    {
                        // Прервать.
                        statement = _current_module.StatementGet(patch_line);
                        statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                    }
                }

                // Удалить массив.
                _loop.RemoveAt(_loop.Count - 1);

                // Исправить переход для условия.
                statement = _current_module.StatementGet(patch_if_line);
                statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));

                // Снять запрет.
                expression.Users = 2;

                return true;
            }
            return false;
        }


        /// <summary>
        /// Парсим оператора Пока, КонецЦикла.
        /// </summary>
        /// <returns></returns>
        private bool ParseWhile()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_WHILE))
            {
                ScriptStatement statement;
                IVariable expression = null;
                IList<int> break_list = new List<int>();
                int continue_line, patch_if_line;

                // Установить точку входа модуля.
                SetModuleEntryPoint();

                if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                // Сохранить начало выполнения условия.
                continue_line = _current_module.ProgrammLine;
                expression = ParseExpression((int)Priority.TOP);
                // Сохранить позицию условия.
                patch_if_line = _current_module.ProgrammLine;
                EmitCode(OP_CODES.OP_IFNOT, expression, null);

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOOP);

                // Добавить новый массив для добавления позиций Продолжить, Прервать.
                _loop.Add(new List<int>());

                // Парсинг тела цикла.
                ParseModuleBody(TokenSubTypeEnum.I_ENDLOOP);

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDLOOP);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

                // Переход в начало цикла.
                EmitCode(OP_CODES.OP_JMP, _programm.StaticVariables.Create(ValueFactory.Create(continue_line)), null);

                // Обработка операторов Продолжить, Прервать.
                foreach (int patch_line in _loop[_loop.Count - 1])
                {
                    if (patch_line < 0)
                    {
                        // Продолжить.
                        statement = _current_module.StatementGet(patch_line * -1);
                        statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(continue_line));
                    }
                    else
                    {
                        // Прервать.
                        statement = _current_module.StatementGet(patch_line);
                        statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                    }
                }

                // Удалить массив.
                _loop.RemoveAt(_loop.Count - 1);

                // Исправить переход для условия.
                statement = _current_module.StatementGet(patch_if_line);
                statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));

                return true;
            }
            return false;
        }

        #endregion

        #region Если, "короткий" Если ?()

        /// <summary>
        /// Парсин оператор Если, ИначеЕсли.
        /// </summary>
        /// <returns></returns>
        private bool ParseIf()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_IF))
            {
                int line;
                ScriptStatement statement;
                IVariable expression = null;
                IList<int> jmp = new List<int>();

                line = int.MinValue;

                // Установить точку входа модуля.
                SetModuleEntryPoint();

                do
                {
                    if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_THEN))
                        throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                    expression = ParseExpression((int)Priority.TOP);

                    if (expression == null)
                        throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                    // Установка перехода для предыдущего условия.
                    if (line != int.MinValue)
                    {
                        statement = _current_module.StatementGet(line);
                        statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
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

                    ParseModuleBody(stop);

                    // Если КонецЕсли или конец файла, то выход из цикла.
                    if (_iterator.Current.SubType == TokenSubTypeEnum.I_ENDIF || _iterator.Current.SubType == TokenSubTypeEnum.EOF)
                        break;

                    // Выход после выполнения кода условия.
                    jmp.Add(_current_module.ProgrammLine);
                    EmitCode(OP_CODES.OP_JMP, null, null);

                    // Блок Если
                    if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ELSE))
                    {
                        // Фикс перехода с предыдущего Если или ИначеЕсли
                        statement = _current_module.StatementGet(line);
                        statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                        line = int.MinValue;

                        ParseModuleBody(TokenSubTypeEnum.I_ENDIF);
                        break;
                    }

                } while (_iterator.MoveNext());

                // Для последнего или первого (единственного) Если, установить переход.
                if (line != int.MinValue)
                {
                    statement = _current_module.StatementGet(line);
                    statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                }

                // Установить Jmp актуальную строку кода для выхода.
                foreach (int index in jmp)
                {
                    statement = _current_module.StatementGet(index);
                    statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));
                }
                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDIF);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);
                return true;
            }
            return false;
        }

        /// <summary>
        /// Парсер оператора "короткий" Если ?().
        /// </summary>
        /// <returns></returns>
        private IVariable ParseIfShort()
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_QUESTION))
            {
                IVariable result, expression, true_result, false_result = null;
                ScriptStatement if_statement, jmp_statement;

                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN);

                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                result = _current_module.Variables.Create("", false, _scope);

                // Условие.
                expression = ParseExpression((int)Priority.TOP);
                EmitCode(OP_CODES.OP_IFNOT, expression, null);
                if_statement = _current_module.StatementGet(_current_module.ProgrammLine - 1);

                // Результат для true.
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA);
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                true_result = ParseExpression((int)Priority.TOP);
                EmitCode(OP_CODES.OP_STORE, result, true_result);
                // Защита от очистки значения.
                result.Users = 1;
                // Переход в конец false.
                EmitCode(OP_CODES.OP_JMP, null, null);
                jmp_statement = _current_module.StatementGet(_current_module.ProgrammLine - 1);

                // Патч перехода условия.
                if_statement.Variable3 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));

                // Результат для false.
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA);
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COMMA))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Ожидается выражение.");

                false_result = ParseExpression((int)Priority.TOP);

                EmitCode(OP_CODES.OP_STORE, result, false_result);
                // Защита от очистки значения.
                result.Users = 1;
                jmp_statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(_current_module.ProgrammLine));


                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);

                return result;
            }

            return null;
        }
        #endregion

        #region Перейти ( Goto )

        /// <summary>
        /// Парсинг оператора Перейти ( Goto ).
        /// </summary>
        /// <returns></returns>
        private bool ParseGoto()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_GOTO))
            {
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_TILDE);

                IToken token = _iterator.Current;

                _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);
                _iterator.MoveNext();

                _goto_jmp.Add((token.Content + "-" + _scope.Name, _current_module.ProgrammLine));
                EmitCode(OP_CODES.OP_JMP, null, null);

                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Парсинг меток оператора Перейти ( Goto ).
        /// </summary>
        /// <returns></returns>
        private bool ParseGotoLabel()
        {
            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_TILDE))
            {
                IToken token = _iterator.Current;
                _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);
                _iterator.MoveNext();

                if (_goto_labels.ContainsKey(token.Content + "-" + _scope.Name))
                    throw new CompilerException(_iterator.Current.CodeInformation, $"Метка с указанным именем уже определена [{token.Content}]");

                _goto_labels.Add(token.Content + "-" + _scope.Name, _current_module.ProgrammLine);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_COLON);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка меток переходов. Установка правильных значений переходов.
        /// </summary>
        private void ProcessGoto()
        {
            int line;
            ScriptStatement jmp_statement;

            foreach ((string, int) value in _goto_jmp)
            {
                if (!_goto_labels.TryGetValue(value.Item1, out line))
                    throw new CompilerException(_iterator.Current.CodeInformation, $"Метка не определена в процедуре, функции или теле модуля [{value.Item1.Substring(0, value.Item1.IndexOf('-'))}]");
                else
                {
                    jmp_statement = _current_module.StatementGet(value.Item2);
                    jmp_statement.Variable2 = _programm.StaticVariables.Create(ValueFactory.Create(line));
                }
            }

            _goto_jmp.Clear();
            _goto_labels.Clear();
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
            IToken token;
            do
            {
                if (stop_type.Contains(_iterator.Current.SubType))
                    return;

                // Ошибка "короткий" Если не в выражении.
                if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_QUESTION))
                    throw new CompilerException(_iterator.Current.CodeInformation, "Встроенная функция может быть использована только в выражении.");

                // Парсинг меток оператора Перейти(Goto ).
                if (ParseGotoLabel())
                    continue;

                _iterator.IsTokenType(TokenTypeEnum.IDENTIFIER);
                token = _iterator.Current;

                // Парсинг оператора вызвать исключение.
                if (ParseRaise())
                    continue;

                // Парсинг оператора Исключение(Try).
                if (ParseTryCatch())
                    continue;

                // Парсинг оператора Перейти(Goto ).
                if (ParseGoto())
                    continue;

                // Парсим объявление переменных, оператор Перем.
                if (ParseVariableDefine())
                    continue;

                // Парсим объявление функций и процедур.
                if (ParseFunctionDefine())
                    continue;

                // Парсим оператор Возврат (return).
                if (ParseReturn())
                    continue;

                // Парсим оператор Если (if).
                if (ParseIf())
                    continue;

                // Парсим оператор Пока (while).
                if (ParseWhile())
                    continue;

                // Парсим оператор Для (for).
                if (ParseFor())
                    continue;

                // Парсим операторы  Продолжить (Continue) и Прервать (Break).
                if (ParseLoopCommands())
                    continue;

                // Парсим вызов методов, свойств, а так же присвоение значений.
                Parse(token);
                _iterator.IsTokenType(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_SEMICOLON);

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

            _programm.LoadDefaultLibraries();

            foreach (KeyValuePair<ScriptModule, string> module in modules)
            {

                parser = new ParserClass(module.Key.Name, module.Value);
                Console.WriteLine(module.Value);
                PrecompilerClass precompiler = new PrecompilerClass(parser.GetEnumerator(), defines);
                _iterator = precompiler.GetEnumerator();

                _module_entry_point = -1;
                _function_entry_point = -1;
                _current_module = module.Key;
                _scope = _current_module.ModuleScope;

                _iterator.MoveNext();
                ParseModuleBody();

                // Виртуальная функция, код модуля.
                IFunction entry_point;
                if (!module.Key.AsObject)
                    entry_point = _current_module.Functions.Create("<<entry_point>>");
                else
                    entry_point = _current_module.Functions.Create("<<constructor>>");

                entry_point.Type = FunctionTypeEnum.PROCEDURE;
                entry_point.EntryPoint = _module_entry_point;
                entry_point.Scope = _scope;

                EmitCode(OP_CODES.OP_RETURN, null, null);

                _programm.Modules.Add(_current_module);

                if (_current_module.AsGlobal)
                {
                    IValue module_object = ValueFactory.Create();
                    _programm.GlobalVariables.Create(_current_module.Name, module_object);
                    if (_current_module.Name != _current_module.Alias)
                        _programm.GlobalVariables.Create(_current_module.Alias, module_object);
                }

                ProcessGoto();

                if (_iterator.Current.Type != TokenTypeEnum.PUNCTUATION && _iterator.Current.SubType != TokenSubTypeEnum.EOF)
                    throw new CompilerException("Есть не разобранный участок кода.");
            }

            ProcessDifferedVars();
            ProcessDifferedFunctions();
            return _programm;
        }

    }
}
