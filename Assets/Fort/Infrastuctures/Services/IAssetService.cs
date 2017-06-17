using Fort.Info.GameItem;
using UnityEngine;

namespace Fort
{
    public interface IAssetService
    {
        Object Resolve(GameItemInfo gameItem);
        bool IsReady(GameItemInfo gameItem);
    }
}
