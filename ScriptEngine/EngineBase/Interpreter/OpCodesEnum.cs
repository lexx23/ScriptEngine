using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Parser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter
{
    public enum OP_CODES
    {
        // Добавить значение в стек
        OP_PUSH = 0,
        // Забрать значение из стека
        OP_POP = 1,
        // Вызов функции
        OP_CALL = 2,
        // Выход из функции
        OP_RETURN = 3,
        // Вызов метода обьекта
        OP_OBJECT_CALL = 4,
        // Вызов свойства обьекта
        OP_OBJECT_RESOLVE_VAR = 5,
        // Переход если ложь
        OP_IFNOT = 6,
        // Переход без условий
        OP_JMP = 7,
        //  Логическое >
        OP_GT = 8,
        //  Логическое <
        OP_LT = 9,
        //  Логическое >=
        OP_GE = 10,
        //  Логическое <=
        OP_LE = 11,
        //  Логическое НЕ
        OP_NOT = 12,
        //  Логическое ИЛИ
        OP_OR = 13,
        //  Логическое И
        OP_AND = 14,
        // Логическое равенство
        OP_EQ = 15,
        // Логическое неравенство
        OP_UNEQ = 16,
        
        // Очистка переменной
        OP_VAR_CLR = 17,
        // Присвоить значение переменной
        OP_STORE = 18,

        // Сложение
        [EnumStringAttribute("сумма { + }")]
        OP_ADD = 19,
        // Разница
        [EnumStringAttribute("произведение { - }")]
        OP_SUB = 20,
        // Умножение
        [EnumStringAttribute("произведение { * }")]
        OP_MUL = 21,
        // Деление
        [EnumStringAttribute("деление { / }")]
        OP_DIV = 22,
        // Остаток от деления
        [EnumStringAttribute("остаток от деления { % }")]
        OP_MOD = 23,
        // Новый
        OP_NEW,
        // Получить элемент массива.
        OP_ARRAY_GET,
        // Получить итератор.
        OP_GET_ITERATOR,
        // Следующий шаг итератора.
        OP_ITERATOR_NEXT,
        // Остановка итератора.
        OP_ITERATOR_STOP
    }
}
