﻿using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
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
        public int StackNumber { get; set; }
        public int Users { get; set; }


        public Variable()
        {
            Users = 1;
            StackNumber = -1;
            Reference = new SimpleReference();
        }
    }
}
