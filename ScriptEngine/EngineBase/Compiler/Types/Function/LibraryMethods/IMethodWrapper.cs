using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;


namespace ScriptEngine.EngineBase.Compiler.Types.Function.LibraryMethods
{
    /// <summary>
    /// Обертка для вызова внешних процедур/функций.
    /// </summary>
    public interface IMethodWrapper
    {

        /// <summary>
        /// Выполнить функцию/процедуру.
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        IValue Run(IValue[] param);

        /// <summary>
        /// Копия функции с указанием нового объекта.
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        IMethodWrapper Clone(object instance);
    }
}
