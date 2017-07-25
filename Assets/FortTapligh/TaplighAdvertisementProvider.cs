using System;
using Fort;
using Fort.Advertisement;
using Fort.Info;
using FortTapligh.Info;
using UnityEngine;

namespace FortTapligh
{
    public class TaplighAdvertisementProvider : IAdvertisementProvider
    {
        private ErrorDeferred<ShowVideoFailed> _deferred;
        private bool _initialized;
        public void Initialize()
        {
            if (_initialized)
                return;
            _initialized = true;
            TaplighInterface.Instance.InitializeTapligh(InfoResolver.Resolve<TaplighInfo>().Key);
            TaplighInterface.Instance.OnShowAdListener = (result, s) =>
            {
                
                switch (result)
                {
                    case ShowAdResult.NO_INTERNET_ACSSES:
                    case ShowAdResult.BAD_TOKEN_USED:
                    case ShowAdResult.INTERNAL_ERROR:
                        {
                            if (_deferred == null)
                                return;
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Reject(ShowVideoFailed.ProviderError);
                        }
                        break;
                    case ShowAdResult.NO_AD_AVAILABLE:
                    case ShowAdResult.NO_AD_READY:
                        {
                            if (_deferred == null)
                                return;
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Reject(ShowVideoFailed.NoVideoAvilable);
                        }
                        break;
                    case ShowAdResult.AD_AVAILABLE:
                        break;
                    case ShowAdResult.AD_VIEWED_COMPLETELY:
                        break;
                    case ShowAdResult.AD_CLICKED:
                        break;
                    case ShowAdResult.AD_IMAGE_CLOSED:
                        break;
                    case ShowAdResult.AD_VIDEO_CLOSED_AFTER_FULL_VIEW:
                        {
                            if (_deferred == null)
                                return;
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Resolve();
                        }
                        break;
                    case ShowAdResult.AD_VIDEO_CLOSED_ON_VIEW:
                        {
                            if (_deferred == null)
                                return;
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Reject(ShowVideoFailed.Cancel);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("result", result, null);
                }
            };
        }
        #region Implementation of IAdvertisementProvider

        public string Name { get { return "Tapligh"; } }
        public bool IsVideoSupported { get { return true; } }
        public bool IsStandardBannerSupported { get { return true; } }
        public bool IsInterstitialBannerSupported { get { return true; } }
        public ErrorPromise<ShowVideoFailed> ShowVideo(string zone, bool skipable)
        {
            if (_deferred != null)
            {
                ErrorDeferred<ShowVideoFailed> deferred = new ErrorDeferred<ShowVideoFailed>();
                deferred.Reject(ShowVideoFailed.AlreadyShowingVideo);
                return deferred.Promise();
            }
            _deferred = new ErrorDeferred<ShowVideoFailed>();
            TaplighInterface.Instance.ShowVideoAd(skipable);
            return _deferred.Promise();
        }

        public void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment)
        {


        }

        public void ShowStandardBanner()
        {
            TaplighInterface.Instance.ShowImageAd();
        }

        public void HideStandardBanner()
        {

        }

        public void ShowInterstiatialBanner()
        {
            TaplighInterface.Instance.ShowInterstitialAd();
        }


        #endregion
    }
}
