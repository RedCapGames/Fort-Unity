using System;

namespace Fort.Info
{
    [AttributeUsage(AttributeTargets.Property)]
    public class CustomExportAttribute:Attribute
    {
        public CustomExportAttribute()
        {
            AddType = true;
        }
        public string DisplayName { get; set; }
        public bool AddType { get; set; }
    }
}
