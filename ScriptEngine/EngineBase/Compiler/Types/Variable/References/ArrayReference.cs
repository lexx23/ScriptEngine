using ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class ArrayReference : IVariableReference
    {
        private int _index;
        private IScriptArray _array;


        /// <summary>
        /// Простой конструктор.
        /// </summary>
        public ArrayReference(IScriptArray array,int index)
        {
            _index = index;
            _array = array;
        }

        /// <summary>
        /// Получить значение.
        /// </summary>
        /// <returns></returns>
        public IValue Get()
        {
            return _array.Get(_index);
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void Set(IValue value)
        {
            //if (_array.Set == null)
            //  new Exception("Поле объекта недоступно для записи");
            _array.Set(_index, value);        }

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
