#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  21.06.2020 17:30
#endregion

using System.Globalization;

namespace NAddressParser
{
    /// <summary>
    /// Расширения для поиска
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Поиск игнорируя регистр.
        /// </summary>
        /// <param name="paragraph">Строка для поиска</param>
        /// <param name="word">Подстрока поиска</param>
        /// <returns></returns>
        public static bool ContainsIgnoreCase(this string paragraph, string word)
        {
            if (word == null) return false;
            return CultureInfo.CurrentCulture.CompareInfo.IndexOf(paragraph, word, CompareOptions.IgnoreCase) >= 0;
        }
        /// <summary>
        /// Перезапись без учета регистра
        /// </summary>
        /// <param name="src">Исходная строка</param>
        /// <param name="what">Что ищем</param>
        /// <param name="that">Чем перезаписываем</param>
        /// <returns></returns>
        public static string ReplaceIgnoreCase(this string src, string what, string that)
        {
            var start = CultureInfo.CurrentCulture.CompareInfo.IndexOf(src, what, CompareOptions.IgnoreCase);
            string result;
            if (start == 0)
            {
                result = that + src.Substring(what.Length);
            }
            else
            {
                result = src.Substring(0, start) + that + src.Substring(start + what.Length);
            }

            return result;
        }
    }
}
