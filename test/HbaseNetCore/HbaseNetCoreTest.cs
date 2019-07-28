using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Threading;
using Thrift;
using Thrift.Protocols;
using Thrift.Transports;
using Thrift.Transports.Client;
using Xunit;
using static Hbase;
using Utilities.Converts;
using System.Linq;
using Utilities.Parsers;
using Utilities.Attributes;

namespace HbaseNetCore
{
    public class HbaseNetCoreTest
    {
        private readonly TClientTransport _clientTransport;
        private readonly IAsync _client;
        private readonly string _table = "student";
        private readonly List<string> _tableOfCols = new List<string> { HbaseColumnAttribute.DefaultFamily };
        public HbaseNetCoreTest()
        {
            _clientTransport = new TSocketClientTransport(IPAddress.Loopback, 9090);
            TProtocol protocol = new TBinaryProtocol(_clientTransport);
            _client = new Hbase.Client(protocol);

        }
        [Fact(DisplayName = "1.创建表")]
        public async void CreateTableTest()
        {
            await _clientTransport.OpenAsync();

            var cancel = new CancellationToken();

            var tables = await _client.getTableNamesAsync(cancel);

            if (tables.Select(t => t.ToUTF8String()).Contains(_table)) return;

            var columnFamilies = _tableOfCols
                .Select(t => new ColumnDescriptor { Name = t.ToUTF8Bytes() })
                .ToList();

            await _client.createTableAsync(_table.ToUTF8Bytes(), columnFamilies, cancel);

            tables = await _client.getTableNamesAsync(cancel);

            Assert.Contains(tables, t => t.ToUTF8String() == _table);

            _clientTransport.Close();
        }
        [Fact(DisplayName = "2.写入数据")]
        public async void WriteDataTest()
        {
            await _clientTransport.OpenAsync();

            var cancel = new CancellationToken();

            var batch = new List<BatchMutation>
            {
                new BatchMutation
                {
                    Row=12.ToUTF8Bytes(),
                    Mutations=new List<Mutation>
                    {
                        new Mutation
                        {
                            Column=$"{HbaseColumnAttribute.DefaultFamily}:name".ToUTF8Bytes(),
                            Value="好l熟悉".ToUTF8Bytes()
                        },
                        new Mutation
                        {
                            Column=$"{HbaseColumnAttribute.DefaultFamily}:height".ToUTF8Bytes(),
                            Value=166.ToUTF8Bytes()
                        },
                        new Mutation
                        {
                            Column=$"{HbaseColumnAttribute.DefaultFamily}:isWork".ToUTF8Bytes(),
                            Value=true.ToUTF8Bytes()
                        },
                    }
                }
            };

            await _client.mutateRowsAsync(_table.ToUTF8Bytes(), batch, null, cancel);

            _clientTransport.Close();
        }


        [Fact(DisplayName = "3.读取数据")]
        public async void ReadDataTest()
        {
            await _clientTransport.OpenAsync();

            var cancel = new CancellationToken();

            var rows = await _client.getRowsAsync(_table.ToUTF8Bytes(), new List<byte[]> { 12.ToUTF8Bytes() }, null, cancel);
            foreach (TRowResult row in rows)
            {
                Debug.WriteLine($"Row:{row.Row.ToUTF8String()}");
                foreach (var dict in row.Columns)
                {
                    Debug.WriteLine($"\tColumn:{dict.Key.ToUTF8String()}, Value:{dict.Value.Value.ToUTF8String()}");
                }
            }
            _clientTransport.Close();
        }

        [Fact(DisplayName = "4.映射方式读写数据")]
        public async void WriteReadWithMappingTest()
        {
            await _clientTransport.OpenAsync();

            var cancel = new CancellationToken();

            var range = Enumerable.Range(0, 100).ToList();
            var batchs = range
                .Select(t => new BatchMutation
                {
                    Row = t.ToUTF8Bytes(),
                    Mutations = HbaseParser.ToMutations(new Student { Name = $"hsx{t}", Age = t })
                })
                .ToList()
                ;

            await _client.mutateRowsAsync(_table.ToUTF8Bytes(), batchs, null, cancel);

            var students = (await _client.getRowsAsync(
                _table.ToUTF8Bytes(),
                range.Select(t => t.ToUTF8Bytes()).ToList(),
                null,
                cancel))
                .Select(t => HbaseParser.ToReal<Student>(t))
                .ToList();

            Assert.Equal(students.Count, range.Count());
            Assert.Equal(students.Last().Name, $"hsx{range.Last()}");
            Assert.Equal(students.Last().Age, range.Last());

            _clientTransport.Close();
        }
    }
}
