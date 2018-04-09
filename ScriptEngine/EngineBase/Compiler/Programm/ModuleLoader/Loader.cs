using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using System.Reflection;
using System.Linq;
using System;

namespace ScriptEngine.EngineBase.Compiler.Programm.ModuleLoader
{
    public class Loader
    {
        private ScriptProgramm _programm;

        public IVariable IValue { get; private set; }

        public Loader(ScriptProgramm programm)
        {
            _programm = programm;
        }

        /// <summary>
        /// Добавить параметры функции.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="method"></param>
        private void GetFunctionParameters(IFunction function, MethodInfo method)
        {
            IValue default_value = null;
            FunctionParameters parameters = new FunctionParameters();


            var array_param = method.GetParameters().Where(x => x.ParameterType == typeof(IValue[]));

            if (array_param.Count() > 0)
            {
                parameters.AnyCount = true;
                if (array_param.Count() > 1)
                    throw new Exception("Тип параметра IValue[] может быть только один.");
                if (array_param.First().HasDefaultValue)
                    throw new Exception("Тип параметра IValue[] не может иметь значение по умолчанию.");

                function.DefinedParameters = parameters;
                return;
            }

            foreach (ParameterInfo param_info in method.GetParameters())
            {
                if (param_info.HasDefaultValue)
                    default_value = ValueFactory.Create(param_info.ParameterType, param_info.DefaultValue);

                parameters.CreateByRef(param_info.Name, null, default_value);
            }

            function.DefinedParameters = parameters;
        }


        /// <summary>
        /// Загрузка перечисления.
        /// </summary>
        /// <param name="type"></param>
        private void LoadEnum(Type type)
        {
            LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryClassAttribute), false);

            ScriptModule enum_module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.ENUM, true)
            {
                InstanceType = type
            };

            _programm.InternalTypes.Add(new InternalScriptType() { Name = "Enum"+attribute.Name, Alias = "Перечисление" + attribute.Alias, Description = "Перечисление" + attribute.Alias, Module = enum_module, Type = type });

            var generic_type = type.BaseType.GetGenericArguments()[0];
            _programm.InternalTypes.Add(new InternalScriptType() { Name = attribute.Name, Alias = attribute.Alias, Description = attribute.Alias, Type = generic_type });

            _programm.Modules.Add(enum_module);
            _programm.GlobalVariables.Add(new Variable() { Name = attribute.Name, Alias = attribute.Alias, Reference = new SimpleReference() });
        }


        /// <summary>
        /// Добавить в программу обьект используя его тип.
        /// </summary>
        /// <param name="type"></param>
        /// <param name="as_global"></param>
        public void AddObjectOfType(Type type, LibraryClassAttribute attribute = null)
        {
            if (attribute == null)
                attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryClassAttribute), false);

            ScriptModule module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.OBJECT, attribute.AsGlobal)
            {
                InstanceType = type
            };

            foreach (PropertyInfo property in type.GetTypeInfo().GetProperties().Where(m => m.GetCustomAttributes(typeof(LibraryClassPropertyAttribute), false).Length > 0))
            {
                LibraryClassPropertyAttribute property_attr = property.GetCustomAttribute<LibraryClassPropertyAttribute>(false);

                IVariableReference reference = ReferenceFactory.Create(type, property);
                IVariable var = new Variable() { Name = property_attr.Name, Alias = property_attr.Alias, Public = true, Reference = reference };
                module.Variables.Add(property_attr.Name, var);
                if (attribute.AsGlobal)
                    _programm.GlobalVariables.Add(var);
            }


            foreach (MethodInfo method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetCustomAttributes(typeof(LibraryClassMethodAttribute), false).Length > 0))
            {
                LibraryClassMethodAttribute method_attr = method.GetCustomAttribute<LibraryClassMethodAttribute>(false);

                IFunction function = module.Functions.Create(method_attr.Name, true);
                function.Alias = method_attr.Alias;
                function.Type = method.ReturnType == typeof(void) ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                GetFunctionParameters(function, method);
                function.Method = LibraryMethodFactory.Create(type, method);

                if (attribute.AsGlobal)
                    _programm.GlobalFunctions.Add(function);
            }

            if (module.Variables.Get("ЭтотОбъект") == null && !attribute.AsGlobal)
                module.Variables.Create("ЭтотОбъект", true, module.ModuleScope);

            _programm.Modules.Add(module);
            if(attribute.RegisterType)
                _programm.InternalTypes.Add(new InternalScriptType() { Name = attribute.Name, Alias = attribute.Alias, Description = attribute.Alias, Module = module, Type = type });

            if (attribute.AsGlobal)
                _programm.GlobalVariables.Add(new Variable() { Name = attribute.Name, Alias = attribute.Alias, Reference = new SimpleReference() });
        }


        /// <summary>
        /// Загрузка данных из сборки c#.
        /// </summary>
        /// <param name="assembly"></param>
        public void LoadAssembly(Assembly assembly)
        {
            foreach (Type type in assembly.ExportedTypes.Where(m => m.GetCustomAttributes(typeof(LibraryClassAttribute), false).Length > 0))
            {
                LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryClassAttribute), false);

                // Класс содержит объект который добавляется в глобальный модуль.

                if (type.BaseType.Name == "BaseEnum`1")
                    LoadEnum(type);
                else
                    AddObjectOfType(type, attribute);

                continue;
            }
        }
    }
}
