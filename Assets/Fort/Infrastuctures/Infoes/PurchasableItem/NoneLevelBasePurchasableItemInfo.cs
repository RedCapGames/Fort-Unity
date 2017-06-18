using System;

namespace Fort.Info.PurchasableItem
{
    public abstract class NoneLevelBasePurchasableItemInfo : PurchasableItemInfo
    {
        public ItemCosts Costs { get; set; }
        public PurchasableItemInfo[] ChildrenPurchasableItems { get; set; }
        public bool ChildrenPurchased { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ChildrenPurchasedOnParentPurchaseAttribute : Attribute
    {
        
    }
}