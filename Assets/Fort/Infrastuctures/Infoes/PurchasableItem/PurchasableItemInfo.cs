using System;
using Fort.Inspector;

namespace Fort.Info
{
    public abstract class PurchasableItemInfo
    {
        protected PurchasableItemInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnoreProperty]
        public string Id { get; set; }
        public bool ChildrenPurchased { get; set; }
        public string Category { get; set; }
        public string DisplayName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool DefaultBought { get; set; }
        public string Tag { get; set; }
        public Balance Cost { get; set; }
    }
}
