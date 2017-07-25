using System.IO;
using System.Linq;
using Fort.Info.GameItem;
using Fort.Inspector;
using UnityEditor;

namespace Fort.CustomEditor
{
    public class AssetBundleGameItemPresentation : Presentation
    {
        #region Overrides of Presentation

        public override PresentationResult OnInspectorGui(PresentationParamater parameter)
        {            
            AssetBundleGameItemPresentationData presentationData =
                parameter.PresentationData as AssetBundleGameItemPresentationData ??
                new AssetBundleGameItemPresentationData();
            Change change = new Change();
            AssetBundleGameItem assetBundleGameItem = parameter.Instance as AssetBundleGameItem ?? new AssetBundleGameItem();
            string[] bundleItems = new[] { "none" }.Concat(AssetDatabase.GetAllAssetBundleNames()).ToArray();
            int bundleIndex = AssetDatabase.GetAllAssetBundleNames().IndexOf(s => s == assetBundleGameItem.AssetBundle) + 1;
            int newIndex = EditorGUILayout.Popup("Asset bundle", bundleIndex, bundleItems);
            string assetBundleName = newIndex > 0 ? AssetDatabase.GetAllAssetBundleNames()[newIndex - 1] : string.Empty;
            change.IsDataChanged |= newIndex != bundleIndex;
            assetBundleGameItem.AssetBundle = assetBundleName;
            if (newIndex > 0)
            {

                string[] assets = AssetDatabase.GetAssetPathsFromAssetBundle(assetBundleName).Select(s => Path.GetFileNameWithoutExtension(s)).ToArray();
                string[] assetItems = new[] { "none" }.Concat(assets).ToArray();
                int assetIndex = assets.IndexOf(s => s == assetBundleGameItem.ItemName) + 1;
                int newAssetIndex = EditorGUILayout.Popup("Asset", assetIndex, assetItems);
                assetBundleGameItem.ItemName = newAssetIndex == 0 ? string.Empty : assets[newAssetIndex - 1];
                change.IsDataChanged |= assetIndex != newAssetIndex;
            }
            return new PresentationResult
            {
                Change = change,
                PresentationData = presentationData,
                Result = assetBundleGameItem
            };

        }

        #endregion
    }

    public class AssetBundleGameItemPresentationData
    {
        public bool IsFoldout { get; set; }
    }
}
