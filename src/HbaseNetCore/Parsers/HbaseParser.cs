using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using HbaseNetCore.Attributes;
using HbaseNetCore.Converts;
using System.Threading.Tasks;
using HbaseNetCore.Interfaces;
using System.Collections.Concurrent;
using HbaseNetCore.Utility;

namespace HbaseNetCore.Parsers
{
    public class HbaseParser : IHbaseParser
    {

        public Dictionary<string, string> GetPropertyNameWithFamilyStr<T>() where T : class
        {
            var hcaType = typeof(HbaseColumnAttribute);

            var properties = typeof(T).GetProperties()
                            .ToList();

            var result = new Dictionary<string, string>();

            foreach (var property in properties)
            {
                var att = property.GetCustomAttribute(hcaType) as HbaseColumnAttribute;
                if (att == null) continue;
                var family = att.Family;
                var column = att.Column;
                if (string.IsNullOrWhiteSpace(family))
                {
                    family = HbaseColumnAttribute.DefaultFamily;
                }
                if (string.IsNullOrWhiteSpace(column))
                {
                    column = property.Name;
                }
                result.Add(property.Name, $"{family}:{column}");
            }
            return result;
        }
        public Dictionary<string, byte[]> GetPropertyNameWithFamilyBytes<T>() where T : class
        {
            var result = new Dictionary<string, byte[]>();
            var strDic = GetPropertyNameWithFamilyStr<T>();
            foreach (var item in strDic)
            {
                result.Add(item.Key, item.Value.ToBytes());
            }
            return result;
        }
        public List<Mutation> ToMutations<T>(T obj) where T : class
        {
            var result = new List<Mutation>();

            var nameWithFamily = GetPropertyNameWithFamilyBytes<T>();
            var properties = typeof(T).GetProperties()
                .Where(t => nameWithFamily.ContainsKey(t.Name))
                .ToList();

            foreach (var property in properties)
            {
                var v = property.GetValue(obj);
                if (v == null) continue;
                var mut = new Mutation
                {
                    Column = nameWithFamily[property.Name],
                    Value = v.ToBytes()
                };
                result.Add(mut);
            }
            return result;
        }

        public BatchMutation ToBatchMutation<T>(T obj) where T : class, IHbaseTable
        {
            var result = new BatchMutation
            {
                Row = obj.RowKey.ToBytes(),
                Mutations = ToMutations(obj)
            };
            return result;
        }

        public T ToReal<T>(TRowResult trr) where T : class, IHbaseTable, new()
        {
            var real = new T
            {
                RowKey = trr.Row.ToObject<string>()
            };

            var nameWithFamily = GetPropertyNameWithFamilyStr<T>();
            var properties = typeof(T).GetProperties()
                .Where(t => nameWithFamily.ContainsKey(t.Name))
                .ToList();

            var dict = trr.Columns.ToDictionary(t => t.Key.ToObject<string>());

            foreach (var property in properties)
            {
                if (dict.TryGetValue(nameWithFamily[property.Name], out var tCell))
                {
                    object v = tCell.Value.Value.ToObject(property.PropertyType);
                    property.SetValue(real, v);
                }
            }
            return real;
        }

        public List<BatchMutation> ToBatchMutations<T>(IEnumerable<T> objs) where T : class, IHbaseTable
        {
            var nameWithFamily = GetPropertyNameWithFamilyBytes<T>();
            var properties = typeof(T).GetProperties()
                .Where(t => nameWithFamily.ContainsKey(t.Name))
                .ToList();

            var result = new List<BatchMutation>();

            foreach (var obj in objs)
            {
                var batch = new BatchMutation
                {
                    Row = obj.RowKey.ToBytes(),
                    Mutations = new List<Mutation>()
                };
                foreach (var property in properties)
                {
                    var v = property.GetValue(obj);
                    if (v == null) continue;
                    var mut = new Mutation
                    {
                        Column = nameWithFamily[property.Name],
                        Value = v.ToBytes()
                    };
                    batch.Mutations.Add(mut);
                }
                result.Add(batch);
            }
            return result;
        }

        public List<T> ToReals<T>(IEnumerable<TRowResult> trrs) where T : class, IHbaseTable, new()
        {

            var nameWithFamily = GetPropertyNameWithFamilyStr<T>();
            var properties = typeof(T).GetProperties()
                .Where(t => nameWithFamily.ContainsKey(t.Name))
                .ToList();

            var result = new List<T>();
            foreach (var trr in trrs)
            {
                var real = new T
                {
                    RowKey = trr.Row.ToObject<string>()
                };
                var dict = trr.Columns.ToDictionary(t => t.Key.ToObject<string>());
                foreach (var property in properties)
                {
                    if (dict.TryGetValue(nameWithFamily[property.Name], out var tCell))
                    {
                        object v = tCell.Value.Value.ToObject(property.PropertyType);
                        property.SetValue(real, v);
                    }
                }
                result.Add(real);
            }
            return result;
        }
    }
}