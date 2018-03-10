using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System;
using ScriptBaseFunctionsLibrary.BuildInTypes;

namespace ScriptBaseLibrary
{
    [LibraryClassAttribute(AsGlobal = true, AsObject = false,Name = "global_library")]
    public class ScriptBaseFunctionsLibrary
    {
        private string test = "";

        [LibraryClassProperty(Alias = "ПараметрЗапуска", Name = "LaunchParameter")]
        public string LaunchParameter
        {
            get => test;
        }


        [LibraryClassMethodAttribute(Alias = "Сообщить", Name = "Message")]
        public void Message(string text, MessageStatusEnumInner type = MessageStatusEnumInner.WithoutStatus)
        {
            Console.WriteLine(text);
        }

        [LibraryClassMethodAttribute(Alias = "ТекущаяУниверсальнаяДатаВМиллисекундах", Name = "CurrentUniversalDateInMilliseconds")]
        public IValue CurrentUniversalDateInMilliseconds()
        {
            return ValueFactory.Create(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
        }

        [LibraryClassMethodAttribute(Alias = "ИнформацияОбОшибке", Name = "ErrorInfo")]
        public IValue ErrorInfo()
        {
            return ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.ErrorInfo;
        }

    }
}
