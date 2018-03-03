using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Library.BaseTypes
{

    public class ScriptIterator : IEnumerable<IValue>
    {
        public IList<IValue> _array;
        private int i;

        public ScriptIterator(IList<IValue> collection)
        {
            _array = collection;
            i = 0;
        }

        public IEnumerator<IValue> GetEnumerator()
        {
            while (i < _array.Count)
            {
                yield return _array[i++];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

    }


    
}
