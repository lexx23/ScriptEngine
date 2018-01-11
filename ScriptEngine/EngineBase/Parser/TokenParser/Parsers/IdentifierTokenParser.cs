using ScriptEngine.EngineBase.Parser.Token;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Parser.TokenParser.Parsers
{
    /// <summary>
    /// Парсер токенов - имен.
    /// </summary>
    public class IdentifierTokenParser : ITokenParser
    {
        private static IDictionary<string, TokenSubTypeEnum> _table;


        public IdentifierTokenParser()
        {
            _table = new Dictionary<string, TokenSubTypeEnum>(StringComparer.OrdinalIgnoreCase);

            _table.Add("или", TokenSubTypeEnum.I_LOGIC_OR);
            _table.Add("or", TokenSubTypeEnum.I_LOGIC_OR);
            _table.Add("не", TokenSubTypeEnum.I_LOGIC_NOT);
            _table.Add("not", TokenSubTypeEnum.I_LOGIC_NOT);
            _table.Add("и", TokenSubTypeEnum.I_LOGIC_AND);
            _table.Add("and", TokenSubTypeEnum.I_LOGIC_AND);

            _table.Add("Область", TokenSubTypeEnum.I_REGION);
            _table.Add("Region", TokenSubTypeEnum.I_REGION);
            _table.Add("КонецОбласти", TokenSubTypeEnum.I_ENDREGION);
            _table.Add("EndRegion", TokenSubTypeEnum.I_ENDREGION);

            _table.Add("If", TokenSubTypeEnum.I_IF);
            _table.Add("Если", TokenSubTypeEnum.I_IF);
            _table.Add("Then", TokenSubTypeEnum.I_THEN);
            _table.Add("Тогда", TokenSubTypeEnum.I_THEN);
            _table.Add("Else", TokenSubTypeEnum.I_ELSE);
            _table.Add("Иначе", TokenSubTypeEnum.I_ELSE);
            _table.Add("ElsIf", TokenSubTypeEnum.I_ELSEIF);
            _table.Add("ИначеЕсли", TokenSubTypeEnum.I_ELSEIF);
            _table.Add("EndIf", TokenSubTypeEnum.I_ENDIF);
            _table.Add("КонецЕсли", TokenSubTypeEnum.I_ENDIF);

            _table.Add("Пока", TokenSubTypeEnum.I_WHILE);
            _table.Add("While", TokenSubTypeEnum.I_WHILE);
            _table.Add("Цикл", TokenSubTypeEnum.I_LOOP);
            _table.Add("Do", TokenSubTypeEnum.I_LOOP);
            _table.Add("КонецЦикла", TokenSubTypeEnum.I_ENDLOOP);
            _table.Add("EndDo", TokenSubTypeEnum.I_ENDLOOP);

            _table.Add("Продолжить", TokenSubTypeEnum.I_CONTINUE);
            _table.Add("Сontınue", TokenSubTypeEnum.I_CONTINUE);
            _table.Add("Прервать", TokenSubTypeEnum.I_BREAK);
            _table.Add("Break", TokenSubTypeEnum.I_BREAK);

            _table.Add("Перем", TokenSubTypeEnum.I_VARDEF);
            _table.Add("Var", TokenSubTypeEnum.I_VARDEF);

            _table.Add("Знач", TokenSubTypeEnum.I_VAL);
            _table.Add("Val", TokenSubTypeEnum.I_VAL);

            _table.Add("Экспорт", TokenSubTypeEnum.I_EXPORT);
            _table.Add("Export", TokenSubTypeEnum.I_EXPORT);

            _table.Add("Истина", TokenSubTypeEnum.I_LOGIC_TRUE);
            _table.Add("True", TokenSubTypeEnum.I_LOGIC_TRUE);
            _table.Add("Ложь", TokenSubTypeEnum.I_LOGIC_FALSE);
            _table.Add("False", TokenSubTypeEnum.I_LOGIC_FALSE);

            _table.Add("Неопределено", TokenSubTypeEnum.N_NULL);
            _table.Add("Null", TokenSubTypeEnum.N_NULL);

            _table.Add("Функция", TokenSubTypeEnum.I_FUNCTION);
            _table.Add("Function", TokenSubTypeEnum.I_FUNCTION);
            _table.Add("КонецФункции", TokenSubTypeEnum.I_ENDFUNCTION);
            _table.Add("EndFunction", TokenSubTypeEnum.I_ENDFUNCTION);

            _table.Add("Процедура", TokenSubTypeEnum.I_PROCEDURE);
            _table.Add("Procedure", TokenSubTypeEnum.I_PROCEDURE);
            _table.Add("КонецПроцедуры", TokenSubTypeEnum.I_ENDPROCEDURE);
            _table.Add("EndProcedure", TokenSubTypeEnum.I_ENDPROCEDURE);

            _table.Add("Возврат", TokenSubTypeEnum.I_RETURN);
            _table.Add("Return", TokenSubTypeEnum.I_RETURN);

        }


        public bool Parse(SourceIterator iterator, out TokenClass token)
        {
            token = null;
            string content = string.Empty;
            TokenSubTypeEnum subtype;

            if (Char.IsLetter(iterator.Current) || iterator.Current == '_')
            {
                content = iterator.GetLettersAndDigits();

                if(!_table.TryGetValue(content, out subtype))
                    subtype = TokenSubTypeEnum.NA;

                token = new TokenClass()
                {
                    Content = content,
                    Type = TokenTypeEnum.IDENTIFIER,
                    SubType = subtype
                };

                return true;
            }
            return false;
        }
    }
}
