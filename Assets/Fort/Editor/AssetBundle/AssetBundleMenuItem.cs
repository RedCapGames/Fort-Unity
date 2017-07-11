using UnityEditor;

namespace Fort.AssetBundle
{
    public class AssetBundleMenuItem
    {
        [MenuItem("Fort/AssetBundles/Build AssetBundles")]
        static public void BuildAssetBundles()
        {
            AssetBundleBuilder.Build();
        }
        [MenuItem("Fort/AssetBundles/Sync AssetBundles")]
        static public void SyncAssetBundles()
        {
            AssetBundleBuilder.SyncAssetBundles();
        }

    }
}
