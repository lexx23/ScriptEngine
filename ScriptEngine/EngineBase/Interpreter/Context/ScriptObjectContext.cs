using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Exceptions;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.BaseTypes;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptObjectContext
    {
        private ScriptModule _module;
        private ContextMethodReferenceHolder[] _functions;

        public ScriptModule Module { get => _module; }
        public object Instance { get; set; }
        private ContextVariableReferenceHolder[] Context { get; set; }

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


            for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
            {
                _module.ModuleScope.Vars[i].Reference = _module.ModuleScope.Vars[i].Reference.Clone(Instance);
                Context[i] = new ContextVariableReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference);
            }

            _functions = new ContextMethodReferenceHolder[_module.Functions.Count];
            IFunction[] functions_array = _module.Functions.ToArray();
            for (int i = 0; i < _module.Functions.Count; i++)
            {
                IMethodWrapper wrapper = null;
                if (functions_array[i].Method != null)
                {
                    wrapper = functions_array[i].Method.Clone(Instance);
                    functions_array[i].Method = wrapper;
                }
                _functions[i] = new ContextMethodReferenceHolder(functions_array[i], wrapper);
            }

        }

        /// <summary>
        /// Установка функциям и свойствам модуля
        /// </summary>
        public void Set()
        {
            if (!_module.AsGlobal && Instance == null)
            {
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

            ContextMethodReferenceHolder context_function = GetContextFunction(function.Name);

            if (context_function == null)
                throw new Exception($"Процедура или функция с именем [{function.Name}] не определена, у объекта [{Module.Name}].");

            work_function = GetContextFunction(function.Name).Function;

            if (!work_function.Public)
                throw new Exception($"Функция [{function.Name}] не имеет оператора Экспорт, и не доступна.");

            // Проверка типа использования. Если используется процедура как функция то ошибка.
            if (work_function.Type == FunctionTypeEnum.PROCEDURE && work_function.Type != function.Type)
                throw new Exception($"Обращение к процедуре [{function.Name}] как к функции.");

            context_function.Set();
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

        /// <summary>
        /// Получить функцию из контекста обьекта.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ContextMethodReferenceHolder GetContextFunction(string name)
        {
            for (int i = 0; i < _functions.Length; i++)
            {
                if (String.Equals(_functions[i].Function.Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(_functions[i].Function.Alias,name,StringComparison.OrdinalIgnoreCase))
                    return _functions[i];
            }
            return null;
        }

        /// <summary>
        /// Получить значение публичного свойства объекта.
        /// </summary>
        public IVariableReference GetPublicVariable(string name)
        {
            IVariableReference context_variable = GetContextVariable(name,true);
            return context_variable;
        }

        /// <summary>
        /// Получить значение, любого в том числе и приватного, свойства объекта. 
        /// </summary>
        public IVariableReference GetAnyVaribale(string name)
        {
            IVariableReference context_variable = GetContextVariable(name, false);
            return context_variable;
        }


        /// <summary>
        /// Получить свойство из контекста объекта, по его имени.
        /// </summary>
        private IVariableReference GetContextVariable(string name,bool only_public)
        {
            // Поиск в статических свойствах обьекта.
            for (int i = 0; i < Context.Length; i++)
            {
                ContextVariableReferenceHolder context_variable = Context[i];
                if (String.Equals(context_variable.Variable.Name,name,StringComparison.OrdinalIgnoreCase) || String.Equals(context_variable.Variable.Alias,name,StringComparison.OrdinalIgnoreCase))
                {
                    if (!context_variable.Variable.Public && only_public)
                        throw new Exception($"Свойство [{name}] не имеет оператора Экспорт, и не доступно.");

                    return context_variable.Reference;
                }
            }

            // Поиск в динамических свойствах обьекта.
            if (typeof(IScriptDynamicProperties).IsAssignableFrom(Instance.GetType()))
            {
                if((Instance as IScriptDynamicProperties).Exist(name))
                    return new DynamicPropertiesReference(name, Instance as IScriptDynamicProperties);
            }

           throw new Exception($"У объекта [{Module.Name}] нет свойства [{name}].");
        }
    }
}
