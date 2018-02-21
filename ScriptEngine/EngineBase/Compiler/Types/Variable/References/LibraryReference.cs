using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Linq.Expressions;
using System.Reflection;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class LibraryReference<T> : IVariableReference
    {
        private T _instance;
        private Func<T, IValue> _getter;
        private Action<T, IValue> _setter;

        public LibraryReference(T instance, Func<T, IValue> getter, Action<T, IValue> setter)
        {
            _instance = instance;
            _getter = getter;
            _setter = setter;
        }

        public LibraryReference(Type instance_type, PropertyInfo property)
        {
            _getter = CreateGetter(property);
            _setter = CreateSetter(property); ;
        }

        private Func<T, IValue> CreateGetter(PropertyInfo property_info)
        {
            ParameterExpression instance_parameter = Expression.Parameter(typeof(T), "target");
            var getter = property_info.GetGetMethod();
            Expression call = Expression.Call(instance_parameter, getter);
            MethodInfo factory = typeof(ValueFactory).GetMethod("Create", new Type[] { getter.ReturnType });
            Expression result = Expression.Call(factory, call);

            return Expression.Lambda<Func<T, IValue>>(result, instance_parameter).Compile();
        }


        private Action<T, IValue> CreateSetter(PropertyInfo property_info)
        {
            //ParameterExpression instance_parameter = Expression.Parameter(typeof(T), "target");
            //ParameterExpression argument_parameter = Expression.Parameter(typeof(IValue), "argument");

            //var setter = property_info.GetSetMethod();
            //Expression call;
            //if (setter == null)
            //    call = Expression.Throw(Expression.Constant(new Exception("Поле объекта недоступно для записи")));
            //else
            //{
            //    call = Expression.Call(instance_parameter, setter,argument_parameter);
            //}

            //return Expression.Lambda<Action<T, IValue>>(call, instance_parameter, argument_parameter).Compile();

            return null;
        }


        public IValue Get()
        {
            return _getter(_instance);
        }

        public void Set(IValue value)
        {
            _setter(_instance, value);
        }

        public IVariableReference Clone(object instance)
        {
            return new LibraryReference<T>((T)instance, _getter, _setter);
        }
    }
}
