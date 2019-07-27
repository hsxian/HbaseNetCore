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
    }
}
