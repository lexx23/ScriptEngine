using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;
using System.Collections;

namespace ScriptEngine.EngineBase.Library
{
    public class BaseEnum<T> : IEnumerable<IValue> where T : struct, IConvertible
    {
        public IList<IVariable> Properties { get; set; }

        public BaseEnum()
        {
            Properties = new List<IVariable>();
            foreach (FieldInfo field in typeof(T).GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
            {
                EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First();

                IValue value = ValueFactory.Create(attr.Value);

                IVariable var = new Variable() { Name = field.Name, Alias = attr.Value, Public = true, Reference = new ReferenceReadOnly(value) };
                Properties.Add(var);
            }

        }

        public IEnumerator<IValue> GetEnumerator()
        {
            int i = 0;
            while (i < Properties.Count)
            {
                yield return Properties[i++].Value;
            }
            yield break;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
