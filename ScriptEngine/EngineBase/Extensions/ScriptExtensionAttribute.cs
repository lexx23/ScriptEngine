using System;

namespace ScriptEngine.EngineBase.Extensions
{
    /// <summary>
    /// Параметры Global и Object задают поведение расширения.
    /// <para>Name это имя обьекта с которым он будет доступен в операторе Новый или в глобальном контексте.</para>
    /// <para>Global = true, Object = false Методы и свойства добавляются в глобальный контекст.</para>
    /// <para>Global = true, Object = true  Расширение представляется как обьект и, этот обьект, добавляется в глобальный контекст.</para>
    /// <para>Global = false, Object = true  Расширение представляется как обьект и, этот обьект, становится доступным в операторе Новый.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ScriptExtensionAttribute : Attribute
    {
        public bool AsGlobal { get; set; }
        public bool AsObject {get; set;}
        public string Name { get; set; }
        public string Alias { get; set; }
    }
}