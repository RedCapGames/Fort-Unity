using UnityEngine;
using System.Collections;
using Fort.Info;


namespace Fort
{
    public interface IAssetService
    {
        Object Resolve(GameItemInfo gameItem);
        bool IsReady(GameItemInfo gameItem);
    }
}
