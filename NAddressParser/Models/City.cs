#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 18:02
#endregion

using System;

namespace NAddressParser.Models
{
    /// <summary>
    /// Город
    /// </summary>
    public class City
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Регион к которому принадлежит город
        /// </summary>
        public int RegionId { get; set; }
        /// <summary>
        /// Имя города
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Longitude
        /// </summary>
        public long Lon { get; set; }

        /// <summary>
        /// Lattitude
        /// </summary>
        public long Lat { get; set; }

        
        /// <summary>
        /// Префикс образования, например район или область
        /// </summary>
        public string Prefix { get; set; }
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Guid { get; set; }
        /// <summary>
        /// Вышестоящий объект, используется для вывода иерархии
        /// </summary>
        public string Parent { get; set; }
    }
}