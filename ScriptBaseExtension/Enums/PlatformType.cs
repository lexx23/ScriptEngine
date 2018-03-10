using ScriptEngine.EngineBase.Library;
using ScriptEngine.EngineBase.Library.Attributes;

namespace ScriptBaseFunctionsLibrary.Enums
{
    public enum PlatformTypeEnum
    {
        Linux_x86,
        Linux_x86_64,
        MacOS_x86,
        MacOS_x86_64,
        Windows_x8,
        Windows_x86_64
    }


    [LibraryClassAttribute(Name = "ТипПлатформы", Alias = "PlatformType", AsGlobal = true, AsObject = true)]
    public class PlatformTypeEnumClass : BaseEnum<PlatformTypeEnum>
    {

    }
}
