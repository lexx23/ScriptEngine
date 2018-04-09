using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Collections.Generic;
using System;
using System.Collections;
using ScriptEngine.EngineBase.Interpreter.Context;

/// <summary>
///  Класс содержит все типы обьектов программы.
/// </summary>
namespace ScriptEngine.EngineBase.Compiler.Programm.Parts
{
    public class InternalTypes : IEnumerable<InternalScriptType>
    {
        private readonly IList<InternalScriptType> _types;

        public int Count { get => _types.Count; }

        public InternalTypes()
        {
            _types = new List<InternalScriptType>();
            Add(new InternalScriptType() { Name = "Неопределено", Alias = "Null", Description = "Неопределено" });
            Add(new InternalScriptType() { Name = "Булево", Alias = "Boolean", Description = "Булево" });
            Add(new InternalScriptType() { Name = "Дата", Alias = "Date", Description = "Дата" });
            Add(new InternalScriptType() { Name = "Число", Alias = "Number", Description = "Число" });
            Add(new InternalScriptType() { Name = "Строка", Alias = "String", Description = "Строка" });
            Add(new InternalScriptType() { Name = "Тип", Alias = "Type", Description = "Тип" });
            Add(new InternalScriptType() { Name = "ОбщийМодуль", Alias = "CommonModule", Description = "ОбщийМодуль" });
        }

        /// <summary>
        /// Добавить тип в программу.
        /// </summary>
        /// <param name="script_type"></param>
        public void Add(InternalScriptType script_type)
        {
            if (Get(script_type.Name) != null)
                throw new Exception($"Тип {script_type.Name} уже зарегистрирован.");

            script_type.Index = _types.Count;
            _types.Add(script_type);
        }

        /// <summary>
        /// Получить тип по его имени.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public InternalScriptType Get(string name)
        {
            for (int i = 0; i < _types.Count; i++)
            {
                if (String.Equals(_types[i].Name, name, StringComparison.OrdinalIgnoreCase) || String.Equals(_types[i].Alias, name, StringComparison.OrdinalIgnoreCase))
                    return _types[i];
            }

            return null;
        }

        /// <summary>
        /// Получить тип по типу C#.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public InternalScriptType Get(object instance)
        {
            if (instance != null)
            {
                Type type = instance.GetType();
                for (int i = 0; i < _types.Count; i++)
                {
                    if (_types[i].Type == type)
                        return _types[i];
                }
            }
            return null;
        }

        /// <summary>
        /// Получить тип используя контекст.
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public InternalScriptType Get(ScriptObjectContext context)
        {
            if (context.Module.Type == Module.ModuleTypeEnum.COMMON)
                return _types[6];

            return Get(context.Instance);
        }

        public IEnumerator<InternalScriptType> GetEnumerator()
        {
            return _types.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
