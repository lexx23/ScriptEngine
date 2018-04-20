using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;
using System;

namespace ScriptBaseFunctionsLibrary.BaseFunctions.Numbers
{
    [LibraryClassAttribute(AsGlobal = true, Name = "global_numbers")]
    public class ScriptNumbers
    {
        /// <summary>
        /// Определяет максимальное значение из полученных параметров.
        /// </summary>
        /// <param name="values"></param>
        /// <returns></returns>
        [LibraryClassMethodAttribute(Alias = "Макс", Name = "Max")]
        public IValue MaxValue(IValue[] values)
        {
            if (values.Length == 0 || values == null)
                throw new Exception("Не верные параметры.");

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

    }
}
