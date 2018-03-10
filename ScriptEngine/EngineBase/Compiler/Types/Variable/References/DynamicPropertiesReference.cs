using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System;

namespace ScriptEngine.EngineBase.Compiler.Types.Variable.References
{
    class DynamicPropertiesReference : IVariableReference
    {
        private string _property_name;
        private IScriptDynamicProperties _obj;

        /// <summary>
        /// Создает класс на основе имени и обьекта с динамическими свойствами.
        /// </summary>
        /// <param name="property_name"></param>
        /// <param name="obj"></param>
        public DynamicPropertiesReference(string property_name,IScriptDynamicProperties obj)
        {
            _property_name = property_name;
            _obj = obj;
        }

        /// <summary>
        /// Получить значение.
        /// </summary>
        /// <returns></returns>
        public IValue Get()
        {
            return _obj.Get(_property_name);
        }

        /// <summary>
        /// Установить значение.
        /// </summary>
        /// <param name="value"></param>
        public void Set(IValue value)
        {
            _obj.Set(_property_name, value);
        }

        public IVariableReference Clone(object instance)
        {
            throw new NotImplementedException();
        }
    }
}
