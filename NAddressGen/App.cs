#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 12:07
#endregion


using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NAddressGen.Abstraction;
using NAddressGen.Configuration;
using NAddressParser;
using NAddressParser.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace NAddressGen
{
    /// <summary>
    /// Хостовое приложение для импорта данных из баз данных.
    /// </summary>
    public class App
    {
        private const string BaseDir = @"..\..\..\..\NAddressParser\Data";
        private readonly IImporter _importer;
        private readonly ILogger<App> _logger;
        private readonly AppConfig _appConfig;
        /// <summary>
        /// Приложение для импорта данных
        /// </summary>
        /// <param name="importer">Импортер фиас который и запускаем для импорта</param>
        /// <param name="logger">Экземпляр логгера</param>
        /// <param name="appSettings">Настройки приложения</param>
        public App(IImporter importer, ILogger<App> logger, IOptions<AppConfig> appSettings)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _appConfig = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _importer = importer;

        }
        /// <summary>
        /// Запуск задачи импорта данных
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            await _importer.Run();
            SaveRegionListToJson(_importer.Regions.ToArray());
            SaveCityListToJson(_importer.Сities.ToArray());
            SaveSokrListToJson(_importer.Socr.ToArray());
            SaveFiasObjects(_importer.FiasEntries.ToArray());
        }

        private void SaveSokrListToJson(Socr[] socr)
        {
            var result = JsonConvert.SerializeObject(socr.ToArray());
            File.WriteAllText(Path.Combine(BaseDir, FileNames.SocrName), result);
        }

        private static void SaveCityListToJson(City[] cities)
        {
            var result = JsonConvert.SerializeObject(cities);
            File.WriteAllText(Path.Combine(BaseDir, FileNames.Cities), result);
        }

        private static void SaveRegionListToJson(Region[] regions)
        {
            var result = JsonConvert.SerializeObject(regions);
            File.WriteAllText(Path.Combine(BaseDir, FileNames.Regions), result);
        }

        private static void SaveFiasObjects(FiasEntry[] entrys)
        {
            var result = JsonConvert.SerializeObject(entrys, Formatting.Indented);
            using var zipStream = new FileStream(Path.Combine(BaseDir,FileNames.ObjectsZip), FileMode.Create);
            using var archive = new ZipArchive(zipStream, ZipArchiveMode.Update);
            var readmeEntry = archive.CreateEntry(FileNames.Objects,CompressionLevel.Optimal);
            using var writer = new StreamWriter(readmeEntry.Open());
            writer.Write(result);
        }
    }
}