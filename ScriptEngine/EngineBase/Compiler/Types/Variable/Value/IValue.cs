/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Interpreter.Context;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public interface IValue : IEquatable<IValue>, IComparable<IValue>
    {
        ValueTypeEnum BaseType { get; }
        InternalScriptType ScriptType { get; }

        int AsInt();
        bool AsBoolean();
        string AsString();
        DateTime AsDate();
        decimal AsNumber();
        object AsObject();
        ScriptObjectContext AsScriptObject();
    }

}
