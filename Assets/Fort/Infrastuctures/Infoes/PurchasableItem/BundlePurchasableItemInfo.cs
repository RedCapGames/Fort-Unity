namespace Fort.Info.PurchasableItem
{
    public class BundlePurchasableItemInfo:NoneLevelBasePurchasableItemInfo
    {
        public BundlePurchasableItemInfo()
        {
            PurchaseDatas = new PurchaseData[0];
        }
        public PurchaseData[] PurchaseDatas { get; set; }
    }
}
