using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{
    public class SimpleArray: ISimpleArray
    {
        public IVariable[] Array { get; set; }

        public int Count { get => Array.Length; }
        
        public IValue GetByName(string name)
        {
            for (int i = 0; i < Array.Length; i++)
            {
                if (Array[i].Name == name)
                    return Array[i].Value;
            }

            return null;
        }

        public IValue GetByIndex(int index)
        {
            return Array[index].Value;
        }
    }
}
