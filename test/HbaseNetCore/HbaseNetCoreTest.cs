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

namespace HbaseNetCore
{
    public class HbaseNetCoreTest
    {
        private readonly TClientTransport _clientTransport;
        private readonly IAsync _client;
        private readonly string _table = "student";
        private readonly List<string> _tableOfCols = new List<string> { "info" };
        public HbaseNetCoreTest()
        {
            _clientTransport = new TSocketClientTransport(IPAddress.Loopback, 9090);
            TProtocol protocol = new TBinaryProtocol(_clientTransport);
            _client = new Hbase.Client(protocol);

        }
        [Fact]
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

            Assert.True(tables.Select(t => t.ToUTF8String()).Contains(_table));

            _clientTransport.Close();
        }
        [Fact]
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
                            Column="info:name".ToUTF8Bytes(),
                            Value="好l熟悉".ToUTF8Bytes()
                        },
                        new Mutation
                        {
                            Column="info:height".ToUTF8Bytes(),
                            Value=166.ToUTF8Bytes()
                        },
                        new Mutation
                        {
                            Column="info:isWork".ToUTF8Bytes(),
                            Value=true.ToUTF8Bytes()
                        },
                    }
                }
            };

            await _client.mutateRowsAsync(_table.ToUTF8Bytes(), batch, null, cancel);

            _clientTransport.Close();
        }


        [Fact]
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
    }
}
