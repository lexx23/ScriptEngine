using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library;

namespace ScriptBaseFunctionsLibrary.Enums
{
    public enum ScriptSearchDirectionInner
    {
        [EnumStringAttribute("СНачала")]
        FromBegin,
        [EnumStringAttribute("СКонца")]
        FromEnd
    }

    [LibraryClassAttribute(Name = "SearchDirection", Alias = "НаправлениеПоиска", AsGlobal = true)]
    public class ScriptSearchDirection : BaseEnum<ScriptSearchDirectionInner>
    {
        public ScriptSearchDirection()
        {
            Create();
        }

    }
}
