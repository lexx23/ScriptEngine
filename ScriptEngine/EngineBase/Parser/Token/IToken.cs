namespace ScriptEngine.EngineBase.Parser.Token
{
    public interface IToken
    {
        TokenTypeEnum Type { get; set; }

        TokenSubTypeEnum SubType { get; set; }

        /// <summary>
        /// Текст токена из модуля. 
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Информация о коде где был получен токен.
        /// </summary>
        CodeInformation CodeInformation { get; set; }


        IToken Clone();
        string ToString();
    }
}
