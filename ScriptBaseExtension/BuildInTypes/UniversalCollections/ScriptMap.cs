using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections.Generic;
using System.Collections;


namespace ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections
{
    [LibraryClassAttribute(Name = "Map", Alias = "Соответствие", RegisterType = true, AsGlobal = false)]
    public class ScriptMap : LibraryModule<ScriptMap>, IEnumerable<IValue>, IUniversalCollection, ICollectionIndexer
    {
        private readonly Dictionary<IValue, IValue> _values = new Dictionary<IValue, IValue>(new IValueComparer());

        [LibraryClassMethod(Alias = "Вставить", Name = "Insert")]
        public void Insert(IValue index, IValue value)
        {
            Set(index, value);
        }

        [LibraryClassMethod(Alias = "Количество", Name = "Count")]
        public int Count()
        {
            return _values.Count;
        }

        [LibraryClassMethod(Alias = "Очистить", Name = "Clear")]
        public void Clear()
        {
            _values.Clear();
        }

        [LibraryClassMethod(Alias = "Получить", Name = "Get")]
        public IValue Retrieve(IValue key)
        {
            return this.Get(key);
        }

        [LibraryClassMethod(Alias = "Удалить", Name = "Delete")]
        public void Delete(IValue value)
        {
            _values.Remove(value);
        }

        public IValue Get(IValue index)
        {
            if (!_values.TryGetValue(index, out IValue result))
                return ValueFactory.Create();
            else
                return result;
        }

        public void Set(IValue index, IValue value)
        {
            _values[index] = value;
        }

        public IEnumerator<IValue> GetEnumerator()
        {
            foreach (IValue key in _values.Keys)
            {
                yield return new ScriptKeyValuePair(key, _values[key]);
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            return new ScriptMap();
        }
    }
}
