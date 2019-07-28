using System;
using System.Collections.Generic;
using HbaseNetCore.Attributes;

namespace HbaseNetCoreTest.Models
{
    [HbaseTable]
    public class Student
    {
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