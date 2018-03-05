using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections;
using System.Collections.Generic;

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


    [LibraryClassAttribute(Name = "СтатусСообщения", Alias = "MessageStatus", AsGlobal = true, AsObject = true)]
    public class MessageStatusEnumClass : BaseEnum<MessageStatusEnumInner>
    {

    }
}
