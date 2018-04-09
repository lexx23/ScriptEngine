﻿///////////////////////////////////////////////////////////////////////
//
// Приемочные тесты объекта ЧтениеXML
// 
//
///////////////////////////////////////////////////////////////////////

Перем юТест;

////////////////////////////////////////////////////////////////////
// Программный интерфейс

Функция Версия() Экспорт
	Возврат "0.1";
КонецФункции

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ПрочитатьЭлементыИзСтроки");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтениеПустыхЭлементов");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтениеПустыхЭлементовСАтрибутами");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтениеНепустыхЭлементовСАтрибутами");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтениеПустыхЭлементовCЗаголовкомXML");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоПропускПереходитНаКонецЭлемента");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьРаботуКонтекстаПространствИмен");
	
	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ПрочитатьЭлементыИзСтроки() Экспорт
	
	ЧтениеXML = Новый ЧтениеXML;
	ЧтениеXML.УстановитьСтроку(СтрокаXML());
	
	юТест.ПроверитьИстину(ЧтениеXML.Прочитать(),"Данные XML пусты");
	юТест.ПроверитьРавенство(ЧтениеXML.Имя, "xml");
	юТест.ПроверитьРавенство(ЧтениеXML.ТипУзла, ТипУзлаXML.НачалоЭлемента);

	ЧтениеXML.Прочитать();
	юТест.ПроверитьРавенство(ЧтениеXML.ТипУзла, ТипУзлаXML.НачалоЭлемента, "тест типа узла: НачалоЭлемента data");
	ЧтениеXML.Прочитать();
	юТест.ПроверитьРавенство(ЧтениеXML.Значение, "hello");
	ЧтениеXML.Прочитать();
	юТест.ПроверитьРавенство(ЧтениеXML.ТипУзла, ТипУзлаXML.КонецЭлемента, "тест типа узла: КонецЭлемента data");
	ЧтениеXML.Прочитать();
	юТест.ПроверитьРавенство(ЧтениеXML.ТипУзла, ТипУзлаXML.КонецЭлемента, "тест типа узла: Текст КонецЭлемента xml");
	
	ЧтениеXML.Закрыть();
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтениеПустыхЭлементов() Экспорт

	Ч = Новый ЧтениеXml();
	Ч.УстановитьСтроку("<Data />");
	
	ТипыУзлов = Новый Массив;

	Пока Ч.Прочитать() Цикл

		ТипыУзлов.Добавить(Ч.ТипУзла);
		Пока Ч.ПрочитатьАтрибут() Цикл
			ТипыУзлов.Добавить(Ч.ТипУзла);
		КонецЦикла;

	КонецЦикла;

 	юТест.ПроверитьРавенство(ТипыУзлов.Количество(), 2);
	юТест.ПроверитьРавенство(ТипыУзлов[0], ТипУзлаXML.НачалоЭлемента);
	юТест.ПроверитьРавенство(ТипыУзлов[1], ТипУзлаXML.КонецЭлемента);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтениеПустыхЭлементовСАтрибутами() Экспорт

	Ч = Новый ЧтениеXml();
	Ч.УстановитьСтроку("<Data f=""123"" d=""222"" b=""&gt;""/>");
	
	ТипыУзлов = Новый Массив;
	
	Пока Ч.Прочитать() Цикл

		ТипыУзлов.Добавить(Ч.ТипУзла);
		Пока Ч.ПрочитатьАтрибут() Цикл
			ТипыУзлов.Добавить(Ч.ТипУзла);
		КонецЦикла;

	КонецЦикла;

	юТест.ПроверитьРавенство(ТипыУзлов.Количество(),8);
	юТест.ПроверитьРавенство(ТипыУзлов[0], ТипУзлаXML.НачалоЭлемента,"0");
	юТест.ПроверитьРавенство(ТипыУзлов[1], ТипУзлаXML.Атрибут,"1");
	юТест.ПроверитьРавенство(ТипыУзлов[2], ТипУзлаXML.Атрибут,"2");
	юТест.ПроверитьРавенство(ТипыУзлов[3], ТипУзлаXML.Атрибут,"3");
	юТест.ПроверитьРавенство(ТипыУзлов[4], ТипУзлаXML.КонецЭлемента,"4");
	юТест.ПроверитьРавенство(ТипыУзлов[5], ТипУзлаXML.КонецЭлемента,"5");
	юТест.ПроверитьРавенство(ТипыУзлов[6], ТипУзлаXML.КонецЭлемента,"6");
	юТест.ПроверитьРавенство(ТипыУзлов[7], ТипУзлаXML.КонецЭлемента,"7");

	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтениеНепустыхЭлементовСАтрибутами() Экспорт

	Ч = Новый ЧтениеXml();
	Ч.УстановитьСтроку("<Data f=""123"" d=""222"" b=""&gt;""></Data>");
	
	ТипыУзлов = Новый Массив;
	
	Пока Ч.Прочитать() Цикл

		ТипыУзлов.Добавить(Ч.ТипУзла);
		Пока Ч.ПрочитатьАтрибут() Цикл
			ТипыУзлов.Добавить(Ч.ТипУзла);
		КонецЦикла;

	КонецЦикла;

	юТест.ПроверитьРавенство(ТипыУзлов.Количество(),5);
	юТест.ПроверитьРавенство(ТипыУзлов[0], ТипУзлаXML.НачалоЭлемента,"0");
	юТест.ПроверитьРавенство(ТипыУзлов[1], ТипУзлаXML.Атрибут,"1");
	юТест.ПроверитьРавенство(ТипыУзлов[2], ТипУзлаXML.Атрибут,"2");
	юТест.ПроверитьРавенство(ТипыУзлов[3], ТипУзлаXML.Атрибут,"3");
	юТест.ПроверитьРавенство(ТипыУзлов[4], ТипУзлаXML.КонецЭлемента,"4");
	
	// EvilBeaver
	// 1C переходит к атрибутам даже в конце элемента
	// В документации сказано про то, что метод ПрочитатьАтрибут работает при НачалеЭлемента
	// Вообще востребованность такого поведения сомнительна и не думаю, что стоит его эмулировать.
	// Если раскомментировать строки ниже, то тест будет соответствовать поведению 1С.
	
	// юТест.ПроверитьРавенство(ТипыУзлов[5], ТипУзлаXML.Атрибут,"5");
	// юТест.ПроверитьРавенство(ТипыУзлов[6], ТипУзлаXML.Атрибут,"6");
	// юТест.ПроверитьРавенство(ТипыУзлов[7], ТипУзлаXML.Атрибут,"7");
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтениеПустыхЭлементовCЗаголовкомXML() Экспорт

	Ч = Новый ЧтениеXml();
	Ч.УстановитьСтроку("<?xml version=""1.0"" encoding=""UTF-8"" standalone=""yes""?><Data />");
	
	ТипыУзлов = Новый Массив;
	Ч.ПерейтиКСодержимому();
	юТест.ПроверитьРавенство(ТипУзлаXML.НачалоЭлемента, Ч.ТипУзла);
	Пока Ч.Прочитать() Цикл
		ТипыУзлов.Добавить(Ч.ТипУзла);
	КонецЦикла;

	юТест.ПроверитьРавенство(ТипыУзлов.Количество(),1);
	юТест.ПроверитьРавенство(ТипыУзлов[0], ТипУзлаXML.КонецЭлемента);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоПропускПереходитНаКонецЭлемента() Экспорт

	Строка = "<build>
	|	<statusText>Step 5/6</statusText>
	|	<buildType id=""123123123""/>
	|	<tags/>
	|	<running-info percentageComplete=""97"" elapsedSeconds=""102"" estimatedTotalSeconds=""58"" currentStageText=""Step 5/6: Process exited with code 0"" outdated=""false"" probablyHanging=""false""/>
	|	<queuedDate>20141218T110711+0400</queuedDate>
	|	<startDate>20141218T110712+0400</startDate>
	|	<artifact-dependencies/>
	|</build>";

	ЧтениеXML = Новый ЧтениеXML();
	ЧтениеXML.УстановитьСтроку(Строка);
	ЧтениеXML.ПерейтиКСодержимому();

	Если ЧтениеXML.Прочитать() Тогда
		юТест.ПроверитьРавенство(ТипУзлаXML.НачалоЭлемента, ЧтениеXML.ТипУзла, "Проверка узла " + ЧтениеXML.ЛокальноеИмя);
		Пока ЧтениеXML.Прочитать() и ЧтениеXML.ЛокальноеИмя <> "artifact-dependencies" Цикл
			ЧтениеXML.Пропустить();
			юТест.ПроверитьРавенство(ТипУзлаXML.КонецЭлемента, ЧтениеXML.ТипУзла, "Проверка узла " + ЧтениеXML.ЛокальноеИмя);
		КонецЦикла;
	КонецЕсли;
	
КонецПроцедуры

Функция СтрокаXML()

	Текст = 
	"<xml>
	|	<data>hello</data>
	|</xml>";
	
	Возврат Текст;

КонецФункции

Процедура ТестДолжен_ПроверитьРаботуКонтекстаПространствИмен() Экспорт

	СтрокаСПространствами = 
	"<m:root xmlns:m='ns1'>
	|	<d2p1:child xmlns:d2p1='ns2'>
	|		<d3p1:child xmlns:d3p1='ns3' />
	|		<d3p1:child xmlns:d3p1='ns4' />
	|	</d2p1:child>
	|</m:root>";

	Чтение = Новый ЧтениеXML;
	Чтение.УстановитьСтроку(СтрокаСПространствами);
	
	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 1, "root:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено, "Пространства в root");
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено, "Пространства в root");

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 2, "child2:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено, "Пространства в child2:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено, "Пространства в child2:start");

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 3, "child3:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено, "Пространства в child3:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено, "Пространства в child3:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns3"), Неопределено, "Пространства в child3:start");

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 2, "child3:end");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено, "Пространства в child3:end");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено, "Пространства в child3:end");

	// TODO: Несовместимость поведения чтения XML в .NET (*1)
	// юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns3"), Неопределено, "Пространства в child3:end");

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 3, "child4:start");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено);
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено);
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns3"), Неопределено);
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns4"), Неопределено);
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.НайтиURIПространстваИмен("d2p1"), "ns2");
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.НайтиURIПространстваИмен("nsx"), Неопределено);

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 2, "child4:end");
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns1"), Неопределено);
	юТест.ПроверитьНеРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns2"), Неопределено);
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns3"), Неопределено);

	// TODO: Несовместимость поведения чтения XML в .NET (*1)
	// юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.URIПространствИмен().Найти("ns4"), Неопределено);

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 1, "child2:end");

	Чтение.Прочитать();
	юТест.ПроверитьРавенство(Чтение.КонтекстПространствИмен.Глубина, 0, "root:end");

	// *1) При чтении пустого элемента на конце элемента Чтение должно забыть все пространства имён,
	//установленные в начале элемента.

КонецПроцедуры