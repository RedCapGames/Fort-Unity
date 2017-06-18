using Fort.Info.PurchasableItem;
using Newtonsoft.Json;

namespace Fort.Info.SkinnerBox
{
    public abstract class PurchasableItemSkinnerBoxItemInfo : SkinnerBoxItemInfo
    {
        public PurchaseData[] PurchaseDatas { get; set; }
    }
}