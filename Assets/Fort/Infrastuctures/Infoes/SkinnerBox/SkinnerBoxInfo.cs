using System;
using Fort.Inspector;

namespace Fort.Info.SkinnerBox
{
    public abstract class SkinnerBoxInfo
    {
        protected SkinnerBoxInfo()
        {
            Id = Guid.NewGuid().ToString();
            Items = new SkinnerBoxItemInfo[0];
        }
        [IgnoreProperty]
        public string Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }

        public SkinnerBoxItemInfo[] Items { get; set; }
    }
}