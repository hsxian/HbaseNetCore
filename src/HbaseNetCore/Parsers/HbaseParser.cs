using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using HbaseNetCore.Attributes;
using HbaseNetCore.Converts;
using System.Threading.Tasks;
using HbaseNetCore.Interfaces;

namespace HbaseNetCore.Parsers
{
    public class HbaseParser : IHbaseParser
    {
        public Task<List<Mutation>> ToMutationsAsync<T>(T obj) where T : class
        {
            return Task.Run(() =>
            {
                var result = new List<Mutation>();
                var type = typeof(T);

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
                    if (mut.Value.ToUTF8String() != "null")
                    {
                        result.Add(mut);
                    }
                }
                return result;
            });
        }

        public Task<T> ToRealAsync<T>(TRowResult trr) where T : class, new()
        {
            return Task.Run(() =>
            {
                var real = new T();
                var type = real.GetType();
                var fps = type.GetMembers()
                    .Where(t => t.MemberType == MemberTypes.Property || t.MemberType == MemberTypes.Field)
                    .ToList();

                var hcaType = typeof(HbaseColumnAttribute);
                var strType = typeof(string);

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
                            object v = vlaueStr;
                            if (pp.PropertyType != strType)
                            {
                                v = JsonConvert.DeserializeObject(vlaueStr, pp.PropertyType);
                            }

                            // object v = Convert.ChangeType(vlaueStr, pp.PropertyType);
                            pp.SetValue(real, v);
                        }
                        else if (fp is FieldInfo fd)
                        {
                            object v = vlaueStr;
                            if (fd.FieldType != strType)
                            {
                                v = JsonConvert.DeserializeObject(vlaueStr, fd.FieldType);
                            }
                            // object v = Convert.ChangeType(vlaueStr, fd.FieldType);
                            fd.SetValue(real, v);
                        }
                    }
                }
                return real;
            });
        }

        public async Task<BatchMutation> ToBatchMutationAsync<T>(T obj) where T : class, IHbaseTable
        {
            var result = new BatchMutation
            {
                Row = obj.GenerateRowKey().ToUTF8Bytes(),
                Mutations = await ToMutationsAsync(obj)
            };
            return result;
        }
    }
}