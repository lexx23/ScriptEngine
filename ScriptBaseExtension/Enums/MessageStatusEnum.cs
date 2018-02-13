using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Parser.Token;

namespace ScriptBaseFunctionsLibrary.Enums
{

    [LibraryEnum(Name = "СтатусСообщения", Alias = "MessageStatus")]
    public enum MessageStatusEnumInner
    {
        [EnumStringAttribute("БезСтатуса")]
        WithoutStatus,

        [EnumStringAttribute("Важное")]
        Important,

        [EnumStringAttribute("Внимание")]
        Attention,

        [EnumStringAttribute("Информация")]
        Information,

        [EnumStringAttribute("ОченьВажное")]
        VeryImportant,

        [EnumStringAttribute("Обычное")]
        Ordinary
    }
}
