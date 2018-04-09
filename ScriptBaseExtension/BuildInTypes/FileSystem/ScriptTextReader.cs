using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem
{
    /// <summary>
    /// Предназначен для последовательного чтения файлов, в том числе большого размера.
    /// </summary>

    [LibraryClassAttribute(Name = "TextReader", Alias = "ЧтениеТекста", RegisterType = true, AsGlobal = false)]
    public class ScriptTextReader : LibraryModule<ScriptTextReader>, IDisposable
    {
        // TextReader _reader;
        CustomLineFeedStreamReader _reader;
        string _lineDelimiter = "\n";

        public ScriptTextReader()
        {
            AnalyzeDefaultLineFeed = true;
        }

        private StreamReader OpenReader(string filename, FileShare shareMode, Encoding encoding = null)
        {
            var input = new FileStream(filename, FileMode.Open, FileAccess.Read, shareMode);

            if (encoding == null)
            {
                var enc = AssumeEncoding(input);
                return new StreamReader(input, enc, true);
            }

            return new StreamReader(input, encoding);
        }

        private Encoding AssumeEncoding(Stream inputStream, Encoding fallbackEncoding = null)
        {
            if (fallbackEncoding == null)
            {
                if (System.Environment.OSVersion.Platform == PlatformID.Unix || System.Environment.OSVersion.Platform == PlatformID.MacOSX)
                    fallbackEncoding = Encoding.UTF8;
                else
                    fallbackEncoding = Encoding.Default;
            }

            var enc = fallbackEncoding;

            // *** Detect byte order mark if any - otherwise assume default
            byte[] buffer = new byte[5];

            inputStream.Read(buffer, 0, 5);
            inputStream.Position = 0;

            if (buffer[0] == 0xef && buffer[1] == 0xbb && buffer[2] == 0xbf)
                enc = Encoding.UTF8;
            else if (buffer[0] == 0xfe && buffer[1] == 0xff)
                enc = Encoding.Unicode;
            else if (buffer[0] == 0 && buffer[1] == 0 && buffer[2] == 0xfe && buffer[3] == 0xff)
                enc = Encoding.UTF32;
            else if (buffer[0] == 0x2b && buffer[1] == 0x2f && buffer[2] == 0x76)
                enc = Encoding.UTF7;
            else if (buffer[0] == '#' && buffer[1] == '!')
            {
                /* Если в начале файла присутствует shebang, считаем, что файл в UTF-8*/
                enc = Encoding.UTF8;
            }

            return enc;
        }


        /// <summary>
        /// Открывает текстовый файл для чтения. Ранее открытый файл закрывается. 
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="encoding">Кодировка</param>
        /// <param name="lineDelimiter">Раздедитель строк</param>
        /// <param name="eolDelimiter">Разделитель строк в файле</param>
        /// <param name="monopoly">Открывать монопольно</param>
        [LibraryClassMethod(Alias = "Открыть", Name = "Open")]
        public void Open(string path, IValue encoding = null, string lineDelimiter = "\n", string eolDelimiter = null, bool? monopoly = null)
        {
            Close();
            TextReader imReader;
            var shareMode = (monopoly ?? true) ? FileShare.None : FileShare.ReadWrite;
            if (encoding == null)
            {
                imReader = OpenReader(path, shareMode);
            }
            else
            {
                var enc = TextEncoding.GetEncoding(encoding);
                imReader = OpenReader(path, shareMode, enc);
            }
            _lineDelimiter = lineDelimiter ?? "\n";
            if (eolDelimiter != null)
                _reader = new CustomLineFeedStreamReader(imReader, eolDelimiter, AnalyzeDefaultLineFeed);
            else
                _reader = new CustomLineFeedStreamReader(imReader, "\r\n", AnalyzeDefaultLineFeed);

        }

        private bool AnalyzeDefaultLineFeed { get; set; }

        private int ReadNext()
        {
            return _reader.Read();
        }

        /// <summary>
        /// Считывает строку указанной длины или до конца файла.
        /// </summary>
        /// <param name="size">Размер строки. Если не задан, текст считывается до конца файла</param>
        /// <returns>Строка - считанная строка, Неопределено - в файле больше нет данных</returns>
        [LibraryClassMethod(Alias = "Прочитать", Name = "Read")]
        public IValue ReadAll(int size = 0)
        {
            RequireOpen();

            var sb = new StringBuilder();
            var read = 0;
            do
            {
                var ic = ReadNext();
                if (ic == -1)
                    break;
                sb.Append((char)ic);
                ++read;
            } while (size == 0 || read < size);

            if (sb.Length == 0)
                return ValueFactory.Create();

            return ValueFactory.Create(sb.ToString());
        }

        /// <summary>
        /// Считывает очередную строку текстового файла.
        /// </summary>
        /// <param name="overridenLineDelimiter">Подстрока, считающаяся концом строки. Переопределяет РазделительСтрок, 
        /// переданный в конструктор или в метод Открыть</param>
        /// <returns>Строка - в случае успешного чтения, Неопределено - больше нет данных</returns>
        [LibraryClassMethod(Alias = "ПрочитатьСтроку", Name = "ReadLine")]
        public IValue ReadLine(string overridenLineDelimiter = null)
        {
            RequireOpen();
            string l = _reader.ReadLine(overridenLineDelimiter ?? _lineDelimiter);

            if (l == null)
                return ValueFactory.Create();

            return ValueFactory.Create(l);
        }

        /// <summary>
        /// Закрывает открытый текстовый файл. Если файл был открыт монопольно, то после закрытия он становится доступен.
        /// </summary>
        [LibraryClassMethod(Alias = "Закрыть", Name = "Close")]
        public void Close()
        {
            Dispose();
        }

        private void RequireOpen()
        {
            if (_reader == null)
            {
                throw new Exception("Файл не открыт");
            }
        }


        /// <summary>
        /// Открывает текстовый файл для чтения. Работает аналогично методу Открыть.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="encoding">Кодировка файла</param>
        /// <param name="lineDelimiter">Разделитель строк</param>
        /// <param name="eolDelimiter">Разделитель строк в файле</param>
        /// <param name="monopoly">Открывать файл монопольно</param>
        /// <returns>ЧтениеТекста</returns>
        //[LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        //public static IValue Constructor(IValue path, IValue encoding = null,IValue lineDelimiter = null, IValue eolDelimiter = null, IValue monopoly = null)
        //{
        //    var reader = new ScriptTextReader();
        //    if (lineDelimiter != null)
        //        reader.AnalyzeDefaultLineFeed = false;

        //    reader.Open(path.AsString(), encoding,
        //        lineDelimiter?.GetRawValue().AsString() ?? "\n",
        //        eolDelimiter?.GetRawValue().AsString(),
        //        monopoly?.AsBoolean() ?? true);

        //    return reader;
        //}

        /// <summary>
        /// Открывает текстовый файл для чтения. Работает аналогично методу Открыть.
        /// </summary>
        /// <param name="path">Путь к файлу</param>
        /// <param name="encoding">Кодировка файла</param>
        /// <param name="lineDelimiter">Разделитель строк</param>
        /// <param name="eolDelimiter">Разделитель строк в файле</param>
        /// <param name="monopoly">Открывать файл монопольно</param>
        /// <returns>ЧтениеТекста</returns>        
        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            // path, encoding, lineDelimiter,eolDelimiter, monopoly
            var reader = new ScriptTextReader();
            if (parameters.Length == 0)
            {
                reader.AnalyzeDefaultLineFeed = false;
                return reader;
            }

            if (parameters.Length == 1)
            {
                reader.AnalyzeDefaultLineFeed = false;
                reader.Open(parameters[0].AsString(), null, "\n", "\r\n");
                return reader;
            }

            var encoding = ValueFactory.Create();
            var lineDelimiter = "\n";
            var eolDelimiter = "";
            var monopoly = false;

            if (parameters.Length > 1)
                encoding = parameters[1];
            if (parameters.Length > 2)
                lineDelimiter = parameters[2].AsString();
            if (parameters.Length > 3)
                eolDelimiter = parameters[3].AsString();
            if (parameters.Length > 4)
                monopoly = parameters[4].AsBoolean();

            if (lineDelimiter != string.Empty)
                reader.AnalyzeDefaultLineFeed = false;

            reader.Open(parameters[0].AsString(), encoding, lineDelimiter,  eolDelimiter, monopoly);

            return reader;
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Dispose();
                _reader = null;
            }
        }

        #endregion
    }
}
