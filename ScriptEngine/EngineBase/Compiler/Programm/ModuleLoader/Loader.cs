using ScriptEngine.EngineBase.Compiler.Types.Function.ExternalMethods;
using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Function.Parameters;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Extensions;
using ScriptEngine.EngineBase.Library;
using System.Linq.Expressions;
using System.Reflection;
using System.Linq;
using System;
using System.ComponentModel;

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

        private void GetFunctionParameters(IFunction function, MethodInfo method)
        {
            IValue default_value = null;
            FunctionParameters parameters = new FunctionParameters();
            foreach (ParameterInfo param_info in method.GetParameters())
            {
                if (param_info.HasDefaultValue)
                    default_value = ValueFactory.Create(param_info.ParameterType, param_info.DefaultValue);

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

                ScriptModule enum_module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.ENUM, true, true);
                enum_module.InstanceType = type;
                //IFunction function = enum_module.Functions.Create("GetPropertyByName", true);
                //function.Param = new IVariable[] { new Variable() {Type = VariableTypeEnum.STACKVARIABLE } };

                foreach (IVariable var in new_enum.Properties)
                    enum_module.Variables.Add(var.Name, var);

                _programm.ModuleAdd(enum_module);
                attribute.Module = enum_module;
            }
        }


        private IVariableReference CreatePropertyReference(Type target_type, PropertyInfo property)
        {
            ParameterExpression target_expr = Expression.Parameter(typeof(Type), "target");
            ParameterExpression property_expr = Expression.Parameter(typeof(PropertyInfo), "property");

            var instance_type = typeof(LibraryReference<>).MakeGenericType(target_type);
            var constructor_type = instance_type.GetConstructor(new Type[] { typeof(Type), typeof(PropertyInfo) });
            var constructor = Expression.New(constructor_type, target_expr, property_expr);

            Func<Type, PropertyInfo, IVariableReference> result = Expression.Lambda<Func<Type, PropertyInfo, IVariableReference>>(constructor, target_expr, property_expr).Compile();
            return result(target_type, property);
        }

        private IMethodWrapper CreateMethod(Type target, MethodInfo method)
        {
            ParameterExpression target_expr = Expression.Parameter(typeof(Type), "target");
            ParameterExpression method_expr = Expression.Parameter(typeof(MethodInfo), "method");

            Type instance_type;
            if (method.ReturnType != typeof(void))
                instance_type = typeof(FunctionWrapper<>).MakeGenericType(target);
            else
                instance_type = typeof(ProcedureWrapper<>).MakeGenericType(target);
            var constructor_type = instance_type.GetConstructor(new Type[] { typeof(MethodInfo) });
            var constructor = Expression.New(constructor_type, method_expr);

            Func<Type, MethodInfo, IMethodWrapper> result = Expression.Lambda<Func<Type, MethodInfo, IMethodWrapper>>(constructor, target_expr, method_expr).Compile();
            return result(target, method);
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
                    ScriptModule global_module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.COMMON, true, true);
                    global_module.InstanceType = type;

                    foreach (PropertyInfo property in type.GetTypeInfo().GetProperties().Where(m => m.GetCustomAttributes(typeof(LibraryClassPropertyAttribute), false).Length > 0))
                    {
                        LibraryClassPropertyAttribute property_attr = property.GetCustomAttribute<LibraryClassPropertyAttribute>(false);

                        IVariableReference reference = CreatePropertyReference(type, property);
                        //Action<string> setter = CreateSetter<string>(extension, property.GetSetMethod());
                        //IValue value = new StringValue(getter, setter);


                        IVariable var = new Variable() { Name = property_attr.Name, Public = true, Reference = reference };
                        //_programm.GlobalVariables.Create(property_attr.Name,value);

                        //_programm.GlobalVariables.Create(property_attr.Alias, value);
                    }


                    foreach (MethodInfo method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetCustomAttributes(typeof(LibraryClassMethodAttribute), false).Length > 0))
                    {
                        LibraryClassMethodAttribute method_attr = method.GetCustomAttribute<LibraryClassMethodAttribute>(false);

                        IFunction function = _programm.GlobalFunctions.Create(method_attr.Name);
                        function.Name = method_attr.Name;
                        function.Type = method.ReturnType == typeof(void) ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                        GetFunctionParameters(function, method);
                        function.Method = CreateMethod(type, method).Clone(extension);
                        _programm.GlobalFunctions.Add(method_attr.Alias, function);

                    }

                    _programm.ModuleAdd(global_module);
                    continue;
                }

                // Класс содержит объект который добавляется в глобальный модуль.
                if (attribute.AsGlobal && attribute.AsObject)
                {
                    continue;
                }

                // Класс содержит объект который добавляется в список объектов оператора Новый.
                if (!attribute.AsGlobal && attribute.AsObject)
                {
                    ScriptModule module = new ScriptModule(attribute.Name, attribute.Alias, ModuleTypeEnum.OBJECT, false, true);
                    module.InstanceType = type;

                    foreach (MethodInfo method in type.GetTypeInfo().DeclaredMethods.Where(m => m.GetCustomAttributes(typeof(LibraryClassMethodAttribute), false).Length > 0))
                    {
                        LibraryClassMethodAttribute method_attr = method.GetCustomAttribute<LibraryClassMethodAttribute>(false);

                        IFunction function = module.Functions.Create(method_attr.Name, true);
                        function.Name = method_attr.Name;
                        function.Type = method.ReturnType == typeof(void) ? FunctionTypeEnum.PROCEDURE : FunctionTypeEnum.FUNCTION;
                        GetFunctionParameters(function, method);
                        function.Method = CreateMethod(type, method);
                        module.Functions.Add(method_attr.Alias, function);

                    }

                    _programm.ModuleAdd(module);
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
