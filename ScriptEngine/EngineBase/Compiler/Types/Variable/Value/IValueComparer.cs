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
            if (left.Type == right.Type)
                return left.CompareTo(right);
            else
                return left.AsString().CompareTo(right.AsString());
        }
    }
}
