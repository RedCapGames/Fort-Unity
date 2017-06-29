using System;
using System.Linq;
using Fort.Info.GameItem;
using Fort.ServerConnection;
using Object = UnityEngine.Object;

namespace Fort
{
    public interface IAssetService
    {
        Object Resolve(GameItemInfo gameItem);
        bool IsReady(GameItemInfo gameItem);
        string[] GetAssetBundleList();
        bool IsAssetBundleDownloaded(string assetBundle);
        Promise DownloadAssetBundle(string assetBundle, Action<DownloadProgress> progress);
        Promise DownloadAllAssetBundles( Action<AllAssetBundleDownloadProgressInfo> progress);
        AssetBundlesDownloadInfo GetNotDownloadedInfo();
    }

    public class AllAssetBundleDownloadProgressInfo
    {
        public AllAssetBundleDownloadProgressInfo(long[] sizes, long totalPosition, long currentPosition,
            string[] allAssetBundles, string[] downloadedAssetBundles, int currentIndex)
        {
            Sizes = sizes;
            TotalPosition = totalPosition;
            CurrentPosition = currentPosition;
            AllAssetBundles = allAssetBundles;
            DownloadedAssetBundles = downloadedAssetBundles;
            CurrentIndex = currentIndex;
        }

        public long TotalSize { get { return Sizes.Sum(); } }
        public long[] Sizes { get; private set; }
        public float TotalNormalPosition { get { return (float)TotalPosition/TotalSize; } }
        public float CurrentNormalPosition { get { return (float)CurrentPosition/Sizes[CurrentIndex]; } }
        public long TotalPosition { get; private set; }
        public long CurrentPosition { get; private set; }
        public string[] AllAssetBundles { get; private set; }
        public string[] DownloadedAssetBundles { get; private set; }

        public string[] RemainingAssetBundles
        {
            get { return AllAssetBundles.Where(s => DownloadedAssetBundles.Contains(s)).ToArray(); }
        }

        public string CurrentAssetBundle { get { return AllAssetBundles[CurrentIndex]; } }
        public int CurrentIndex { get; private set; }
    }

    public class AssetBundlesDownloadInfo
    {
        public AssetBundlesDownloadInfo(long[] sizes, string[] bundles)
        {
            Sizes = sizes;
            Bundles = bundles;
        }

        public long[] Sizes { get; private set; }
        public string[] Bundles { get; private set; }
    }
}
