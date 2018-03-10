using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptEngine.EngineBase.Library.BaseTypes;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    /// <summary>
    /// Представляет собой пару из ключа и соответствующего ключу значения
    /// </summary>
    [LibraryClassAttribute(Alias = "КлючИЗначение", Name = "KeyAndValue", AsGlobal = false, AsObject = true)]
    public class ScriptKeyValuePair: LibraryModule<ScriptKeyValuePair>
    {
        private readonly IValue _key;
        private readonly IValue _value_data;

        public ScriptKeyValuePair(IValue key, IValue value)
        {
            _key = key;
            _value_data = value;
        }

        [LibraryClassProperty(Alias = "Ключ", Name = "Key")]
        public IValue Key { get => _key; }

        [LibraryClassProperty(Alias = "Значение", Name = "Value")]
        public IValue Value { get => _value_data; }
    }

}
