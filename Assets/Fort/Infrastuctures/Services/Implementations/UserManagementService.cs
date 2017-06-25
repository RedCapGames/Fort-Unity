using System;
using System.Collections.Generic;
using System.Linq;
//using Backtory.Core.Public;
using Fort.Info;
using Fort.Info.PurchasableItem;
using Fort.ServerConnection;
using Newtonsoft.Json;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(IUserManagementService))]
    public class UserManagementService : MonoBehaviour, IUserManagementService
    {
        List<Deferred> _fullUpdateDeferreds = new List<Deferred>();

        void Start()
        {
            if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider != null && IsRegistered)
                FullUpdate();
        }
        #region Implementation of IUserManagementService

        public bool IsRegistered
        {
            get
            {
                AuthenticationInfo authenticationInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<AuthenticationInfo>();
                return authenticationInfo != null;
            }
        }

        public string Username
        {
            get
            {
                AuthenticationInfo authenticationInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<AuthenticationInfo>();
                if (authenticationInfo == null)
                    return string.Empty;
                return authenticationInfo.UserName;
            }
        }

        public int Score
        {
            get
            {
                UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>();
                if (userInfo == null)
                    return 0;
                return userInfo.Score;
            }
        }

        public Balance Balance
        {
            get
            {
                UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>();
                if (userInfo == null)
                {
                    Balance result = new Balance();
                    result.SyncValues();
                    return result;
                }
                return userInfo.Balance;

            }
        }

        public ErrorPromise<RegisterationErrorResultStatus> Register(string username, string password)
        {
            ErrorDeferred<RegisterationErrorResultStatus> deferred = new ErrorDeferred<RegisterationErrorResultStatus>();
            if(InfoResolver.Resolve<FortInfo>().ServerConnectionProvider == null)
                throw new Exception("No Server connection provider added");
            InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.Register(username,password).Then(() =>
            {
                ServiceLocator.Resolve<IAnalyticsService>().StatUserRegisterd();
                ServiceLocator.Resolve<IStorageService>().UpdateData(new AuthenticationInfo { UserName = username});
                deferred.Resolve();
            }, status => deferred.Reject(status));
            return deferred.Promise();
        }

        public Promise Login(string username, string password)
        {
            if (InfoResolver.Resolve<FortInfo>().ServerConnectionProvider == null)
                throw new Exception("No Server connection provider added");
            return InfoResolver.Resolve<FortInfo>().ServerConnectionProvider.UserConnection.Login(username, password);
        }

        public void AddScoreAndBalance(int score, Balance balance)
        {
            UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>() ?? new UserInfo();
            userInfo.Balance.SyncValues();
            userInfo.Balance += balance;
            userInfo.Score += score;
            userInfo.AddedScoreBalance.Score += score;
            if (userInfo.AddedScoreBalance.Balance == null)
                userInfo.AddedScoreBalance.Balance = balance;
            else
                userInfo.AddedScoreBalance.Balance += balance;
            ServiceLocator.Resolve<IStorageService>().UpdateData(userInfo);
        }

        public Promise FullUpdate()
        {
            Deferred deferred = new Deferred();
            if (_fullUpdateDeferreds.Count == 0)
            {
                _fullUpdateDeferreds.Add(deferred);
                InternalFullUpdate().Then(() =>
                {
                    Deferred[] deferreds = _fullUpdateDeferreds.ToArray();
                    _fullUpdateDeferreds.Clear();
                    foreach (Deferred def in deferreds)
                    {
                        def.Resolve();
                    }
                }, () =>
                {
                    Deferred[] deferreds = _fullUpdateDeferreds.ToArray();
                    _fullUpdateDeferreds.Clear();
                    foreach (Deferred def in deferreds)
                    {
                        def.Reject();
                    }
                });
            }
            else
            {
                _fullUpdateDeferreds.Add(deferred);
            }
            return deferred.Promise();
        }

        public string GetSystemId()
        {
            if (Application.platform == RuntimePlatform.OSXEditor || Application.platform == RuntimePlatform.WindowsEditor)
                return Guid.NewGuid().ToString();
            if (Application.platform == RuntimePlatform.Android)
            {
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
                AndroidJavaObject jo = jc.GetStatic<AndroidJavaObject>("currentActivity");
                AndroidJavaObject tm = jo.Call<AndroidJavaObject>("getSystemService", new object[] { "phone" });
                string imei = tm.Call<string>("getDeviceId");
                return imei;
            }
            return Guid.NewGuid().ToString();
        }

        private Promise InternalFullUpdate()
        {
            Deferred deferred = new Deferred();
            if (!IsRegistered)
            {
                deferred.Reject();
                return deferred.Promise();
            }
            UserInfo userInfo = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>() ?? new UserInfo();
            if (!userInfo.AddedScoreBalance.IsEmpty())
                userInfo.NotSyncedScoreBalances.Add(Guid.NewGuid().ToString(), userInfo.AddedScoreBalance);
            userInfo.AddedScoreBalance = new ScoreBalance();
            FullData fullData = new FullData();
            fullData.AddScoreDatas =
                userInfo.NotSyncedScoreBalances.Select(
                    pair => new AddScoreData { Token = pair.Key, Score = pair.Value.Score, Values = pair.Value.Balance })
                    .ToArray();
            fullData.AchievementIds =
                ((AchievementService)ServiceLocator.Resolve<IAchievementService>()).GetNotSyncAchievementIds();
            fullData.PurchasedItemIds = ((StoreService)ServiceLocator.Resolve<IStoreService>()).GetNotPurchasableIds();
            if (fullData.AddScoreDatas.Length == 0 && fullData.AchievementIds.Length == 0 &&
                fullData.PurchasedItemIds.Length == 0)
            {
                deferred.Resolve();
                return deferred.Promise();
            }
            ServiceLocator.Resolve<IServerService>().CallTokenFull<FullDataResult>("FullUpdateData", fullData).Then(
                result =>
                {
                    UserInfo info = ServiceLocator.Resolve<IStorageService>().ResolveData<UserInfo>() ?? new UserInfo();
                    info.Score = result.UserData.Score;
                    info.Balance = result.UserData.Values;
                    if (!userInfo.AddedScoreBalance.IsEmpty())
                    {
                        info.Score += userInfo.AddedScoreBalance.Score;
                        info.Balance += userInfo.AddedScoreBalance.Balance;
                    }
                    info.NotSyncedScoreBalances = new Dictionary<string, ScoreBalance>();
                    ServiceLocator.Resolve<IStorageService>().UpdateData(info);
                    ServiceLocator.Resolve<IServerService>().Call<ServerAchievement[]>("GetAchievements", null).Then(
                        achievements =>
                        {
                            ServiceLocator.Resolve<IServerService>()
                                .Call<string[]>("GetClaimedAchievements", null)
                                .Then(
                                    claimedAchievements =>
                                    {
                                        ((AchievementService)ServiceLocator.Resolve<IAchievementService>())
                                            .OnServerAchievementResolved(
                                                achievements.ToDictionary(achievement => achievement.AchievementId,
                                                    achievement =>
                                                        new AchievementService.ServerAchievementInfo
                                                        {
                                                            Balance = new Balance { Values = achievement.Values },
                                                            Score = achievement.Score
                                                        }), claimedAchievements);
                                        ServiceLocator.Resolve<IServerService>().Call<ServerPurchasableItem[]>("GetItems", null).Then(
                                            items =>
                                            {
                                                ServiceLocator.Resolve<IServerService>().Call<string[]>("GetPurchasedItems", null).Then(
                                                    purchasedItems =>
                                                    {
                                                        ((StoreService)ServiceLocator.Resolve<IStoreService>())
                                                            .OnServerPurchaseResolved(
                                                                items.ToDictionary(item => item.ItemId,
                                                                    item => new ItemCosts
                                                                    {
                                                                        Purchase =
                                                                            new Balance { Values = item.PurchaseCost },
                                                                        Rent = new Balance { Values = item.RentCost }
                                                                    }), purchasedItems);
                                                        deferred.Resolve();
                                                    }, () => deferred.Reject());
                                            }, () => deferred.Reject());
                                    }, () => deferred.Reject());
                        }, () => deferred.Reject());

                }, () => deferred.Reject());
            return deferred.Promise();

        }

        #endregion

        private class FullData
        {
            [JsonProperty("AddScoreValuesDatas")]
            public AddScoreData[] AddScoreDatas { get; set; }
            [JsonProperty("AchievementData")]
            public string[] AchievementIds { get; set; }
            [JsonProperty("PurchaseData")]
            public string[] PurchasedItemIds { get; set; }
        }

        private class FullDataResult
        {
            [JsonProperty("userData")]
            public FullDataUserData UserData { get; set; }
        }

        public class FullDataUserData
        {
            public int Score { get; set; }
            public Balance Values { get; set; }
        }
        private class AddScoreData
        {
            [JsonProperty("addToken")]
            public string Token { get; set; }
            [JsonProperty("score")]
            public int Score { get; set; }
            [JsonProperty("values")]            
            public Balance Values { get; set; }
        }
    }

    public class AuthenticationInfo
    {
        public string UserName { get; set; }
    }

    public class UserInfo
    {
        public UserInfo()
        {
            AddedScoreBalance = new ScoreBalance();
            NotSyncedScoreBalances = new Dictionary<string, ScoreBalance>();
            Balance = new Balance();
        }
        public int Score { get; set; }
        public Balance Balance { get; set; }
        public ScoreBalance AddedScoreBalance { get; set; }
        public Dictionary<string, ScoreBalance> NotSyncedScoreBalances { get; set; }
    }
}
