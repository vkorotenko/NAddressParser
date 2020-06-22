#region License

// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 11:45

#endregion

namespace NAddressGen.Configuration
{
    public class AppConfig
    {
        public string AppName { get; set; }
        public LoggingConfig LoggingConfig { get; set; }
        public string FiasConnection { get; set; }
    }
}