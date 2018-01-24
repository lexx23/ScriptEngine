using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Extensions;
using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptBaseExtension
{
    [ScriptExtension(AsGlobal = true, AsObject = false)]
    public class ScriptBaseFunctionsExtension : ExtensionBase
    {
        private static Value val;

        //[ScriptExtensionMethod(Name = "Message",Alias = "Сообщить")]
        //public void Message([FunctionParameter("123123")]IVariable text,IVariable type)
        //{
        //    Console.Write(text.Value.Content);
        //    //text.Value.Object.GetValue("asdasd");

        //}

        //[ScriptExtensionMethod(Name = "Message2", Alias = "Сообщить2")]
        //public IVariable Message2([FunctionParameter("123123")]IVariable text, IVariable type)
        //{
        //    return new Variable() {Value = text.Value + type.Value };

        //}

        [ScriptExtensionMethod(Name = "ТекущаяУниверсальнаяДатаВМиллисекундах", Alias = "CurrentUniversalDateInMilliseconds")]
        public Value CurrentUniversalDateInMilliseconds()
        {
            if (val == null)
            {
                val = new Value();
                val.Type = ValueTypeEnum.NUMBER;
            }
            val.Number = DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond;
            return val;
        }
    }
}
