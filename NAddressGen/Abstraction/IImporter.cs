#region License

// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 11:24

#endregion

using NAddressParser.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace NAddressGen.Abstraction
{
    public interface IImporter
    {
        Task Run();
        List<Region> Regions { get;  }
        List<City> Сities { get;  }
        List<Socr> Socr { get; }
        IList<FiasEntry> FiasEntries { get; }
    }
}