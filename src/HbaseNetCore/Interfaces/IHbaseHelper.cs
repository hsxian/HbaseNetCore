using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Hbase;
namespace HbaseNetCore.Interfaces
{
    public interface IHbaseHelper
    {
        Task<string> GetTableName<T>() where T : class, new();
        Task<List<string>> GetTableColumnNames<T>() where T : class, new();
    }
}