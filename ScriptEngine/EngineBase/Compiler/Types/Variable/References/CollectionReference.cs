/*----------------------------------------------------------
	This Source Code Form is subject to the terms of the 
	Mozilla Public License, v.2.0. If a copy of the MPL 
	was not distributed with this file, You can obtain one 
	at http://mozilla.org/MPL/2.0/.
----------------------------------------------------------*/

using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;


namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    public class CollectionReference : IVariableReference
    {
        private IValue _index;
        private ICollectionIndexer _array;


        /// <summary>
        /// Простой конструктор.
        /// </summary>
        public CollectionReference(ICollectionIndexer array,IValue index)
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
            _array.Set(_index, value);
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
