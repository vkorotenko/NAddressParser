#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  23.06.2020 8:10
#endregion


namespace NAddressParser.Interfaces
{
    /// <summary>
    /// Интерфейс для расширения функционала парсера. 
    /// </summary>
    public interface IAddressFix
    {
        /// <summary>
        /// Вызывается каждый раз при получении строки для парсинга, может быть несколько типов. 
        /// </summary>
        /// <param name="address">Адрес передаваемый из потока обработки, внимание перезапись из файла replaces.txt происходит до этого</param>
        /// <returns>Возвращает нормализованную строку для дальнейшей обработки парсером</returns>
        string Fix(string address);
    }
}
