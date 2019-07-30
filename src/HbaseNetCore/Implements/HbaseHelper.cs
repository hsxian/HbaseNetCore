using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using HbaseNetCore.Attributes;
using HbaseNetCore.Interfaces;

namespace HbaseNetCore.Implements
{
    public class HbaseHelper : IHbaseHelper
    {
        public async Task<List<string>> GetTableColumnNames<T>() where T : class, new()
        {
            return await Task.Run(() =>
            {
                var type = typeof(T);
                var hcaType = typeof(HbaseColumnAttribute);
                var cols = type.GetMembers()
                    .Where(t => t.MemberType == MemberTypes.Property || t.MemberType == MemberTypes.Field)
                    .Select(t =>
                    {
                        var att = t.GetCustomAttribute(hcaType) as HbaseColumnAttribute;
                        if (att == null) return null;
                        if (string.IsNullOrWhiteSpace(att.Column))
                            return att.Column;
                        return t.Name;
                    })
                    .Where(t => t != null)
                    .ToList();
                return cols;
            });
        }

        public async Task<string> GetTableName<T>() where T : class, new()
        {
            return await Task.Run(() =>
            {
                var type = typeof(T);
                var tableAtt = type.GetCustomAttributes(true).Where(t => t is HbaseTableAttribute).FirstOrDefault() as HbaseTableAttribute;

                if (string.IsNullOrWhiteSpace(tableAtt.Table))
                    return type.Name;
                else
                    return tableAtt.Table;
            });
        }
    }
}