using Fort.Info;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IAssetService))]
    public class AssetService : MonoBehaviour,IAssetService
    {
        #region Implementation of IAssetService

        public Object Resolve(GameItemInfo gameItem)
        {
            ResourceGameItem item = gameItem as ResourceGameItem;
            if (item != null)
                return Resources.Load(item.Address);
            AssetBundleGameItem assetBundleGameItem = gameItem as AssetBundleGameItem;
            if (assetBundleGameItem != null)
                return null;
            return (Object) gameItem.GetType().GetProperty("GameItem").GetValue(gameItem, new object[0]);
        }

        public bool IsReady(GameItemInfo gameItem)
        {
            ResourceGameItem item = gameItem as ResourceGameItem;
            if (item != null)
                return true;
            AssetBundleGameItem assetBundleGameItem = gameItem as AssetBundleGameItem;
            if (assetBundleGameItem != null)
                return false;
            return true;

        }

        #endregion
    }
}
