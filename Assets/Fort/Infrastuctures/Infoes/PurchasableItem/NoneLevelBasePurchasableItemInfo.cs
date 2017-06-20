using System;

namespace Fort.Info.PurchasableItem
{
    public abstract class NoneLevelBasePurchasableItemInfo : PurchasableItemInfo
    {
        protected NoneLevelBasePurchasableItemInfo()
        {
            Costs = new ItemCosts();
            ChildrenPurchasableItems= new PurchasableItemInfo[0];
        }
        public ItemCosts Costs { get; set; }
        public PurchasableItemInfo[] ChildrenPurchasableItems { get; set; }
        public bool ChildrenPurchased { get; set; }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ChildrenPurchasedOnParentPurchaseAttribute : Attribute
    {
        
    }
}