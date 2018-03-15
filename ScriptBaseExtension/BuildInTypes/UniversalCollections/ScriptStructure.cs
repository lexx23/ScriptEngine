using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections.Generic;
using System.Collections;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections
{
    /// <summary>
    /// Представляет собой коллекцию пар КлючИЗначение.
    /// </summary>
    /// <example>
    /// Запись = Новый Структура;
    /// Запись.Вставить("Настройки");
    /// Запись.Вставить("Отчет","123");

    /// Запись.Настройки = "Основные";
    /// Запись.Отчет = "Первый";
    /// настройки = неопределено;

    /// результат = Запись.Свойство("Настройки");
    /// результат = Запись.Свойство("НетНастройки");
    /// результат = Запись.Свойство("Настройки",настройки);
    /// настройки = неопределено;

    /// счетчик = 0;
    /// Для Каждого значение из Запись Цикл
    ///     счетчик = счетчик + 1;
    ///	    Сообщить(значение.Ключ);
    ///     Сообщить(значение.Value);
    /// КонецЦикла;
    /// 
    /// Запись = Новый Структура("Настройки,Отчет");
    /// количество = Запись.количество();
    /// Запись = Новый Структура("Настройки,Отчет",124);
    /// Запись = Новый Структура("Настройки,Отчет",124,"Отчет");
    /// </example>
    [LibraryClassAttribute(Name = "Structure", Alias = "Структура", AsGlobal = false, AsObject = true)]
    public class ScriptStructure : LibraryModule<ScriptStructure>, IEnumerable<IValue>, IScriptDynamicProperties, IUniversalCollection
    {
        private readonly IDictionary<string, IValue> _values;

        public ScriptStructure()
        {
            _values = new Dictionary<string, IValue>(StringComparer.OrdinalIgnoreCase);
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

        [LibraryClassMethod(Alias = "Вставить", Name = "Insert")]
        public void Insert(IValue name, IValue value = null)
        {
            _values[name.AsString()] = value;
        }

        public void Insert(string name, IValue value = null)
        {
            _values[name] = value;
        }

        [LibraryClassMethod(Alias = "Удалить", Name = "Delete")]
        public void Delete(IValue value)
        {
            _values.Remove(value.AsString());
        }

        [LibraryClassMethod(Alias = "Свойство", Name = "Property")]
        public bool Property(string name, IVariable variable = null)
        {
            if (_values.TryGetValue(name, out IValue out_value))
            {
                variable.Value = out_value;
                return true;
            }
            else
                out_value = ValueFactory.Create();

            variable.Value = out_value;
            return false;
        }

        public IEnumerator<IValue> GetEnumerator()
        {
            foreach (string key in _values.Keys)
            {
                yield return new ScriptKeyValuePair(ValueFactory.Create(key), _values[key]);
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Exist(string name)
        {
            return _values.ContainsKey(name);
        }

        public IValue Get(string name)
        {
            return _values[name];
        }

        public void Set(string name, IValue value)
        {
            _values[name] = value;
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            if (parameters.Length > 0)
            {
                string[] roperties = parameters[0].AsString().Split(',');

                ScriptStructure structure = new ScriptStructure();

                for (int i = 0; i < roperties.Length; i++)
                {
                    string name = roperties[i].Trim();
                    if (i+1 < parameters.Length)
                        structure.Insert(name, parameters[i+1]);
                    else
                        structure.Insert(name,ValueFactory.Create());
                }

                return structure;
            }

            return new ScriptStructure();
        }
    }
}
