# README #

LINUX ![Build Status](https://lexx23.visualstudio.com/_apis/public/build/definitions/b433d1a6-320a-4ade-874e-b6b1510f640c/2/badge)

MAC OS ![Build Status](https://lexx23.visualstudio.com/_apis/public/build/definitions/b433d1a6-320a-4ade-874e-b6b1510f640c/3/badge)

WINDOWS ![Build Status](https://lexx23.visualstudio.com/_apis/public/build/definitions/b433d1a6-320a-4ade-874e-b6b1510f640c/1/badge)

### Цель проекта, реализация платформы 1С:Предприятие.

## Ближайшие цели:

[ ] 1. Компилятор/интерпретатор (backend C#).

      [X] Реализация всех конструкций языка.
      [ ] Поддержка многопоточности.
      
[ ] 2. Компилятор интерпретатор (frontend JavaScript).


## Пример использования:

	string _path = Directory.GetCurrentDirectory() + "\\Scripts\\OneScript\\";
	// Модули для компиляции.
	IList<ScriptModule> modules = new List<ScriptModule>()
	{
		new ScriptModule("global", "global", ModuleTypeEnum.STARTUP,true, _path + "main_module.scr"),
		new ScriptModule("testrunner", "testrunner", ModuleTypeEnum.OBJECT,true, _path + "testrunner.scr"),
		new ScriptModule("Утверждения", "Approval", ModuleTypeEnum.OBJECT, true, _path + "xunit.scr"),
		new ScriptModule("Ожидаем", "Expect", ModuleTypeEnum.OBJECT, true, _path + "bdd.scr")
	};

	ScriptCompiler compiler = new ScriptCompiler();
	// Компиляция программы.
	ScriptProgramm programm = compiler.CompileProgramm(modules);
	// Передача программы интепретатору.
	ScriptInterpreter interpreter = new ScriptInterpreter(programm);
	// Добавляю точку останова для модуля testrunner строка 358.
	interpreter.Debugger.AddBreakpoint("testrunner", 358, (interpreter1) =>
	{
		// Получить значение переменных.
		IValue val1 =  interpreter1.Debugger.Eval("ИмяКлассаТеста");
	});
	// Запуск отладки.
	interpreter.Debug();