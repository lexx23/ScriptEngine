using ScriptEngine.EngineBase.Praser.Token;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Parser.Precompiler
{
    public class PrecompilerStack
    {
        private Stack<PrecompilerStackStruct> _stack;
        private int _skip;

        public bool Skip { get => _skip > 0; }
        public int Count { get => _stack.Count; }

        public PrecompilerStack()
        {
            _skip = 0;
            _stack = new Stack<PrecompilerStackStruct>();
        }

        public void Push(TokenClass token)
        {

            _stack.Push(new PrecompilerStackStruct
            {
                Token = token,
                Skip = false
            });
        }

        public void Push(TokenClass token, bool skip_value,bool run=false)
        {
            _stack.Push(new PrecompilerStackStruct
            {
                Token = token,
                Skip = skip_value,
                Run = run
            });

            if (skip_value)
                _skip++;
        }


        public PrecompilerStackStruct Pop()
        {
            if (_stack.Count == 0)
                throw new Exception("Стек пустой.");
            PrecompilerStackStruct directive;

            directive = _stack.Pop();

            if (directive.Skip)
                _skip--;

            return directive;
        }

    public PrecompilerStackStruct Peek()
    {
        if (_stack.Count == 0)
            throw new Exception("Стек пустой.");
        return _stack.Peek();
    }
}
}
