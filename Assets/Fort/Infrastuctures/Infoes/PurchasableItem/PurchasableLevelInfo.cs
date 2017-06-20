using System;
using Fort.Inspector;

namespace Fort.Info.PurchasableItem
{
    public class PurchasableLevelInfo
    {
        
        public PurchasableLevelInfo()
        {
            Id = Guid.NewGuid().ToString();
            Costs = new ItemCosts();
        }
        [IgnoreProperty]
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
        public bool DefaultBought { get; set; }
        public ItemCosts Costs { get; set; }

    }
}