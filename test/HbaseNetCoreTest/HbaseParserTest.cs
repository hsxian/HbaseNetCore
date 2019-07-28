using System;
using System.Collections.Generic;
using HbaseNetCore.Attributes;
using HbaseNetCore.Converts;
using HbaseNetCore.Parsers;
using HbaseNetCoreTest.Models;
using Xunit;

namespace HbaseNetCoreTest
{
    public class HbaseParserTest
    {
        private readonly Student _student;
        public HbaseParserTest()
        {
            _student = new Student
            {
                Name = "hsx",
                Age = 5,
                isWork = true
            };
        }
        [Fact]
        public void ToMutationTest()
        {
            var mutatons = HbaseParser.ToMutations(_student);
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
            var student = HbaseParser.ToReal<Student>(tRow);

            Assert.Equal(_student.Age, student.Age);
            Assert.Equal(_student.Name, student.Name);
            Assert.Equal(_student.isWork, student.isWork);
        }
    }
}