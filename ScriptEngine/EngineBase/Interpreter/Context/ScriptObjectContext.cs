using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptObjectContext
    {
        private ScriptModule _module;
        private ContextMethodReferenceHolder[] _functions;

        public ScriptModule Module { get => _module; }
        public object Instance { get; set; }
        private ContextVariableReferenceHolder[] Context { get; set; }

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

        public ScriptObjectContext(ScriptModule module, object instance = null)
        {
            _module = module;

            IVariable[] vars = _module.ModuleScope.Vars.ToArray();
            Context = new ContextVariableReferenceHolder[_module.ModuleScope.Vars.Count];

            if (instance == null)
            {
                if (module.InstanceType != null)
                    Instance = Activator.CreateInstance(module.InstanceType);
            }
            else
                Instance = instance;

            module.CurrentInstance = Instance;

            for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
            {
                _module.ModuleScope.Vars[i].Reference = _module.ModuleScope.Vars[i].Reference.Clone(Instance);
                Context[i] = new ContextVariableReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference);
            }

            _functions = new ContextMethodReferenceHolder[_module.Functions.Count];
            IFunction[] functions_array = _module.Functions.ToArray();
            for (int i = 0; i < _module.Functions.Count; i++)
                if (functions_array[i].Method != null)
                {
                    IMethodWrapper wrapper = functions_array[i].Method.Clone(Instance);
                    //if (instance == null)
                        functions_array[i].Method = wrapper;
                    _functions[i] = new ContextMethodReferenceHolder(functions_array[i], wrapper);
                }

        }

        /// <summary>
        /// Установка функциям и свойствам модуля
        /// </summary>
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


        /// <summary>
        /// Проверка параметров вызываемой функции, на соответствие прототипу.
        /// </summary>
        public IFunction CheckFunction(IFunction function)
        {
            IFunction work_function;

            work_function = Module.Functions.Get(function.Name);

            if (work_function == null)
                throw new Exception($"Процедура или функция с именем [{function.Name}] не определена, у объекта [{Module.Name}].");

            if (!work_function.Public)
                throw new Exception($"Функция [{function.Name}] не имеет оператора Экспорт, и не доступна.");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new Exception($"Обращение к процедуре [{function.Name}] как к функции.");

            function.EntryPoint = work_function.EntryPoint;

            Set();

            if (function.CallParameters.Count == work_function.DefinedParameters.Count || work_function.DefinedParameters.AnyCount)
                return work_function;

            // Блок проверки параметров.
            if (function.CallParameters.Count > work_function.DefinedParameters.Count)
                throw new Exception($"Много фактических параметров [{work_function.Name}].");

            int i, param_count;
            param_count = i = function.CallParameters.Count;

            while (i < work_function.DefinedParameters.Count)
            {
                if (work_function.DefinedParameters[i].DefaultValue != null)
                    param_count++;
                i++;
            }

            if (param_count < work_function.DefinedParameters.Count)
                throw new Exception($"Недостаточно фактических параметров [{work_function.Name}].");

            return work_function;
        }

        public IMethodWrapper GetFunctionMethod(string name)
        {
            for (int i = 0; i < _functions.Length; i++)
            {
                if (_functions[i].Function.Name == name || _functions[i].Function.Alias == name)
                    return _functions[i].Wrapper;
            }
            return null;
        }

        /// <summary>
        /// Получить значение свойства объекта.
        /// </summary>
        public IVariableReference GetPublicReference(string name)
        {
            IVariable context_var = GetVariable(name);
            if (context_var == null)
                throw new Exception($"У объекта [{Module.Name}] нет свойства [{name}].");
            if (!context_var.Public)
                throw new Exception($"Свойство [{name}] не имеет оператора Экспорт, и не доступно.");

            return context_var.Reference;
        }

        /// <summary>
        /// Получить значение свойства объекта.
        /// </summary>
        public IVariableReference GetReference(string name)
        {
            for (int i = 0; i < Context.Length; i++)
            {
                if (Context[i].Variable.Name == name || Context[i].Variable.Alias == name)
                    return Context[i].Reference;
            }

            return null;
        }


        /// <summary>
        /// Получить значение свойства объекта по его имени.
        /// </summary>
        private IVariable GetVariable(string name)
        {
            for (int i = 0; i < Context.Length; i++)
            {
                if (Context[i].Variable.Name == name || Context[i].Variable.Alias == name)
                    return Context[i].Variable;
            }

            return null;
        }
    }
}
