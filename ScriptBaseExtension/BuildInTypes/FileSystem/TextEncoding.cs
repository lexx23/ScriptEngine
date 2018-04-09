using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptBaseFunctionsLibrary.Enums;
using System.Text;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem
{
    static class TextEncoding
    {
        static TextEncoding()
        {
            Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }

        public static Encoding GetEncodingByName(string encoding, bool addBOM = true)
        {
            Encoding enc;
            if (encoding == string.Empty)
                enc = new UTF8Encoding(addBOM);
            else
            {
                switch (encoding.ToUpper())
                {
                    case "UTF-8":
                        enc = new UTF8Encoding(addBOM);
                        break;
                    case "UTF-16":
                    case "UTF-16LE":
                    // предположительно, варианты UTF16_PlatformEndian\UTF16_OppositeEndian
                    // зависят от платформы x86\m68k\SPARC. Пока нет понимания как корректно это обработать.
                    // Сейчас сделано исходя из предположения что PlatformEndian должен быть LE поскольку 
                    // платформа x86 более широко распространена
                    case "UTF16_PLATFORMENDIAN":
                        enc = new UnicodeEncoding(false, addBOM);
                        break;
                    case "UTF-16BE":
                    case "UTF16_OPPOSITEENDIAN":
                        enc = new UnicodeEncoding(true, addBOM);
                        break;
                    case "UTF-32":
                    case "UTF-32LE":
                    case "UTF32_PLATFORMENDIAN":
                        enc = new UTF32Encoding(false, addBOM);
                        break;
                    case "UTF-32BE":
                    case "UTF32_OPPOSITEENDIAN":
                        enc = new UTF32Encoding(true, addBOM);
                        break;
                    default:
                        enc = Encoding.GetEncoding(encoding);
                        break;

                }
            }

            return enc;
        }

        public static Encoding GetEncoding(IValue encoding, bool addBOM = true)
        {
            if (encoding.BaseType == ValueTypeEnum.STRING)
                return GetEncodingByName(encoding.AsString(), addBOM);
            else
            {
                if (encoding.BaseType != ValueTypeEnum.OBJECT)
                    throw new Exception("Неверный тип аргумента");

                var encodingEnum = (TextEncodingEnumInner)encoding.AsObject();

                Encoding enc;
                if (encodingEnum == TextEncodingEnumInner.ANSI)
                    enc = Encoding.GetEncoding(1251);
                else if (encodingEnum == TextEncodingEnumInner.OEM)
                    enc = Encoding.GetEncoding(866);
                else if (encodingEnum == TextEncodingEnumInner.UTF16)
                    enc = new UnicodeEncoding(false, addBOM);
                else if (encodingEnum == TextEncodingEnumInner.UTF8)
                    enc = new UTF8Encoding(addBOM);
                else if (encodingEnum == TextEncodingEnumInner.UTF8NoBOM)
                    enc = new UTF8Encoding(false);
                else if (encodingEnum == TextEncodingEnumInner.System)
                    enc = Encoding.Default;
                else
                    throw new Exception("Неверный тип аргумента");

                return enc;
            }
        }
    }
}
