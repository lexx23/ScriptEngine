using ScriptEngine.EngineBase.Library.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public enum TokenSubTypeEnum
    {
        [EnumStringAttribute("символ препроцессора { # } ")]
        PRECOMP,
        COMP,
        [EnumStringAttribute("Область { Region }")]
        I_REGION,
        [EnumStringAttribute("КонецОбласти { EndRegion }")]
        I_ENDREGION,

        [EnumStringAttribute("Новый { New }")]
        I_NEW,

        [EnumStringAttribute("Перейти { Goto }")]
        I_GOTO,
        [EnumStringAttribute("тильда { ~ }")]
        P_TILDE,

        [EnumStringAttribute("Если { If }")]
        I_IF,
        [EnumStringAttribute("Тогда { Then }")]
        I_THEN,
        [EnumStringAttribute("Иначе { Else }")]
        I_ELSE,
        [EnumStringAttribute("ИначеЕсли { ElsIf }")]
        I_ELSEIF,
        [EnumStringAttribute("КонецЕсли { EndIf }")]
        I_ENDIF,

        [EnumStringAttribute("Для { For }")]
        I_FOR,
        [EnumStringAttribute("Из { In }")]
        I_IN,
        [EnumStringAttribute("Каждого { Each }")]
        I_EACH,
        [EnumStringAttribute("По { To }")]
        I_TO,
        [EnumStringAttribute("Пока { While }")]
        I_WHILE,
        [EnumStringAttribute("Цикл { Do }")]
        I_LOOP,
        [EnumStringAttribute("КонецЦикла { EndDo }")]
        I_ENDLOOP,
        [EnumStringAttribute("Продолжить { Continue }")]
        I_CONTINUE,
        [EnumStringAttribute("Прервать { Break }")]
        I_BREAK,

        [EnumStringAttribute("Функция { Function }")]
        I_FUNCTION,
        [EnumStringAttribute("КонецФункции { EndFunction }")]
        I_ENDFUNCTION,

        [EnumStringAttribute("Процедура { Procedure }")]
        I_PROCEDURE,
        [EnumStringAttribute("КонецПроцедуры { EndProcedure }")]
        I_ENDPROCEDURE,

        [EnumStringAttribute("Возврат { Return }")]
        I_RETURN,


        [EnumStringAttribute("Не { Not }")]
        I_LOGIC_NOT,
        [EnumStringAttribute("И { And }")]
        I_LOGIC_AND,
        [EnumStringAttribute("Или { Or )")]
        I_LOGIC_OR,

        [EnumStringAttribute("Перем { Var }")]
        I_VARDEF,
        [EnumStringAttribute("Знач { Val }")]
        I_VAL,

        [EnumStringAttribute("Экспорт { Export }")]
        I_EXPORT,

        [EnumStringAttribute("Истина { True }")]
        I_LOGIC_TRUE,
        [EnumStringAttribute("Ложь { False }")]
        I_LOGIC_FALSE,

        [EnumStringAttribute("больше либо равно { >= }")]
        P_LOGIC_GEQ,
        [EnumStringAttribute("меньше либо равно { <= }")]
        P_LOGIC_LEQ,
        [EnumStringAttribute("не равен { <> }")]
        P_LOGIC_UNEQ,
        [EnumStringAttribute("больше { > }")]
        P_LOGIC_GREATER,
        [EnumStringAttribute("меньше { < }")]
        P_LOGIC_LESS,


        [EnumStringAttribute("произведение { * }")]
        P_MUL,
        [EnumStringAttribute("деление { / }")]
        P_DIV,
        [EnumStringAttribute("модуль { % }")]
        P_MOD,

        [EnumStringAttribute("плюс { + }")]
        P_ADD,
        [EnumStringAttribute("минус { - }")]
        P_SUB,
        [EnumStringAttribute("равно { = }")]
        P_ASSIGN,

        [EnumStringAttribute("точка { . }")]
        P_DOT,

        [EnumStringAttribute("запятая { , }")]
        P_COMMA,
        [EnumStringAttribute("двоеточие { : }")]
        P_COLON,
        [EnumStringAttribute("точка с запятой { ; }")]
        P_SEMICOLON,
        [EnumStringAttribute("вопрос { ? }")]
        P_QUESTION,


        [EnumStringAttribute("открывающая скобка { ( }")]
        P_PARENTHESESOPEN,
        [EnumStringAttribute("закрывающая скобка { ) }")]
        P_PARENTHESESCLOSE,
        [EnumStringAttribute("открывающая квадратная скобка { [ }")]
        P_SQBRACKETOPEN,
        [EnumStringAttribute("закрывающая квадратная скобка { ] }")]
        P_SQBRACKETCLOSE,


        [EnumStringAttribute("дата")]
        L_DATE,
        [EnumStringAttribute("строка")]
        L_STRING,
        [EnumStringAttribute("неопределенно ( null )")]
        N_NULL,

        [EnumStringAttribute("")]
        NA,
        [EnumStringAttribute("конец файла")]
        EOF,
        ANY


    }

}
