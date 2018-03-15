using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System;

namespace ScriptBaseLibrary
{
    [LibraryClassAttribute(AsGlobal = true, AsObject = false, Name = "global_library")]
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

        [LibraryClassMethodAttribute(Alias = "ЗначениеЗаполнено", Name = "ValueIsFilled")]
        public bool ValueIsFilled(IValue value)
        {
            switch (value.Type)
            {
                case ValueTypeEnum.NULL:
                    return false;
                case ValueTypeEnum.BOOLEAN:
                    return true;
                case ValueTypeEnum.STRING:
                    return !String.IsNullOrWhiteSpace(value.AsString());
                case ValueTypeEnum.NUMBER:
                    return value.AsNumber() != 0;
                case ValueTypeEnum.DATE:
                    var emptyDate = new DateTime(1, 1, 1, 0, 0, 0);
                    return value.AsDate() != emptyDate;
                case ValueTypeEnum.SCRIPT_OBJECT:
                    if (value.AsScriptObject().Instance != null)
                    {
                        if (typeof(IUniversalCollection).IsAssignableFrom(value.AsScriptObject().Instance.GetType()))
                            return (value.AsScriptObject().Instance as IUniversalCollection).Count() > 0;
                        else
                            return false;
                    }
                    else
                        return false;
                default:
                    return true;
            }
        }
    }
}
