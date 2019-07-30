using System;
using System.Collections.Generic;
using HbaseNetCore.Attributes;
using HbaseNetCore.Converts;
using HbaseNetCore.Interfaces;
using HbaseNetCore.Parsers;
using HbaseNetCoreTest.Models;
using Xunit;

namespace HbaseNetCoreTest
{
    public class HbaseParserTest
    {
        private readonly Student _student;
        private readonly IHbaseParser _HbaseParser;
        public HbaseParserTest()
        {
            _student = new Student
            {
                Name = "hsx",
                Age = 5,
                isWork = true
            };
            _HbaseParser = new HbaseParser();
        }
        [Fact]
        public void ToMutationTest()
        {
            var mutatons = _HbaseParser.ToMutationsAsync(_student).Result;
            Assert.True(mutatons.Count > 0);
        }

        [Fact]
        public void ToRealTest()
        {
            var tRow = new TRowResult
            {
                Row = 1.ToUTF8Bytes(),
                Columns = new Dictionary<byte[], TCell>
                {
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.Name)}".ToUTF8Bytes(),new TCell {Value=_student.Name.ToUTF8Bytes()}},
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.Age)}".ToUTF8Bytes(),new TCell {Value=_student.Age.ToUTF8Bytes()}},
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.isWork)}".ToUTF8Bytes(),new TCell {Value=_student.isWork.ToUTF8Bytes()}},
                }

            };
            var student = _HbaseParser.ToRealAsync<Student>(tRow).Result;

            Assert.Equal(_student.Age, student.Age);
            Assert.Equal(_student.Name, student.Name);
            Assert.Equal(_student.isWork, student.isWork);
        }
    }
}