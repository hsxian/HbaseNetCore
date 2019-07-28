using System;

namespace Utilities.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class HbaseColumnAttribute : Attribute
    {
        public const string DefaultFamily = "default";
        public string Family { get; set; }
        public string Column { get; set; }
        public HbaseColumnAttribute(string family = DefaultFamily, string column = null)
        {
            Family = family;
            Column = column;
        }
    }
}