#region License

// // Разработано: Коротенко Владимиром Николаевичем (Vladimir N. Korotenko)
// // email: koroten@ya.ru
// // skype:vladimir-korotenko
// // https://vkorotenko.ru
// // Создано:  20.06.2020 9:51

#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using NAddressGen.Abstraction;
using NAddressGen.Configuration;
using NAddressParser.Models;

namespace NAddressGen.Code
{
    /// <summary>
    ///     Импортер из базы ФИАС
    /// </summary>
    public class FiasImporter : IImporter
    {

        #region SqlCommand

        private const string SokrSql = @"SELECT [LEVEL]
                                         ,[SCNAME]
                                         ,[SOCRNAME]      
                                        FROM [SOCRBASE]";

        private const string FiasObjSql = @"SELECT offname, shortname , AOLEVEL, PARENTGUID
  FROM ADDROBJ 
  WHERE LIVESTATUS = 1";
        #endregion

        private readonly AppConfig _appConfig;
        private readonly string _connectionStr;
        private List<Parent> _parent;

        /// <summary>
        ///     Импортер из базы ФИАС
        /// </summary>
        /// <param name="appSettings">Настройки приложения</param>
        public FiasImporter(IOptions<AppConfig> appSettings)
        {
            _appConfig = appSettings?.Value ?? throw new ArgumentNullException(nameof(appSettings));
            _connectionStr = _appConfig.FiasConnection;
        }

        /// <summary>
        ///     Список регионов
        /// </summary>
        public List<Region> Regions { get; private set; }

        /// <summary>
        ///     Список городов
        /// </summary>
        public List<City> Сities { get; private set; }

        /// <summary>
        ///     Список сокращений
        /// </summary>
        public List<Socr> Socr { get; private set; }
        /// <summary>
        /// Элементы ФИАС
        /// </summary>
        public IList<FiasEntry> FiasEntries { get; private set; } 

        /// <summary>
        ///     Запуск парсинга
        /// </summary>
        /// <returns></returns>
        public async Task Run()
        {
            Regions = new List<Region>();
            Сities = new List<City>();
            Socr = new List<Socr>();
            FiasEntries = new List<FiasEntry>();
            _parent = new List<Parent>();
            await FillRegion();
            await FillCity();
            await FillSocr();
            await FillFiasObj();
        }

        private async Task FillFiasObj()
        {
            await using var connection = new SqlConnection(_connectionStr);
            var command = new SqlCommand(FiasObjSql, connection);
            await command.Connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ReadSingleFiasEntry(reader);
            await reader.CloseAsync();
            await command.ExecuteNonQueryAsync();
        }
        private void ReadSingleFiasEntry(IDataRecord record)
        {
            var entry = new FiasEntry();
            // offname, shortname , AOLEVEL, PARENTGUID
            try
            {
                entry.Name = record.GetString(0);
                entry.ShortName = record.GetString(1);
                entry.AoLevel = (int)record.GetInt64(2);
                entry.Parent = record.GetGuid(3);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            FiasEntries.Add(entry);
        }
        private async Task FillSocr()
        {
            await using var connection = new SqlConnection(_connectionStr);
            var command = new SqlCommand(SokrSql, connection);
            await command.Connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ReadSingleSocr(reader);
            await reader.CloseAsync();
            await command.ExecuteNonQueryAsync();
        }

        private void ReadSingleSocr(IDataRecord record)
        {
            var soc = new Socr();
            try
            {
                soc.Level = record.GetInt32(0);
                soc.Name = record.GetString(2);
                soc.Abr = null;
                soc.Abr = record.GetString(1);
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }
            Socr.Add(soc);
        }

        #region Методы класса
        private async Task FillCity()
        {
            const string sql = @"SELECT  [AOGUID]   
                               ,[REGIONCODE]      
                               ,[OFFNAME]
                               ,[POSTALCODE]      
	                           ,[SHORTNAME]
                               ,[PARENTGUID]
                               ,[AREACODE]
	                           ,[CITYCODE]
                      FROM [ADDROBJ]
                      WHERE  [SHORTNAME] = N'г' AND actstatus = 1   ORDER BY REGIONCODE";

            await using var connection = new SqlConnection(_connectionStr);
            var command = new SqlCommand(sql, connection);
            await command.Connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) await ReadSingleCity(reader);
            await reader.CloseAsync();
            await command.ExecuteNonQueryAsync();
        }

        private async Task ReadSingleCity(IDataRecord record)
        {
            var aoGuid = record.GetGuid(record.GetOrdinal("AOGUID"));
            var code = record.GetInt32(record.GetOrdinal("REGIONCODE"));
            var offName = record.GetString(record.GetOrdinal("OFFNAME"));
            var prefix = record.GetString(record.GetOrdinal("SHORTNAME"));
            var cityCode = record.GetInt32(record.GetOrdinal("CITYCODE"));
            var areaCode = record.GetInt32(record.GetOrdinal("AREACODE"));
            var id = int.Parse($"{code}{areaCode}{cityCode}");

            var city = new City
            {
                Id = id,
                Name = offName,
                Prefix = $"{prefix}.",
                Guid = aoGuid,
                RegionId = code
            };
            var parents = record.GetOrdinal("PARENTGUID");
            if (!record.IsDBNull(parents))
            {
                var parentGuid = record.GetGuid(parents);
                city.Parent = await GetParent(parentGuid);
            }

            Сities.Add(city);
        }

        private async Task<string> GetParent(Guid parent)
        {
            var par = Regions.FirstOrDefault(x => x.Guid == parent);
            if (par != null) return $"{par.Name} {par.Prefix} ";

            var cachedParent = await TryGetFromCache(parent);
            var stout = $"{cachedParent.OffName} {cachedParent.ShortName}.";

            if (cachedParent.ParentGuid.HasValue)
            {
                var p1 = await TryGetFromCache(cachedParent.ParentGuid.Value);
                stout = $"{p1.OffName} {p1.ShortName}., {stout}";

                if (p1.ParentGuid.HasValue)
                {
                    var p2 = await TryGetFromCache(p1.ParentGuid.Value);
                    stout = $"{p2.OffName} {p2.ShortName}., {stout}";
                }
            }

            return stout;
        }

        private async Task<Parent> TryGetFromCache(Guid parent)
        {
            Parent p0;
            var pp = _parent.FirstOrDefault(x => x.AoGuid == parent);
            if (pp != null)
            {
                p0 = pp;
            }
            else
            {
                p0 = await GetParentItem(parent);
                _parent.Add(p0);
            }

            return p0;
        }

        private async Task FillRegion()
        {
            const string sql = @"SELECT [AOGUID]   
                              ,[REGIONCODE]      
                              ,[OFFNAME]
                              ,[SHORTNAME]      
                              FROM [ADDROBJ]
                               WHERE AOLEVEL = 1 AND ACTSTATUS = 1 ORDER BY REGIONCODE";

            await using var connection = new SqlConnection(_connectionStr);
            var command = new SqlCommand(sql, connection);
            await command.Connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync()) ReadSingleRegion(reader);
            await reader.CloseAsync();
            await command.ExecuteNonQueryAsync();
        }

        private void ReadSingleRegion(IDataRecord record)
        {
            var aoGuid = record.GetGuid(0);
            var code = record.GetInt32(1);
            var offName = record.GetString(2);
            var prefix = record.GetString(3);

            var region = new Region
            {
                Id = code,
                Name = offName,
                Prefix = $"{prefix}.",
                Guid = aoGuid
            };
            Regions.Add(region);
        }

        private async Task<Parent> GetParentItem(Guid parent)
        {
            var sql = @"select 
                        aoguid, 
                        offname, 
                        parentguid, 
                        shortname 
	                   FROM [ADDROBJ]

                       WHERE actstatus=1 and  aoguid = @Parent";


            await using var connection = new SqlConnection(_connectionStr);
            var command = new SqlCommand(sql, connection);
            var param = new SqlParameter("@Parent", SqlDbType.UniqueIdentifier)
            {
                Value = parent
            };
            command.Parameters.Add(param);
            await command.Connection.OpenAsync();

            var reader = await command.ExecuteReaderAsync();
            await reader.ReadAsync();
            var record = (IDataRecord)reader;

            var aoGuid = record.GetGuid(record.GetOrdinal("AOGUID"));
            var offName = record.GetString(record.GetOrdinal("OFFNAME"));
            var shortName = record.GetString(record.GetOrdinal("SHORTNAME"));
            var ordinal = record.GetOrdinal("PARENTGUID");
            Guid? parentGuid = null;
            if (!record.IsDBNull(ordinal)) parentGuid = record.GetGuid(ordinal);

            var result = new Parent
            {
                AoGuid = aoGuid,
                OffName = offName,
                ParentGuid = parentGuid,
                ShortName = shortName
            };
            await reader.CloseAsync();
            return result;
        }
        #endregion
    }
}