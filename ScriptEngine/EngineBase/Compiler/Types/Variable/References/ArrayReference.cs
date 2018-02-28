using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class ArrayReference : IVariableReference
    {
        private int _index;
        private IMethodWrapper _getter;
        private IMethodWrapper _setter;


        /// <summary>
        /// Простой конструктор.
        /// </summary>
        /// <param name="instance_type"></param>
        /// <param name="property"></param>
        public ArrayReference(IMethodWrapper getter, IMethodWrapper setter,int index)
        {
            _index = index;
            _getter = getter;
            _setter = setter;
        }

        /// <summary>
        /// Получить значение.
        /// </summary>
        /// <returns></returns>
        public IValue Get()
        {
            return _getter.Run(new IValue[] { ValueFactory.Create(_index) } );
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void Set(IValue value)
        {
            if (_setter == null)
                new Exception("Поле объекта недоступно для записи");
            _setter.Run(new IValue[] { ValueFactory.Create(_index), value });
        }

        /// <summary>
        /// Клонировать класс с новым объектом.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public IVariableReference Clone(object instance)
        {
            throw new NotImplementedException();
        }
    }
}
