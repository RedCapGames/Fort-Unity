using System;
using Fort.Inspector;

namespace Fort.Info.PurchasableItem
{
    public abstract class PurchasableItemInfo
    {
        protected PurchasableItemInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnorePresentation]
        public string Id { get; set; }
        
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool DefaultBought { get; set; }
        public string Tag { get; set; }
    }
}
