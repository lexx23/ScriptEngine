using System;

namespace ScriptEngine.EngineBase.Compiler.Programm.Parts.Module
{
    /// <summary>
    /// Параметры Global и Object задают поведение расширения.
    /// <para>Name это имя обьекта с которым он будет доступен в операторе Новый или в глобальном контексте.</para>
    /// <para>Global = true, Object = false Методы и свойства добавляются в глобальный контекст.</para>
    /// <para>Global = true, Object = true  Расширение представляется как обьект и, этот обьект, добавляется в глобальный контекст.</para>
    /// <para>Global = false, Object = true  Расширение представляется как обьект и, этот обьект, становится доступным в операторе Новый.</para>
    /// </summary>
    public interface IModulePlace
    {
        bool AsGlobal { get; set; }
        bool AsObject {get; set;}
    }
}