#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 13:27
#endregion

using Newtonsoft.Json;
using System;

namespace NAddressParser.Models
{
    /// <summary>
    /// Модель регионов
    /// </summary>
    public class Region
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Название региона
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Край, область и т.д.
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Идентификатор региона
        /// </summary>
        public Guid Guid { get; set; }

        /// <summary>
        /// Полное имя расширения
        /// </summary>
        [JsonIgnore]
        public string FullName { get; set; }
    }
}