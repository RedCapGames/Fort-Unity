using Fort.Info.PurchasableItem;
using Newtonsoft.Json;

namespace Fort.Info.SkinnerBox
{
    public abstract class PurchasableItemSkinnerBoxItemInfo : SkinnerBoxItemInfo
    {
        protected PurchasableItemSkinnerBoxItemInfo()
        {
            PurchaseDatas = new PurchaseData[0];
        }
        public PurchaseData[] PurchaseDatas { get; set; }
    }
}