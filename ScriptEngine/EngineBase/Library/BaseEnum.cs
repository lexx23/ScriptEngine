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
    public class BaseEnum : SimpleArray,ILibraryModule
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
                IList<IVariable> array = new List<IVariable>();

                foreach (FieldInfo field in enum_type.GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
                {
                    EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First();

                    IValue value = ValueFactory.Create(field.FieldType, field.GetValue(null), ValueFactory.Create(attr.Value));

                    IVariable var = new Variable() { Name = field.Name, Public = true, Reference = new SimpleReference(), Value = value };
                    array.Add(var);
                    Properties.Add(var);
                    IVariable var_alias = new Variable() { Name = attr.Value, Public = true, Reference = new SimpleReference(), Value = value };
                    Properties.Add(var_alias);
                }

                Array = array.ToArray();
                array = null;
            }
        }


        public void Constructor(params IVariable[] parameters)
        {
        }
    }
}
