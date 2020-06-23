#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  21.06.2020 8:15
#endregion


using System;
using System.Collections.Generic;
using System.Linq;

namespace NAddressParser
{
    /// <summary>
    /// Результат обработки адреса
    /// </summary>
    public class AddressResult
    {
        private readonly AddressParser _parser;
        private const string CountryLabel = "Россия";
        private const int IndexLength = 6;
        private readonly List<string> _regions;
        /// <summary>
        /// Нормализация строки адреса
        /// </summary>
        /// <param name="parser">Парсер</param>
        /// <param name="address">Строка адреса для нормализации</param>
        public AddressResult(AddressParser parser, string address)
        {
            Raw = address;
            _parser = parser;
            _regions = new List<string>(_parser.Socrs.Where(l => l.Level < 4).Select(x => x.Name));
            _regions.AddRange(_parser.Socrs.Where(l => l.Level < 4).Select(x => x.Abr));

            address = address.Replace("\t", " ");
            if (string.IsNullOrWhiteSpace(address))
            {
                IsEmpty = true;
                return;
            }

            var entrees = address.Split(new[] { " ", ", " }, StringSplitOptions.None);
            foreach (var line in entrees)
            {
                address = ReplaceIndex(address, line);
                address = ReplaceCountry(address, line);
            }
            if (address.Contains(","))
            {
                entrees = address.Split(new[] { ", " }, StringSplitOptions.None);
                foreach (var s in entrees)
                {
                    address = ProcessRegion(address, s);


                    if (parser.Cities.Any(x => string.Equals(s, x.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        var city = parser.Cities.First(x => string.Equals(s, x.Name, StringComparison.OrdinalIgnoreCase));
                        var region = parser.Regions.First(x => x.Id == city.RegionId);
                        City = city.Name;
                        HasCity = true;
                        address = ReplaceSubstring(address, s);
                        var b = address.Split(new[] { ", " }, StringSplitOptions.None);
                        foreach (var s1 in b)
                        {
                            if (s1.Contains(region.Name))
                            {

                                address = ReplaceSubstring(address, s1);
                                HasRegion = true;
                                Region = region.Name;
                                break;
                            }
                        }
                    }
                }
            }
        }

        private string ProcessRegion(string address, string s)
        {
            if (HasRegion) return address;
            var orig = s;
            if (!_regions.Any(region => s.ContainsIgnoreCase(region))) return address;
            foreach (var region in _regions.Where(region => s.ContainsIgnoreCase(region)))
            {
                if (string.IsNullOrWhiteSpace(region)) continue;
                s = s.Replace(region, "").Trim().ToUpperInvariant();
                var r = _parser.Regions.First(x => x.Name.ToUpperInvariant() == s);
                RegionObj = r;
                RegionObj.FullName = _parser.Socrs.First(x => x.Abr.ToUpperInvariant() == r.Prefix.ToUpperInvariant()).Name;
                HasRegion = true;
                Region = r.Name;
                return ReplaceSubstring(address, orig);
            }
            return address;
        }

        private string ReplaceIndex(string address, string s)
        {
            if (!int.TryParse(s, out var ix) || ix.ToString().Length != IndexLength) return address;
            Index = ix;
            HasIndex = true;
            return ReplaceSubstring(address, Index.ToString());
        }

        private static string ReplaceCountry(string address, string s)
        {
            return string.Equals(s, CountryLabel, StringComparison.OrdinalIgnoreCase) ? ReplaceSubstring(address, s) : address;
        }

        /// <summary>
        /// Удаляет подстроку из адреса
        /// </summary>
        /// <param name="src"></param>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string ReplaceSubstring(string src, string entry)
        {
            if (entry.Length > src.Length) return src.Trim();
            var pos = src.IndexOf(entry, StringComparison.Ordinal);
            if (pos == 0)
                return src.Substring(entry.Length + 1).Trim();
            var start = src.Substring(0, pos);
            var end = pos + entry.Length + 1;
            if (end > src.Length)
                end--;
            return (start + src.Substring(end)).Trim();
        }

        #region Свойства

        /// <summary>
        /// Адрес является пустой строкой.
        /// </summary>
        public bool IsEmpty { get; private set; }
        #region Страна
        /// <summary>
        /// Страна, всегда равна Россия
        /// </summary>
        public string Country { get; set; } = CountryLabel;
        #endregion
        #region Почтовый индекс
        /// <summary>
        /// Почтовый индекс
        /// </summary>
        public int Index { get; private set; }
        /// <summary>
        /// Адрес содержит индекс
        /// </summary>
        public bool HasIndex { get; private set; }
        #endregion
        #region Регион
        /// <summary>
        /// Регион, может быть пустым для городов федерального уровня
        /// </summary>
        public string Region { get; private set; }

        /// <summary>
        /// Объект региона
        /// </summary>
        public Models.Region RegionObj { get; private set; }

        /// <summary>
        /// Определен регион
        /// </summary>
        public bool HasRegion { get; set; }
        #endregion
        #region Город
        /// <summary>
        /// Город
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// Есть город
        /// </summary>
        public bool HasCity { get; set; }
        #endregion
        /// <summary>
        /// Изначально переданный адрес без обработки
        /// </summary>
        public string Raw { get; set; }
        #endregion
    }
}