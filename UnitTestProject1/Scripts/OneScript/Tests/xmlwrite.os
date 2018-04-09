﻿///////////////////////////////////////////////////////////////////////
//
// Приемочные тесты объекта ЗаписьXML
// 
//
///////////////////////////////////////////////////////////////////////

Перем юТест;

////////////////////////////////////////////////////////////////////
// Программный интерфейс

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ЗаписатьВСтроку");
	ВсеТесты.Добавить("ТестДолжен_ЗаписатьВФайл");
	ВсеТесты.Добавить("ТестДолжен_ЗаписатьСоответствиеПространстваИмен");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьКонтекстПространствИменВоВремяЗаписи");
	
	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ЗаписатьВСтроку() Экспорт

	З = Новый ЗаписьXML;
	З.УстановитьСтроку("windows-1251");
	ЗаписатьСодержимое(З);
	С = З.Закрыть();
	НормализоватьПереводыСтрок(С);
	юТест.ПроверитьРавенство(ЭталонДокумента(), С);
	
КонецПроцедуры

Процедура ТестДолжен_ЗаписатьВФайл() Экспорт

	ВремФайл = КаталогВременныхФайлов() + "/os-xml-write-test.xml";
	
	З = Новый ЗаписьXML;
	З.ОткрытьФайл(ВремФайл, "windows-1251");
	С = "";
	
	Попытка
		ЗаписатьСодержимое(З);
		З.Закрыть();
		
		Чтение = Новый ЧтениеТекста(ВремФайл, "windows-1251");
		С = Чтение.Прочитать();
		Чтение.Закрыть();
		
	Исключение
		УдалитьВременныйФайл(ВремФайл);
		ВызватьИсключение;
	КонецПопытки;
	
	УдалитьВременныйФайл(ВремФайл);
	НормализоватьПереводыСтрок(С);
	юТест.ПроверитьРавенство(ЭталонДокумента(), С);
	
КонецПроцедуры

Функция ЭталонДокумента()
	
	Текст = 
	"<?xml version=""1.0"" encoding=""windows-1251""?>
	|<Привет xmlns:тест=""http://beaversoft.ru/oscript/test"">
	|    <Сообщение>Это текст &lt;---&gt;</Сообщение>
	|</Привет>";
	
	НормализоватьПереводыСтрок(Текст);
	
	Возврат Текст;

КонецФункции

Процедура ЗаписатьСодержимое(ЗаписьXML)
	ЗаписьXML.ЗаписатьОбъявлениеXML();
	ЗаписьXML.ЗаписатьНачалоЭлемента("Привет");
	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("тест","http://beaversoft.ru/oscript/test");
	ЗаписьXML.ЗаписатьНачалоЭлемента("Сообщение");
	ЗаписьXML.ЗаписатьТекст("Это текст <--->");
	ЗаписьXML.ЗаписатьКонецЭлемента();
	ЗаписьXML.ЗаписатьКонецЭлемента();
КонецПроцедуры

Процедура УдалитьВременныйФайл(Знач ВремФайл)
	
	Ф = Новый Файл(ВремФайл);
		
	Если Ф.Существует() Тогда
		//УдалитьФайлы(Ф.ПолноеИмя);
	КонецЕсли;
	
КонецПроцедуры

Процедура НормализоватьПереводыСтрок(Строка)
	Строка = СтрЗаменить(Строка, Символы.ВК + Символы.ПС, Символы.ПС);
КонецПроцедуры

Процедура ТестДолжен_ЗаписатьСоответствиеПространстваИмен() Экспорт
	ЗаписьXML = Новый ЗаписьXML;
	ЗаписьXML.УстановитьСтроку();
	ЗаписьXML.ЗаписатьОбъявлениеXML();
	ЗаписьXML.ЗаписатьНачалоЭлемента("opm-metadata");
	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("", "http://oscript.io/schemas/opm-metadata/1.0");
	ЗаписьXML.ЗаписатьНачалоЭлемента("test");

	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("v2", "http://oscript.io/schemas/opm-metadata/2.0");

	юТест.ПроверитьРавенство(ЗаписьXML.НайтиПрефикс("http://oscript.io/schemas/opm-metadata/2.0"), "v2", "Пространство имён 2");
	юТест.ПроверитьРавенство(ЗаписьXML.НайтиПрефикс("http://oscript.io/schemas/opm-metadata/1.0"), "", "Пространство имён по-умолчанию");
	юТест.ПроверитьРавенство(ЗаписьXML.НайтиПрефикс("http://oscript.io/schemas/opm-metadata/x.x"), Неопределено, "Не заданное пространство имён");

	ЗаписьXML.ЗаписатьКонецЭлемента();
	ЗаписьXML.ЗаписатьКонецЭлемента();
	Текст = ЗаписьXML.Закрыть();
	
	Чтение = Новый ЧтениеXML;
	Чтение.УстановитьСтроку(Текст);
	Чтение.ПерейтиКСодержимому();
	юТест.ПроверитьРавенство(Чтение.ПолучитьАтрибут("xmlns"), "http://oscript.io/schemas/opm-metadata/1.0");
	Чтение.Закрыть();
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьКонтекстПространствИменВоВремяЗаписи() Экспорт
	
	ЗаписьXML = Новый ЗаписьXML;
	ЗаписьXML.УстановитьСтроку();
	
	// Проверим, что скрипт не падает, когда ничего не начали записывать
	НеПадает = ЗаписьXML.КонтекстПространствИмен.СоответствияПространствИмен();

	ЗаписьXML.ЗаписатьНачалоЭлемента("Корень");

	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.Глубина, 1);

	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("", "Пространство");
	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("П1", "ЕщёПространство");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.ПространствоИменПоУмолчанию, "Пространство");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.НайтиURIПространстваИмен("П1"), "ЕщёПространство");

	ЗаписьXML.ЗаписатьНачалоЭлемента("Дитё");

	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.ПространствоИменПоУмолчанию, "Пространство");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.НайтиURIПространстваИмен("П1"), "ЕщёПространство");

	ЗаписьXML.ЗаписатьСоответствиеПространстваИмен("П2", "Пространство2");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.НайтиURIПространстваИмен("П2"), "Пространство2");
	
	ЗаписьXML.ЗаписатьКонецЭлемента();

	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.ПространствоИменПоУмолчанию, "Пространство");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.НайтиURIПространстваИмен("П1"), "ЕщёПространство");
	юТест.ПроверитьРавенство(ЗаписьXML.КонтекстПространствИмен.НайтиURIПространстваИмен("П2"), Неопределено);

	ЗаписьXML.ЗаписатьКонецЭлемента();

КонецПроцедуры