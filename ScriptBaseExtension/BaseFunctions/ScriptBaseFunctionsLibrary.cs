using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System;

namespace ScriptBaseLibrary
{
    [LibraryClassAttribute(AsGlobal = true, AsObject = false)]
    public class ScriptBaseFunctionsLibrary
    {
        //[LiПараметрЗапуска


        [LibraryClassMethodAttribute(Name = "Сообщить", Alias = "Message")]
        public void Message([FunctionByValueParameter]string text, MessageStatusEnumInner type = MessageStatusEnumInner.WithoutStatus)
        {
            //if (type == null)
            //    type.Value = MessageStatusEnum.Instance["Обычное"];
            Console.Write(text);
        }

        [LibraryClassMethodAttribute(Name = "ТекущаяУниверсальнаяДатаВМиллисекундах", Alias = "CurrentUniversalDateInMilliseconds")]
        public IValue CurrentUniversalDateInMilliseconds()
        {
            return ValueFactory.Create(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}
