/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using System;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    class ScriptObjectContext : IScriptObjectContext
    {
        private ScriptModule _module;
        private ContextMethodReferenceHolder[] _functions;

        public ScriptModule Module { get => _module; }
        public object Instance { get; set; }
        private ContextVariableReferenceHolder[] _properties { get; set; }

        public ScriptObjectContext(ScriptModule module, object instance = null)
        {
            _module = module;

            // Если есть внешний тип модуля (загружен из библиотеки) то инициализирую его.
            if (instance == null)
            {
                if (module.InstanceType != null)
                {
                    Instance = Activator.CreateInstance(module.InstanceType);
                    if (Instance == null)
                        throw new Exception($"Ошибка при создании обьекта {module.Name}");
                }
            }
            else
                Instance = instance;

            // Создаю хранилище для переменных модуля.
            _properties = new ContextVariableReferenceHolder[_module.ModuleScope.Vars.Count];
            // Клонирование ссылок переменных для нового инстанса.
            for (int i = 0; i < _module.ModuleScope.Vars.Count; i++)
            {
                _module.ModuleScope.Vars[i].Reference = _module.ModuleScope.Vars[i].Reference.Clone(Instance);
                _properties[i] = new ContextVariableReferenceHolder(_module.ModuleScope.Vars[i], _module.ModuleScope.Vars[i].Reference);
            }

            // Создаю хранилище для функций модуля.
            _functions = new ContextMethodReferenceHolder[_module.Functions.Count];
            // Клонирование ссылок функций.
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

            // Инициализация переменной ЭтотОбъект.
            if (module.Variables.Get("ЭтотОбъект") != null)
                GetAnyVaribale("ЭтотОбъект").Set(ValueFactory.Create(this));

            // Инициализация обьекта. Запуск кода в конце модуля.
            IFunction function = module.Functions.Get("<<entry_point>>");
            if (function != null && function.EntryPoint != -1)
                ScriptInterpreter.Interpreter.FunctionCall(this, function);
        }

        /// <summary>
        /// Установка функциям и свойствам модуля текущих значений.
        /// </summary>
        public void Set()
        {
            for (int i = 0; i < _functions.Length; i++)
                _functions[i].Set();

            for (int i = 0; i < _properties.Length; i++)
                _properties[i].Set();
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
                if (String.Equals(_functions[i].Function.Name, name, StringComparison.OrdinalIgnoreCase) || String.Equals(_functions[i].Function.Alias, name, StringComparison.OrdinalIgnoreCase))
                    return _functions[i];
            }
            return null;
        }

        /// <summary>
        /// Получить значение публичного свойства объекта.
        /// </summary>
        public IVariableReference GetPublicVariable(string name)
        {
            IVariableReference context_variable = GetContextVariable(name, true);
            return context_variable;
        }

        /// <summary>
        /// Получить значение публичного свойства объекта по индексу.
        /// </summary>
        public ContextVariableReferenceHolder GetPublicVariable(int index)
        {
            return _properties[index];
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
        /// Количество переменных.
        /// </summary>
        /// <returns></returns>
        public int VariablesCount()
        {
            return _properties.Length;
        }

        /// <summary>
        /// Получить свойство из контекста объекта, по его имени.
        /// </summary>
        private IVariableReference GetContextVariable(string name, bool only_public)
        {
            // Поиск в статических свойствах обьекта.
            for (int i = 0; i < _properties.Length; i++)
            {
                ContextVariableReferenceHolder context_variable = _properties[i];
                if (String.Equals(context_variable.Variable.Name, name, StringComparison.OrdinalIgnoreCase) || String.Equals(context_variable.Variable.Alias, name, StringComparison.OrdinalIgnoreCase))
                {
                    if (!context_variable.Variable.Public && only_public)
                        throw new Exception($"Свойство [{name}] не имеет оператора Экспорт, и не доступно.");

                    return context_variable.Reference;
                }
            }

            // Поиск в динамических свойствах обьекта.
            if (typeof(IScriptDynamicProperties).IsAssignableFrom(Instance.GetType()))
            {
                if ((Instance as IScriptDynamicProperties).Exist(name))
                    return new DynamicPropertiesReference(name, Instance as IScriptDynamicProperties);
            }

            throw new Exception($"У объекта [{Module.Name}] нет свойства [{name}].");
        }
    }
}
