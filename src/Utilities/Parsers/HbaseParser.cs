using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Utilities.Attributes;
using Utilities.Converts;

namespace Utilities.Parsers
{
    public class HbaseParser
    {
        public static List<Mutation> ToMutations(object obj)
        {
            var result = new List<Mutation>();
            var type = obj.GetType();

            var fps = type.GetMembers()
                .Where(t => t.MemberType == MemberTypes.Property || t.MemberType == MemberTypes.Field)
                .ToList();

            var hcaType = typeof(HbaseColumnAttribute);

            foreach (var fp in fps)
            {
                var att = fp.GetCustomAttribute(hcaType) as HbaseColumnAttribute;
                if (att == null) continue;
                var family = att.Family;
                var column = att.Column;
                if (string.IsNullOrWhiteSpace(family))
                {
                    family = HbaseColumnAttribute.DefaultFamily;
                }
                if (string.IsNullOrWhiteSpace(column))
                {
                    column = fp.Name;
                }

                var mut = new Mutation
                {
                    Column = $"{family}:{column}".ToUTF8Bytes()
                };

                if (fp is PropertyInfo pp)
                {
                    mut.Value = pp.GetValue(obj).ToUTF8Bytes();
                }
                else if (fp is FieldInfo fd)
                {
                    mut.Value = fd.GetValue(obj).ToUTF8Bytes();
                }
                result.Add(mut);
            }
            return result;
        }

        public static T ToReal<T>(TRowResult trr) where T : class, new()
        {
            var real = new T();
            var type = real.GetType();
            var fps = type.GetMembers()
                .Where(t => t.MemberType == MemberTypes.Property || t.MemberType == MemberTypes.Field)
                .ToList();

            var hcaType = typeof(HbaseColumnAttribute);

            var dict = trr.Columns.ToDictionary(t => t.Key.ToUTF8String());

            foreach (var fp in fps)
            {
                var att = fp.GetCustomAttribute(hcaType) as HbaseColumnAttribute;
                if (att == null) continue;
                var family = att.Family;
                var column = att.Column;
                if (string.IsNullOrWhiteSpace(family))
                {
                    family = HbaseColumnAttribute.DefaultFamily;
                }
                if (string.IsNullOrWhiteSpace(column))
                {
                    column = fp.Name;
                }
                var familycolumn = $"{family}:{column}";
                if (dict.TryGetValue(familycolumn, out var tCell))
                {
                    var vlaueStr = tCell.Value.Value.ToUTF8String();
                    if (fp is PropertyInfo pp)
                    {
                        object v = Convert.ChangeType(vlaueStr, pp.PropertyType);
                        pp.SetValue(real, v);
                    }
                    else if (fp is FieldInfo fd)
                    {
                        object v = Convert.ChangeType(vlaueStr, fd.FieldType);
                        fd.SetValue(real, v);
                    }
                }
            }
            return real;
        }

    }
}