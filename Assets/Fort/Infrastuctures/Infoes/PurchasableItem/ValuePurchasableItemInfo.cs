namespace Fort.Info.PurchasableItem
{
    public class ValuePurchasableItemInfo:NoneLevelBasePurchasableItemInfo
    {
        public ValuePurchasableItemInfo()
        {
            Values = new Balance();
        }
        public Balance Values { get; set; }
    }
}
