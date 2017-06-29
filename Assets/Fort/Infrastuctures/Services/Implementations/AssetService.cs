using System;
using System.Collections.Generic;
using System.Linq;
using Fort.AssetBundle;
using Fort.Info;
using Fort.Info.GameItem;
using Fort.ServerConnection;
using Newtonsoft.Json;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Fort
{
    [Service(ServiceType = typeof(IAssetService))]
    public class AssetService : MonoBehaviour,IAssetService
    {
        private bool _isInitialized;
        private Dictionary<string, ServerAssetBundleVersion> _serverAssetBundleVersions = new Dictionary<string, ServerAssetBundleVersion>();
        private readonly Dictionary<string,UnityEngine.AssetBundle> _assetBundles = new Dictionary<string, UnityEngine.AssetBundle>(); 
        private void Initialize()
        {
            if(_isInitialized)
                return;
            _isInitialized = true;
            string path = string.Format("BundleConfig/{0}/AssetBundleBundleInfo.json",
                                                AssetBundleUtility.GetPlatformName());
            TextAsset textAsset = Resources.Load<TextAsset>(path);
            if (textAsset == null)
                return;
            _serverAssetBundleVersions = JsonConvert.DeserializeObject<Dictionary<string, ServerAssetBundleVersion>>(textAsset.text);
            //UnityEngine.AssetBundle.lo
        }
        #region Implementation of IAssetService

        public Object Resolve(GameItemInfo gameItem)
        {
            Initialize();
            ResourceGameItem item = gameItem as ResourceGameItem;
            if (item != null)
                return Resources.Load(item.Address);
            AssetBundleGameItem assetBundleGameItem = gameItem as AssetBundleGameItem;
            if (assetBundleGameItem != null)
            {
                if (_assetBundles.ContainsKey(assetBundleGameItem.AssetBundle))
                    return _assetBundles[assetBundleGameItem.AssetBundle].LoadAsset(assetBundleGameItem.ItemName);
                if(!IsAssetBundleDownloaded(assetBundleGameItem.AssetBundle))
                    throw new Exception(string.Format("Asset bundle {0} is not downloaded",assetBundleGameItem.AssetBundle));
                Uri bundleServerAddress = GetBundleServerAddress(assetBundleGameItem.AssetBundle);
                UnityEngine.AssetBundle assetBundle =
                    UnityEngine.AssetBundle.LoadFromFile(
                        InfoResolver.Resolve<FortInfo>()
                            .ServerConnectionProvider.UserConnection.LoadFromCache(bundleServerAddress));
                _assetBundles[assetBundleGameItem.AssetBundle] = assetBundle;
                return assetBundle.LoadAsset(assetBundleGameItem.ItemName);

            }
            return (Object) gameItem.GetType().GetProperty("GameItem").GetValue(gameItem, new object[0]);
        }

        public bool IsReady(GameItemInfo gameItem)
        {
            Initialize();
            ResourceGameItem item = gameItem as ResourceGameItem;
            if (item != null)
                return true;
            AssetBundleGameItem assetBundleGameItem = gameItem as AssetBundleGameItem;
            if (assetBundleGameItem != null)
            {
                if (_assetBundles.ContainsKey(assetBundleGameItem.AssetBundle))
                    return _assetBundles[assetBundleGameItem.AssetBundle].LoadAsset(assetBundleGameItem.ItemName);
                if (!IsAssetBundleDownloaded(assetBundleGameItem.AssetBundle))
                    return false;
                return true;
            }
            return true;

        }

        public string[] GetAssetBundleList()
        {
            Initialize();
            return _serverAssetBundleVersions.Keys.ToArray();
        }

        private Uri GetBundleServerAddress(string assetBundle)
        {
            if (!_serverAssetBundleVersions.ContainsKey(assetBundle))
                throw new Exception("Asset bundle not found in server");

            return new Uri(InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.GetStorageAddress(),new Uri(_serverAssetBundleVersions[assetBundle].Path,UriKind.Relative));
        }
        public bool IsAssetBundleDownloaded(string assetBundle)
        {
            Uri bundleServerAddress = GetBundleServerAddress(assetBundle);
            return InfoResolver.Resolve<FortInfo>()
                .ServerConnectionProvider.UserConnection.IsCached(bundleServerAddress);
        }

        public Promise DownloadAssetBundle(string assetBundle, Action<DownloadProgress> progress)
        {
            Deferred deferred = new Deferred();
            Uri bundleServerAddress = GetBundleServerAddress(assetBundle);
            InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.LoadFromStorage(bundleServerAddress,progress).Then(s => deferred.Resolve(),deferred.Reject);
            return deferred.Promise();
        }

        public Promise DownloadAllAssetBundles(Action<AllAssetBundleDownloadProgressInfo> progress)
        {
            Deferred deferred = new Deferred();
            string[] notDownloadedAssetBundles =
                _serverAssetBundleVersions.Keys.Where(s => !IsAssetBundleDownloaded(s)).ToArray();
            Action<int> download = null;
            long[] sizes = notDownloadedAssetBundles.Select(s => _serverAssetBundleVersions[s].Size).ToArray();
            List<string> downloadedBundles = new List<string>();
            long downloadedSize = 0;
            download = i =>
            {
                if (i == notDownloadedAssetBundles.Length)
                {
                    deferred.Resolve();
                }
                else
                {
                    DownloadAssetBundle(notDownloadedAssetBundles[i], p =>
                    {
                        progress(new AllAssetBundleDownloadProgressInfo(sizes, downloadedSize + p.Progress, p.Progress,
                            notDownloadedAssetBundles, downloadedBundles.ToArray(), i));
                    }).Then(() =>
                    {
                        downloadedBundles.Add(notDownloadedAssetBundles[i]);
                        downloadedSize += _serverAssetBundleVersions[notDownloadedAssetBundles[i]].Size;
                        download(i + 1);
                        
                    },() => deferred.Reject());
                }
            };
            download(0);
            return deferred.Promise();
        }

        public AssetBundlesDownloadInfo GetNotDownloadedInfo()
        {
            string[] notDownloadedAssetBundles =
                _serverAssetBundleVersions.Keys.Where(s => !IsAssetBundleDownloaded(s)).ToArray();
            long[] sizes = notDownloadedAssetBundles.Select(s => _serverAssetBundleVersions[s].Size).ToArray();
            return new AssetBundlesDownloadInfo(sizes,notDownloadedAssetBundles);
        }

        #endregion
    }

    public class ServerAssetBundleVersion
    {
        public string Path { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
    }
}
