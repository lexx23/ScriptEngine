using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Precompiler.Directives
{
    /// <summary>
    /// Обработчик конструкции директив #Область(#Region) #КонецОбласти(#EndRegion)
    /// </summary>
    public class RegionDirective : DirectiveBase
    {
        public RegionDirective(TokenIteratorBase iterator, PrecompilerStack stack, IDictionary<string, bool> defines) : base(iterator,stack,defines)
        {
        }

        /// <summary>
        /// Обработка директивы #КонецОбласти(#EndRegion)
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool ProcessEndRegion()
        {
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_ENDREGION))
            {

                if (_stack.Count == 0 || _stack.Peek().Token.SubType != TokenSubTypeEnum.I_REGION)
                    throw new CompilerException(_iterator.Current.CodeInformation, $"Ожидается оператор препроцессора #Область(#Region), а найдена #{_stack.Peek().Token.Content}.");

                _stack.Pop();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Обработка директивы #Область(#Region).
        /// </summary>
        /// <param name="stack"></param>
        /// <returns></returns>
        private bool ProcessRegion()
        {
            IToken token = _iterator.Current.Clone();
            if (_iterator.CheckToken(TokenTypeEnum.IDENTIFIER, TokenSubTypeEnum.I_REGION))
            {
                _stack.Push(token);
                _iterator.ExpectToken(TokenTypeEnum.IDENTIFIER);

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
            if (ProcessRegion())
                return true;
            if (ProcessEndRegion())
                return true;

            return false;
        }

    }

}
