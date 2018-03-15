using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using System;

namespace ScriptBaseFunctionsLibrary.BaseFunctions.Strings
{
    [LibraryClassAttribute(AsGlobal = true, AsObject = false, Name = "strings_library")]
    public class Strings
    {
        [LibraryClassMethodAttribute(Alias = "Строка", Name = "String")]
        public IValue String(IValue value)
        {
            return ValueFactory.Create(value.AsString());
        }

        [LibraryClassMethodAttribute(Alias = "Формат", Name = "Format")]
        public string Format(IValue value, string format)
        {
            return FormatFunction.Format(value, format);
        }

        [LibraryClassMethodAttribute(Alias = "Найти", Name = "Find")]
        public int Find(IValue str, IValue substring)
        {
            return str.AsString().IndexOf(substring.AsString(), StringComparison.Ordinal) + 1;
        }
    }
}
