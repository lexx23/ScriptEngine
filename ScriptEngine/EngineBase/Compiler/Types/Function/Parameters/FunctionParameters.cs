using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System.Collections.Generic;
using System.Collections;

namespace ScriptEngine.EngineBase.Compiler.Types.Function.Parameters
{
    public class FunctionParameters : IEnumerable<FunctionParameter>
    {
        private IList<FunctionParameter> _vars;

        public FunctionParameter this[int index] { get => _vars[index]; set => _vars[index] = value; }

        /// <summary>
        /// Любое количество параметров. На пример у функции/процедуры из библиотеки Constructor(IValue[] parameters) может быть любое количество параметров.
        /// </summary>
        public bool AnyCount { get; set; }
        public int Count { get => _vars.Count; }

        public FunctionParameters()
        {
            _vars = new List<FunctionParameter>();
        }


        public void CreateByRef(string name,ScriptScope scope,IValue default_value = null)
        {
            IVariable var = new Variable.Variable()
            {
                Name = name,
                Scope = scope,
                Type = VariableTypeEnum.REFERENCE,
                Value = default_value
            };


            _vars.Add(new FunctionParameter(var));
        }

        public void CreateByVal(string name, ScriptScope scope, IValue default_value = null)
        {
            IVariable var = new Variable.Variable()
            {
                Name = name,
                Scope = scope,
                Type = VariableTypeEnum.CONSTANTVARIABLE,
                Value = default_value
            };


            _vars.Add(new FunctionParameter(var));
        }

        public IEnumerator<FunctionParameter> GetEnumerator()
        {
            return _vars.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
