using System;

namespace Fort.Inspector
{
    [AttributeUsage(AttributeTargets.Class|AttributeTargets.Interface|AttributeTargets.Property)]
    public class PresentationTitleAttribute : Attribute
    {
        public string Title { get; private set; }

        public PresentationTitleAttribute(string title)
        {
            Title = title;
        }
    }
}