using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScriptBaseFunctionsExtension.Enums
{

    [ScriptExtension(AsGlobal = true, AsObject = true,Name = "СтатусСообщения", Alias = "MessageStatus")]
    public class MessageStatusEnum
    {
        private IDictionary<string, Value> _propertys;

        public MessageStatusEnum()
        {
            _propertys = new Dictionary<string, Value>();

            Value tmp_value;

            tmp_value = new Value("БезСтатуса");
            AddProperty("БезСтатуса", tmp_value);
            AddProperty("WithoutStatus", tmp_value);

            tmp_value = new Value("Важное");
            AddProperty("Важное", tmp_value);
            AddProperty("Important", tmp_value);

            tmp_value = new Value("Внимание");
            AddProperty("Внимание", tmp_value);
            AddProperty("Attention", tmp_value);

            tmp_value = new Value("Информация");
            AddProperty("Информация", tmp_value);
            AddProperty("Information", tmp_value);

            tmp_value = new Value("Обычное");
            AddProperty("Обычное", tmp_value);
            AddProperty("Ordinary", tmp_value);

            tmp_value = new Value("ОченьВажное");
            AddProperty("ОченьВажное", tmp_value);
            AddProperty("VeryImportant", tmp_value);
        }

        public Value GetProperty(string name)
        {
            return _propertys[name];
        }

        private void AddProperty(string name, Value value)
        {
            _propertys[name] = value;
        }
    }
}
