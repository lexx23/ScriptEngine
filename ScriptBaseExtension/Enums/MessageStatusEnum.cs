using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library;

namespace ScriptBaseFunctionsLibrary.Enums
{
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


    [LibraryClassAttribute(Name = "MessageStatus", Alias = "СтатусСообщения", AsGlobal = true)]
    public class MessageStatusEnumClass : BaseEnum<MessageStatusEnumInner>
    {
        public MessageStatusEnumClass()
        {
            Create();
        }
    }
}
