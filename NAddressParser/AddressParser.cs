#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 14:04
#endregion

using System;
using NAddressParser.Models;
using Newtonsoft.Json;
using System.IO;
using System.IO.Compression;

namespace NAddressParser
{
    /// <summary>
    /// Офлайн нормализатор адресов на основе справочников ФИАС.
    /// </summary>
    public class AddressParser
    {
        /// <summary>
        /// Инициализация с загрузкой словарей по умолчанию папка Data
        /// </summary>
        public AddressParser()
        {
            LoadDictionary();
        }
        /// <summary>
        /// Инициализация с загрузкой словарей из пользовательской папки. 
        /// </summary>
        /// <param name="dictionaryPath">Путь к папке со словарями</param>
        public AddressParser(string dictionaryPath)
        {
            DictionaryPath = dictionaryPath;
            LoadDictionary();
        }
        private void LoadDictionary()
        {
            var data = File.ReadAllText(Path.Combine(DictionaryPath, FileNames.Cities));
            Cities = JsonConvert.DeserializeObject<City[]>(data);

            data = File.ReadAllText(Path.Combine(DictionaryPath, FileNames.Regions));
            Regions = JsonConvert.DeserializeObject<Region[]>(data);

            data = File.ReadAllText(Path.Combine(DictionaryPath, FileNames.SocrName));
            Socrs = JsonConvert.DeserializeObject<Socr[]>(data);


            using (var archive = ZipFile.OpenRead(Path.Combine(DictionaryPath, FileNames.ObjectsZip)))
            {
                foreach (var entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(FileNames.Objects, StringComparison.OrdinalIgnoreCase))
                    {
                        using (var reader = new StreamReader(entry.Open()))
                        {
                            Entries = JsonConvert.DeserializeObject<FiasEntry[]>(reader.ReadToEnd());
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Путь к словарям библиотеки
        /// </summary>
        public string DictionaryPath { get; private set; } = "Data";
        /// <summary>
        /// Города
        /// </summary>
        public City[] Cities { get; private set; }
        /// <summary>
        /// Объекты  системы ФИАС
        /// </summary>
        public FiasEntry[] Entries { get; private set; }
        /// <summary>
        /// Регионы
        /// </summary>
        public Region[] Regions { get; private set; }
        /// <summary>
        /// Сокращения
        /// </summary>
        public Socr[] Socrs { get; private set; }

        /// <summary>
        /// Обработка не нормализованной строки адреса.
        /// </summary>
        /// <param name="address"></param>
        public AddressResult Parse(string address)
        {
            return new AddressResult(this, address);
        }
    }
}
