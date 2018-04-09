using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library;

namespace ScriptBaseFunctionsLibrary.Enums
{
    public enum TextEncodingEnumInner
    {
        ANSI,
        OEM,
        UTF16,
        UTF8,
        UTF8NoBOM,
        [EnumStringAttribute("Системная")]
        System
    }

    [LibraryClassAttribute(Name = "TextEncoding", Alias = "КодировкаТекста", AsGlobal = true)]
    public class TextEncodingEnumClass : BaseEnum<TextEncodingEnumInner>
    {
        public TextEncodingEnumClass()
        {
            Create();
        }

    }
}
