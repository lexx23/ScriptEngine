using ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System.Collections.Generic;
using System.Linq;
using System;


namespace ScriptBaseFunctionsLibrary.BuildInTypes.XML
{
    [LibraryClassAttribute(Name = "XMLNamespaceContext", Alias = "КонтекстПространствИменXML", RegisterType = true, AsGlobal = false)]
    public class ScriptXmlNamespace: LibraryModule<ScriptXmlNamespace>
    {
        readonly IDictionary<string, string> _nsmap;

        public ScriptXmlNamespace(int depth, IDictionary<string, string> map)
        {
            Depth = depth;
            _nsmap = map;
        }

        [LibraryClassProperty(Alias = "Глубина", Name = "Depth")]
        public int Depth { get; }

        [LibraryClassProperty(Alias = "ПространствоИменПоУмолчанию", Name = "DefaultNamespace")]
        public string DefaultNamespace
        {
            get
            {
                if (_nsmap.ContainsKey(""))
                    return _nsmap[""];
                return "";
            }
        }


        [LibraryClassMethodAttribute(Name = "NamespaceURIs", Alias = "URIПространствИмен")]
        public ScriptArray NamespaceUris()
        {
            var result = ScriptArray.Constructor(null) as ScriptArray;
            foreach (var ns in _nsmap.Values.Distinct())
            {
                result.Add(ValueFactory.Create(ns));
            }
            return result;
        }

        [LibraryClassMethodAttribute(Name = "LookupNamespaceURI", Alias = "НайтиURIПространстваИмен")]
        public IValue LookupNamespaceUri(string prefix)
        {
            if (_nsmap.ContainsKey(prefix))
                return ValueFactory.Create(_nsmap[prefix]);
            return ValueFactory.Create();
        }

        [LibraryClassMethodAttribute(Name = "LookupPrefix", Alias = "НайтиПрефикс")]
        public IValue LookupPrefix(string namespaceUri)
        {
            foreach (var kv in _nsmap)
            {
                if (kv.Value.Equals(namespaceUri, StringComparison.Ordinal))
                    return ValueFactory.Create(kv.Key);
            }
            return ValueFactory.Create();
        }

        [LibraryClassMethodAttribute(Name = "Prefixes", Alias = "Префиксы")]
        public ScriptArray Prefixes(string namespaceUri)
        {
            var result = ScriptArray.Constructor(null) as ScriptArray;
            foreach (var prefix in _nsmap
                     .Where((arg) => arg.Value.Equals(namespaceUri, StringComparison.Ordinal))
                     .Select((arg) => arg.Key))
            {
                result.Add(ValueFactory.Create(prefix));
            }

            return result;
        }

        [LibraryClassMethodAttribute(Name = "NamespaceMappings", Alias = "СоответствияПространствИмен")]
        public ScriptMap NamespaceMappings()
        {
            var result = ScriptMap.Constructor(null) as ScriptMap;
            foreach (var data in _nsmap)
            {
                result.Insert(ValueFactory.Create(data.Key), ValueFactory.Create(data.Value));
            }

            return result;
        }

    }
}
