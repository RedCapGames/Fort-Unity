using System;
using Fort;
using Fort.Advertisement;
using Fort.Info;
using FortTapsell.Info;
using TapsellSDK;
using UnityEngine;

namespace FortTapsell
{
    public class TapsellAdvertisementProvider:IAdvertisementProvider
    {
        private ErrorDeferred<ShowVideoFailed> _deferred;
        private bool _isInitialized;
        private bool _isSkipable;
        public void Initialize()
        {
            if(_isInitialized)
                return;
            _isInitialized = true;
            Tapsell.initialize(InfoResolver.Resolve<TapsellInfo>().Key);

            Tapsell.setRewardListener(
                result =>
                {
                    if (!result.completed)
                    {
                        if (result.rewarded && _isSkipable)
                        {
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Resolve();
                        }
                        else
                        {
                            FailedDefered(ShowVideoFailed.Cancel);
                        }
                        
                    }
                    else
                    {
                        if(!result.rewarded)
                            FailedDefered(ShowVideoFailed.ProviderError);
                        else
                        {
                            ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                            _deferred = null;
                            errorDeferred.Resolve();
                        }
                    }
                }
            );
        }
        #region Implementation of IAdvertisementProvider

        public string Name { get { return "Tapsell"; } }
        public bool IsVideoSupported { get {return true;} }
        public bool IsStandardBannerSupported { get { return false; } }
        public bool IsInterstitialBannerSupported { get { return false; } }

        private void FailedDefered(ShowVideoFailed failed)
        {
            if (_deferred != null)
            {
                ErrorDeferred<ShowVideoFailed> errorDeferred = _deferred;
                _deferred = null;
                errorDeferred.Reject(failed);
            }
        }
        public ErrorPromise<ShowVideoFailed> ShowVideo(string zone, bool skipable)
        {
            Initialize();            
            ErrorDeferred<ShowVideoFailed> deferred = new ErrorDeferred<ShowVideoFailed>();
            if (_deferred != null)
            {
                deferred.Reject(ShowVideoFailed.AlreadyShowingVideo);
                return deferred.Promise();
            }
            _deferred = deferred;
            _isSkipable = skipable;
            Tapsell.requestAd(zone, false,
                result =>
                {
                    Debug.Log("On ad exists");
                    // onAdAvailable
                    TapsellShowOptions showOptions = new TapsellShowOptions();
                    showOptions.backDisabled = false;
                    showOptions.immersiveMode = false;
                    showOptions.rotationMode = TapsellShowOptions.ROTATION_UNLOCKED;
                    showOptions.showDialog = true;
                    Tapsell.showAd(result.adId, showOptions);
                },

                zoneId =>
                {
                    Debug.Log("On no ad available");
                    // onNoAdAvailable
                    FailedDefered(ShowVideoFailed.NoVideoAvilable);
                },

                error =>
                {
                    Debug.Log("On Error");
                    // onError
                    FailedDefered(ShowVideoFailed.ProviderError);
                },

                zoneId =>
                {
                    Debug.Log("On no network");
                    // onNoNetwork
                    FailedDefered(ShowVideoFailed.ProviderError);
                },

                result =>
                {
                    Debug.Log("On Expire");
                    // onExpiring
                    FailedDefered(ShowVideoFailed.ProviderError);
                    // this ad is expired, you must download a new ad for this zone
                });
            return deferred.Promise();
            
        }

        public void ChangeStandardBannerPosition(StandardBannerVerticalAlignment verticalAlignment,
            StandardBannerHorizantalAlignment horizantalAlignment)
        {
            throw new NotSupportedException();
        }

        public void ShowStandardBanner()
        {
            throw new NotSupportedException();
        }

        public void HideStandardBanner()
        {
            throw new NotSupportedException();
        }

        public void ShowInterstiatialBanner()
        {
            throw new NotSupportedException();
        }

        #endregion
    }
}
