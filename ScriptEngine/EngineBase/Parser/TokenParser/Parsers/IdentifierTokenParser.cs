using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;
using System;

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
            _table = new Dictionary<string, TokenSubTypeEnum>(StringComparer.OrdinalIgnoreCase)
            {
                { "или", TokenSubTypeEnum.I_LOGIC_OR },
                { "or", TokenSubTypeEnum.I_LOGIC_OR },
                { "не", TokenSubTypeEnum.I_LOGIC_NOT },
                { "not", TokenSubTypeEnum.I_LOGIC_NOT },
                { "и", TokenSubTypeEnum.I_LOGIC_AND },
                { "and", TokenSubTypeEnum.I_LOGIC_AND },

                { "Область", TokenSubTypeEnum.I_REGION },
                { "Region", TokenSubTypeEnum.I_REGION },
                { "КонецОбласти", TokenSubTypeEnum.I_ENDREGION },
                { "EndRegion", TokenSubTypeEnum.I_ENDREGION },

                { "If", TokenSubTypeEnum.I_IF },
                { "Если", TokenSubTypeEnum.I_IF },
                { "Then", TokenSubTypeEnum.I_THEN },
                { "Тогда", TokenSubTypeEnum.I_THEN },
                { "Else", TokenSubTypeEnum.I_ELSE },
                { "Иначе", TokenSubTypeEnum.I_ELSE },
                { "ElsIf", TokenSubTypeEnum.I_ELSEIF },
                { "ИначеЕсли", TokenSubTypeEnum.I_ELSEIF },
                { "EndIf", TokenSubTypeEnum.I_ENDIF },
                { "КонецЕсли", TokenSubTypeEnum.I_ENDIF },

                { "Для", TokenSubTypeEnum.I_FOR },
                { "For", TokenSubTypeEnum.I_FOR },
                { "Каждого", TokenSubTypeEnum.I_EACH },
                { "Each", TokenSubTypeEnum.I_EACH },
                { "Из", TokenSubTypeEnum.I_IN },
                { "In", TokenSubTypeEnum.I_IN },
                { "По", TokenSubTypeEnum.I_TO },
                { "To", TokenSubTypeEnum.I_TO },
                { "Пока", TokenSubTypeEnum.I_WHILE },
                { "While", TokenSubTypeEnum.I_WHILE },
                { "Цикл", TokenSubTypeEnum.I_LOOP },
                { "Do", TokenSubTypeEnum.I_LOOP },
                { "КонецЦикла", TokenSubTypeEnum.I_ENDLOOP },
                { "EndDo", TokenSubTypeEnum.I_ENDLOOP },

                { "Продолжить", TokenSubTypeEnum.I_CONTINUE },
                { "Continue", TokenSubTypeEnum.I_CONTINUE },
                { "Прервать", TokenSubTypeEnum.I_BREAK },
                { "Break", TokenSubTypeEnum.I_BREAK },

                { "Перем", TokenSubTypeEnum.I_VARDEF },
                { "Var", TokenSubTypeEnum.I_VARDEF },

                { "Знач", TokenSubTypeEnum.I_VAL },
                { "Val", TokenSubTypeEnum.I_VAL },

                { "Экспорт", TokenSubTypeEnum.I_EXPORT },
                { "Export", TokenSubTypeEnum.I_EXPORT },

                { "Истина", TokenSubTypeEnum.I_LOGIC_TRUE },
                { "True", TokenSubTypeEnum.I_LOGIC_TRUE },
                { "Ложь", TokenSubTypeEnum.I_LOGIC_FALSE },
                { "False", TokenSubTypeEnum.I_LOGIC_FALSE },

                { "Неопределено", TokenSubTypeEnum.N_NULL },
                { "Null", TokenSubTypeEnum.N_NULL },

                { "Функция", TokenSubTypeEnum.I_FUNCTION },
                { "Function", TokenSubTypeEnum.I_FUNCTION },
                { "КонецФункции", TokenSubTypeEnum.I_ENDFUNCTION },
                { "EndFunction", TokenSubTypeEnum.I_ENDFUNCTION },

                { "Процедура", TokenSubTypeEnum.I_PROCEDURE },
                { "Procedure", TokenSubTypeEnum.I_PROCEDURE },
                { "КонецПроцедуры", TokenSubTypeEnum.I_ENDPROCEDURE },
                { "EndProcedure", TokenSubTypeEnum.I_ENDPROCEDURE },

                { "Возврат", TokenSubTypeEnum.I_RETURN },
                { "Return", TokenSubTypeEnum.I_RETURN },

                { "Вычислить", TokenSubTypeEnum.I_EVAL },
                { "Eval", TokenSubTypeEnum.I_EVAL },
                { "Выполнить", TokenSubTypeEnum.I_EXECUTE },
                { "Execute", TokenSubTypeEnum.I_EXECUTE },


                { "Перейти", TokenSubTypeEnum.I_GOTO },
                { "Goto", TokenSubTypeEnum.I_GOTO },

                { "Новый", TokenSubTypeEnum.I_NEW },
                { "New", TokenSubTypeEnum.I_NEW },

                { "Попытка", TokenSubTypeEnum.I_TRY},
                { "Try", TokenSubTypeEnum.I_TRY},
                { "КонецПопытки", TokenSubTypeEnum.I_ENDTRY},
                { "EndTry", TokenSubTypeEnum.I_TRY},
                { "Исключение", TokenSubTypeEnum.I_EXCEPT},
                { "Except", TokenSubTypeEnum.I_EXCEPT},
                { "ВызватьИсключение", TokenSubTypeEnum.I_RAISE},
                { "Raise", TokenSubTypeEnum.I_RAISE}
            };

        }


        public bool Parse(SourceIterator iterator, out IToken token)
        {
            token = null;
            string content = string.Empty;
            TokenSubTypeEnum subtype;

            if (Char.IsLetter(iterator.Current) || iterator.Current == '_')
            {
                CodeInformation information = iterator.CodeInformation.Clone();
                content = iterator.GetLettersAndDigits();

                if (!_table.TryGetValue(content, out subtype))
                    subtype = TokenSubTypeEnum.NA;

                token = new TokenClass()
                {
                    Content = content,
                    CodeInformation = information,
                    Type = TokenTypeEnum.IDENTIFIER,
                    SubType = subtype
                };

                return true;
            }
            return false;
        }
    }
}
