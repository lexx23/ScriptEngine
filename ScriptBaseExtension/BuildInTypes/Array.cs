using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "array", Alias = "Массив", AsGlobal = false, AsObject = true)]
    public class Array
    {
        private readonly List<IValue> _values;

        public Array()
        {
            _values = new List<IValue>();
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
        public void Insert(int index, IValue value)
        {
            _values.Insert(index, value);
        }

        [LibraryClassMethod(Alias = "Найти", Name = "Find")]
        public IValue Find(IValue value)
        {
            var idx = _values.FindIndex(x => x.Equals(value));
            if (idx < 0)
            {
                return ValueFactory.Create();
            }
            else
            {
                return ValueFactory.Create(idx);
            }
        }

        [LibraryClassMethod(Alias = "Удалить", Name = "Delete")]
        public void Remove(int index)
        {
            _values.RemoveAt(index);
        }

        [LibraryClassMethod(Alias = "ВГраница", Name = "UBound")]
        public int UpperBound()
        {
            return _values.Count - 1;
        }

        [LibraryClassMethod(Alias = "Получить", Name = "Get")]
        public IValue Get(int index)
        {
            return _values[index];
        }

        [LibraryClassMethod(Alias = "Установить", Name = "Set")]
        public void Set(int index, IValue value)
        {
            _values[index] = value;
        }

        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static ScriptObjectContext Constructor(IValue[] parameters)
        {
            return new ScriptObjectContext((ScriptModule)parameters[0].AsObject());
        }
    }
}
