﻿Перем юТест;

Функция Версия() Экспорт
	Возврат "0.1";
КонецФункции

Функция ПолучитьСписокТестов(ЮнитТестирование) Экспорт
	
	юТест = ЮнитТестирование;
	
	ВсеТесты = Новый Массив;
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоСуществованиеФайлаНеКешируетсяПослеУдаления");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоСуществованиеФайлаНеКешируетсяПослеСоздания");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоСуществованиеКаталогаНеКешируетсяПослеСоздания");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоСуществованиеКаталогаНеКешируетсяПослеУдаления");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоДляДиректорииВозвращаетсяРодитель");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоВозвращаетсяРазмер");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоДляКаталогаВозвращаетсяВремяИзменения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьЧтоНесуществующийФайлНельзяПроверитьНаФайлКаталог");
	
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеИмени");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеПолногоИмени");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеРасширения");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеИмениБезРасширения");
	//ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеКаталогаВПолномПути");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеКаталогаВКорневомПути");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьСвойстваОбъектаИнициированногоПустойСтрокой");
	ВсеТесты.Добавить("ТестДолжен_ПроверитьПолучениеДатСозданияИИзменения");
	
	Возврат ВсеТесты;
КонецФункции

Процедура ТестДолжен_ПроверитьЧтоСуществованиеФайлаНеКешируетсяПослеУдаления() Экспорт
	
	ВремФайл = ПолучитьИмяВременногоФайла("os");
	ЗаписьТекста = Новый ЗаписьТекста(ВремФайл);
	ЗаписьТекста.ЗаписатьСтроку("---");
	ЗаписьТекста.Закрыть();
	
	Файл = Новый Файл(ВремФайл);
	ФайлФактический = Новый Файл(ВремФайл);
	
	юТест.ПроверитьИстину(Файл.Существует());
	УдалитьФайлы(Файл.ПолноеИмя);
	// проверили, что он реально удален
	юТест.ПроверитьЛожь(ФайлФактический.Существует());
	юТест.ПроверитьЛожь(Файл.Существует());
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоСуществованиеФайлаНеКешируетсяПослеСоздания() Экспорт
	
	ВремФайл = ПолучитьИмяВременногоФайла("os");
	Файл = Новый Файл(ВремФайл);
	
	юТест.ПроверитьЛожь(Файл.Существует());
	
	ЗаписьТекста = Новый ЗаписьТекста(ВремФайл);
	ЗаписьТекста.ЗаписатьСтроку("---");
	ЗаписьТекста.Закрыть();
	
	юТест.ПроверитьИстину(Файл.Существует());
	
	УдалитьФайлы(Файл.ПолноеИмя);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоСуществованиеКаталогаНеКешируетсяПослеСоздания() Экспорт
	
	ВремФайл = ПолучитьИмяВременногоФайла("os");
	
	Файл = Новый Файл(ВремФайл);
	ФайлФактический = Новый Файл(ВремФайл);
	юТест.ПроверитьЛожь(Файл.Существует(), "файл существует но не должен");
	
	СоздатьКаталог(ВремФайл);
	
	юТест.ПроверитьИстину(Файл.Существует());
	
	УдалитьФайлы(Файл.ПолноеИмя);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоСуществованиеКаталогаНеКешируетсяПослеУдаления() Экспорт
	
	ВремФайл = ПолучитьИмяВременногоФайла("os");
	
	СоздатьКаталог(ВремФайл);
	
	Файл = Новый Файл(ВремФайл);
	юТест.ПроверитьИстину(Файл.Существует());
	УдалитьФайлы(Файл.ПолноеИмя);
	юТест.ПроверитьЛожь(Файл.Существует());
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоВозвращаетсяРазмер() Экспорт
	
	ФайлТеста = Новый Файл(ТекущийСценарий().Источник);
	юТест.ПроверитьНеравенство(0, ФайлТеста.Размер());
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоНесуществующийФайлНельзяПроверитьНаФайлКаталог() Экспорт	
	
	Путь = ПолучитьИмяВременногоФайла();
	Файл = Новый Файл(Путь);

	Попытка
		Файл.ЭтоФайл();
	Исключение
		юТест.ТестПройден();
	КонецПопытки;
	
	Попытка
		Файл.ЭтоКаталог();
	Исключение
		юТест.ТестПройден();
		Возврат;
	КонецПопытки;
	
	юТест.ТестПровален("Ожидаемое исключение не возникло.");
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьЧтоДляДиректорииВозвращаетсяРодитель() Экспорт
	
	КаталогВФ = КаталогВременныхФайлов();
	ВременныйКаталог = ПолучитьИмяВременногоФайла();
	СоздатьКаталог(ВременныйКаталог);
	
	Попытка
		ФайлПроверка = Новый Файл(ВременныйКаталог);
		юТест.ПроверитьРавенство(КаталогВФ, ФайлПроверка.Путь);
		юТест.ПроверитьРавенство(ПолучитьРазделительПути(), Прав(ФайлПроверка.Путь,1));
	Исключение
		УдалитьФайлы(ВременныйКаталог);
		ВызватьИсключение;
	КонецПопытки;
	
КонецПроцедуры

Функция ТекущийСценарий()

	данные = Новый Структура("Каталог,Источник");
	данные.Каталог =  ТекущийКаталог() + ПолучитьРазделительПути() +  "Scripts" + ПолучитьРазделительПути() + "OneScript" + ПолучитьРазделительПути() + "Tests";
	данные.Источник =  данные.Каталог + ПолучитьРазделительПути() +"file-object.os";

	возврат данные;

КонецФункции


Процедура ТестДолжен_ПроверитьЧтоДляКаталогаВозвращаетсяВремяИзменения() Экспорт
	
	Каталог = Новый Файл(ТекущийСценарий().Каталог);
	
	юТест.ПроверитьИстину(Каталог.ПолучитьВремяИзменения() <> Неопределено);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПолучениеИмени() Экспорт
	
	Файл = Новый Файл(ТекущийСценарий().Источник);
	юТест.ПроверитьРавенство("file-object.os", Файл.Имя);
	
	КаталогСкрипта = Новый Файл(Файл.Путь);
	юТест.ПроверитьРавенство("Tests", КаталогСкрипта.Имя);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПолучениеИмениБезРасширения() Экспорт
	
	Файл = Новый Файл(ТекущийСценарий().Источник);
	юТест.ПроверитьРавенство("file-object", Файл.ИмяБезРасширения);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПолучениеПолногоИмени() Экспорт
	
	Файл = Новый Файл(ТекущийСценарий().Источник);
	юТест.ПроверитьРавенство(ТекущийСценарий().Источник, Файл.ПолноеИмя);
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьПолучениеРасширения() Экспорт
	
	Файл = Новый Файл(ТекущийСценарий().Источник);
	юТест.ПроверитьРавенство(".os", Файл.Расширение);
	
КонецПроцедуры


Процедура ТестДолжен_ПроверитьПолучениеКаталогаВКорневомПути() Экспорт
	
	СИ = Новый СистемнаяИнформация();
	Если Найти(СИ.ВерсияОС, "Windows") > 0 Тогда
		КореньАбсолютный = "C:\";
	Иначе
		КореньАбсолютный = "/";
	КонецЕсли;
	
	Файл = Новый Файл(КореньАбсолютный);
	юТест.ПроверитьРавенство(КореньАбсолютный, Файл.Путь, "Абсолютный корень");
	
	КореньОтносительный = "ЧтоТоГдеТо";
	Файл = Новый Файл(КореньОтносительный);
	юТест.ПроверитьРавенство("", Файл.Путь, "Относительный корень");
	
КонецПроцедуры

Процедура ТестДолжен_ПроверитьСвойстваОбъектаИнициированногоПустойСтрокой() Экспорт
	
	Ф = Новый Файл("");
	юТест.ПроверитьРавенство(Ф.Имя,	"", "Свойство Имя");
	юТест.ПроверитьРавенство(Ф.ИмяБезРасширения, "", "Свойство ИмяБезРасширения");
	юТест.ПроверитьРавенство(Ф.ПолноеИмя, "", "Свойство ПолноеИмя");
	юТест.ПроверитьРавенство(Ф.Путь, ""	,"Свойство Путь");
	юТест.ПроверитьРавенство(Ф.Расширение, "", "Свойство Расширение");
	юТест.ПроверитьЛожь(Ф.Существует(), "Файл существует");
	
КонецПроцедуры


//Функция ОпределитьДискДляФайла(Знач Путь)
//
//	Перем СИ, КорневойКаталог;
//
//	СИ = Новый СистемнаяИнформация;
//
//	ДискПроверяемогоФайла = Неопределено;
//	Для Каждого мИмяДиска  Из СИ.ИменаЛогическихДисков Цикл
//
//		мДиск = Новый ИнформацияОДиске(мИмяДиска);
//		Если СтрНачинаетсяС(Путь, мДиск.КорневойКаталог.Путь) Тогда
//			Если (ДискПроверяемогоФайла = Неопределено)
//				Или (СтрДлина(мДиск.КорневойКаталог.Путь) > СтрДлина(ДискПроверяемогоФайла.КорневойКаталог.Путь)) Тогда
//
//				ДискПроверяемогоФайла = мДиск;
//
//			КонецЕсли;
//
//		КонецЕсли;
//
//	КонецЦикла;
//
//	Возврат ДискПроверяемогоФайла;
//
//КонецФункции

Функция ФайловаяСистемаПоддерживаетВремяСоздания(Знач ПроверяемоеИмяФайла)

	Си = Новый СистемнаяИнформация;
	ЭтоWindows = Найти(Си.ВерсияОС, "Windows") > 0;
	
	Если ЭтоWindows Тогда
		Возврат Истина;
	Иначе
		Возврат Ложь;
	КонецЕсли;
//	мДиск = ОпределитьДискДляФайла(ПроверяемоеИмяФайла);
//	Если мДиск = Неопределено Тогда
//		Возврат Ложь;
//	КонецЕсли;

//	Возврат мДиск.ИмяФС = "NTFS"
//		Или мДиск.ИмяФС = "FAT32";

КонецФункции

Процедура ТестДолжен_ПроверитьПолучениеДатСозданияИИзменения() Экспорт	
	ТекущаяДата1 = ТекущаяДата();
	ВремФайл = ПолучитьИмяВременногоФайла("os");

	Если Не ФайловаяСистемаПоддерживаетВремяСоздания(ВремФайл) Тогда
		Возврат;
	КонецЕсли;

	ЗаписьТекста = Новый ЗаписьТекста(ВремФайл);
	ЗаписьТекста.ЗаписатьСтроку("---");
	ЗаписьТекста.Закрыть();	
	Файл = Новый Файл(ВремФайл);
	ВремяСоздания = Файл.ПолучитьВремяСоздания();

	юТест.ПроверитьРавенствоДатСТочностью2Секунды(ТекущаяДата1, ВремяСоздания);
	
	ф = 0;
	пока ф < 100000 Цикл
		ф = ф + 1;
	КонецЦикла;
	
	ТекущаяДата1 = ТекущаяДата();
	ЗаписьТекста.Открыть(ВремФайл);
	ЗаписьТекста.ЗаписатьСтроку("---");
	ЗаписьТекста.Закрыть();
	Файл = Новый Файл(ВремФайл);
	ВремяСоздания = Файл.ПолучитьВремяСоздания();
	ВремяИзменения = Файл.ПолучитьВремяИзменения();

	юТест.ПроверитьРавенствоДатСТочностью2Секунды(ТекущаяДата1, ВремяИзменения);
	юТест.ПроверитьМеньше(ВремяСоздания, ВремяИзменения);

	УдалитьФайлы(ВремФайл);
КонецПроцедуры
