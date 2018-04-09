using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Library.BaseTypes.UniversalCollections;

namespace ScriptEngine.EngineBase.Library
{
    public class BaseEnum<T> : IScriptDynamicProperties,IEnumerable<IValue> where T : struct, IConvertible
    {
        public IList<IVariable> Properties { get; set; }

        public BaseEnum()
        {
            Properties = new List<IVariable>();
        }

        public void Create()
        {
            foreach (FieldInfo field in typeof(T).GetFields())
            {
                if (field.FieldType == typeof(Int32))
                    continue;

                EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().FirstOrDefault();
                string alias = string.Empty;
                if (attr != null)
                    alias = attr.Value;

                IValue value = ValueFactory.Create(field.GetValue(null));

                IVariable var = new Variable() { Name = field.Name, Alias = alias, Public = true, Reference = new ReferenceReadOnly(value) };
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

        private IValue Find(string name)
        {
            for (int i = 0; i < Properties.Count; i++)
                if (String.Equals(Properties[i].Name, name, StringComparison.OrdinalIgnoreCase) || String.Equals(Properties[i].Alias, name, StringComparison.OrdinalIgnoreCase))
                    return Properties[i].Value;
            return null;
        }

        public bool Exist(string name)
        {
            return Find(name) != null;
        }

        public IValue Get(string name)
        {
            return Find(name);
        }

        public void Set(string name, IValue value)
        {
            throw new Exception("Свойство только для чтения.");
        }
    }
}
