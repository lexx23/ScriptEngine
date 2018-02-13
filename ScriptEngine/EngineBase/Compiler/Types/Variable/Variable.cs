using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public interface IVariableReference
    {
        IValue Get();
        void Set(IValue value);

    }

    public class SimpleReference : IVariableReference
    {
        private IValue _value;

        public IValue Get()
        {
            return _value;
        }

        public void Set(IValue value)
        {
            _value = value;
        }


    }

    public class ScriptReference : IVariableReference
    {
        private IVariableReference _value;

        public ScriptReference(IVariableReference reference)
        {
            _value = reference;
        }

        public IValue Get()
        {
            return _value.Get();
        }
        public void Set(IValue value)
        {
            _value.Set(value);
        }


    }



    public class LibraryReference : IVariableReference
    {
        private IValue _value;

        public LibraryReference(object obj,IValue value)
        {
            _value = value;
        }

        public IValue Get()
        {
            return _value;
        }

        public void Set(IValue value)
        {
            if (_value.Type != value.Type)
                throw new Exception();
            _value = value;
        }

    }



    public class Variable : IVariable
    {
        public String Name { get; set; }
        public ScriptScope Scope { get; set; }

        public IVariableReference Reference { get; set; }
        public IValue Value { get => Reference.Get(); set => Reference.Set(value); }
        public VariableTypeEnum Type { get; set; }
        public bool Public { get; set; }

        public int StackNumber { get; set; }
        public int Users { get; set; }


        public Variable()
        {
            Users = 1;
            StackNumber = -1;
            Reference = new SimpleReference();
        }


        public bool HaveValue()
        {
            return Value != null;
        }
    }
}
