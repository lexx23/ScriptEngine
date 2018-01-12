using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System.Collections.Generic;


namespace ScriptEngine.EngineBase.Parser.Precompiler.Directives
{
    /// <summary>
    /// Обработчик конструкции директив #Если(#If) #КонецЕсли(#EndIf)
    /// </summary>
    public class IfDirective : DirectiveBase
    {
        public IfDirective(TokenIteratorBase iterator, PrecompilerStack stack, IDictionary<string, bool> defines) : base(iterator, stack, defines)
        {
        }


        /// <summary>
        /// Получить значение переменной
        /// </summary>
        /// <returns></returns>
        private bool GetDefine()
        {
            bool reverse = false;
            bool result = false;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_NOT))
                reverse = true;

            if (_iterator.CheckToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESOPEN))
            {
                result = Expression(1);
                _iterator.ExpectToken(TokenTypeEnum.PUNCTUATION, TokenSubTypeEnum.P_PARENTHESESCLOSE);
                return !reverse ? result : !result;
            }

            string content = _iterator.Current.Content;
            _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

            if (_defines.ContainsKey(content))
                result = _defines[content];


            return !reverse ? result : !result;
        }

        /// <summary>
        /// Расчитать логическое выражение.
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        private bool Expression(int level)
        {
            bool result = false;

            if (level == 0)
            {
                return GetDefine();
            }

            result = Expression(level - 1);

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_OR))
                result = Expression(1) || result;

            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_LOGIC_AND))
                result = Expression(1) && result;

            return result;
        }


        /// <summary>
        /// Обработка директивы #Если(#If)
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool ProcessIf()
        {
            IToken token = _iterator.Current.Clone();
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_IF))
            {

                _stack.Push(token, !Expression(1),true);

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_THEN);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка директивы #КонецЕсли(#EndIf)
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool ProcessEndIf()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDIF))
            {
                if (_stack.Count == 0)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Пропущен оператор препроцессора #Если(#If).");

                PrecompilerStackStruct directive;
                directive = _stack.Peek();

                if (directive.Token.SubType != TokenSubTypeEnum.I_IF && directive.Token.SubType != TokenSubTypeEnum.I_ELSE && directive.Token.SubType != TokenSubTypeEnum.I_ELSEIF)
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Пропущен оператор препроцессора #Если(#If).");

                _stack.Pop();

                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка директивы #Иначе(#Else)
        /// </summary>
        /// <returns></returns>
        private bool ProcessElse()
        {
            IToken token = _iterator.Current.Clone();
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ELSE))
            {
                PrecompilerStackStruct directive = _stack.Peek();
                if (_stack.Count == 0 || (directive.Token.SubType != TokenSubTypeEnum.I_IF && directive.Token.SubType != TokenSubTypeEnum.I_ELSEIF))
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Пропущен оператор препроцессора #Если(#If).");


                _stack.Pop();
                if(directive.Run)
                    _stack.Push(token, !directive.Skip);
                else
                    _stack.Push(token, true);
                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка директивы #ИначеЕсли(#ElseIf)
        /// </summary>
        /// <returns></returns>
        private bool ProcessElseIf()
        {
            bool result;
            IToken token = _iterator.Current.Clone();
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ELSEIF))
            {
                if (_stack.Count == 0 || (_stack.Peek().Token.SubType != TokenSubTypeEnum.I_IF && _stack.Peek().Token.SubType != TokenSubTypeEnum.I_ELSEIF))
                    throw new ExceptionBase(_iterator.Current.CodeInformation, "Пропущен оператор препроцессора #Если(#If).");


                PrecompilerStackStruct directive = _stack.Pop();

                result = Expression(1);
                if(directive.Skip && directive.Run)
                    _stack.Push(token,!result,true);
                else
                    _stack.Push(token, true, false);

                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_THEN);

                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка директивы.
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        public override bool ProcessDirective()
        {
            if (ProcessIf())
                return true;
            if (ProcessEndIf())
                return true;
            if (ProcessElse())
                return true;
            if (ProcessElseIf())
                return true;

            return false;
        }

    }
}
