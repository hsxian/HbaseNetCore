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
using HbaseNetCore.Converts;
using System.Linq;
using HbaseNetCore.Parsers;
using HbaseNetCore.Attributes;
using HbaseNetCoreTest.Models;
using System.IO;
using System.Threading.Tasks;
using HbaseNetCore.Interfaces;
using HbaseNetCore.Implements;
using HbaseNetCore.Utility;

namespace HbaseNetCoreTest
{
    public class HbaseRWTest
    {
        private readonly TClientTransport _clientTransport;
        private readonly IAsync _client;
        private readonly IHbaseHelper _hbaseHelper;
        private readonly IHbaseParser _HbaseParser;
        public HbaseRWTest()
        {
            _clientTransport = new TSocketClientTransport(IPAddress.Loopback, 9090);
            TProtocol protocol = new TBinaryProtocol(_clientTransport);
            _client = new Hbase.Client(protocol);
            _hbaseHelper = new HbaseHelper();
            _HbaseParser = new HbaseParser();
        }
        
        [Fact]
        public async void HbaseRWAllTest()
        {
            await _clientTransport.OpenAsync();

            await CreateTableTest();
            for (int i = 0; i < 1; i++)
            {
                await WriteWithMappingTest(100);
            }

            await ReadWithMappingTest();

            _clientTransport.Close();
        }

        private async Task CreateTableTest()
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            var cancel = new CancellationToken();

            var tables = await _client.getTableNamesAsync(cancel);

            if (tables.Select(t => t.ToUTF8String()).Contains(table)) return;

            var colNames = (await _hbaseHelper.GetTableColumnNames<Student>());

            var columnFamilies = colNames
                .Select(t => new ColumnDescriptor { Name = t.ToUTF8Bytes() })
                .ToList();

            await _client.createTableAsync(table.ToUTF8Bytes(), columnFamilies, cancel);

            tables = await _client.getTableNamesAsync(cancel);

            Assert.Contains(tables, t => t.ToUTF8String() == table);

        }
        private async Task WriteWithMappingTest(int count)
        {
            int start = 0;
            var path = "count.ini";
            var table = await _hbaseHelper.GetTableName<Student>();

            if (File.Exists(path))
            {
                if (int.TryParse(await File.ReadAllTextAsync(path), out int ct))
                {
                    start = ct;
                }
            }
            var cancel = new CancellationToken();

            var range = Enumerable.Range(start, count).ToList();
            var students = range
                .Select(t => new Student { id = t, Name = $"hsx{t}", Age = t })
                .ToList();

            students.Last().Hobbies = new List<string> { "running", "dance" };
            students.Last().isWork = true;

            var batchs = students
                .Select(t => _HbaseParser.ToBatchMutationAsync(t).Result)
                .ToList();

            await _client.mutateRowsAsync(table.ToUTF8Bytes(), batchs, null, cancel);

            await File.WriteAllTextAsync(path, (start + count).ToString());
        }

        private async Task ReadWithMappingTest()
        {
            int count = 0;
            var path = "count.ini";
            var table = await _hbaseHelper.GetTableName<Student>();

            if (File.Exists(path))
            {
                if (int.TryParse(await File.ReadAllTextAsync(path), out int ct))
                {
                    count = ct - 1;
                }
            }
            var cancel = new CancellationToken();

            var studentsFromHb = (await _client.getRowAsync(
                table.ToUTF8Bytes(),
                count.ToString().Reverse2String().ToUTF8Bytes(),
                null,
                cancel))
                .Select(t => _HbaseParser.ToRealAsync<Student>(t).Result)
                .ToList();

            Assert.True(studentsFromHb.Count > 0);
            Assert.Equal(studentsFromHb.Last().Name, $"hsx{count}");
            Assert.Equal(studentsFromHb.Last().Age, count);
            Assert.True(studentsFromHb.Last().isWork);
            Assert.Contains(studentsFromHb.Last().Hobbies, t => t == "running");
        }
    }
}
