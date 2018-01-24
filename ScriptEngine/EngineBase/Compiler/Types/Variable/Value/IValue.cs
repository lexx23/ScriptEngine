using ScriptEngine.EngineBase.Interpreter.Context;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public interface IValue
    {
        ValueTypeEnum Type { get; set; }

        string String { get; set; }
        decimal Number { get; set; }
        bool Boolean { get; set; }
        DateTime Date { get; set; }
        ObjectContext Object { get; set; }

        int ToInt();
        bool ToNumber(out decimal left_result);
        bool ToBoolean();
        void SetValue(IValue value);
        void SetValue(bool value);
        IValue Clone();
    }
}
