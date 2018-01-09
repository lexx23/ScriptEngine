using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter
{
    public enum OP_CODES
    {
        // Очистка переменной
        OP_VAR_CLR,
        // Присвоить значение переменной
        OP_STORE,
        // Сложение
        OP_ADD,
        // Вычитание
        OP_SUB,
        // Умножение
        OP_MUL,
        // Деление
        OP_DIV,
        // Изменить знак переменной на - 
        OP_NEG,

        // Переход без условий
        OP_JMP,
        // Переход если ложь
        OP_IFNOT,
        // Логическое равенство
        OP_EQ,
        // Логическое неравенство
        OP_UNEQ,
        //  Логическое >=
        OP_GE,
        //  Логическое <=
        OP_LE,
        //  Логическое <
        OP_LT,
        //  Логическое >
        OP_GT,
        //  Логическое И
        OP_AND,
        //  Логическое ИЛИ
        OP_OR,
        //  Логическое НЕ
        OP_NOT,

        // Вызов метода обьекта
        OP_OBJECT_CALL,
        // Вызов свойства обьекта
        OP_OBJECT_RESOLVE_VAR,

        // Вызов функции
        OP_CALL,
        // Выход из функции
        OP_RETURN,
        // Забрать значение из стека
        OP_POP,
        // Добавить значение в стек
        OP_PUSH

    }

}
