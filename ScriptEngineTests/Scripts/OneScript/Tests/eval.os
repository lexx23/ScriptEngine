﻿Перем юТест;

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьВычислениеПростогоВыражения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьВычислениеВызоваФункции");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьВычислениеБроскаИсключения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьОператорВыполнить");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьОбращениеКЛокальнымПеременным");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьОператорВыполнитьСВыбросомИсключения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоВОператореВыполнитьЗапрещенВозврат");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьСвойствоЭтотОбъект_issue712");
	
	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ПроверитьВычислениеПростогоВыражения() Экспорт

	юТест.ПроверитьРавенство(4, Вычислить("2 + 2"));
	
	ВнешняяПеременная = 1;
	юТест.ПроверитьРавенство(1, Вычислить("ВнешняяПеременная"));

КонецПроцедуры

Функция НехорошийМетод()
	ВызватьИсключение "ААА";
КонецФункции

Функция ХорошийМетод()
	Возврат Сред("ААА",2,1);
КонецФункции

Процедура ТестДолжен_ПроверитьВычислениеВызоваФункции() Экспорт
	Текст = "";
	Для Сч = 1 По 3 Цикл
		Текст = Текст + Вычислить("ХорошийМетод()");
	КонецЦикла;
	
	юТест.ПроверитьРавенство("ААА", Текст);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьВычислениеБроскаИсключения() Экспорт
	
	Перем ОК;
	
	Попытка
		А = Вычислить("НехорошийМетод()");
	Исключение
		ТекстОшибки = ИнформацияОбОшибке().Описание;
		Сообщить("Получено исключение: " + ТекстОшибки);
		ОК = Истина;
	КонецПопытки;
	
	юТест.ПроверитьИстину(ОК, "Проверяем, что после исключения вернулись в тот же кадр стека вызовов");
		
КонецПроцедуры

Процедура ТестДолжен_ПроверитьОператорВыполнить() Экспорт
	
	ВнешнийКонтекст = "Привет";
	
	Выполнить "ВнешнийКонтекст = ""Пока""";
	
	юТест.ПроверитьРавенство("Пока", ВнешнийКонтекст);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьОбращениеКЛокальнымПеременным() Экспорт

	Массив = Новый Массив();
	Массив.Добавить(1);
	Массив.Добавить(2);
	Массив.Добавить(3);

	Результат = 0;
	
	КодДляВыполнения = "
	|Для Каждого ОчередноеНечто Из Массив Цикл
	|	Результат = Результат + ОчередноеНечто;
	|КонецЦикла;";

	Выполнить(КодДляВыполнения);
	
	юТест.ПроверитьРавенство(6, Результат);

КонецПроцедуры

Процедура ТестДолжен_ПроверитьОператорВыполнитьСВыбросомИсключения() Экспорт
	
	ВнешнийКонтекст = "Привет";
	
	Попытка
		Выполнить "ВнешнийКонтекст = ""Пока"";
		|ВызватьИсключение 123;";
	Исключение
		юТест.ПроверитьРавенство("Пока", ВнешнийКонтекст);
		юТест.ПроверитьРавенство("123", ИнформацияОбОшибке().Описание);
		Возврат;
	КонецПопытки;
	
	ВызватьИсключение "Должно было быть выдано исключение, но его не было";
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоВОператореВыполнитьЗапрещенВозврат() Экспорт
	
	ТекстМетода = "А = 1;
	|Возврат А;";
	
	Попытка
		Выполнить ТекстМетода;
	Исключение
		
		Эталон = "Процедура не может возвращать значение.";
		юТест.ПроверитьИстину(Найти(ИнформацияОбОшибке().Описание, Эталон)>0);
		Возврат;
	КонецПопытки;
	
	ВызватьИсключение "Должно было быть выдано исключение, но его не было";
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьСвойствоЭтотОбъект_issue712() Экспорт
	
	//ПутьСценарий = ОбъединитьПути(ТекущийСценарий().Каталог, "testdata", "thisObjClass.os");
	//Сценарий = ЗагрузитьСценарий(ПутьСценарий);
	//Сценарий.Идентификатор = "Это класс";
	
	//Результат = "";
	//КодВыполнения = "Результат = Сценарий.ПолучитьИдентификатор()";
	//Выполнить(КодВыполнения);
	//юТест.ПроверитьРавенство("Это класс", Результат);
	
КонецПроцедуры
