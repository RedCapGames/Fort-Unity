using System;

namespace Fort.Inspector
{
    public class EnumPresentationAttribute : Attribute
    {
        public string DisplayName { get; set; }
    }
}