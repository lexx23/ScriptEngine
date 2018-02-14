using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Extensions;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library;
using ScriptEngine.EngineBase.Library.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace ScriptEngine.EngineBase.Compiler.Programm.ModuleLoader
{
    public class Loader
    {
        private ScriptProgramm _programm;

        public Loader(ScriptProgramm programm)
        {
            _programm = programm;
        }

        private void GetFunctionParameters(IFunction function, MethodInfo method)
        {
            IValue default_value = null;
            FunctionParameters parameters = new FunctionParameters();
            foreach (ParameterInfo param_info in method.GetParameters())
            {
                if (param_info.HasDefaultValue)
                    default_value = ValueFactory.Create(param_info.ParameterType, param_info.DefaultValue);

                FunctionByValueParameterAttribute[] by_val_attribute = param_info.GetCustomAttributes<FunctionByValueParameterAttribute>().ToArray();
                if (by_val_attribute.Length > 0)
                    parameters.CreateByVal(param_info.Name, null, default_value);
                else
                    parameters.CreateByRef(param_info.Name, null, default_value);

            }

            function.DefinedParameters = parameters;
        }

        private void LoadEnums(Assembly assembly)
        {
            foreach (Type type in assembly.ExportedTypes.Where(m => m.GetCustomAttributes(typeof(LibraryEnumAttribute), false).Length > 0))
            {
                LibraryEnumAttribute attribute = (LibraryEnumAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryEnumAttribute), false);
                BaseEnum new_enum = new BaseEnum(type);

                ScriptModule enum_module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.ENUM);
                IFunction function = enum_module.Functions.Create("GetPropertyByName", true);
                //function.Param = new IVariable[] { new Variable() {Type = VariableTypeEnum.STACKVARIABLE } };

                foreach (IVariable var in new_enum)
                    enum_module.Variables.Create(var.Name, var);

                _programm.ModuleAdd(enum_module);
            }
        }


        private void LoadClasses(Assembly assembly)
        {
            foreach (Type type in assembly.ExportedTypes.Where(m => m.GetCustomAttributes(typeof(LibraryClassAttribute), false).Length > 0))
            {
                Object extension = Activator.CreateInstance(type);

                LibraryClassAttribute attribute = (LibraryClassAttribute)Attribute.GetCustomAttribute(type, typeof(LibraryClassAttribute), false);

                // Класс содержит только глобальные функции и процедуры.
                if (attribute.AsGlobal && !attribute.AsObject)
                {
                    foreach (MethodInfo method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetCustomAttributes(typeof(LibraryClassMethodAttribute), false).Length > 0))
                    {
                        LibraryClassMethodAttribute method_attr = method.GetCustomAttribute<LibraryClassMethodAttribute>(false);

                        IFunction function = _programm.GlobalFunctions.Create(method_attr.Name);
                        function.Name = method_attr.Name;
                        function.Type = method.ReturnType == typeof(void) ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                        GetFunctionParameters(function, method);
                        function.Method = Library.MethodInfoExtensions.Bind(method, extension);
                        _programm.GlobalFunctions.Add(method_attr.Alias, function);

                    }

                    continue;
                }

                // Класс содержит обьект который добавляется в глобальный модуль.
                if (attribute.AsGlobal && attribute.AsObject)
                {
                    continue;
                }

                // Класс содержит обьект который добавляется в список обьектов оператора Новый.
                if (!attribute.AsGlobal && attribute.AsObject)
                {
                    continue;
                }
            }
        }


        public void LoadFromAssembly(Assembly assembly)
        {
            LoadEnums(assembly);
            LoadClasses(assembly);
        }


    }
}
