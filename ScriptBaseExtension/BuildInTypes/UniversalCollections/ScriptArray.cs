using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections.Generic;
using System.Collections;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections
{
    [LibraryClassAttribute(Name = "Array", Alias = "Массив", RegisterType = true, AsGlobal = false)]
    public class ScriptArray : LibraryModule<ScriptArray>, IEnumerable<IValue>, IUniversalCollection, ICollectionIndexer
    {
        private readonly List<IValue> _values;

        public ScriptArray()
        {
            _values = new List<IValue>();
        }

        public ScriptArray(IEnumerable<IValue> values)
        {
            _values = new List<IValue>(values);
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


        [LibraryClassMethod(Alias = "Добавить", Name = "Add")]
        public void Add(IValue value)
        {
            _values.Add(value);
        }

        [LibraryClassMethod(Alias = "Вставить", Name = "Insert")]
        public void Insert(IValue index, IValue value)
        {
            _values.Insert(index.AsInt(), value);
        }

        [LibraryClassMethod(Alias = "Найти", Name = "Find")]
        public IValue Find(IValue value)
        {
            var index = _values.FindIndex(x => x.Equals(value));
            if (index < 0)
                return ValueFactory.Create();
            else
                return _values[index];
        }

        [LibraryClassMethod(Alias = "Удалить", Name = "Delete")]
        public void Delete(IValue value)
        {
            _values.RemoveAt(value.AsInt());
        }

        [LibraryClassMethod(Alias = "ВГраница", Name = "UBound")]
        public int UBound()
        {
            return _values.Count - 1;
        }

        public IValue Get(IValue index)
        {
            return _values[index.AsInt()];
        }

        public void Set(IValue index, IValue value)
        {
            _values[index.AsInt()] = value;
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return new ScriptArray();

            ScriptArray new_array = null;
            for (int dim = parameters.Length - 1; dim >= 0; dim--)
            {
                if (parameters[dim].BaseType != ValueTypeEnum.NULL)
                {
                    int bound = parameters[dim].AsInt();
                    var new_instance = new ScriptArray();
                    FillArray(new_instance, bound);
                    if (new_array != null)
                    {
                        for (int i = 0; i < bound; i++)
                        {
                            new_instance._values[i] = CloneArray(new_array);
                        }
                    }
                    new_array = new_instance;
                }
            }

            return new_array;
        }


        private static void FillArray(ScriptArray currentArray, int bound)
        {
            for (int i = 0; i < bound; i++)
            {
                currentArray._values.Add(ValueFactory.Create());
            }
        }

        private static IValue CloneArray(ScriptArray cloneable)
        {
            ScriptArray clone = new ScriptArray();
            foreach (var item in cloneable._values)
            {
                if (item.BaseType == ValueTypeEnum.NULL)
                    clone._values.Add(ValueFactory.Create());
                else
                    clone._values.Add(item);
            }
            return clone;
        }


        public IEnumerator<IValue> GetEnumerator()
        {
            int i = 0;
            while (i < _values.Count)
            {
                yield return _values[i++];
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
