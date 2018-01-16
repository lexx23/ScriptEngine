using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using ScriptEngine.EngineBase.Compiler.Types.Function;
using ScriptEngine.EngineBase.Compiler.Types.Variable;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptProgrammContext
    {
        internal ScriptSimpleContext[] _contexts;
        internal ScriptModule _current_module;
        internal IFunction _current_function;

        public ScriptModuleContexts ModuleContextsHolder { get; }
        public ScriptFunctionContexts FunctionContextsHolder {get;}

        public ScriptModule CurrentModule { get => _current_module; }
        public IFunction CurrentFunction { get => _current_function; }

        public ScriptProgrammContext(int global_context_size)
        {
            _contexts = new ScriptSimpleContext[3];
            _contexts[0] = new ScriptSimpleContext("global", global_context_size);

            ModuleContextsHolder = new ScriptModuleContexts(this);
            FunctionContextsHolder = new ScriptFunctionContexts(this);
        }

        public void SetStartModule(ScriptModule current_module)
        {
            _current_module = current_module;
        }


        /// <summary>
        /// Получить значение из контекста выполнения.
        /// </summary>
        /// <param name="variable"></param>
        /// <returns></returns>
        public Value GetValue(IVariable variable)
        {
            if (variable.Status == VariableStatusEnum.CONSTANTVARIABLE)
                return variable.Value;

            return _contexts[variable.Scope.StackIndex].GetValue(variable.StackNumber);
        }

        /// <summary>
        /// Присвоить переменной контекста значение. Операция с значением.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void SetValue(IVariable variable, Value value)
        {
            _contexts[variable.Scope.StackIndex].SetValue(variable.StackNumber,value);
        }

        /// <summary>
        /// Копировать переменную. Операция с адресом а не с значением.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void CopyValue(IVariable variable, Value value)
        {
            _contexts[variable.Scope.StackIndex].CopyValue(variable.StackNumber, value);
        }

        /// <summary>
        /// Очистка значения, которе используется еще раз.
        /// </summary>
        /// <param name="variable"></param>
        /// <param name="value"></param>
        public void ClearValue(IVariable variable)
        {
            _contexts[variable.Scope.StackIndex].ClearValue(variable.StackNumber);
        }


        /// <summary>
        /// Установка предыдущего контекста, и предыдущей функции.
        /// </summary>
        /// <returns></returns>
        public int RestoreContext()
        {
            int position = FunctionContextsHolder.RestoreFunctionContext();
            if (position < 0)
                return ModuleContextsHolder.RestoreModuleContext();
            else
                return position;
        }
    }
}
