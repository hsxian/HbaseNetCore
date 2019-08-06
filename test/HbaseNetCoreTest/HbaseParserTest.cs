using System;
using System.Collections.Generic;
using HbaseNetCore.Attributes;
using HbaseNetCore.Converts;
using HbaseNetCore.Interfaces;
using HbaseNetCore.Parsers;
using HbaseNetCoreTest.Models;
using Xunit;
using HbaseNetCore.Utility;

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
                IsWork = true
            };
            _HbaseParser = new HbaseParser();
        }
        [Fact]
        public void ToMutationTest()
        {
            var mutatons = _HbaseParser.ToMutations(_student);
            Assert.True(mutatons.Count > 0);
        }

        [Fact]
        public void ToRealTest()
        {
            var tRow = new TRowResult
            {
                Row = 1.ToBytes(),
                Columns = new Dictionary<byte[], TCell>
                {
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.Name)}".ToBytes(),new TCell {Value=_student.Name.ToBytes()}},
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.Age)}".ToBytes(),new TCell {Value=_student.Age.ToBytes()}},
                    {$"{HbaseColumnAttribute.DefaultFamily}:{nameof(_student.IsWork)}".ToBytes(),new TCell {Value=_student.IsWork.ToBytes()}},
                }

            };
            var student = _HbaseParser.ToReal<Student>(tRow);

            Assert.Equal(_student.Age, student.Age);
            Assert.Equal(_student.Name, student.Name);
            Assert.Equal(_student.IsWork, student.IsWork);
        }
        [Fact]
        public void ObjectAndBytes()
        {
            Assert.Equal(5, 5.ToBytes().ToObject<int>());
            Assert.Equal((short)5, ((short)5).ToBytes().ToObject<short>());
            Assert.Equal(5L, 5L.ToBytes().ToObject<long>());
            Assert.Equal(5.3, 5.3.ToBytes().ToObject<double>());
            Assert.Equal(5.3f, 5.3f.ToBytes().ToObject<float>());
            Assert.Equal('但', '但'.ToBytes().ToObject<char>());
            Assert.True(true.ToBytes().ToObject<bool>());
            Assert.Equal("true", "true".ToBytes().ToObject<string>());
            var student = new Student
            {
                Name = "hsx",
                Age = 6,
                IsWork = true,
                JoinSchool = DateTime.Now,
                Hobbies = new List<string> { "running" }
            };
            var student1 = student.ToBytes().ToObject<Student>();
            Assert.Equal(student.Name, student1.Name);
            Assert.Equal(student.Age, student1.Age);
            Assert.Equal(student.IsWork, student1.IsWork);
            Assert.Equal(student.JoinSchool, student1.JoinSchool);
            Assert.Contains(student1.Hobbies, t => t == "running");
        }
    }
}