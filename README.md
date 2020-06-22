# NAddressParser

Оффлайн разборщик адресов. Используется для массовой проверки адресов при импорте. 
В отличие от множества сервисов не требует подключения к интернету, поэтому возможны некоторые неточности, но это лучше чем ничего :)


## Использование


```C#
var instance = new AddressParser();
var pathToCustomDictionary = @"c:\temp\data";
var customData = new AddressParser(pathToCustomDictionary);
```

## Расширение
Механизм для расширения функциональности, если вы нашли ошибку 

```C#
public class MoscowFixer : IAddressFix
{
   public MoscowFixer(){
   }
   // нормализует логику работы в соответствии с вашими правилами
   public string Fix(string source){

   }

}

var fixer = new MoscowFixer();
AddressParser.AddFixer(fixer);
```



## Изменения

20  июня 2020 - Первый релиз


