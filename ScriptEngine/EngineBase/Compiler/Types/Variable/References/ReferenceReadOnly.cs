using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class ReferenceReadOnly : IVariableReference
    {
        private IValue _value;

        public ReferenceReadOnly(IValue value)
        {
            _value = value;
        }

        public IValue Get()
        {
            return _value;
        }

        public void Set(IValue value)
        {
            throw new Exception("Поле объекта недоступно для записи");
        }

        public IVariableReference Clone(object instance)
        {
            return new ReferenceReadOnly(_value);
        }
    }
}
