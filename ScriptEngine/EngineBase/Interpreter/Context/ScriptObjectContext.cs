using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptObjectContext
    {
        private ScriptModule _module;
        public ScriptModule Module { get => _module; }
        public ContextReferenceHolder[] Context { get; set; }

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
            Context = new ContextReferenceHolder[_module.ModuleScope.Vars.Count];

            if (module.Instance == null)
            {
                for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
                    Context[i] = new ContextReferenceHolder(_module.ModuleScope.Vars[i], new SimpleReference());
            }
            else
                for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
                    Context[i] = new ContextReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference);
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
