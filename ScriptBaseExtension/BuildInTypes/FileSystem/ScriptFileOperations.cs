using ScriptEngine.EngineBase.Library.Attributes;
using System.IO;
using System;
using System.Linq;
using ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections;
using System.Collections.Generic;
using System.Security;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;

namespace ScriptBaseFunctionsLibrary.BuildInTypes.FileSystem
{
    [LibraryClassAttribute(AsGlobal = true, Name = "file_operations")]
    public class ScriptFileOperations
    {
        /// <summary>
        /// Возвращает каталог временных файлов ОС
        /// </summary>
        /// <returns>Строка. Путь к каталогу временных файлов</returns>
        [LibraryClassMethodAttribute(Alias = "КаталогВременныхФайлов", Name = "TempFilesDir")]
        public string TempFilesDir()
        {
            return Path.GetTempPath();
        }

        /// <summary>
        /// Получает имя файла во временом каталоге.
        /// </summary>
        /// <param name="ext">Расширение будущего файла. Если не указано, то по умолчанию расширение равно ".tmp"</param>
        /// <returns>Строка. Полный путь ко временному файлу.</returns>
        [LibraryClassMethodAttribute(Alias = "ПолучитьИмяВременногоФайла", Name = "GetTempFileName")]
        public string GetTempFilename(string ext = null)
        {
            // примитивная реализация "в лоб"
            var fn = Path.GetRandomFileName();
            if (ext != null && !String.IsNullOrWhiteSpace(ext))
            {
                if (ext[0] == '.')
                    fn += ext;
                else
                    fn += "." + ext;
            }

            return Path.Combine(TempFilesDir(), fn);

        }

        /// <summary>
        /// Получает разделитель пути в соответствии с текущей операционной системой
        /// </summary>
        [LibraryClassMethodAttribute(Alias = "ПолучитьРазделительПути", Name = "GetPathSeparator")]
        public string GetPathSeparator()
        {
            return new string(new char[] { Path.DirectorySeparatorChar });
        }

        public static void DeleteDirectory(string path, bool recursive)
        {
            if (recursive)
            {
                var subfolders = Directory.GetDirectories(path);
                foreach (var s in subfolders)
                {
                    DeleteDirectory(s, recursive);
                }
            }

            var files = Directory.GetFiles(path);
            foreach (var f in files)
            {
                File.Delete(f);
            }

            Directory.Delete(path);
        }

        /// <summary>
        /// Получить текущий каталог
        /// </summary>
        [LibraryClassMethodAttribute(Alias = "ТекущийКаталог", Name = "CurrentDirectory")]
        public string CurrentDirectory()
        {
            return System.IO.Directory.GetCurrentDirectory();
        }

        /// <summary>
        /// Создать каталог
        /// </summary>
        /// <param name="path">Имя нового каталога</param>
        [LibraryClassMethodAttribute(Alias = "СоздатьКаталог", Name = "CreateDirectory")]
        public void CreateDirectory(string path)
        {
            System.IO.Directory.CreateDirectory(path);
        }

        /// <summary>
        /// Выполняет поиск файлов по маске
        /// </summary>
        /// <param name="dir">Каталог, в котором выполняется поиск</param>
        /// <param name="mask">Маска имени файла (включая символы * и ?)</param>
        /// <param name="recursive">Флаг рекурсивного поиска в поддиректориях</param>
        /// <returns>Массив объектов Файл, которые были найдены.</returns>
        [LibraryClassMethodAttribute(Alias = "НайтиФайлы", Name = "FindFiles")]
        public IValue FindFiles(string dir, string mask = null, bool recursive = false)
        {
            if (mask == null)
            {
                // fix 225, 227, 228
                var fObj = new ScriptFile(dir);
                if (fObj.Exist())
                {
                    return new ScriptArray(new[] { fObj });
                }
                else
                {
                    return new ScriptArray();
                }
            }
            else if (File.Exists(dir))
            {
                return new ScriptArray();
            }

            if (!Directory.Exists(dir))
                return new ScriptArray();

            var filesFound = FindFilesV8Compatible(dir, mask, recursive);

            return new ScriptArray(filesFound);

        }

        private static IEnumerable<ScriptFile> FindFilesV8Compatible(string dir, string mask, bool recursive)
        {
            var collectedFiles = new List<ScriptFile>();
            IEnumerable<ScriptFile> entries;
            IEnumerable<ScriptFile> folders = null;
            try
            {
                if (recursive)
                    folders = Directory.GetDirectories(dir).Select(x => new ScriptFile(x));

                entries = Directory.EnumerateFileSystemEntries(dir, mask)
                                   .Select(x => new ScriptFile(x));
            }
            catch (SecurityException)
            {
                return collectedFiles;
            }
            catch (UnauthorizedAccessException)
            {
                return collectedFiles;
            }

            if (recursive)
            {
                foreach (var fileFound in entries)
                {
                    try
                    {
                        var attrs = fileFound.GetAttributes();
                        if (attrs.HasFlag(FileAttributes.ReparsePoint))
                        {
                            collectedFiles.Add(fileFound);
                            continue;
                        }
                    }
                    catch (SecurityException)
                    {
                        continue;
                    }
                    catch (UnauthorizedAccessException)
                    {
                        continue;
                    }

                    collectedFiles.Add(fileFound);
                }

                foreach (var folder in folders)
                {
                    try
                    {
                        var attrs = folder.GetAttributes();
                        if (!attrs.HasFlag(FileAttributes.ReparsePoint))
                        {
                            collectedFiles.AddRange(FindFilesV8Compatible(folder.FullName, mask, true));
                        }
                    }
                    catch (SecurityException)
                    {
                    }
                    catch (UnauthorizedAccessException)
                    {
                    }
                }
            }
            else
            {
                collectedFiles.AddRange(entries);
            }

            return collectedFiles;
        }

        /// <summary>
        /// Удаление файлов
        /// </summary>
        /// <param name="path">Каталог из которого удаляются файлы, или сам файл.</param>
        /// <param name="mask">Маска файлов. Необязательный параметр. Если указан, то первый параметр трактуется, как каталог.</param>
        [LibraryClassMethodAttribute(Alias = "УдалитьФайлы", Name = "DeleteFiles")]
        public void DeleteFiles(string path, string mask = null)
        {
            if (mask == null)
            {
                if (Directory.Exists(path))
                {
                    System.IO.Directory.Delete(path, true);
                }
                else
                {
                    System.IO.File.Delete(path);
                }
            }
            else
            {
                // bugfix #419
                if (!Directory.Exists(path))
                    return;

                var entries = System.IO.Directory.EnumerateFileSystemEntries(path, mask)
                    .AsParallel()
                    .ToArray();
                foreach (var item in entries)
                {
                    System.IO.FileInfo finfo = new System.IO.FileInfo(item);
                    if (finfo.Attributes.HasFlag(System.IO.FileAttributes.Directory))
                    {
                        //recursively delete directory
                        DeleteDirectory(item, true);
                    }
                    else
                    {
                        System.IO.File.Delete(item);
                    }
                }
            }
        }
    }
}
