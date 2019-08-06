using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using static Hbase;
namespace HbaseNetCore.Interfaces
{
    public interface IHbaseHelper
    {
        string GetTableName<T>() where T : class, new();
        List<string> GetTableColumnNames<T>() where T : class, new();
    }
}