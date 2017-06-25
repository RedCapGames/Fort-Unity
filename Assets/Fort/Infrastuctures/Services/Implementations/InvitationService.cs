using System;
using System.Collections.Specialized;
using System.Text;
using System.Text.RegularExpressions;
using Fort.Info;
using Fort.Info.Market;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof (IInvitationService))]
    public class InvitationService : MonoBehaviour, IInvitationService
    {
        #region IInvitationService Members

        public void ShareLink()
        {
            try
            {
                if (!ServiceLocator.Resolve<IUserManagementService>().IsRegistered)
                    throw new Exception("User not registered yet");
                ServiceLocator.Resolve<IServerService>().Call<string>("GetInvitaionToken", null).Then(token =>
                {
                    ServiceLocator.Resolve<IAnalyticsService>().StatInvitationShare();
                    AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                    AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaClass intentClass = new AndroidJavaClass("com.redcap.textshare.ShareHelper");
                    StringBuilder builder = new StringBuilder();
                    foreach (MarketInfo marketInfo in InfoResolver.Resolve<FortInfo>().MarketInfos)
                    {
                        builder.AppendLine(string.Format("لینک بازی در {0}", marketInfo.MarketDisplayName));
                        builder.AppendLine(marketInfo.ApplicationUrl);
                    }
                    builder.AppendLine("لینک اضافه شدن سکه به " +
                                       ServiceLocator.Resolve<IUserManagementService>().Username + "(بعد از نصب)");

                    builder.AppendLine(InfoResolver.Resolve<FortInfo>().InvitationInfo.ShareUrl + token);
                    intentClass.CallStatic("ShareText", jo, builder.ToString(), "دعوت و دریافت سکه");
                });
            }
            catch (Exception)
            {
                Debug.LogError("Invitation not supported in editor app");
            }
        }

        public ComplitionPromise<InvitationData> ResolveInvitationData()
        {
            return ServiceLocator.Resolve<IServerService>().Call<InvitationData>("GetUserInvitationInfo", null);
        }

        public Promise ApplyInvitation()
        {
            string invitationToken = ResolveInvitationToken();
            Deferred deferred = new Deferred();
            if (string.IsNullOrEmpty(invitationToken))
            {
                deferred.Reject();
                return deferred.Promise();
            }
            InvitationSaveData invitationSaveData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<InvitationSaveData>();
            ServiceLocator.Resolve<IServerService>().CallTokenFull<object>("ApplyInvitation", new ApplyInvitationData
            {
                InvitorToken = invitationToken,
                InvitationToken = ServiceLocator.Resolve<IUserManagementService>().GetSystemId()
            }).Then(o =>
            {
                invitationSaveData.Applied = true;
                invitationSaveData.InvitationToken = invitationToken;
                ServiceLocator.Resolve<IStorageService>().UpdateData(invitationSaveData);
                ServiceLocator.Resolve<IAnalyticsService>().StatInvitationApplied();
                deferred.Resolve();
            }, () =>
            {
                invitationSaveData.Applied = true;
                invitationSaveData.InvitationToken = invitationToken;
                ServiceLocator.Resolve<IStorageService>().UpdateData(invitationSaveData);
                deferred.Reject();
            });
            return deferred.Promise();
        }

        public string ResolveInvitationToken()
        {
            string invitationToken = ResolveNewInvitationToken();
            InvitationSaveData invitationSaveData =
                ServiceLocator.Resolve<IStorageService>().ResolveData<InvitationSaveData>();
            if (invitationSaveData != null && invitationSaveData.Applied)
                return null;
            if (string.IsNullOrEmpty(invitationToken) && invitationSaveData != null)
                return invitationSaveData.InvitationToken;
            return invitationToken;
        }

        #endregion

        #region Private Methods

        private string ResolveNewInvitationToken()
        {
            try
            {
                AndroidJavaClass intentClass = new AndroidJavaClass("com.redcap.textshare.IntentFilterActivity");
                string url = intentClass.CallStatic<string>("GetUrl");
                if (string.IsNullOrEmpty(url))
                    return null;
                return ParseQueryString(intentClass.CallStatic<string>("GetUrl"))["name"];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static NameValueCollection ParseQueryString(string s)
        {
            NameValueCollection nvc = new NameValueCollection();
            // remove anything other than query string from url
            if (s.Contains("?"))
            {
                s = s.Substring(s.IndexOf('?') + 1);
            }
            foreach (string vp in Regex.Split(s, "&"))
            {
                string[] singlePair = Regex.Split(vp, "=");
                if (singlePair.Length == 2)
                {
                    nvc.Add(singlePair[0], singlePair[1]);
                }
                else
                {
                    // only one key with no value specified in query string
                    nvc.Add(singlePair[0], string.Empty);
                }
            }
            return nvc;
        }

        private void Update()
        {
            string invitationToken = ResolveNewInvitationToken();
            if (!string.IsNullOrEmpty(invitationToken))
            {
                InvitationSaveData invitationSaveData =
                    ServiceLocator.Resolve<IStorageService>().ResolveData<InvitationSaveData>();
                if (invitationSaveData == null || !invitationSaveData.Applied)
                {
                    invitationSaveData = new InvitationSaveData {InvitationToken = invitationToken, Applied = false};
                    ServiceLocator.Resolve<IStorageService>().UpdateData(invitationSaveData);
                }
            }
        }

        #endregion

        #region Nested types

        private class ApplyInvitationData
        {
            #region Properties

            [JsonProperty("invitorToken")]
            public string InvitorToken { get; set; }

            [JsonProperty("invitationToken")]
            public string InvitationToken { get; set; }

            #endregion
        }

        #endregion
    }

    [Serializable]
    public class InvitationSaveData
    {
        #region Properties

        public string InvitationToken { get; set; }
        public bool Applied { get; set; }

        #endregion
    }
}