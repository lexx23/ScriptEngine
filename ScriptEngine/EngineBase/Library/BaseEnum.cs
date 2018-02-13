using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Parser.Token;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using System.Linq;
using System;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Library.Attributes;

namespace ScriptEngine.EngineBase.Library
{
    public class BaseEnum : IEnumerable<IVariable>
    {
        private IList<IVariable> _property_holder;

        public BaseEnum()
        {
            _property_holder = new List<IVariable>();
        }

        public BaseEnum(Type enum_type) : this()
        {
            if (enum_type.IsEnum)
            {
                foreach (FieldInfo field in enum_type.GetFields().Where(x => x.GetCustomAttributes(typeof(EnumStringAttribute), false).Length > 0))
                {
                    EnumStringAttribute attr = field.GetCustomAttributes<EnumStringAttribute>().First<EnumStringAttribute>();

                    //IValue value = new Value(field.Name)
                    //{
                    //    ReadOnly = true
                    //};
                    //value.SetValue(field.GetValue(null));

                    //IVariable var = new Variable() { Name = field.Name, Value = value, Public = true,Type = VariableTypeEnum.CONSTANTVARIABLE };
                    //AddProperty(var);
                    //IVariable var_alias = new Variable() { Name = attr.Value, Value = value, Public = true, Type = VariableTypeEnum.CONSTANTVARIABLE };
                    //AddProperty(var_alias);
                }
            }
        }


        public bool GetPropertyByName(string name, out IVariable variable)
        {
            foreach (IVariable var in _property_holder.AsParallel().Where(x => x.Name == name))
            {
                if (var != null)
                {
                    variable = var;
                    return true;
                }
            }

            variable = null;
            return false;
        }

        public void AddProperty(IVariable value)
        {
            _property_holder.Add(value);
        }


        public IEnumerator<IVariable> GetEnumerator()
        {
            return _property_holder.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
