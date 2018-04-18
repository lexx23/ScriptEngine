/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using System.Collections.Generic;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.Value
{
    public class IValueComparer : IEqualityComparer<IValue>, IComparer<IValue>
    {
        public bool Equals(IValue x, IValue y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(IValue obj)
        {
            if (obj.AsObject() == null)
                return 0;

            return obj.AsObject().GetHashCode();
        }

        public int Compare(IValue left, IValue right)
        {
            if (left.BaseType == right.BaseType)
                return left.CompareTo(right);
            else
                return left.AsString().CompareTo(right.AsString());
        }
    }
}
