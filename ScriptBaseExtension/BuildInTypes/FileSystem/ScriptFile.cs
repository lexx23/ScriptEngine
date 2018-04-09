using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using ScriptEngine.EngineBase.Extensions;
using System.IO;
using System;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem
{
    [LibraryClassAttribute(Name = "File", Alias = "Файл", RegisterType = true, AsGlobal = false)]
    public class ScriptFile : LibraryModule<ScriptFile>
    {
        private readonly string _file_name;
        private string _name;
        private string _base_name;
        private string _full_name;
        private string _path;
        private string _extension;

        public ScriptFile(string file)
        {
            if (String.IsNullOrWhiteSpace(file))
            {
                _name = "";
                _base_name = "";
                _full_name = "";
                _path = "";
                _extension = "";
            }
            _file_name = file;
        }

        private string LazyField(ref string value, Func<string, string> algo)
        {
            if (value == null)
                value = algo(_file_name);

            return value;
        }

        [LibraryClassProperty(Alias = "Имя", Name = "Name")]
        public string Name { get => LazyField(ref _name, GetFileNameV8Compatible); }

        [LibraryClassProperty(Alias = "ИмяБезРасширения", Name = "BaseName")]
        public string BaseName { get => LazyField(ref _base_name, System.IO.Path.GetFileNameWithoutExtension); }

        [LibraryClassProperty(Alias = "ПолноеИмя", Name = "FullName")]
        public string FullName { get => LazyField(ref _full_name, System.IO.Path.GetFullPath); }

        [LibraryClassProperty(Alias = "Путь", Name = "Path")]
        public string Path { get => LazyField(ref _path, GetPathWithEndingDelimiter); }

        private string GetFileNameV8Compatible(string arg)
        {
            return System.IO.Path.GetFileName(arg.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
        }

        private string GetPathWithEndingDelimiter(string src)
        {
            src = src.Trim();
            if (src.Length == 1 && src[0] == System.IO.Path.DirectorySeparatorChar)
                return src; // корневой каталог

            var path = System.IO.Path.GetDirectoryName(src.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
            if (path == null)
                return src; // корневой каталог

            if (path.Length > 0 && path[path.Length - 1] != System.IO.Path.DirectorySeparatorChar)
                path += System.IO.Path.DirectorySeparatorChar;

            return path;
        }

        [LibraryClassProperty(Alias = "Расширение", Name = "Extension")]
        public string Extension { get => LazyField(ref _extension, System.IO.Path.GetExtension); }

        [LibraryClassMethod(Alias = "Существует", Name = "Exist")]
        public bool Exist()
        {
            if (_file_name == String.Empty)
                return false;

            try
            {
                File.GetAttributes(FullName);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                return false;
            }

            return true;
        }

        [LibraryClassMethod(Alias = "Размер", Name = "Size")]
        public long Size()
        {
            return new FileInfo(FullName).Length;
        }

        [LibraryClassMethod(Alias = "ПолучитьНевидимость", Name = "GetHidden")]
        public bool GetHidden()
        {
            var attr = File.GetAttributes(FullName);
            return attr.HasFlag(System.IO.FileAttributes.Hidden);
        }

        [LibraryClassMethod(Alias = "ПолучитьТолькоЧтение", Name = "GetReadOnly")]
        public bool GetReadOnly()
        {
            var attr = File.GetAttributes(FullName);
            return attr.HasFlag(System.IO.FileAttributes.ReadOnly);
        }

        [LibraryClassMethod(Alias = "ПолучитьВремяИзменения", Name = "GetModificationTime")]
        public DateTime GetModificationTime()
        {
            return File.GetLastWriteTime(FullName);
        }

        [LibraryClassMethod(Alias = "ПолучитьВремяСоздания", Name = "GetCreationTime")]
        public DateTime GetCreationTime()
        {
            return File.GetCreationTime(FullName);
        }

        [LibraryClassMethod(Alias = "УстановитьНевидимость", Name = "SetHidden")]
        public void SetHidden(bool value)
        {
            FileSystemInfo entry = new FileInfo(FullName);

            if (value)
                entry.Attributes |= System.IO.FileAttributes.Hidden;
            else
                entry.Attributes &= ~System.IO.FileAttributes.Hidden;
        }

        [LibraryClassMethod(Alias = "УстановитьТолькоЧтение", Name = "SetReadOnly")]
        public void SetReadOnly(bool value)
        {
            FileSystemInfo entry = new FileInfo(FullName);
            if (value)
                entry.Attributes |= System.IO.FileAttributes.ReadOnly;
            else
                entry.Attributes &= ~System.IO.FileAttributes.ReadOnly;
        }

        [LibraryClassMethod(Alias = "УстановитьВремяИзменения", Name = "SetModificationTime")]
        public void SetModificationTime(DateTime dateTime)
        {
            FileSystemInfo entry = new FileInfo(FullName);
            entry.LastWriteTime = dateTime;
        }

        [LibraryClassMethod(Alias = "ЭтоКаталог", Name = "IsDirectory")]
        public bool IsDirectory()
        {
            var attr = File.GetAttributes(FullName);
            return attr.HasFlag(FileAttributes.Directory);
        }

        [LibraryClassMethod(Alias = "ЭтоФайл", Name = "IsFile")]
        public bool IsFile()
        {
            var attr = File.GetAttributes(FullName);
            return !attr.HasFlag(FileAttributes.Directory);
        }

        public FileAttributes GetAttributes()
        {
            return File.GetAttributes(FullName);
        }


        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            string name = string.Empty;
            if (parameters.Length > 0)
                name = parameters[0].AsString();

            return new ScriptFile(name);
        }
    }
}
