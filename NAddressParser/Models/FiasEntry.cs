#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  21.06.2020 6:25
#endregion

using System;
using Newtonsoft.Json;

namespace NAddressParser.Models
{
    /// <summary>
    /// Объект из базы ФИАС
    /// </summary>
    [JsonObject("f")]
    public class FiasEntry
    {
        /*
         SELECT offname, shortname , AOLEVEL, PARENTGUID
  FROM ADDROBJ 
  WHERE LIVESTATUS = 1
         */
        /// <summary>
        /// Имя
        /// </summary>
        [JsonProperty("n")]
        public string Name { get; set; }
        /// <summary>
        /// Короткое имя (префикс образования) 
        /// </summary>
        [JsonProperty("s")]
        public string ShortName { get; set; }
        /// <summary>
        /// Уровень вложенности
        /// </summary>
        [JsonProperty("l")]
        public int AoLevel { get; set; }
        /// <summary>
        /// Вышестоящий узел
        /// </summary>
        [JsonProperty("p")]
        public Guid? Parent { get; set; }
    }
}
