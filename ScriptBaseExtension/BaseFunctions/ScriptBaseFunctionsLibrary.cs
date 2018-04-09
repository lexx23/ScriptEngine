using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Extensions;
using ScriptBaseFunctionsLibrary.Enums;
using System;
using System.Xml;
using ScriptBaseFunctionsLibrary.BuildInTypes;
using ScriptEngine.EngineBase.Compiler.Programm.Parts.Module;
using System.Collections.Generic;
using ScriptEngine.EngineBase.Interpreter.Context;
using System.Linq;

namespace ScriptBaseLibrary
{
    [LibraryClassAttribute(AsGlobal = true, Name = "global_library")]
    public class ScriptBaseFunctionsLibrary
    {
        private string test = "";

        [LibraryClassProperty(Alias = "ПараметрЗапуска", Name = "LaunchParameter")]
        public string LaunchParameter
        {
            get => test;
        }


        [LibraryClassMethodAttribute(Alias = "Сообщить", Name = "Message")]
        public void Message(string text, MessageStatusEnumInner type = MessageStatusEnumInner.WithoutStatus)
        {
            Console.WriteLine(text);
        }

        [LibraryClassMethodAttribute(Alias = "ТекущаяУниверсальнаяДатаВМиллисекундах", Name = "CurrentUniversalDateInMilliseconds")]
        public IValue CurrentUniversalDateInMilliseconds()
        {
            return ValueFactory.Create(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
        }

        [LibraryClassMethodAttribute(Alias = "ИнформацияОбОшибке", Name = "ErrorInfo")]
        public IValue ErrorInfo()
        {
            return ScriptInterpreter.Interpreter.ErrorInfo;
        }

        [LibraryClassMethodAttribute(Alias = "ОписаниеОшибки", Name = "ErrorDescription")]
        public IValue ErrorDescription()
        {
            return ValueFactory.Create(ScriptInterpreter.Interpreter.ErrorInfo.Description);
        }

        [LibraryClassMethodAttribute(Alias = "ПодключитьСценарий", Name = "AttachScript")]
        public void AttachScript(string path, string type_name)
        {
            ScriptModule module = new ScriptModule(type_name, type_name, ModuleTypeEnum.OBJECT, false, path);
            ScriptInterpreter.Interpreter.Programm.AttachScript(module);
            InternalScriptType type = new InternalScriptType()
            {
                Name = type_name,
                Alias = type_name,
                Module = module,
            };
            ScriptInterpreter.Interpreter.Programm.InternalTypes.Add(type);
        }

        [LibraryClassMethodAttribute(Alias = "ТипЗнч", Name = "TypeOf")]
        public IValue TypeOf(IValue value)
        {
            InternalScriptType script_type = value.ScriptType;
            if (script_type == null)
                throw new Exception($"Тип [{value.ToString()}] не определен.");

            return ValueFactory.Create(script_type);
        }

        [LibraryClassMethodAttribute(Alias = "Тип", Name = "Type")]
        public IValue Type(IValue value)
        {
            InternalScriptType script_type = ScriptInterpreter.Interpreter.Programm.InternalTypes.Get(value.AsString());
            if (script_type == null)
                throw new Exception($"Тип [{value.ToString()}] не определен.");

            return ValueFactory.Create(script_type);
        }

        [LibraryClassMethodAttribute(Alias = "ПодробноеПредставлениеОшибки", Name = "DetailErrorDescription")]
        public IValue DetailErrorDescription(ErrorInfo error_info)
        {
            return ValueFactory.Create($"{{{error_info.ModuleName}({error_info.LineNumber})}} : " + error_info.Description + "\n" + error_info.SourceLine);
        }

        [LibraryClassMethodAttribute(Alias = "Макс", Name = "Max")]
        public IValue MaxValue(IValue[] values)
        {
            if (values.Length == 0 || values == null)
                throw new Exception("sdfsdf");

            IValue max = values[0];
            int i = 1;
            while (i < values.Length)
            {
                var current = values[i];
                if (current.CompareTo(max) > 0)
                    max = current;
                i++;
            }
            return max;
        }

        [LibraryClassMethodAttribute(Alias = "Число", Name = "Number")]
        public decimal Number(IValue value)
        {
            return value.AsNumber();
        }

        /// <summary>
        /// Текущая дата машины
        /// </summary>
        /// <returns>Дата</returns>
        [LibraryClassMethodAttribute(Alias = "ТекущаяДата", Name = "CurrentDate")]
        public DateTime CurrentDate()
        {
            return DateTime.Now;
        }

        [LibraryClassMethodAttribute(Alias = "ЗначениеЗаполнено", Name = "ValueIsFilled")]
        public bool ValueIsFilled(IValue value)
        {
            switch (value.BaseType)
            {
                case ValueTypeEnum.NULL:
                    return false;
                case ValueTypeEnum.BOOLEAN:
                    return true;
                case ValueTypeEnum.STRING:
                    return !String.IsNullOrWhiteSpace(value.AsString());
                case ValueTypeEnum.NUMBER:
                    return value.AsNumber() != 0;
                case ValueTypeEnum.DATE:
                    var emptyDate = new DateTime(1, 1, 1, 0, 0, 0);
                    return value.AsDate() != emptyDate;
                case ValueTypeEnum.SCRIPT_OBJECT:
                    if (value.AsScriptObject().Instance != null)
                    {
                        if (typeof(IUniversalCollection).IsAssignableFrom(value.AsScriptObject().Instance.GetType()))
                            return (value.AsScriptObject().Instance as IUniversalCollection).Count() > 0;
                        else
                            return false;
                    }
                    else
                        return false;
                default:
                    return true;
            }
        }


        [LibraryClassMethodAttribute(Alias = "XMLСтрока", Name = "XMLString")]
        public string XMLString(IValue value)
        {
            switch (value.BaseType)
            {
                case ValueTypeEnum.BOOLEAN:
                    return XmlConvert.ToString(value.AsBoolean());
                case ValueTypeEnum.DATE:
                    return XmlConvert.ToString(value.AsDate(), XmlDateTimeSerializationMode.Unspecified);
                case ValueTypeEnum.NUMBER:
                    return XmlConvert.ToString(value.AsNumber());
            }

            return value.AsString(); 
        }

        /// <summary>
        /// Заполняет одноименные значения свойств одного объекта из другого
        /// </summary>
        /// <param name="acceptor">Объект-приемник</param>
        /// <param name="source">Объект-источник</param>
        /// <param name="filledProperties">Заполняемые свойства (строка, через запятую)</param>
        /// <param name="ignoredProperties">Игнорируемые свойства (строка, через запятую)</param>
        [LibraryClassMethodAttribute(Alias = "ЗаполнитьЗначенияСвойств", Name = "FillPropertyValues")]
        public void FillPropertyValues(ScriptObjectContext acceptor, ScriptObjectContext source, string properties = null, string ignored_properties = null)
        {
            IEnumerable<string> sourceProperties;
            IEnumerable<string> ignoredPropCollection;

            if (properties == String.Empty)
            {
                IList<string> names = new List<string>();
                for (int i = 0; i < source.VariablesCount(); i++)
                {
                    names.Add(source.GetPublicVariable(i).Variable.Name);
                    names.Add(source.GetPublicVariable(i).Variable.Alias);
                }

                sourceProperties = names;
            }
            else
            {
                sourceProperties = properties.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0)
                    .ToArray();

            }

            if (ignored_properties != String.Empty)
            {
                ignoredPropCollection = ignored_properties.Split(',')
                    .Select(x => x.Trim())
                    .Where(x => x.Length > 0);
            }
            else
            {
                ignoredPropCollection = new string[0];
            }

            foreach (var srcProperty in sourceProperties.Where(x => !ignoredPropCollection.Contains(x)))
            {
                try
                {
                    var propIdx = acceptor.GetPublicVariable(srcProperty);
                    var srcPropIdx = source.GetPublicVariable(srcProperty);

                    propIdx.Set(srcPropIdx.Get());
                }
                catch { }
            }

        }
    }
}
