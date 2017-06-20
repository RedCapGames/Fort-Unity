using System;
using System.Linq;
using Fort.Advertisement;
using Fort.Info;
using Fort.Info.Advertisement;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IAdvertisementService))]
    public class AdvertisementService : MonoBehaviour, IAdvertisementService
    {
        #region Implementation of IAdvertisementService

        public bool IsVideoSupported
        {
            get
            {
                return
                    InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Any(
                        priority => priority.AdvertisementProvider.IsVideoSupported);
            }
        }
        public bool IsStandardBannerSupported
        {
            get
            {
                return
                    InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Any(
                        priority => priority.AdvertisementProvider.IsStandardBannerSupported);
            }
        }
        public bool IsInterstitialBannerSupported
        {
            get
            {
                return
                    InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Any(
                        priority => priority.AdvertisementProvider.IsInterstitialBannerSupported);
            }
        }
        public ErrorPromise<ShowVideoFailed> ShowVideo(int zone, bool skipable, bool removabale)
        {
            AdvertisementSavedData advertisementSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AdvertisementSavedData>() ??
                new AdvertisementSavedData();
            if (advertisementSavedData.IsAdRemoved && removabale)
            {
                ErrorDeferred<ShowVideoFailed> result = new ErrorDeferred<ShowVideoFailed>();
                result.Reject(ShowVideoFailed.AdvertisementRemoved);
                return result.Promise();
            }
            ServerSettings serverSettings = ServiceLocator.Resolve<ISettingService>().ResolveCachedServerSetting();
            string[] videoProviderPriority = new string[0];
            int serverCount = 0;
            if (serverSettings != null && serverSettings.AdvertisementSettings != null &&
                serverSettings.AdvertisementSettings.VideoPriority != null &&
                serverSettings.AdvertisementSettings.VideoPriority.Length > 0)
            {
                videoProviderPriority = serverSettings.AdvertisementSettings.VideoPriority;
                serverCount = videoProviderPriority.Select(
                    s =>
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.First(
                            priority =>
                                priority.AdvertisementProvider != null && priority.AdvertisementProvider.Name == s)
                            .AdvertisementProvider).Count();
            }

            if (serverCount == 0)
            {
                videoProviderPriority =
                    InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Where(
                        priority =>
                            priority.AdvertisementProvider != null && priority.AdvertisementProvider.IsVideoSupported)
                        .Select((priority, i) => new { Priority = priority, Index = i })
                        .OrderBy(arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                        .Select(arg => arg.Priority.AdvertisementProvider.Name)
                        .ToArray();
            }
            IAdvertisementProvider[] providers =
                videoProviderPriority.Select(
                    s =>
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.First(
                            priority =>
                                priority.AdvertisementProvider != null && priority.AdvertisementProvider.Name == s)
                            .AdvertisementProvider).ToArray();
            if (providers.Length == 0)
                throw new Exception("No video provider Defined");
            ErrorDeferred<ShowVideoFailed> deferred = new ErrorDeferred<ShowVideoFailed>();
            int index = 0;
            Action showVideo = null;
            showVideo = () =>
            {
                ServiceLocator.Resolve<IAnalyticsService>().StatVideoRequest(providers[index].Name, zone, skipable);
                providers[index].ShowVideo(zone, skipable).Then(() =>
                 {
                     ServiceLocator.Resolve<IAnalyticsService>().StatVideoResult(providers[index].Name,zone,skipable,ShowVideoResult.Succeeded);
                     deferred.Resolve();
                 }, failed =>
                 {
                     switch (failed)
                     {
                         case ShowVideoFailed.Cancel:
                             ServiceLocator.Resolve<IAnalyticsService>().StatVideoResult(providers[index].Name, zone, skipable, ShowVideoResult.Cancel);
                             deferred.Reject(ShowVideoFailed.Cancel);
                             break;
                         case ShowVideoFailed.NoVideoAvilable:
                         case ShowVideoFailed.ProviderError:
                             ServiceLocator.Resolve<IAnalyticsService>().StatVideoResult(providers[index].Name, zone, skipable, failed==ShowVideoFailed.NoVideoAvilable?ShowVideoResult.NoVideoAvilable : ShowVideoResult.ProviderError);
                             if (index == providers.Length)
                                 deferred.Reject(failed);
                             else
                             {
                                 index++;
                                 showVideo();
                             }
                             break;
                         default:
                             throw new ArgumentOutOfRangeException("failed", failed, null);
                     }
                 });
            };
            showVideo();
            return deferred.Promise();
        }

        public void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment)
        {
            foreach (AdvertisementPriority advertisementPriority in InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Where(priority => priority.AdvertisementProvider != null && priority.AdvertisementProvider.IsStandardBannerSupported))
            {
                advertisementPriority.AdvertisementProvider.ChangeStandardBannerPosition(verticalAlignment, horizantalAlignment);
            }

        }

        public void ShowStandardBanner()
        {
            AdvertisementSavedData advertisementSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AdvertisementSavedData>() ??
                new AdvertisementSavedData();
            if(advertisementSavedData.IsAdRemoved)
                return;
            ServerSettings serverSettings = ServiceLocator.Resolve<ISettingService>().ResolveCachedServerSetting();
            string advertisementProvider = string.Empty;
            if (serverSettings != null && serverSettings.AdvertisementSettings != null &&
                string.IsNullOrEmpty(serverSettings.AdvertisementSettings.StandartBannerPriority))
            {
                if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
                {
                    AdvertisementPriority advertisementPriority =
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.FirstOrDefault(
                            priority => priority.AdvertisementProvider != null &&
                                        priority.AdvertisementProvider.IsStandardBannerSupported);
                    if (advertisementPriority != null)
                    {
                        advertisementProvider = advertisementPriority.AdvertisementProvider.Name;
                    }
                }
            }
            if (string.IsNullOrEmpty(advertisementProvider))
            {
                if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
                {
                    AdvertisementPriority advertisementPriority =
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Where(
                            priority =>
                                priority.AdvertisementProvider != null &&
                                priority.AdvertisementProvider.IsStandardBannerSupported)
                            .Select((priority, i) => new { Priority = priority, Index = i })
                            .OrderBy(
                                arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                            .Select(arg => arg.Priority)
                            .FirstOrDefault();
                    if (advertisementPriority != null)
                        advertisementProvider = advertisementPriority.AdvertisementProvider.Name;
                }
            }
            if (string.IsNullOrEmpty(advertisementProvider))
                throw new Exception("No Advertisement provider found for Standard Banner");
            if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
            {
                InfoResolver.FortInfo.Advertisement.AdvertisementProviders.First(priority => priority.AdvertisementProvider.Name == advertisementProvider).AdvertisementProvider.ShowStandardBanner();
                ServiceLocator.Resolve<IAnalyticsService>().StatStandardBanner(advertisementProvider);
            }
        }

        public void HideStandardBanner()
        {
            foreach (AdvertisementPriority advertisementPriority in InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Where(priority => priority.AdvertisementProvider != null && priority.AdvertisementProvider.IsStandardBannerSupported))
            {
                advertisementPriority.AdvertisementProvider.HideStandardBanner();
            }
        }

        public void ShowInterstiatialBanner()
        {
            AdvertisementSavedData advertisementSavedData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<AdvertisementSavedData>() ??
                new AdvertisementSavedData();
            if (advertisementSavedData.IsAdRemoved)
                return;

            ServerSettings serverSettings = ServiceLocator.Resolve<ISettingService>().ResolveCachedServerSetting();
            string advertisementProvider = string.Empty;
            if (serverSettings != null && serverSettings.AdvertisementSettings != null &&
                string.IsNullOrEmpty(serverSettings.AdvertisementSettings.InterstiatialBannerPriority))
            {
                if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
                {
                    AdvertisementPriority advertisementPriority =
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.FirstOrDefault(
                            priority => priority.AdvertisementProvider != null &&
                                        priority.AdvertisementProvider.IsInterstitialBannerSupported);
                    if (advertisementPriority != null)
                    {
                        advertisementProvider = advertisementPriority.AdvertisementProvider.Name;
                    }
                }
            }
            if (string.IsNullOrEmpty(advertisementProvider))
            {
                if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
                {
                    AdvertisementPriority advertisementPriority =
                        InfoResolver.FortInfo.Advertisement.AdvertisementProviders.Where(
                            priority =>
                                priority.AdvertisementProvider != null &&
                                priority.AdvertisementProvider.IsInterstitialBannerSupported)
                            .Select((priority, i) => new { Priority = priority, Index = i })
                            .OrderBy(
                                arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                            .Select(arg => arg.Priority)
                            .FirstOrDefault();
                    if (advertisementPriority != null)
                        advertisementProvider = advertisementPriority.AdvertisementProvider.Name;
                }
            }
            if (string.IsNullOrEmpty(advertisementProvider))
                throw new Exception("No Advertisement provider found for Interstitial Banner");
            if (InfoResolver.FortInfo.Advertisement.AdvertisementProviders != null)
            {
                InfoResolver.FortInfo.Advertisement.AdvertisementProviders.First(priority => priority.AdvertisementProvider.Name == advertisementProvider).AdvertisementProvider.ShowStandardBanner();
                ServiceLocator.Resolve<IAnalyticsService>().StatInterstitialBanner(advertisementProvider);
            }
        }

        #endregion
    }

    internal class AdvertisementSavedData
    {
        public bool IsAdRemoved { get; set; }
    }
}