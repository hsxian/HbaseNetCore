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
        public List<string> GetTableColumnNames<T>() where T : class, new()
        {

            var type = typeof(T);
            var hcaType = typeof(HbaseColumnAttribute);
            var cols = type.GetMembers()
                .Where(t => t.MemberType == MemberTypes.Property)
                .Select(t =>
                {
                    var att = t.GetCustomAttribute(hcaType) as HbaseColumnAttribute;
                    if (att == null) return null;
                    if (!string.IsNullOrWhiteSpace(att.Column))
                        return att.Column;
                    return HbaseColumnAttribute.DefaultFamily;
                })
                .Where(t => t != null)
                .Distinct()
                .ToList();
            return cols;
        }

        public string GetTableName<T>() where T : class, new()
        {
            var type = typeof(T);
            var tableAtt = type.GetCustomAttributes(true).Where(t => t is HbaseTableAttribute).FirstOrDefault() as HbaseTableAttribute;

            if (string.IsNullOrWhiteSpace(tableAtt.Table))
                return type.Name;
            else
                return tableAtt.Table;
        }
    }
}