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

namespace HbaseNetCoreTest
{
    public class HbaseRWTest
    {
        private readonly TClientTransport _clientTransport;
        private readonly IAsync _client;
        private readonly string _table = "student";
        private readonly List<string> _tableOfCols = new List<string> { HbaseColumnAttribute.DefaultFamily };
        public HbaseRWTest()
        {
            _clientTransport = new TSocketClientTransport(IPAddress.Loopback, 9090);
            TProtocol protocol = new TBinaryProtocol(_clientTransport);
            _client = new Hbase.Client(protocol);

        }
        [Fact]
        public async void HbaseRWAllTest()
        {
            await _clientTransport.OpenAsync();

            await CreateTableTest();
            for (int i = 0; i < 10; i++)
            {
                await WriteWithMappingTest(1000);
            }

            await ReadWithMappingTest();

            _clientTransport.Close();
        }

        private async Task CreateTableTest()
        {

            var cancel = new CancellationToken();

            var tables = await _client.getTableNamesAsync(cancel);

            if (tables.Select(t => t.ToUTF8String()).Contains(_table)) return;

            var columnFamilies = _tableOfCols
                .Select(t => new ColumnDescriptor { Name = t.ToUTF8Bytes() })
                .ToList();

            await _client.createTableAsync(_table.ToUTF8Bytes(), columnFamilies, cancel);

            tables = await _client.getTableNamesAsync(cancel);

            Assert.Contains(tables, t => t.ToUTF8String() == _table);

        }
        private string Reverse(string original)
        {
            char[] arr = original.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }
        private async Task WriteWithMappingTest(int count)
        {
            int start = 0;
            var path = "count.ini";
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
                .Select(t => new { Id = t, stu = new Student { Name = $"hsx{t}", Age = t } })
                .ToList();

            students.Last().stu.Hobbies = new List<string> { "running", "dance" };
            students.Last().stu.isWork = true;

            var batchs = students
                .Select(t => new BatchMutation
                {
                    Row = Reverse(t.Id.ToString()).ToUTF8Bytes(),
                    Mutations = HbaseParser.ToMutations(t.stu)
                })
                .ToList();

            await _client.mutateRowsAsync(_table.ToUTF8Bytes(), batchs, null, cancel);

            await File.WriteAllTextAsync(path, (start + count).ToString());
        }

        private async Task ReadWithMappingTest()
        {
            int count = 0;
            var path = "count.ini";
            if (File.Exists(path))
            {
                if (int.TryParse(await File.ReadAllTextAsync(path), out int ct))
                {
                    count = ct - 1;
                }
            }
            var cancel = new CancellationToken();

            var studentsFromHb = (await _client.getRowAsync(
                _table.ToUTF8Bytes(),
                Reverse(count.ToString()).ToUTF8Bytes(),
                null,
                cancel))
                .Select(t => HbaseParser.ToReal<Student>(t))
                .ToList();

            Assert.True(studentsFromHb.Count > 0);
            Assert.Equal(studentsFromHb.Last().Name, $"hsx{count}");
            Assert.Equal(studentsFromHb.Last().Age, count);
            Assert.True(studentsFromHb.Last().isWork);
            Assert.Contains(studentsFromHb.Last().Hobbies, t => t == "running");
        }
    }
}
