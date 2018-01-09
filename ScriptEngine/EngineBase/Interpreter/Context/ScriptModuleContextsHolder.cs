using ScriptEngine.EngineBase.Compiler.Programm;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptEngine.EngineBase.Interpreter.Context
{
    public class ScriptModuleContextsHolder
    {
        private Stack<(int, ScriptModuleContext)> _history;
        private IDictionary<string, ScriptModuleContext> _contexts;

        public ScriptModuleContext Current { get; set; }

        public ScriptModuleContextsHolder()
        {
            _contexts = new Dictionary<string, ScriptModuleContext>();
            _history = new Stack<(int, ScriptModuleContext)>();
        }

        /// <summary>
        /// Создать контекст модуля.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public ScriptModuleContext CreateModuleContext(string name, ScriptModule module)
        {
            ScriptModuleContext context = new ScriptModuleContext(name, module);
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
        public void SetModuleContext(ScriptModuleContext context, int position)
        {
            // Сохранить текущий контекст.
            _history.Push((position, Current));
            // Установить новый.
            Current = context;
        }

        /// <summary>
        /// Восстановить предыдущий контекст модуля.
        /// </summary>
        public int RestoreModuleContext()
        {
            (int, ScriptModuleContext) context = _history.Pop();
            Current = context.Item2;

            return context.Item1;
        }
    }
}
