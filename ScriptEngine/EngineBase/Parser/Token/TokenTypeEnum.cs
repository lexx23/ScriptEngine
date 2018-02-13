using ScriptEngine.EngineBase.Library.Attributes;

namespace ScriptEngine.EngineBase.Parser.Token
{
    public enum TokenTypeEnum
    {
        [EnumStringAttribute("Число")]
        NUMBER = 0,
        [EnumStringAttribute("Литерал")]
        LITERAL = 1,
        [EnumStringAttribute("Идентификатор")]
        IDENTIFIER = 2,
        [EnumStringAttribute("Символ")]
        PUNCTUATION = 5
    }
}
