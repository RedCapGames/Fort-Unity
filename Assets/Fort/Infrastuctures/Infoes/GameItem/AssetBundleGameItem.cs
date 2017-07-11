using Fort.Inspector;

namespace Fort.Info.GameItem
{
    [Inspector(Presentation = "Fort.CustomEditor.AssetBundleGameItemPresentation")]
    public sealed class AssetBundleGameItem : GameItemInfo
    {
        public string AssetBundle { get; set; }
        public string ItemName { get; set; }
    }
}