using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using System;


namespace ScriptBaseFunctionsLibrary.BaseFunctions.Dates
{
    [LibraryClassAttribute(AsGlobal = true, Name = "global_dates")]
    public class ScriptDates
    {
        /// <summary>
        /// Получает текущую универсальную дату в миллисекундах (в UTC, начиная с 01.01.0001 00:00:00).
        /// </summary>
        /// <returns></returns>
        [LibraryClassMethodAttribute(Alias = "ТекущаяУниверсальнаяДатаВМиллисекундах", Name = "CurrentUniversalDateInMilliseconds")]
        public IValue CurrentUniversalDateInMilliseconds()
        {
            return ValueFactory.Create(DateTime.UtcNow.Ticks / TimeSpan.TicksPerMillisecond);
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
    }
}
