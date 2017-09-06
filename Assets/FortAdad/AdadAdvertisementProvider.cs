using System;
using Assets.Fort.Infrastuctures.Events;
using Fort;
using Fort.Advertisement;
using UnityEngine;

namespace Assets.FortAdad
{
    public class AdadAdvertisementProvider:IAdvertisementProvider
    {
        private static AndroidJavaObject _activity;
        private static AndroidJavaClass _pluginClass;
        private static AndroidJavaObject _adadUnityObject;
        private static bool _initialized;
        private StandardBannerVerticalAlignment? _verticalAlignment;
        private StandardBannerHorizantalAlignment? _horizantalAlignment;
        //private static AdListener _adListener;
        #region Implementation of IAdvertisementProvider

        public string Name { get { return "Adad"; } }
        public bool IsVideoSupported { get { return false; } }
        public bool IsStandardBannerSupported { get { return true; } }
        public bool IsInterstitialBannerSupported { get { return false; } }
        public ErrorPromise<ShowVideoFailed> ShowVideo(string zone, bool skipable)
        {
            throw new NotSupportedException();
        }

        public void Initialize()
        {
            if(Application.platform != RuntimePlatform.Android)
                return;
            if (!_initialized)
            {
                Debug.Log("Initialized");
                AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                _activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

                _pluginClass = new AndroidJavaClass("ir.adad.client.AdadUnityPlugin");
                if (_pluginClass != null)
                {
                    _adadUnityObject = _pluginClass.CallStatic<AndroidJavaObject>("getInstance");
                    _adadUnityObject.Call("initializeAdad", new object[1] { _activity });
                    _initialized = true;
                }
            }
        }

        private static string GetVertical(StandardBannerVerticalAlignment verticalAlignment)
        {
            switch (verticalAlignment)
            {
                case StandardBannerVerticalAlignment.Top:
                    return "top";
                case StandardBannerVerticalAlignment.Bottom:
                    return "bottom";
                default:
                    throw new ArgumentOutOfRangeException("verticalAlignment", verticalAlignment, null);
            }
        }

        private static string GetHorizantal(StandardBannerHorizantalAlignment horizantalAlignment)
        {
            switch (horizantalAlignment)
            {
                case StandardBannerHorizantalAlignment.Left:
                    return "left";
                case StandardBannerHorizantalAlignment.Center:
                    return "center";
                case StandardBannerHorizantalAlignment.Right:
                    return "right";
                default:
                    throw new ArgumentOutOfRangeException("horizantalAlignment", horizantalAlignment, null);
            }
        }
        public void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment)
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

            _adadUnityObject.Call("createBannerAds", _activity, GetHorizantal(horizantalAlignment),
                GetVertical(verticalAlignment), new AdListener(), 0, 0, 0, 0);
            _verticalAlignment = verticalAlignment;
            _horizantalAlignment = horizantalAlignment;
            AdadPosition adadPosition = ServiceLocator.Resolve<IStorageService>().ResolveData<AdadPosition>() ?? new AdadPosition();
            adadPosition.VerticalAlignment = _verticalAlignment.Value;
            adadPosition.HorizantalAlignment = _horizantalAlignment.Value;
            ServiceLocator.Resolve<IStorageService>().UpdateData(adadPosition);
        }

        public void ShowStandardBanner()
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

            AdadPosition adadPosition = ServiceLocator.Resolve<IStorageService>().ResolveData<AdadPosition>() ?? new AdadPosition();
            if (_verticalAlignment == null || _horizantalAlignment == null)
            {
                ChangeStandardBannerPosition(adadPosition.VerticalAlignment,adadPosition.HorizantalAlignment);
            }
            else
            {
                if (adadPosition.VerticalAlignment != _verticalAlignment.Value ||
                    adadPosition.HorizantalAlignment != _horizantalAlignment.Value)
                {
                    ChangeStandardBannerPosition(adadPosition.VerticalAlignment, adadPosition.HorizantalAlignment);
                }
            }
            _adadUnityObject.Call("enableBannerAds");
        }

        public void HideStandardBanner()
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

            //Initinalize();
            _adadUnityObject.Call("disableBannerAds");
        }

        public void ShowInterstiatialBanner()
        {
            if (Application.platform != RuntimePlatform.Android)
                return;

            //Initinalize();
            _adadUnityObject.Call("prepareInterstitial", new InterstitialAdListener(_adadUnityObject, _activity));
        }

        #endregion
    }
    public class AdListener : AndroidJavaProxy
    {
        public AdListener() : base("ir.adad.client.AdListener") { }
        void onAdLoaded()
        {
            //Debug.Log("Banner ad loaded");
        }
        void onAdFailedToLoad()
        {
            //Debug.Log("Banner ad failed to load");
        }

        void onMessageReceive(string message)
        {
            //Debug.Log("Message recived: " + message);
        }

        void onRemoveAdsRequested()
        {
            //Debug.Log("Banner Ad, remove ads requested");
            ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<RemoveAdRequestedEvent>().Publish(new RemoveAdRequestedEventArgs(true));
        }
    }

    public class InterstitialAdListener : AndroidJavaProxy
    {
        private AndroidJavaObject _adadUnityObject;
        private readonly AndroidJavaObject _activity;

        public InterstitialAdListener(AndroidJavaObject adadUnityObject, AndroidJavaObject activity) : base("ir.adad.client.InterstitialAdListener")
        {
            _adadUnityObject = adadUnityObject;
            _activity = activity;
        }

        void onAdLoaded()
        {
            _adadUnityObject.Call("showInterstitial", new object[1] { _activity });
        }
        void onAdFailedToLoad()
        {
            //Debug.Log("Interstitial ad failed to load");
        }

        void onMessageReceive(string message)
        {
            //Debug.Log("Message recived: " + message);
        }

        void onRemoveAdsRequested()
        {
            //Debug.Log("Interstitial ad, remove Ads requested");
            ServiceLocator.Resolve<IEventAggregatorService>().GetEvent<RemoveAdRequestedEvent>().Publish(new RemoveAdRequestedEventArgs(false));
        }

        void onInterstitialAdDisplayed()
        {
            //Debug.Log("interstitial Ad displayed");
        }

        void onInterstitialClosed()
        {
            //Debug.Log("interstitial Ad closed");
        }
    }

    public class AdadPosition
    {
        public AdadPosition()
        {
            HorizantalAlignment = StandardBannerHorizantalAlignment.Center;
            VerticalAlignment = StandardBannerVerticalAlignment.Bottom;
        }
        public StandardBannerHorizantalAlignment HorizantalAlignment { get; set; }
        public StandardBannerVerticalAlignment VerticalAlignment { get; set; }
    }
}
