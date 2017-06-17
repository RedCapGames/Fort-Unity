using System.Collections;
using System.Linq;
using System.Reflection;

namespace Fort.Info.PurchasableItem
{
    public abstract class LevelBasePurchasableItemInfo : PurchasableItemInfo
    {
        public PurchasableLevelInfo[] GetPurchasableLevelInfos()
        {
            return
                ((IEnumerable)GetType().GetProperty("LevelInfoes", BindingFlags.Public | BindingFlags.Instance).GetValue(this, new object[0]))
                    .Cast<PurchasableLevelInfo>().ToArray();
        }
    }
    public abstract class LevelBasePurchasableItemInfo<T> : LevelBasePurchasableItemInfo where T: PurchasableLevelInfo
    {
        public T[] LevelInfoes { get; set; }
    }
}