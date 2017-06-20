using System.Collections.Generic;
using Backtory.Core.Public;
using Fort.Info;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(ISettingService))]
    public class SettingService : MonoBehaviour,ISettingService
    {
        List<ComplitionDeferred<ServerSettings>> _deferreds = new List<ComplitionDeferred<ServerSettings>>();
        void Start()
        {
            ResolveServerSettings();
        }
        #region Implementation of ISettingService

        public ComplitionPromise<ServerSettings> ResolveServerSettings()
        {
            if (_deferreds.Count > 0)
            {
                ComplitionDeferred<ServerSettings> deferred = new ComplitionDeferred<ServerSettings>();
                _deferreds.Add(deferred);
                return deferred.Promise();
            }
            ComplitionDeferred<ServerSettings> result = new ComplitionDeferred<ServerSettings>();
            _deferreds.Add(result);
            ServiceLocator.Resolve<IServerService>().Call<ServerSettings>("GetSettings").Then(settings =>
            {
                ComplitionDeferred<ServerSettings>[] complitionDeferreds = _deferreds.ToArray();
                _deferreds.Clear();
                ServiceLocator.Resolve<IStorageService>().UpdateData(settings);
                foreach (ComplitionDeferred<ServerSettings> deferred in complitionDeferreds)
                {
                    deferred.Resolve(settings);
                }
            }, () =>
            {
                ComplitionDeferred<ServerSettings>[] complitionDeferreds = _deferreds.ToArray();
                _deferreds.Clear();
                foreach (ComplitionDeferred<ServerSettings> deferred in complitionDeferreds)
                {
                    deferred.Reject();
                }

            });
            return result.Promise();
        }

        public ServerSettings ResolveCachedServerSetting()
        {
            return ServiceLocator.Resolve<IStorageService>().ResolveData<ServerSettings>();
        }

        #endregion
    }

    public class ServerSettings
    {
        [JsonProperty("StartupValues")]
        public Balance StartupBalance { get; set; }
        public Balance InvitationPrize { get; set; }
        public bool IsPublished { get; set; }
        public AdvertisementSettings AdvertisementSettings { get; set; }

    }

    public class AdvertisementSettings
    {
        public string[] VideoPriority { get; set; }
        public string StandartBannerPriority { get; set; }
        public string InterstiatialBannerPriority { get; set; }
    }
}