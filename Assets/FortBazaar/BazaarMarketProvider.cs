using System.Linq;
using Fort;
using Fort.Info;
using Fort.Market;
using FortBazaar.Info;
using Newtonsoft.Json;
using UnityEngine;

namespace FortBazaar
{
    class BazaarMarketProvider:MonoBehaviour, IMarketProvider
    {
        private Deferred<string, MarketPurchaseError> _deferred;

        void Update()
        {
            if(Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                return;
            if (_deferred == null)
                return;
            var jc = new AndroidJavaClass("com.redcap.thugs.PurchaseActivity");
            string lastEvent = jc.CallStatic<string>("GetLastEvent");
            if (!string.IsNullOrEmpty(lastEvent))
            {
                PurchaseResultInfo purchaseResultInfo = JsonConvert.DeserializeObject<PurchaseResultInfo>(lastEvent);

                if (purchaseResultInfo.PurchaseResult == BazaarPurchaseResult.Succeded)
                {
                    Deferred<string, MarketPurchaseError> deferred = _deferred;
                    _deferred = null;
                    deferred.Resolve(purchaseResultInfo.Token);
                }
                else
                {
                    Deferred<string, MarketPurchaseError> deferred = _deferred;
                    _deferred = null;
                    deferred.Reject(purchaseResultInfo.PurchaseResult == BazaarPurchaseResult.Canceled
                        ? MarketPurchaseError.Cancel
                        : MarketPurchaseError.Failed);
                }

            }
        }
        #region Implementation of IMarket

        public Promise<string, MarketPurchaseError> PurchasePackage(string sku, string payload)
        {
            _deferred = new Deferred<string, MarketPurchaseError>();
            if (Application.platform == RuntimePlatform.OSXEditor ||
                Application.platform == RuntimePlatform.WindowsEditor)
            {
                _deferred.Resolve("FiB5nxjC8mxzwxLG");
                return _deferred.Promise();

            }
            var androidJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = androidJc.GetStatic<AndroidJavaObject>("currentActivity");
            // Accessing the class to call a static method on it
            var jc = new AndroidJavaClass("com.redcap.thugs.PurchaseActivity");
            // Calling a Call method to which the current activity is passed
            jc.CallStatic("RunActivity", jo, sku, payload, ((BazaarMarketInfo)FortInfo.Instance.MarketInfos.First(info => info.MarketName == "Bazaar")).Key);
            return _deferred.Promise();
        }

        #endregion

        private class PurchaseResultInfo
        {
            public BazaarPurchaseResult PurchaseResult { get; set; }
            public string Token { get; set; }
            public string Payload { get; set; }
            public string PackageId { get; set; }
        }

        private enum BazaarPurchaseResult
        {
            Succeded,
            Canceled,
            Error,
        }
    }
}
