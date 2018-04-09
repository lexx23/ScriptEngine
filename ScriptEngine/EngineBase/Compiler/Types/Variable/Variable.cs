using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable
{
    public class Variable : IVariable
    {
        public string Name { get; set; }
        public string Alias { get; set; }

        public ScriptScope Scope { get; set; }

        public IVariableReference Reference { get; set; }
        public IValue Value
        {
            get => Reference.Get();
            set => Reference.Set(value);
        }
        public VariableTypeEnum Type { get; set; }

        public bool Public { get; set; }
        public int Users { get; set; }


        public Variable()
        {
            Users = 1;
            Reference = new SimpleReference();
        }

        public Variable(string name,IValue value):this()
        {
            Name = name;
            Value = value;
        }

        public Variable(IValue value) : this()
        {
            Value = value;
        }

        public Variable(IVariableReference reference)
        {
            Reference = reference;
        }

        public static IVariable Create() => new Variable();
        public static IVariable Create(string name, IValue value) => new Variable(name,value);
        public static IVariable Create(IValue value) => new Variable(value);
        public static IVariable Create(IVariableReference reference) => new Variable(reference);
    }
}
