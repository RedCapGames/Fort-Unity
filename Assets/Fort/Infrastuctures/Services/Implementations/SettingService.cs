using System;
using System.Collections.Generic;
using Fort.Info;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(ISettingService))]
    public class SettingService : MonoBehaviour, ISettingService
    {
        List<ComplitionDeferred<ServerSettings>> _deferreds = new List<ComplitionDeferred<ServerSettings>>();
        void Start()
        {
            if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider != null)
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

        public Version GetVersion()
        {
            TextAsset textAsset = Resources.Load<TextAsset>("Version");
            if(textAsset == null)
                return new Version(1,0);
            try
            {
                return new Version(textAsset.text);
            }
            catch (Exception)
            {
                return new Version(1, 0);
            }
            
        }

        #endregion
    }

    public class ServerSettings
    {
        public string[] ValuesDefenition { get; set; }
        [JsonProperty("StartupValues")]
        public Balance StartupBalance { get; set; }
        public Balance InvitationPrize { get; set; }
        //public bool IsPublished { get; set; }
        public AdvertisementSettings AdvertisementSettings { get; set; }

    }

    public class AdvertisementSettings
    {
        public string[] VideoPriority { get; set; }
        public string StandardBannerPriority { get; set; }
        public string InterstiatialBannerPriority { get; set; }
    }
}