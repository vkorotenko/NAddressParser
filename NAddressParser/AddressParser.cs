#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 14:04
#endregion

using NAddressParser.Interfaces;
using NAddressParser.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

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
            LoadReplaces();
        }
        /// <summary>
        /// Инициализация с загрузкой словарей из пользовательской папки. 
        /// </summary>
        /// <param name="dictionaryPath">Путь к папке со словарями</param>
        public AddressParser(string dictionaryPath)
        {
            DictionaryPath = dictionaryPath;
            LoadDictionary();
            LoadReplaces();
        }

        private void LoadReplaces()
        {
            var data = File.ReadAllLines(Path.Combine(DictionaryPath, FileNames.Replaces));
            foreach (var s in data)
            {
                var line = s.Trim();
                if (line.StartsWith("#") || line.StartsWith("//") || !line.Contains("#;")) continue;
                var kv = line.Split(new[] { "#;" }, StringSplitOptions.None);
                Replaces.Add(kv[0].ToUpperInvariant(), kv[1]);
            }
        }

        /// <summary>
        /// Список модулей расширения парсера.
        /// </summary>
        public IList<IAddressFix> Fixers { get; private set; } = new List<IAddressFix>();
        /// <summary>
        /// Добавляет фиксер для обработки адресов. 
        /// </summary>
        /// <param name="fixer"></param>
        public void AddFixer(IAddressFix fixer)
        {
            Fixers.Add(fixer);
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
        /// Словарь перезаписей строк.
        /// </summary>
        public Dictionary<string, string> Replaces { get; private set; } = new Dictionary<string, string>();

        /// <summary>
        /// Обработка не нормализованной строки адреса.
        /// </summary>
        /// <param name="address"></param>
        public AddressResult Parse(string address)
        {
            foreach (var key in Replaces.Keys.Where(key => address.ToUpperInvariant().Contains(key)))
            {
                address = address.ReplaceIgnoreCase(key, Replaces[key]);
                break;
            }
            if (Replaces.ContainsKey(address.ToUpperInvariant()))
                address = Replaces[address];
            address = Fixers.Aggregate(address, (current, fix) => fix.Fix(current));
            return new AddressResult(this, address);
        }
    }
}
