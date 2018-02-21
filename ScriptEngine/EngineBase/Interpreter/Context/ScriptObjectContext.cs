using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptObjectContext
    {
        private ScriptModule _module;
        private ContextMethodReferenceHolder[] _functions;

        public ScriptModule Module { get => _module; }
        public object Instance { get; set; }
        public ContextVariableReferenceHolder[] Context { get; set; }

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

        public ScriptObjectContext(ScriptModule module)
        {
            _module = module;

            IVariable[] vars = _module.ModuleScope.Vars.ToArray();
            Context = new ContextVariableReferenceHolder[_module.ModuleScope.Vars.Count];

            if (module.InstanceType != null)
                Instance = Activator.CreateInstance(module.InstanceType);

            if (!_module.AsGlobal)
            {
                for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
                    Context[i] = new ContextVariableReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference.Clone(Instance));

                _functions = new ContextMethodReferenceHolder[_module.Functions.Count];
                IFunction[] functions_array = _module.Functions.ToArray();
                for (int i = 0; i < _module.Functions.Count; i++)
                    if (functions_array[i].Method != null)
                    { 
                        functions_array[i].Method = functions_array[i].Method.Clone(Instance);
                        _functions[i] = new ContextMethodReferenceHolder(functions_array[i]);
                    }
            }
            else
                for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
                    Context[i] = new ContextVariableReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference);
        }

        public void Set()
        {
            if (!_module.AsGlobal && Module.CurrentInstance != Instance)
            {
                Module.CurrentInstance = Instance;

                for (int i = 0; i < _functions.Length; i++)
                    _functions[i].Set();

                for (int i = 0; i < Context.Length; i++)
                    Context[i].Set();
            }
        }



        public IVariableReference GetReference(IVariable variable)
        {
            return GetReference(variable.Name);
        }


        public IVariableReference GetReference(string name)
        {
            for (int i = 0; i < Context.Length; i++)
            {
                if (Context[i].Variable.Name == name)
                    return Context[i].Reference;
            }

            return null;
        }
    }
}
