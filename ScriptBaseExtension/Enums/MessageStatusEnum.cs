using ScriptEngine.EngineBase.Library;
using ScriptEngine.EngineBase.Library.Attributes;

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
