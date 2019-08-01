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
            var sth = new Stopwatch();
            int perCount = 100000;
            int allCount = 0;

            await _clientTransport.OpenAsync();

            sth.Start();
            await DelectedTableTest();
            Console.WriteLine($"{nameof(DelectedTableTest)},time: {sth.Elapsed}");

            sth.Restart();
            await CreateTableTest();
            Console.WriteLine($"{nameof(CreateTableTest)},time: {sth.Elapsed}");



            sth.Restart();
            for (int i = 0; i < 1; i++)
            {
                await WriteWithMappingTest(allCount, perCount);
                allCount += perCount;
            }
            Console.WriteLine($"{nameof(WriteWithMappingTest)},count: {allCount}, time: {sth.Elapsed}");

            sth.Restart();
            await ReadWithMappingTest(allCount - 1);
            Console.WriteLine($"{nameof(ReadWithMappingTest)},count: 1, time: {sth.Elapsed}");

            sth.Restart();
            await ScanWithStartTest("8", allCount / 5);
            Console.WriteLine($"{nameof(ScanWithStartTest)},count: {allCount / 5}, time: {sth.Elapsed}");

            sth.Restart();
            await ScanWithStopTest("0", "3", 3 * allCount / 10);
            Console.WriteLine($"{nameof(ScanWithStopTest)},count: {3 * allCount / 10}, time: {sth.Elapsed}");

            _clientTransport.Close();
        }

        private async Task DelectedTableTest()
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            var cancel = new CancellationToken();

            var tables = await _client.getTableNamesAsync(cancel);

            if (!tables.Select(t => t.ToUTF8String()).Contains(table)) return;

            await _client.disableTableAsync(table.ToUTF8Bytes(), cancel);
            await _client.deleteTableAsync(table.ToUTF8Bytes(), cancel);
        }

        private async Task CreateTableTest()
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            var cancel = new CancellationToken();

            var tables = await _client.getTableNamesAsync(cancel);

            if (tables.Select(t => t.ToUTF8String()).Contains(table)) return;

            var colNames = await _hbaseHelper.GetTableColumnNames<Student>();

            var columnFamilies = colNames
                .Select(t => new ColumnDescriptor { Name = t.ToUTF8Bytes() })
                .ToList();

            await _client.createTableAsync(table.ToUTF8Bytes(), columnFamilies, cancel);

            tables = await _client.getTableNamesAsync(cancel);

            Assert.Contains(tables, t => t.ToUTF8String() == table);
        }
        private async Task WriteWithMappingTest(int start, int count)
        {
            var sth = new Stopwatch();
            sth.Start();

            var table = await _hbaseHelper.GetTableName<Student>();

            var cancel = new CancellationToken();

            var range = Enumerable.Range(start, count).ToList();
            var students = range
                .Select(t => new Student { RowKey = t.ToString().Reverse2String(), Name = $"hsx{t}", Age = t })
                .ToList();

            students.Last().Hobbies = new List<string> { "running", "dance" };
            students.Last().IsWork = true;
            Console.WriteLine($"\tcreate class,count:{students.Count}, time: {sth.Elapsed}");

            sth.Restart();
            var batchs = _HbaseParser.ToBatchMutations(students);
            Console.WriteLine($"\tParser Class To BatchMutation Async,count:{batchs.Count}, time: {sth.Elapsed}");

            sth.Restart();
            await _client.mutateRowsAsync(table.ToUTF8Bytes(), batchs, null, cancel);
            Console.WriteLine($"\tmutateRowsAsync,count:{batchs.Count}, time: {sth.Elapsed}");
        }

        private async Task ReadWithMappingTest(int index)
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            var cancel = new CancellationToken();

            var studentsFromHb = (await _client.getRowAsync(
                table.ToUTF8Bytes(),
                index.ToString().Reverse2String().ToUTF8Bytes(),
                null,
                cancel))
                .Select(t => _HbaseParser.ToReal<Student>(t))
                .ToList();

            Assert.True(studentsFromHb.Count > 0);
            Assert.Equal(studentsFromHb.Last().Name, $"hsx{index}");
            Assert.Equal(studentsFromHb.Last().Age, index);
            Assert.True(studentsFromHb.Last().IsWork);
            Assert.Contains(studentsFromHb.Last().Hobbies, t => t == "running");
        }
        private async Task ScanWithStartTest(string start, int expectCount)
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            start = start.Reverse2String();

            var cancel = new CancellationToken();

            var id = await _client.scannerOpenAsync(
                table.ToUTF8Bytes(),
                start.ToUTF8Bytes(),
                null,
                null,
                cancel);
            var students = new List<Student>();
            List<Student> perStudents = null;
            do
            {
                var tRows = await _client.scannerGetListAsync(id, 500, cancel);
                perStudents = _HbaseParser.ToReals<Student>(tRows);
                students.AddRange(perStudents);
            } while (perStudents?.Count > 0);

            Assert.Equal(start, students.First().RowKey);
            Assert.Equal(expectCount, students.Count);
        }
        private async Task ScanWithStopTest(string start, string end, int expectCount)
        {
            var table = await _hbaseHelper.GetTableName<Student>();

            start = start.Reverse2String();
            end = end.Reverse2String();

            var cancel = new CancellationToken();

            var id = await _client.scannerOpenWithStopAsync(
                table.ToUTF8Bytes(),
                start.ToUTF8Bytes(),
                end.ToUTF8Bytes(),
                null,
                null,
                cancel);
            var students = new List<Student>();
            List<Student> perStudents = null;
            do
            {
                var tRows = await _client.scannerGetListAsync(id, 1000, cancel);
                perStudents = _HbaseParser.ToReals<Student>(tRows);
                students.AddRange(perStudents);
            } while (perStudents?.Count > 0);

            Assert.Equal(start, students.First().RowKey);
            Assert.Equal(expectCount, students.Count);
        }
    }
}
