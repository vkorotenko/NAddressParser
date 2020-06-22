#region License

// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 18:01

#endregion

namespace NAddressParser.Models
{
    /// <summary>
    /// Сокращения
    /// </summary>
    public class Socr
    {
        /// <summary>
        /// Уровень сокращения
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Поле SCNAME сокращенное название
        /// </summary>
        public string Abr{ get; set; }
        /// <summary>
        /// SOCRNAME полное имя сокращения
        /// </summary>
        public string Name { get; set; }
    }
}