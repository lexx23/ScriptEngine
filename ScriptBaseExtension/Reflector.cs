using ScriptBaseFunctionsLibrary.BuildInTypes.UniversalCollections;
using ScriptEngine.EngineBase.Compiler.Types.Variable.Value;
using ScriptEngine.EngineBase.Interpreter;
using ScriptEngine.EngineBase.Interpreter.Context;
using ScriptEngine.EngineBase.Library.Attributes;
using ScriptEngine.EngineBase.Library.BaseTypes;


namespace ScriptBaseFunctionsLibrary
{
    /// <summary>
    /// Рефлектор предназначен для получения метаданных объектов во время выполнения.
    /// Как правило, рефлексия используется для проверки наличия у объекта определенных свойств/методов.
    /// В OneScript рефлексию можно применять для вызова методов объектов по именам методов.
    /// </summary>
    [LibraryClassAttribute(Name = "Reflector", Alias = "Рефлектор", AsGlobal = false, RegisterType = true)]
    public class ScriptReflector : LibraryModule<ScriptReflector>
    {
        public ScriptReflector()
        {

        }

        /// <summary>
        /// Вызывает метод по его имени.
        /// </summary>
        /// <param name="target">Объект, метод которого нужно вызвать.</param>
        /// <param name="methodName">Имя метода для вызова</param>
        /// <param name="arguments">Массив аргументов, передаваемых методу. Следует учесть, что все параметры нужно передавать явно, в том числе необязательные.</param>
        /// <returns>Если вызывается функция, то возвращается ее результат. В противном случае возвращается Неопределено.</returns>
        [LibraryClassMethodAttribute(Name = "CallMethod", Alias = "ВызватьМетод")]
        public IValue CallMethod(IValue target, string methodName, ScriptArray arguments = null)
        {
            var function = target.AsScriptObject().GetContextFunction(methodName);
            IValue retValue = ValueFactory.Create();

            if(function != null)
                retValue = ScriptInterpreter.Interpreter.FunctionCall(target.AsScriptObject(), function.Function);

            return retValue;
        }

        /// <summary>
        /// Проверяет существование указанного метода у переданного объекта..
        /// </summary>
        /// <param name="target">Объект, из которого получаем таблицу методов.</param>
        /// <param name="methodName">Имя метода для вызова</param>
        /// <returns>Истину, если метод существует, и Ложь в обратном случае. </returns>
        [LibraryClassMethodAttribute(Name = "MethodExists", Alias = "МетодСуществует")]
        public bool MethodExists(IValue target, string methodName)
        {
            var function = target.AsScriptObject().GetContextFunction(methodName);
            if (function != null)
                return true;
            return false;

        }

        [LibraryClassMethodAttribute(Name = "Constructor", Alias = "Конструктор")]
        public static IValue Constructor(IValue[] parameters)
        {
            return new ScriptReflector();
        }
    }
}
