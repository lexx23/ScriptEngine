using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "UUID", Alias = "УникальныйИдентификатор", RegisterType = true, AsGlobal = false)]
    public class ScriptUUID : UUIDValue
    {
        public override InternalScriptType ScriptType => ScriptEngine.EngineBase.Interpreter.ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(this);

        public ScriptUUID()
        {
            _value = Guid.NewGuid();
        }

        public ScriptUUID(string uuid)
        {
            _value = Guid.Parse(uuid);
        }

        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            if (parameters.Length == 0)
                return new ScriptUUID();
            else
                return new ScriptUUID(parameters[0].AsString());
        }
    }
}
