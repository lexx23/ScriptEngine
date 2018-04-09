using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System.Xml;
using System.IO;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.XML
{
    [LibraryClassAttribute(Name = "XMLReader", Alias = "ЧтениеXML", RegisterType = true, AsGlobal = false)]
    public class ScriptXmlReader : LibraryModule<ScriptXmlReader>, IDisposable
    {
        XmlTextReader _reader;
        EmptyElemCompabilityState _emptyElemReadState = EmptyElemCompabilityState.Off;
        bool _attributesLoopReset = false;

        private enum EmptyElemCompabilityState
        {
            Off,
            EmptyElementEntered,
            EmptyElementRead
        }

        public XmlReader GetNativeReader()
        {
            return _reader;
        }

        private void InitReader(TextReader textInput)
        {
            _reader = new XmlTextReader(textInput);
            _reader.WhitespaceHandling = WhitespaceHandling.Significant;
        }

        private void CheckIfOpen()
        {
            if (_reader == null)
                throw new Exception("Файл не открыт");
        }

        #region Свойства

        [LibraryClassProperty(Alias = "URIПространстваИмен", Name = "NamespaceURI")]
        public string NamespaceURI
        {
            get
            {
                return _reader.NamespaceURI;
            }
        }

        [LibraryClassProperty(Alias = "Автономный", Name = "Standalone")]
        public bool Standalone
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        [LibraryClassProperty(Alias = "БазовыйURI", Name = "BaseURI")]
        public string BaseURI
        {
            get
            {
                return _reader.BaseURI;
            }
        }

        [LibraryClassProperty(Alias = "ВерсияXML", Name = "XMLVersion")]
        public string XMLVersion
        {
            get
            {
                return "1.0";
            }
        }

        [LibraryClassProperty(Alias = "Значение", Name = "Value")]
        public string Value
        {
            get
            {
                return _reader.Value;
            }
        }

        [LibraryClassProperty(Alias = "ИмеетЗначение", Name = "HasValue")]
        public bool HasValue
        {
            get
            {
                return _reader.HasValue;
            }
        }

        [LibraryClassProperty(Alias = "ИмеетИмя", Name = "HasName")]
        public bool HasName
        {
            get
            {
                return _reader.LocalName != String.Empty;
            }
        }

        [LibraryClassProperty(Alias = "Имя", Name = "Name")]
        public string Name
        {
            get
            {
                return _reader.Name;
            }
        }

        [LibraryClassProperty(Alias = "ИмяНотации", Name = "NotationName")]
        public string NotationName
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        [LibraryClassProperty(Alias = "КодировкаXML", Name = "XMLEncoding")]
        public string XMLEncoding
        {
            get
            {
                return _reader.Encoding.WebName;
            }
        }

        [LibraryClassProperty(Alias = "КодировкаИсточника", Name = "InputEncoding")]
        public string InputEncoding
        {
            get
            {
                return XMLEncoding;
            }
        }

        private int Depth
        {
            get
            {
                if (_reader.NodeType == XmlNodeType.EndElement)
                    return _reader.Depth;

                if (_emptyElemReadState == EmptyElemCompabilityState.EmptyElementRead)
                    return _reader.Depth;

                return _reader.Depth + 1;
            }
        }

        [LibraryClassProperty(Alias = "КонтекстПространствИмен", Name = "NamespaceContext")]
        public ScriptXmlNamespace NamespaceContext
        {
            get
            {
                return new ScriptXmlNamespace(Depth, _reader.GetNamespacesInScope(XmlNamespaceScope.All));
            }
        }

        [LibraryClassProperty(Alias = "ЛокальноеИмя", Name = "LocalName")]
        public string LocalName
        {
            get
            {
                return _reader.LocalName;
            }
        }


        [LibraryClassProperty(Alias = "Префикс", Name = "Prefix")]
        public string Prefix
        {
            get
            {
                return _reader.Prefix;
            }
        }

        [LibraryClassProperty(Alias = "ПубличныйИдентификатор", Name = "PublicId")]
        public string PublicId
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        [LibraryClassProperty(Alias = "СистемныйИдентификатор", Name = "SystemId")]
        public string SystemId
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        private XmlNodeType _node_type
        {
            get
            {
                if (_emptyElemReadState == EmptyElemCompabilityState.EmptyElementRead)
                   return XmlNodeType.EndElement;
                else
                    return _reader.NodeType;
            }
        }


        [LibraryClassProperty(Alias = "ТипУзла", Name = "NodeType")]
        public IValue NodeType
        {
            get
            {
                return ValueFactory.Create(_node_type);
            }
        }

        [LibraryClassProperty(Alias = "ЭтоАтрибутПоУмолчанию", Name = "IsDefaultAttribute")]
        public bool IsDefaultAttribute
        {
            get
            {
                return _reader.IsDefault;
            }
        }

        [LibraryClassProperty(Alias = "ЭтоПробельныеСимволы", Name = "IsWhitespace")]
        public bool IsWhitespace
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        [LibraryClassProperty(Alias = "Язык", Name = "Lang")]
        public string Lang
        {
            get
            {
                return _reader.XmlLang;
            }
        }

        [LibraryClassProperty(Alias = "ИгнорироватьПробелы", Name = "IgnoreWhitespace")]
        public bool IgnoreWhitespace
        {
            get
            {
                return _reader.WhitespaceHandling == WhitespaceHandling.None;
            }
            set
            {
                _reader.WhitespaceHandling = value ? WhitespaceHandling.None : WhitespaceHandling.All;
            }
        }

        [LibraryClassProperty(Alias = "Параметры", Name = "Settings")]
        public object Settings
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        [LibraryClassProperty(Alias = "ПробельныеСимволы", Name = "Space")]
        public object Space
        {
            get
            {
                throw new NotImplementedException();
                //return _reader.XmlSpace;
            }
        }

        [LibraryClassProperty(Alias = "ЭтоСимвольныеДанные", Name = "IsCharacters")]
        public bool IsCharacters
        {
            get
            {
                return _reader.NodeType == XmlNodeType.Text || _reader.NodeType == XmlNodeType.CDATA || _reader.NodeType == XmlNodeType.SignificantWhitespace;
            }
        }
        #endregion

        #region Методы

        [LibraryClassMethodAttribute(Name = "OpenFile", Alias = "ОткрытьФайл")]
        public void OpenFile(string path)
        {
            if (_reader != null)
                throw new Exception("Поток XML уже открыт");
            var textInput = new StreamReader(path);
            InitReader(textInput);
        }

        [LibraryClassMethodAttribute(Name = "SetString", Alias = "УстановитьСтроку")]
        public void SetString(string content)
        {
            if (_reader != null)
                throw new Exception("Поток XML уже открыт");

            var textInput = new StringReader(content);
            InitReader(textInput);
        }

        [LibraryClassMethodAttribute(Name = "AttributeNamespaceURI", Alias = "URIПространстваИменАтрибута")]
        public string AttributeNamespaceURI(int index)
        {
            throw new NotImplementedException();
        }

        [LibraryClassMethodAttribute(Name = "AttributeValue", Alias = "ЗначениеАтрибута")]
        public IValue AttributeValue(IValue indexOrName, string URIIfGiven = null)
        {
            string attributeValue = null;

            if (indexOrName.BaseType == ValueTypeEnum.NUMBER)
            {
                attributeValue = _reader.GetAttribute((int)indexOrName.AsNumber());
            }
            else if (indexOrName.BaseType == ValueTypeEnum.STRING)
            {
                if (URIIfGiven == String.Empty)
                    attributeValue = _reader.GetAttribute(indexOrName.AsString());
                else
                    attributeValue = _reader.GetAttribute(indexOrName.AsString(), URIIfGiven);
            }
            else
            {
                throw new Exception("Неверный тип аргумента");
            }

            if (attributeValue != null)
                return ValueFactory.Create(attributeValue);
            else
                return ValueFactory.Create();

        }

        [LibraryClassMethodAttribute(Name = "AttributeName", Alias = "ИмяАтрибута")]
        public string AttributeName(int index)
        {
            _reader.MoveToAttribute(index);
            var name = _reader.Name;
            _reader.MoveToElement();

            return name;
        }

        [LibraryClassMethodAttribute(Name = "AttributeCount", Alias = "КоличествоАтрибутов")]
        public int AttributeCount()
        {
            return _reader.AttributeCount;
        }


        [LibraryClassMethodAttribute(Name = "AttributeLocalName", Alias = "ЛокальноеИмяАтрибута")]
        public string AttributeLocalName(int index)
        {
            _reader.MoveToAttribute(index);
            var name = _reader.LocalName;
            _reader.MoveToElement();

            return name;
        }


        [LibraryClassMethodAttribute(Name = "FirstDeclaration", Alias = "ПервоеОбъявление")]
        public bool FirstDeclaration()
        {
            throw new NotImplementedException();
        }


        [LibraryClassMethodAttribute(Name = "FirstAttribute", Alias = "ПервыйАтрибут")]
        public bool FirstAttribute()
        {
            return _reader.MoveToFirstAttribute();
        }

        [LibraryClassMethodAttribute(Name = "GetAttribute", Alias = "ПолучитьАтрибут")]
        public IValue GetAttribute(IValue indexOrName, string URIIfGiven = null)
        {
            return AttributeValue(indexOrName, URIIfGiven);
        }

        [LibraryClassMethodAttribute(Name = "AttributePrefix", Alias = "ПрефиксАтрибута")]
        public string AttributePrefix(int index)
        {
            _reader.MoveToAttribute(index);
            var name = _reader.Prefix;
            _reader.MoveToElement();

            return name;
        }

        [LibraryClassMethodAttribute(Name = "Skip", Alias = "Пропустить")]
        public void Skip()
        {
            if (_emptyElemReadState == EmptyElemCompabilityState.EmptyElementEntered)
            {
                _emptyElemReadState = EmptyElemCompabilityState.EmptyElementRead;
                return;
            }

            V8CompatibleSkip();
            CheckEmptyElementEntering();
        }

        private void V8CompatibleSkip()
        {
            if (_reader.NodeType == XmlNodeType.Element)
            {
                int initialDepth = _reader.Depth;
                while (_reader.Read() && _reader.Depth > initialDepth) ;
                System.Diagnostics.Debug.Assert(_reader.NodeType == XmlNodeType.EndElement);
            }
            else
            {
                _reader.Skip();
            }
        }

        [LibraryClassMethodAttribute(Name = "Read", Alias = "Прочитать")]
        public bool Read()
        {
            if (_emptyElemReadState == EmptyElemCompabilityState.EmptyElementEntered)
            {
                _emptyElemReadState = EmptyElemCompabilityState.EmptyElementRead;
                return true;
            }
            else
            {
                bool readingDone = _reader.Read();
                CheckEmptyElementEntering();
                return readingDone;
            }
        }

        private void CheckEmptyElementEntering()
        {
            _attributesLoopReset = false;
            if (_reader.IsEmptyElement)
                _emptyElemReadState = EmptyElemCompabilityState.EmptyElementEntered;
            else
                _emptyElemReadState = EmptyElemCompabilityState.Off;
        }

        private bool IsEndElement()
        {
            var isEnd = _node_type == XmlNodeType.EndElement;
            return isEnd;
        }

        private bool ReadAttributeInternal()
        {
            if (IsEndElement() && !_attributesLoopReset)
            {
                _attributesLoopReset = true;
                return _reader.MoveToFirstAttribute();
            }

            return _reader.MoveToNextAttribute();
        }

        [LibraryClassMethodAttribute(Name = "ReadAttribute", Alias = "ПрочитатьАтрибут")]
        public bool ReadAttribute()
        {
            return ReadAttributeInternal();
        }

        [LibraryClassMethodAttribute(Name = "NextDeclaration", Alias = "СледующееОбъявление")]
        public void NextDeclaration()
        {
            throw new NotImplementedException();
        }

        [LibraryClassMethodAttribute(Name = "NextAttribute", Alias = "СледующийАтрибут")]
        public bool NextAttribute()
        {
            return ReadAttributeInternal();
        }

        [LibraryClassMethodAttribute(Name = "AttributeType", Alias = "ТипАтрибута")]
        public void AttributeType()
        {
            throw new NotImplementedException();
        }

        [LibraryClassMethodAttribute(Name = "Close", Alias = "Закрыть")]
        public void Close()
        {
            Dispose();
        }

        [LibraryClassMethodAttribute(Name = "MoveToContent", Alias = "ПерейтиКСодержимому")]
        public IValue MoveToContent()
        {
            var nodeType = _reader.MoveToContent();
            CheckEmptyElementEntering();
            return ValueFactory.Create(nodeType);
        }

        #endregion

        public void Dispose()
        {
            if (_reader != null)
            {
                _reader.Close();
                _reader = null;
            }
        }

        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            return new ScriptXmlReader();
        }

    }
}
