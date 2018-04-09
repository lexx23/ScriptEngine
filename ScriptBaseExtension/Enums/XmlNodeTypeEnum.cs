using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library;
using System.Xml;

namespace ScriptBaseFunctionsLibrary.Enums
{
    [LibraryClassAttribute(Name = "XMLNodeType", Alias = "ТипУзлаXML", AsGlobal = true)]
    public class XmlNodeTypeEnumClass : BaseEnum<XmlNodeType>
    {
        public XmlNodeTypeEnumClass()
        {
            IValue value;
            IVariable var;

            value = ValueFactory.Create(XmlNodeType.Attribute);
            var = new Variable() { Name = "Attribute", Alias = "Атрибут", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.ProcessingInstruction);
            var = new Variable() { Name = "ProcessingInstruction", Alias = "ИнструкцияОбработки", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Comment);
            var = new Variable() { Name = "Comment", Alias = "Комментарий", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.EndEntity);
            var = new Variable() { Name = "EndEntity", Alias = "КонецСущности", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.EndElement);
            var = new Variable() { Name = "EndElement", Alias = "КонецЭлемента", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Element);
            var = new Variable() { Name = "StartElement", Alias = "НачалоЭлемента", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.None);
            var = new Variable() { Name = "None", Alias = "Ничего", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Notation);
            var = new Variable() { Name = "Notation", Alias = "Нотация", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.XmlDeclaration);
            var = new Variable() { Name = "XMLDeclaration", Alias = "ОбъявлениеXML", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.DocumentType);
            var = new Variable() { Name = "DocumentTypeDefinition", Alias = "ОпределениеТипаДокумента", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Whitespace);
            var = new Variable() { Name = "Whitespace", Alias = "ПробельныеСимволы", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.CDATA);
            var = new Variable() { Name = "CDATASection", Alias = "СекцияCDATA", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.EntityReference);
            var = new Variable() { Name = "EntityReference", Alias = "СсылкаНаСущность", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Entity);
            var = new Variable() { Name = "Entity", Alias = "Сущность", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);

            value = ValueFactory.Create(XmlNodeType.Text);
            var = new Variable() { Name = "Text", Alias = "Текст", Public = true, Reference = new ReferenceReadOnly(value) };
            Properties.Add(var);
        }
    }
}
