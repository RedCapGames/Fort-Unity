using Newtonsoft.Json;
using UnityEngine;

namespace Fort.Market
{
    class BazzarMarket:MonoBehaviour, IMarket
    {
        private Deferred<string, MarketPurchaseError> _deferred;

        void Update()
        {
#if UNITY_EDITOR
            return;
#endif
            if (_deferred == null)
                return;
            var jc = new AndroidJavaClass("com.redcap.thugs.PurchaseActivity");
            string lastEvent = jc.CallStatic<string>("GetLastEvent");
            if (!string.IsNullOrEmpty(lastEvent))
            {
                PurchaseResultInfo purchaseResultInfo = JsonConvert.DeserializeObject<PurchaseResultInfo>(lastEvent);

                if (purchaseResultInfo.PurchaseResult == BazzarPurchaseResult.Succeded)
                {
                    Deferred<string, MarketPurchaseError> deferred = _deferred;
                    _deferred = null;
                    deferred.Resolve(purchaseResultInfo.Token);
                }
                else
                {
                    Deferred<string, MarketPurchaseError> deferred = _deferred;
                    _deferred = null;
                    deferred.Reject(purchaseResultInfo.PurchaseResult == BazzarPurchaseResult.Canceled
                        ? MarketPurchaseError.Cancel
                        : MarketPurchaseError.Failed);
                }

            }
        }
        #region Implementation of IMarket

        public Promise<string, MarketPurchaseError> PurchasePackage(string sku, string payload)
        {
            _deferred = new Deferred<string, MarketPurchaseError>();
#if UNITY_EDITOR
            _deferred.Resolve("FiB5nxjC8mxzwxLG");
            return _deferred.Promise();
#endif

            var androidJc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var jo = androidJc.GetStatic<AndroidJavaObject>("currentActivity");
            // Accessing the class to call a static method on it
            var jc = new AndroidJavaClass("com.redcap.thugs.PurchaseActivity");
            // Calling a Call method to which the current activity is passed
            jc.CallStatic("RunActivity", jo, sku, payload);
            return _deferred.Promise();
        }

        #endregion

        private class PurchaseResultInfo
        {
            public BazzarPurchaseResult PurchaseResult { get; set; }
            public string Token { get; set; }
            public string Payload { get; set; }
            public string PackageId { get; set; }
        }

        private enum BazzarPurchaseResult
        {
            Succeded,
            Canceled,
            Error,
        }
    }
}
