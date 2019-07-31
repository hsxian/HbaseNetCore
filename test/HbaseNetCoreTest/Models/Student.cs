using System;
using System.Collections.Generic;
using System.Linq;
using HbaseNetCore.Attributes;
using HbaseNetCore.Interfaces;
using HbaseNetCore.Utility;

namespace HbaseNetCoreTest.Models
{
    [HbaseTable("student")]
    public class Student : IHbaseTable
    {
        public string RowKey { get; set; }
        [HbaseColumn]
        public string Name { get; set; }
        [HbaseColumn]
        public int Age { get; set; }
        [HbaseColumn]
        public bool? isWork;
        public DateTime JoinSchool { get; set; }
        [HbaseColumn]
        public List<string> Hobbies { get; set; }
    }
}