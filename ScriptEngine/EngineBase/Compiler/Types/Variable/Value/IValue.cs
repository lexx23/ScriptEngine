using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public interface IValue: IEquatable<IValue>, IComparable<IValue>
    {
        ValueTypeEnum Type { get;}
        bool ReadOnly { get; }

        int AsInt();
        bool AsBoolean();
        string AsString();
        DateTime AsDate();
        decimal AsNumber();
        object AsObject();
        ScriptObjectContext AsScriptObject();
    }
}
