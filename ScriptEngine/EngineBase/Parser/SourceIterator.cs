using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase
{
    /// <summary>
    /// Класс для перебора исходного кода.
    /// </summary>
    public class SourceIterator : IEnumerator<char>
    {
        private string _source;
        private int _cursor;
        private char _current_symbol;
        private static string _skip_chars = " \n\r\t";


        /// <summary>
        /// Текущее положение курсора.
        /// </summary>
        public int Cursor
        {
            get
            {
                return _cursor;
            }
        }


        /// <summary>
        /// Текущий символ итератора. Если симыол запрещен к выдаче, то итератор делает шаг впред, пропускает символы.
        /// </summary>
        public char Current
        {
            get
            {
                SkipComment();

                return _current_symbol;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return (object)Current;
            }
        }

        /// <summary>
        /// Состояние курсора (конец текста или нет).
        /// </summary>
        /// <returns></returns>
        public bool End
        {
            get
            {
                bool result;

                result = _cursor >= _source.Length;
                if (result)
                    _cursor = _source.Length;

                return result;
            }
        }



        /// <summary>
        /// Текущее положение курсора (номер строки,номер столбца,название модуля).
        /// </summary>
        public CodeInformation CodeInformation
        {
            get;
        }

        #region HelpFunctions

        /// <summary>
        /// Добавить символ в буффер. Игнорирует запрещенные символы.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="buffer"></param>
        private void AddToBuffer(char symbol, ref string buffer)
        {
            if (symbol != '\0' && symbol != '\n' && symbol != '\r')
                buffer += symbol;
        }

        /// <summary>
        /// Проверить символ новой строки '\n'
        /// </summary>
        private void CheckNewLine()
        {
            if (_current_symbol == '\n')
            {
                CodeInformation.LineNumber++;
                CodeInformation.ColumnNumber = 0;
            }
        }

        /// <summary>
        /// Проверка что символ необходимо пропустить.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private bool IsSkipChar(char symbol)
        {
            return _skip_chars.IndexOf(symbol) != -1;
        }

        /// <summary>
        /// Пропустить ненужные символы.
        /// </summary>
        private void SkipChars()
        {
            while (IsSkipChar(_current_symbol) && _current_symbol != '\0')
                MoveNext();
        }

        private void SkipComment()
        {
            SkipChars();
            if (_current_symbol == '/' && GetForwardSymbol() == '/')
            {
                if (GoToSymbol('\n'))
                {
                    MoveNext();
                    SkipComment();
                }
            }

        }
        #endregion

        /// <summary>
        /// Итератор исходного кода.
        /// </summary>
        /// <param name="source"></param>
        public SourceIterator(string module_name, string source)
        {
            _source = source;

            CodeInformation = new CodeInformation()
            {
                ModuleName = module_name
            };

            Reset();
        }

        /// <summary>
        /// Получить следующие символы, без перемещения курсора. Текущую позицию курсора, функция не меняет.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public string GetForwardSymbols(int quantity)
        {
            string buffer = String.Empty;
            char symbol;
            int count = 0;

            do
            {
                symbol = GetForwardSymbol(_cursor + count);
                count++;
                buffer += symbol;
            }
            while (buffer.Length < quantity && symbol != '\0');

            return buffer;
        }

        /// <summary>
        /// Получить следующий символ, без перемещения курсора. Текущую позицию курсора, функция не меняет.
        /// </summary>
        /// <param name="number"></param>
        /// <returns></returns>
        public char GetForwardSymbol(int cursor = -1)
        {
            if (cursor == -1)
                cursor = _cursor;

            cursor++;

            if (cursor >= _source.Length)
                return '\0';
            else
                return _source[cursor];
        }



        /// <summary>
        /// Передвинуть курсор вперед на один шаг.
        /// </summary>
        /// <returns></returns>
        public bool MoveNext()
        {
            _cursor++;
            CodeInformation.ColumnNumber++;

            if (_source.Length + 1 == _cursor)
                throw new CompilerException(CodeInformation, "Итератор исходного кода находится в конце файла.");


            if (End)
            {
                _current_symbol = '\0';
                return false;
            }

            _current_symbol = _source[_cursor];
            CheckNewLine();

            return true;

        }


        /// <summary>
        /// Перейти до указанного символа и вернуть пройденные символы.
        /// </summary>
        /// <param name="symbol">Символ до которого передвинуть курсор</param>
        private bool GoToSymbol(char symbol)
        {
            CodeInformation information = CodeInformation.Clone();
            do
            {
                if (!MoveNext())
                    if (Char.IsSymbol(symbol))
                        throw new CompilerException(information, "Ожидается символ: { " + symbol + " }");
                    else
                        return false;
            }
            while (_current_symbol != symbol);
            return true;
        }


        /// <summary>
        /// Получить все буквы и цифры.
        /// </summary>
        /// <returns></returns>
        public string GetLettersAndDigits()
        {
            string buffer = String.Empty;

            while (true)
            {
                if (!Char.IsLetter(_current_symbol) && !Char.IsNumber(_current_symbol) && _current_symbol != '_')
                    break;

                AddToBuffer(_current_symbol, ref buffer);

                if (!MoveNext())
                    break;
            }

            return buffer;
        }

        /// <summary>
        /// Получить только цифры и знак десятичных чисел (.)
        /// </summary>
        /// <returns></returns>
        public string GetDigits()
        {
            string buffer = String.Empty;

            while (true)
            {
                if (!Char.IsNumber(_current_symbol) && _current_symbol != '.')
                    break;

                AddToBuffer(_current_symbol, ref buffer);

                if (!MoveNext())
                    break;
            }

            return buffer;
        }

        /// <summary>
        /// Сброс итератора в начальное состояние.
        /// </summary>
        public void Reset()
        {
            _cursor = -1;

            CodeInformation.ColumnNumber = -1;
            CodeInformation.LineNumber = 1;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
