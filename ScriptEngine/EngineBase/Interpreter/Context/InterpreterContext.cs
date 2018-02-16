using ScriptEngine.EngineBase.Compiler.Programm;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Collections.Generic;
using System.Linq;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    class InterpreterContext
    {
        private ScriptProgramm _programm;
        private ScriptObjectContext _current;
        private IFunction _function;
        private IVariableReference[] _current_function_context { get; set; }

        private Stack<(int, ScriptObjectContext, IVariableReference[], IFunction)> _history;


        public ScriptModule CurrentModule { get => _current?.Module; }
        public ScriptObjectContext Current { get => _current; }
        public IFunction CurrentFunction { get => _function; }


        public InterpreterContext(ScriptProgramm programm)
        {
            _programm = programm;
            _history = new Stack<(int, ScriptObjectContext, IVariableReference[], IFunction)>();
        }

        public ScriptObjectContext CreateObject(ScriptModule type)
        {
            ScriptObjectContext context = new ScriptObjectContext(type);
            Set(context);
            return context;
        }

        public void Reset()
        {
            _history.Clear();
            _function = null;
            _current  = null;
            _current_function_context = null;
        }

        public void Set(ScriptObjectContext context,IFunction function, int position)
        {
            IVariableReference[] _function_context = _current_function_context;

            _current_function_context = new IVariableReference[function.Scope.VarCount];
            for (int i = 0; i < function.Scope.VarCount; i++)
            {
                IVariableReference reference = new SimpleReference();
                function.Scope.Vars[i].Reference = reference;
                _current_function_context[i] = reference;
            }

            if (_current != context)
            {
                _history.Push((position, _current, _function_context, _function));
                Set(context);
            }
            else
                _history.Push((position, null, _function_context, _function));

            _function = function;
        }

        private void Set(ScriptObjectContext context)
        {
            _current = context;
            if (context.Module.Instance == null)
            {
                for (int i = 0; i < _current.Context.Length; i++)
                    _current.Context[i].Set();
            }
        }


        public void Update(IVariable variable, IVariableReference reference)
        {
            _current_function_context[variable.StackNumber] = reference;
            variable.Reference = reference;
        }


        public int Restore()
        {
            (int, ScriptObjectContext, IVariableReference[], IFunction) data = _history.Pop();

            if (data.Item2 != null)
                _current = data.Item2;

            _current_function_context = data.Item3;
            _function = data.Item4;

            if (_function != null)
                for (int i = 0; i < _function.Scope.VarCount; i++)
                {
                    _function.Scope.Vars[i].Reference = _current_function_context[i];
                }

            return data.Item1;
        }
    }
}
