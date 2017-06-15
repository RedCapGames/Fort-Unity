using System;

namespace Fort.Info
{
    public abstract class NoneLevelBasePurchasableItemInfo : PurchasableItemInfo
    {
        public PurchasableItemInfo[] ChildrenPurchasableItems { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ChildrenPurchasedOnParentPurchaseAttribute : Attribute
    {
        
    }
}