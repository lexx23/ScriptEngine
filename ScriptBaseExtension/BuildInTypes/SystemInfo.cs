using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.IO;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes
{
    [LibraryClassAttribute(Name = "SystemInfo", Alias = "СистемнаяИнформация", AsGlobal = false, AsObject = true)]
    public class SystemInfo : LibraryModule<SystemInfo>
    {
        /// <summary>
        /// Версия операционной системы.
        /// </summary>
        [LibraryClassProperty(Alias = "ВерсияОС", Name = "OSVersion")]
        public string OSVersion { get => System.Environment.OSVersion.VersionString; }


        /// <summary>
        /// Версия компилятора.
        /// </summary>
        [LibraryClassProperty(Alias = "ВерсияПриложения", Name = "AppVersion")]
        public string AppVersion { get => System.Reflection.Assembly.GetCallingAssembly().GetName().Version.ToString(); }


        /// <summary>
        /// Уникальный идентификатор клиента.
        /// </summary>
        [LibraryClassProperty(Alias = "ИдентификаторКлиента", Name = "ClientID")]
        public string ClientID { get => new Guid().ToString(); }


        /// <summary>
        /// Информация о программе запуска ( информация о браузере).
        /// </summary>
        [LibraryClassProperty(Alias = "ИнформацияПрограммыПросмотра", Name = "UserAgentInformation")]
        public string UserAgentInformation
        {
            get
            {
                return "";
            }
        }

        /// <summary>
        /// Информация о памяти компьютера.
        /// </summary>
        [LibraryClassProperty(Alias = "ОперативнаяПамять", Name = "RAM")]
        public int RAM
        {
            get
            {
                return SystemInfoInternal.TotalMemory();
            }
        }

        /// <summary>
        /// Информация о процессоре компьютера.
        /// </summary>
        [LibraryClassProperty(Alias = "Процессор", Name = "Processor")]
        public string Processor { get => SystemInfoInternal.CPU(); }

        /// <summary>
        /// Информация о процессоре компьютера.
        /// </summary>
        [LibraryClassProperty(Alias = "ТипПлатформы", Name = "PlatformType")]
        public PlatformTypeEnum PlatformType
        {
            get
            {
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    if (RuntimeInformation.OSArchitecture == Architecture.X86)
                        return PlatformTypeEnum.Linux_x86;
                    if (RuntimeInformation.OSArchitecture == Architecture.X64)
                        return PlatformTypeEnum.Linux_x86_64;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (RuntimeInformation.OSArchitecture == Architecture.X86)
                        return PlatformTypeEnum.Windows_x8;
                    if (RuntimeInformation.OSArchitecture == Architecture.X64)
                        return PlatformTypeEnum.Windows_x86_64;
                }

                if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                {
                    if (RuntimeInformation.OSArchitecture == Architecture.X86)
                        return PlatformTypeEnum.MacOS_x86;
                    if (RuntimeInformation.OSArchitecture == Architecture.X64)
                        return PlatformTypeEnum.MacOS_x86_64;
                }

                throw new Exception("Не удалось распознать операционную систему.");
            }
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            return new SystemInfo();
        }
    }



    class SystemInfoInternal
    {
        [DllImport("kernel32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicallyInstalledSystemMemory(out long TotalMemoryInKilobytes);

        public static int TotalMemory()
        {

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                GetPhysicallyInstalledSystemMemory(out long memory);
                return (int)(memory / 1024);
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return LinuxMemory();

            return 0;
        }

        public static string CPU()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                return Environment.GetEnvironmentVariable("PROCESSOR_IDENTIFIER");

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                return LinuxCpu();

            return "";
        }

        private static string LinuxCpu()
        {
            try
            {
                string[] memory_data = File.ReadAllLines(@"/proc/cpuinfo");
                for (int i = 0; i < memory_data.Length; i++)
                {
                    Match match = new Regex(@"^model name\s+:(.*)", RegexOptions.Compiled).Match(memory_data[i]);
                    if (match.Groups[1].Success)
                    {
                        string value = match.Groups[1].Value;
                        return value;
                    }
                }
                return "";
            }
            catch { return ""; }
        }

        private static int LinuxMemory()
        {
            try
            {
                string[] memory_data = File.ReadAllLines(@"/proc/meminfo");
                for (int i = 0; i < memory_data.Length; i++)
                {
                    Match match = new Regex(@"^MemTotal:\s+(\d+)", RegexOptions.Compiled).Match(memory_data[i]);
                    if (match.Groups[1].Success)
                    {
                        string value = match.Groups[1].Value;
                        return (int)(Convert.ToInt64(value) / 1024);
                    }
                }
                return 0;
            }
            catch { return 0; }
        }
    }
}
