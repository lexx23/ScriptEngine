using ScriptEngine.EngineBase.Compiler.Types.Variable.References;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System;


namespace ScriptEngine.EngineBase.Library
{
    public class BaseEnum
    {
        public IList<IVariable> Properties { get; set; }

        public BaseEnum()
        {
            Properties = new List<IVariable>();
        }

        public BaseEnum(Type enum_type) : this()
        {
            if (enum_type.IsEnum)
            {
                foreach (FieldInfo field in enum_type.GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
                {
                    EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First();

                    IValue value = ValueFactory.Create(attr.Value);

                    IVariable var = new Variable() { Name = field.Name,Alias = attr.Value, Public = true, Reference = new ReferenceReadOnly(value) };
                    Properties.Add(var);
                }
            }
        }
    }
}
