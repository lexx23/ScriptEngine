using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem;
using ScriptBaseFunctionsLibrary.BuildInTypes.XML;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "XMLWriter", Alias = "ЗаписьXML", RegisterType = true, AsGlobal = false)]
    public class ScriptXmlWriter : LibraryModule<ScriptXmlWriter>, IDisposable
    {
        private XmlTextWriter _writer;
        private StringWriter _stringWriter;
        private int _depth;
        private Stack<Dictionary<string, string>> _nsmap = new Stack<Dictionary<string, string>>();

        private const int INDENT_SIZE = 4;

        public ScriptXmlWriter()
        {
            _nsmap.Push(new Dictionary<string, string>());
        }

        private void EnterScope()
        {
            ++_depth;
            var newMap = _nsmap.Peek().ToDictionary((kv) => kv.Key, (kv) => kv.Value);
            _nsmap.Push(newMap);
        }

        private void ExitScope()
        {
            _nsmap.Pop();
            --_depth;
        }

        #region Properties

        [LibraryClassProperty(Alias = "Отступ", Name = "Indent")]
        public bool Indent
        {
            get
            {
                return _writer.Formatting.HasFlag(Formatting.Indented);
            }
            set
            {
                if (value)
                {
                    _writer.Formatting = Formatting.Indented;
                }
                else
                {
                    _writer.Formatting = Formatting.None;
                }
            }
        }

        [LibraryClassProperty(Alias = "КонтекстПространствИмен", Name = "NamespaceContext")]
        public ScriptXmlNamespace NamespaceContext
        {
            get
            {
                return new ScriptXmlNamespace(_depth, _nsmap.Peek());
            }
        }


        [LibraryClassProperty(Alias = "Параметры", Name = "Settings")]
        public object Settings
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        #endregion

        #region Methods

        [LibraryClassMethodAttribute(Name = "WriteAttribute", Alias = "ЗаписатьАтрибут")]
        public void WriteAttribute(string localName, string valueOrNamespace, string value = null)
        {
            if (value == null)
            {
                _writer.WriteAttributeString(localName, valueOrNamespace);
            }
            else
            {
                _writer.WriteAttributeString(localName, valueOrNamespace, value);
            }
        }

        [LibraryClassMethodAttribute(Name = "WriteRaw", Alias = "ЗаписатьБезОбработки")]
        public void WriteRaw(string data)
        {
            _writer.WriteRaw(data);
        }

        [LibraryClassMethodAttribute(Name = "WriteProcessingInstruction", Alias = "ЗаписатьИнструкциюОбработки")]
        public void WriteProcessingInstruction(string name, string text)
        {
            _writer.WriteProcessingInstruction(name, text);
        }

        [LibraryClassMethodAttribute(Name = "WriteComment", Alias = "ЗаписатьКомментарий")]
        public void WriteComment(string text)
        {
            _writer.WriteComment(text);
        }


        [LibraryClassMethodAttribute(Name = "WriteEndAttribute", Alias = "ЗаписатьКонецАтрибута")]
        public void WriteEndAttribute()
        {
            _writer.WriteEndAttribute();
        }

        [LibraryClassMethodAttribute(Name = "WriteEndElement", Alias = "ЗаписатьКонецЭлемента")]
        public void WriteEndElement()
        {
            _writer.WriteEndElement();
            ExitScope();
        }

        [LibraryClassMethodAttribute(Name = "WriteStartAttribute", Alias = "ЗаписатьНачалоАтрибута")]
        public void WriteStartAttribute(string name, string ns = null)
        {
            if (ns == null)
            {
                _writer.WriteStartAttribute(name);
            }
            else
            {
                _writer.WriteStartAttribute(name, ns);
            }

        }

        [LibraryClassMethodAttribute(Name = "WriteStartElement", Alias = "ЗаписатьНачалоЭлемента")]
        public void WriteStartElement(string name, string ns = null)
        {
            if (ns == null)
            {
                _writer.WriteStartElement(name);
            }
            else
            {
                _writer.WriteStartElement(name, ns);
            }
            EnterScope();
        }

        [LibraryClassMethodAttribute(Name = "WriteXMLDeclaration", Alias = "ЗаписатьОбъявлениеXML")]
        public void WriteXMLDeclaration()
        {
            _writer.WriteStartDocument();
        }

        [LibraryClassMethodAttribute(Name = "WriteCDATASection", Alias = "ЗаписатьСекциюCDATA")]
        public void WriteCDATASection(string data)
        {
            _writer.WriteCData(data);
        }

        [LibraryClassMethodAttribute(Name = "WriteNamespaceMapping", Alias = "ЗаписатьСоответствиеПространстваИмен")]
        public void WriteNamespaceMapping(string prefix, string uri)
        {
            _writer.WriteAttributeString("xmlns", prefix, null, uri);
            _nsmap.Peek()[prefix] = uri;
        }

        [LibraryClassMethodAttribute(Name = "WriteEntityReference", Alias = "ЗаписатьСсылкуНаСущность")]
        public void WriteEntityReference(string name)
        {
            _writer.WriteEntityRef(name);
        }

        [LibraryClassMethodAttribute(Name = "WriteText", Alias = "ЗаписатьТекст")]
        public void WriteText(string text)
        {
            _writer.WriteString(text);
        }

        [LibraryClassMethodAttribute(Name = "WriteCurrent", Alias = "ЗаписатьТекущий")]
        public void WriteCurrent(ScriptXmlReader reader)
        {
            _writer.WriteNode(reader.GetNativeReader(), false);
        }

        [LibraryClassMethodAttribute(Name = "WriteDocumentType", Alias = "ЗаписатьТипДокумента")]
        public void WriteDocumentType(string name, string varArg2, string varArg3 = null, string varArg4 = null)
        {
            if (varArg4 != null)
            {
                _writer.WriteDocType(name, varArg2, varArg3, varArg4);
            }
            else if (varArg3 != null)
            {
                _writer.WriteDocType(name, null, varArg2, varArg3);
            }
            else
            {
                _writer.WriteDocType(name, null, null, varArg2);
            }
        }

        [LibraryClassMethodAttribute(Name = "LookupPrefix", Alias = "НайтиПрефикс")]
        public IValue LookupPrefix(string uri)
        {
            string prefix = _writer.LookupPrefix(uri);
            if (prefix == null)
                return ValueFactory.Create();
            return ValueFactory.Create(prefix);
        }

        [LibraryClassMethodAttribute(Name = "Close", Alias = "Закрыть")]
        public IValue Close()
        {
            if (IsOpenForString())
            {
                _writer.Flush();
                _writer.Close();
                _stringWriter.Close();

                var sb = _stringWriter.GetStringBuilder();
                Dispose();

                return ValueFactory.Create(sb.ToString());
            }
            else
            {
                _writer.Flush();
                _writer.Close();
                Dispose();

                return ValueFactory.Create();
            }

        }

        [LibraryClassMethodAttribute(Name = "OpenFile", Alias = "ОткрытьФайл")]
        public void OpenFile(string path, string encoding = null, IValue addBOM = null)
        {
            Encoding enc;
            if (addBOM.BaseType == ValueTypeEnum.NULL)
                enc = TextEncoding.GetEncodingByName(encoding, true);
            else
                enc = TextEncoding.GetEncodingByName(encoding, addBOM.AsBoolean());

            _writer = new XmlTextWriter(path, enc);
            _stringWriter = null;
            SetDefaultOptions();
        }

        [LibraryClassMethodAttribute(Name = "SetString", Alias = "УстановитьСтроку")]
        public void SetString(string encoding = null)
        {
            Encoding enc = TextEncoding.GetEncodingByName(encoding, true);
            _stringWriter = new StringWriterWithEncoding(enc);
            _writer = new XmlTextWriter(_stringWriter);
            SetDefaultOptions();
        }

        private void SetDefaultOptions()
        {
            _writer.Indentation = INDENT_SIZE;
            this.Indent = true;
        }

        #endregion

        private bool IsOpenForString()
        {
            return _stringWriter != null;
        }

        private sealed class StringWriterWithEncoding : StringWriter
        {
            private readonly Encoding encoding;

            public StringWriterWithEncoding(Encoding encoding)
            {
                this.encoding = encoding;
            }

            public override Encoding Encoding
            {
                get { return encoding; }
            }
        }

        public void Dispose()
        {
            if (_writer != null)
                _writer.Close();
            if (_stringWriter != null)
                _stringWriter.Dispose();

            _writer = null;
            _stringWriter = null;
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            return new ScriptXmlWriter();
        }

    }
}
