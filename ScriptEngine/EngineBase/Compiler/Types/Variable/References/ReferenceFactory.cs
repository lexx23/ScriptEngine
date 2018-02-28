using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    class ReferenceFactory
    {
        /// <summary>
        /// Статические переменные.
        /// </summary>
        /// <returns></returns>
        public static IVariableReference Create() => new SimpleReference();

        /// <summary>
        /// Создает обертку для доступа к переменным которые ссылаются на другие переменные.
        /// </summary>
        /// <param name="reference"></param>
        /// <returns></returns>
        public static IVariableReference Create(IVariableReference reference) => new ScriptReference(reference);

        /// <summary>
        /// Создает обертку для доступа к свойствам "внешних" классов, классы которые загружаются из dll.
        /// </summary>
        /// <param name="target_type"></param>
        /// <param name="property"></param>
        /// <returns></returns>
        public static IVariableReference Create(Type target_type, PropertyInfo property)
        {
            ParameterExpression target_expr = Expression.Parameter(typeof(Type), "target");
            ParameterExpression property_expr = Expression.Parameter(typeof(PropertyInfo), "property");

            var instance_type = typeof(LibraryReference<>).MakeGenericType(target_type);
            var constructor_type = instance_type.GetConstructor(new Type[] { typeof(Type), typeof(PropertyInfo) });
            var constructor = Expression.New(constructor_type, target_expr, property_expr);

            Func<Type, PropertyInfo, IVariableReference> result = Expression.Lambda<Func<Type, PropertyInfo, IVariableReference>>(constructor, target_expr, property_expr).Compile();
            return result(target_type, property);
        }


    }
}
