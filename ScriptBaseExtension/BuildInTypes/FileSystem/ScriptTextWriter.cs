using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Text;
using System.IO;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem
{
    [LibraryClassAttribute(Name = "TextWriter", Alias = "ЗаписьТекста", RegisterType = true, AsGlobal = false)]
    public class ScriptTextWrite : LibraryModule<ScriptTextWrite>, IDisposable
    {
        StreamWriter _writer;
        string _lineDelimiter = "";
        string _eolReplacement = "";

        public ScriptTextWrite()
        {

        }

        public ScriptTextWrite(string path, IValue encoding)
        {
            Open(path, encoding);
        }

        public ScriptTextWrite(string path, IValue encoding, bool append)
        {
            Open(path, encoding, null, append);
        }

        /// <summary>
        /// Открывает файл для записи.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="encoding">Кодировка (необязательный). По умолчанию используется utf-8</param>
        /// <param name="lineDelimiter">Разделитель строк (необязательный).</param>
        /// <param name="append">Признак добавления в конец файла (необязательный)</param>
        /// <param name="eolReplacement">Разделитель строк в файле (необязательный).</param>
        [LibraryClassMethod(Alias = "Открыть", Name = "Open")]
        public void Open(string path, IValue encoding = null, string lineDelimiter = null, bool append = false, string eolReplacement = null)
        {
            _lineDelimiter = lineDelimiter ?? "\n";
            _eolReplacement = eolReplacement ?? "\r\n";

            Encoding enc = null;
            if (encoding == null)
                enc = new UTF8Encoding(true);
            if (encoding != null && encoding.BaseType == ValueTypeEnum.NULL)
                enc = new UTF8Encoding(true);

            if(enc == null)
            {
                enc = TextEncoding.GetEncoding(encoding);
                if (enc.WebName == "utf-8" && append == true)
                    enc = new UTF8Encoding(false);
            }

            _writer = new StreamWriter(path, append, enc);
            _writer.AutoFlush = true;
        }

        [LibraryClassMethod(Alias = "Закрыть", Name = "Close")]
        public void Close()
        {
            Dispose();
        }

        /// <summary>
        /// Записывает текст "как есть"
        /// </summary>
        /// <param name="what">Текст для записи</param>
        [LibraryClassMethod(Alias = "Записать", Name = "Write")]
        public void Write(string what)
        {
            ThrowIfNotOpened();

            var stringToOutput = what.Replace("\n", _eolReplacement);

            _writer.Write(stringToOutput);
        }

        /// <summary>
        /// Записывает текст и добавляет перевод строки
        /// </summary>
        /// <param name="what">Текст для записи</param>
        /// <param name="delimiter">Разделитель строк</param>
        [LibraryClassMethod(Alias = "ЗаписатьСтроку", Name = "WriteLine")]
        public void WriteLine(string what, string delimiter = null)
        {
            ThrowIfNotOpened();

            Write(what);

            var sDelimiter = _lineDelimiter;
            if (delimiter != null)
                sDelimiter = delimiter;
            else
                sDelimiter = "\n";

           Write(sDelimiter);
        }

        public void ThrowIfNotOpened()
        {
            if (_writer == null)
                throw new Exception("Файл не открыт");
        }

        public void Dispose()
        {
            if (_writer != null)
            {
                _writer.Dispose();
                _writer = null;
            }
        }

        /// <summary>
        /// Создает объект с начальными значениями имени файла и кодировки.
        /// </summary>
        /// <param name="path">Имя файла</param>
        /// <param name="encoding">Кодировка в виде строки</param>
        /// <param name="lineDelimiter">Символ - разделитель строк</param>
        /// <param name="append">Признак добавления в конец файла (необязательный)</param>
        /// <param name="eolReplacement">Разделитель строк в файле (необязательный).</param>
        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            //IValue path, IValue encoding = null, IValue lineDelimiter = null, IValue append = null, IValue eolReplacement = null)
            string path = null, lineDelimiter = "\n", eolReplacement = "\r\n";
            IValue encoding = null;

            var result = new ScriptTextWrite();
            if (parameters.Length == 0)
                return result;

            if (parameters.Length > 0)
                path = parameters[0].AsString();

            if (parameters.Length > 1)
                encoding = parameters[1];

            if (parameters.Length > 2)
                lineDelimiter = parameters[2].AsString();

            bool isAppend = false;
            if (parameters.Length > 3)
                isAppend = parameters[3].AsBoolean();

            if (parameters.Length > 4)
                eolReplacement = parameters[4].AsString();

            result.Open(path, encoding, lineDelimiter, isAppend, eolReplacement);

            return result;
        }

    }
}
