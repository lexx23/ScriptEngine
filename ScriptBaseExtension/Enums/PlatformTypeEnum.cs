using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library;

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


    [LibraryClassAttribute(Name = "PlatformType", Alias = "ТипПлатформы", AsGlobal = true)]
    public class PlatformTypeEnumClass : BaseEnum<PlatformTypeEnum>
    {
        public PlatformTypeEnumClass()
        {
            Create();
        }
    }
}
