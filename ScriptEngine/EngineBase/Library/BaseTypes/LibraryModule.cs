using ScriptEngine.EngineBase.Compiler.Types.Variable.Value.Values;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Interpreter;
using System;


namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public class LibraryModule<T> : ScriptObjectValue
    {
        public LibraryModule()
        {
            LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(typeof(T), typeof(LibraryClassAttribute), false);

            ScriptModule module = ScriptInterpreter.Interpreter.Programm.Modules.Get(attribute.Name);
            _value = new ScriptObjectContext(module, this);
        }

    }
}
