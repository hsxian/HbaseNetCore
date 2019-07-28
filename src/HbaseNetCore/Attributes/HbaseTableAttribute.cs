using System;

namespace HbaseNetCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class HbaseTableAttribute : Attribute
    {
        public string Table { get; set; }
        public HbaseTableAttribute(string table = null)
        {
            Table = table;
        }
    }
}