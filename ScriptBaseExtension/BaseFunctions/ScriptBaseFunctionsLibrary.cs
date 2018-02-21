using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System;

namespace ScriptBaseLibrary
{
    [LibraryClassAttribute(AsGlobal = true, AsObject = false,Name = "global_library")]
    public class ScriptBaseFunctionsLibrary
    {
        private string test = "";

        [LibraryClassProperty(Name = "ПараметрЗапуска", Alias = "LaunchParameter")]
        public string LaunchParameter
        {
            get => test;
            set => test = value;
        }


        [LibraryClassMethodAttribute(Name = "Сообщить", Alias = "Message")]
        public void Message(string text, MessageStatusEnumInner type = MessageStatusEnumInner.WithoutStatus)
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
