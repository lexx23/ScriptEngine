
using ScriptEngine.EngineBase.Parser.Token;

namespace ScriptEngine.EngineBase.Parser.TokenParser
{
    /// <summary>
    /// Интерфейс парсера лексем.
    /// </summary>
    public interface ITokenParser
    {
        /// <summary>
        /// Проверка текущего символа из итератора на соответствие данному токену.
        /// </summary>
        /// <param name="iterator"></param>
        /// <returns></returns>
        bool Parse(SourceIterator iterator, out IToken token);

    }
}
