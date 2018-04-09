using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Interpreter;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public class LibraryModule<T> : ScriptObjectValue
    {
        public LibraryModule()
        {
            LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(LibraryClassAttribute), false);

            InternalScriptType type = ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(attribute.Name);
            _value = new ScriptObjectContext(type.Module, this);
        }

    }
}
