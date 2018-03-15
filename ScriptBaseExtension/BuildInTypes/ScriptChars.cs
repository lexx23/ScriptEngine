using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "Chars", Alias = "Символы", AsGlobal = true, AsObject = true)]
    public class ScriptChars : LibraryModule<ScriptChars>
    {
        /// <summary>
        /// Символ возврата каретки.
        /// </summary>
        /// <value>Символ возврата каретки.</value>
        [LibraryClassProperty(Alias = "ВК", Name = "CR")]
        public string CR { get => "\r"; }

        /// <summary>
        /// Символ вертикальной табуляции.
        /// </summary>
        /// <value>Символ вертикальной табуляции.</value>
        [LibraryClassProperty(Alias = "ВТаб", Name = "VTab")]
        public string VTab { get => "\v"; }

        /// <summary>
        /// Символ неразрывного пробела.
        /// </summary>
        /// <value>Символ неразрывного пробела.</value>
        [LibraryClassProperty(Alias = "НПП", Name = "NBSp")]
        public string Nbsp { get => "\u00A0"; }


        /// <summary>
        /// Символ перевода строки.
        /// </summary>
        /// <value>Символ перевода строки.</value>
        [LibraryClassProperty(Alias = "ПС", Name = "LF")]
        public string LF { get => "\n"; }

        /// <summary>
        /// Символ промотки.
        /// </summary>
        /// <value>Символ промотки.</value>
        [LibraryClassProperty(Alias = "ПФ", Name = "FF")]
        public string FF { get => "\f"; }

        /// <summary>
        /// Символ табуляции.
        /// </summary>
        /// <value>Символ горизонтальной табуляции.</value>
        [LibraryClassProperty(Alias = "Таб", Name = "Tab")]
        public string Tab { get => "\t"; }
    }
}
