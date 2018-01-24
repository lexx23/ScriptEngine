using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ObjectContext
    {
        private ScriptSimpleContext _context;
        private ScriptModule _module;

        public ScriptSimpleContext Context { get => _context; }
        public ScriptModule Module { get => _module; }

        public string ModuleName
        {
            get
            {
                if (_module != null)
                    return _module.Name;
                else
                    return "";
            }

        }

        public ObjectContext(ScriptModule module,ScriptSimpleContext context)
        {
            _module = module;
            _context = context;
        }

        public IValue GetValue(string name)
        {
            IVariable var;
            var = _module.Variables.Get(name);
            if (var != null)
                return _context.GetValue(var.StackNumber);
            else
                return null;
        }


        public IValue GetValue(int index)
        {
            return _context.GetValue(index);
        }

        public void SetValue(string name, IValue value)
        {
            IVariable var;
            var = _module.Variables.Get(name);
            if (var != null)
                _context.SetValue(var.StackNumber,value);
        }
    }
}
