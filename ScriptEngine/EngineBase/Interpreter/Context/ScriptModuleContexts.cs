using ScriptEngine.EngineBase.Compiler.Programm.Parts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptModuleContexts
    {
        private ScriptProgrammContext _main_context;
        private Stack<(int, ObjectContext)> _history;
        private IDictionary<string, ObjectContext> _contexts;

        public ScriptModuleContexts(ScriptProgrammContext main_context)
        {
            _main_context = main_context;
            _contexts = new Dictionary<string, ObjectContext>();
            _history = new Stack<(int, ObjectContext)>();
        }

        /// <summary>
        /// Создать контекст модуля.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ObjectContext CreateModuleContext(string name, ScriptModule module)
        {
            ObjectContext context = new ObjectContext(module,new ScriptSimpleContext(name, module.ModuleScope.VarCount));
            _contexts.Add(name, context);
            return context;
        }

        /// <summary>
        /// Удалить контекст.
        /// </summary>
        /// <param name="name"></param>
        public void DeleteModuleContext(string name)
        {
            _contexts.Remove(name);
        }

        public bool ExistModuleContext(string name)
        {
            return _contexts.ContainsKey(name);
        }

        /// <summary>
        /// Установить текуший контест модуля, по имени котекста.
        /// </summary>
        /// <param name="name"></param>
        public void SetModuleContext(string name, int position)
        {
            SetModuleContext(_contexts[name], position);
        }

        /// <summary>
        /// Установить текуший контест модуля, используя контекст.
        /// </summary>
        /// <param name="context"></param>
        public void SetModuleContext(ObjectContext context, int position)
        {
            // Сохранить текущий контекст.
            _history.Push((position, new ObjectContext(_main_context.CurrentModule, _main_context._contexts[1])));

            // Установить новый.
            _main_context._contexts[1] = context.Context;
            _main_context._current_module = context.Module;
        }

        /// <summary>
        /// Восстановить предыдущий контекст модуля.
        /// </summary>
        public int RestoreModuleContext()
        {
            (int, ObjectContext) context = _history.Pop();
            _main_context._contexts[1] = context.Item2.Context;
            _main_context._current_module = context.Item2.Module;

            return context.Item1;
        }
    }
}
