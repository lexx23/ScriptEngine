using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Library.Attributes;


namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "UUID", Alias = "УникальныйИдентификатор", AsGlobal = false, AsObject = true)]
    public class ScriptUUID : UUIDValue
    {
        public ScriptUUID()
        {
            _value = new System.Guid();
        }

        public ScriptUUID(string uuid)
        {
            _value = new System.Guid(uuid);
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
