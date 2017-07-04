using System;
using System.Linq;
using Fort.Info.GameItem;
using Fort.ServerConnection;
using Object = UnityEngine.Object;

namespace Fort
{
    /// <summary>
    /// This service is used to manage assets of game. Asset bundle management is provided by this service.
    /// </summary>
    public interface IAssetService
    {
        /// <summary>
        /// Resolving game item
        /// </summary>
        /// <param name="gameItem">Corresponding game item</param>
        /// <returns>The asset</returns>
        Object Resolve(GameItemInfo gameItem);
        /// <summary>
        /// Is game item ready to use
        /// </summary>
        /// <param name="gameItem">Corresponding game item</param>
        /// <returns>Is game item ready to use</returns>
        bool IsReady(GameItemInfo gameItem);
        /// <summary>
        /// Resolving the list of asset bundle
        /// </summary>
        /// <returns>List of asset bundles</returns>
        string[] GetAssetBundleList();
        /// <summary>
        /// Is asset bundle downloaded
        /// </summary>
        /// <param name="assetBundle">Name of asset bundle</param>
        /// <returns>Is asset bundle downloaded</returns>
        bool IsAssetBundleDownloaded(string assetBundle);
        /// <summary>
        /// Download single asset bundle
        /// </summary>
        /// <param name="assetBundle">Name of asset bundle</param>
        /// <param name="progress">A call back that return progress of download</param>
        /// <returns>Promise of download</returns>
        Promise DownloadAssetBundle(string assetBundle, Action<DownloadProgress> progress);
        /// <summary>
        /// Download not downloaded asset bundle
        /// </summary>
        /// <param name="progress">A call back that return progress of download</param>
        /// <returns>Promise of download</returns>
        Promise DownloadAllAssetBundles( Action<AllAssetBundleDownloadProgressInfo> progress);
        /// <summary>
        /// Resolve not downloaded asset bundle name and size of them
        /// </summary>
        /// <returns>Return not downloaded asset bundle name and size of them</returns>
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
