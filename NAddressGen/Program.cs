#region License
// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 11:58
#endregion

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NAddressGen.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;
using NAddressGen.Abstraction;
using NAddressGen.Code;

namespace NAddressGen
{
    /// <summary>
    /// Точка входа в программу
    /// </summary>
    public static class Program
    {
        private static IServiceProvider _serviceProvider;
        /// <summary>
        /// Подготавливаем сервисы и запускаем приложение
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static async Task Main(string[] args)
        {
            RegisterServices(args);
            await _serviceProvider.GetService<App>().Run();
            DisposeServices();
        }

        private static void RegisterServices(string[] args)
        {

            var services = new ServiceCollection();
            services.AddLogging(builder => builder
                .AddConsole());
            var environmentType = Environment.GetEnvironmentVariable("NETCORE_ENVIRONMENT");


            var configuration = (IConfiguration)new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{environmentType}.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddCommandLine(args)
                .Build();


            services.Configure<AppConfig>(configuration.GetSection("AppConfig"));
            services.AddTransient<IImporter, FiasImporter>();
            services.AddTransient<App>();
            _serviceProvider = services.BuildServiceProvider(true);
        }

        private static void DisposeServices()
        {
            switch (_serviceProvider)
            {
                case null:
                    return;
                case IDisposable disposable:
                    disposable.Dispose();
                    break;
            }
        }

    }
}