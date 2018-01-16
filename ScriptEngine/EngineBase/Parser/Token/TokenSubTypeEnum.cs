using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public enum TokenSubTypeEnum
    {
        [StringValue("символ препроцессора { # } ")]
        PRECOMP,
        COMP,
        [StringValue("Область { Region }")]
        I_REGION,
        [StringValue("КонецОбласти { EndRegion }")]
        I_ENDREGION,

        [StringValue("Перейти { Goto }")]
        I_GOTO,
        [StringValue("тильда { ~ }")]
        P_TILDE,

        [StringValue("Если { If }")]
        I_IF,
        [StringValue("Тогда { Then }")]
        I_THEN,
        [StringValue("Иначе { Else }")]
        I_ELSE,
        [StringValue("ИначеЕсли { ElsIf }")]
        I_ELSEIF,
        [StringValue("КонецЕсли { EndIf }")]
        I_ENDIF,

        [StringValue("Для { For }")]
        I_FOR,
        [StringValue("По { To }")]
        I_TO,
        [StringValue("Пока { While }")]
        I_WHILE,
        [StringValue("Цикл { Do }")]
        I_LOOP,
        [StringValue("КонецЦикла { EndDo }")]
        I_ENDLOOP,
        [StringValue("Продолжить { Continue }")]
        I_CONTINUE,
        [StringValue("Пррервать { Break }")]
        I_BREAK,

        [StringValue("Функция { Function }")]
        I_FUNCTION,
        [StringValue("КонецФункции { EndFunction }")]
        I_ENDFUNCTION,

        [StringValue("Процедура { Procedure }")]
        I_PROCEDURE,
        [StringValue("КонецПроцедуры { EndProcedure }")]
        I_ENDPROCEDURE,

        [StringValue("Возврат { Return }")]
        I_RETURN,


        [StringValue("Не { Not }")]
        I_LOGIC_NOT,
        [StringValue("И { And }")]
        I_LOGIC_AND,
        [StringValue("Или { Or )")]
        I_LOGIC_OR,

        [StringValue("Перем { Var }")]
        I_VARDEF,
        [StringValue("Знач { Val }")]
        I_VAL,

        [StringValue("Экспорт { Export }")]
        I_EXPORT,

        [StringValue("Истина { True }")]
        I_LOGIC_TRUE,
        [StringValue("Ложь { False }")]
        I_LOGIC_FALSE,

        [StringValue("больше либо равно { >= }")]
        P_LOGIC_GEQ,
        [StringValue("меньше либо равно { <= }")]
        P_LOGIC_LEQ,
        [StringValue("не равен { <> }")]
        P_LOGIC_UNEQ,
        [StringValue("больше { > }")]
        P_LOGIC_GREATER,
        [StringValue("меньше { < }")]
        P_LOGIC_LESS,


        [StringValue("произведение { * }")]
        P_MUL,
        [StringValue("деление { / }")]
        P_DIV,
        [StringValue("модуль { % }")]
        P_MOD,

        [StringValue("плюс { + }")]
        P_ADD,
        [StringValue("минус { - }")]
        P_SUB,
        [StringValue("равно { = }")]
        P_ASSIGN,

        [StringValue("точка { . }")]
        P_DOT,

        [StringValue("запятая { , }")]
        P_COMMA,
        [StringValue("двоеточие { : }")]
        P_COLON,
        [StringValue("точка с запятой { ; }")]
        P_SEMICOLON,
        [StringValue("вопрос { ? }")]
        P_QUESTION,


        [StringValue("открывающая скобка { ( }")]
        P_PARENTHESESOPEN,
        [StringValue("закрывающая скобка { ) }")]
        P_PARENTHESESCLOSE,
        SQBRACKETOPEN,
        SQBRACKETCLOSE,


        [StringValue("дата")]
        L_DATE,
        [StringValue("строка")]
        L_STRING,
        [StringValue("неопределено ( null )")]
        N_NULL,

        [StringValue("")]
        NA,
        [StringValue("конец файла")]
        EOF,
        ANY


    }

}
