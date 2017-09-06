using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Fort;
using Fort.AssetBundle;
using Fort.Build;
using Fort.Info;
using Fort.Info.Achievement;
using Fort.Info.Advertisement;
using Fort.Info.PurchasableItem;
using UnityEditor;
using UnityEngine;

namespace Assets.Fort.Editor.Publish
{
    public static class PublishMenuItems
    {
        [MenuItem("Fort/Publish/Syncronize Achievements")]
        public static void UpdateAchievementToServer()
        {
            InternalUpdateAchievementToServer(true);
        }

        private static Promise InternalUpdateAchievementToServer(bool showDialog)
        {
            Deferred deferred = new Deferred();
            //BacktoryCloudUrl.Url = "http://localhost:8086";
            UpdateAchievementsRequest request = new UpdateAchievementsRequest();
            List<ServerAchievement> serverAchievements = new List<ServerAchievement>();
            foreach (
                AchievementInfo achievementInfo in
                    InfoResolver.Resolve<FortInfo>().Achievement.AchievementInfos.Where(info => info != null))
            {
                NoneLevelBaseAchievementInfo noneLevelBaseAchievementInfo = achievementInfo as NoneLevelBaseAchievementInfo;
                if (noneLevelBaseAchievementInfo != null)
                {
                    serverAchievements.Add(new ServerAchievement
                    {
                        Name = noneLevelBaseAchievementInfo.Name,
                        AchievementId = noneLevelBaseAchievementInfo.Id,
                        Score = noneLevelBaseAchievementInfo.Score,
                        Values = noneLevelBaseAchievementInfo.Balance.Values
                    });
                }
                else
                {
                    LevelBaseAchievementInfo levelBaseAchievementInfo = (LevelBaseAchievementInfo)achievementInfo;
                    AchievementLevelInfo[] achievementLevelInfos = levelBaseAchievementInfo.GetAchievementLevelInfos();
                    for (int i = 0; i < achievementLevelInfos.Length; i++)
                    {
                        serverAchievements.Add(new ServerAchievement
                        {
                            Name = achievementLevelInfos[i].Name,
                            AchievementId = achievementLevelInfos[i].Id,
                            Score = achievementLevelInfos[i].Score,
                            Values = achievementLevelInfos[i].Balance.Values
                        });
                    }
                }
            }
            request.Items = serverAchievements.ToArray();
            EditorUtility.DisplayProgressBar("Syncronizing Achievements", "Syncronizing Achievements", 0);
            InfoResolver.Resolve<FortInfo>()
                .ServerConnectionProvider.EditorConnection.Call<object>("UpdateAchievements", request)
                .Then(
                    o =>
                    {
                        EditorUtility.ClearProgressBar();
                        if (showDialog)
                            EditorUtility.DisplayDialog("Syncronizing Achievements", "Achievemet syncronization succeeded", "Ok");
                        deferred.Resolve();
                    }, error =>
                    {
                        EditorUtility.ClearProgressBar();
                        deferred.Reject();
                        throw new Exception("Achievemet syncronization failed");
                    });
            return deferred.Promise();
        }

        [MenuItem("Fort/Publish/Syncronize Purchasable Items")]
        public static void UpdatePurchasableItemsToServer()
        {
            InternalUpdatePurchasableItemsToServer(true);
        }

        private static Promise InternalUpdatePurchasableItemsToServer(bool showDialog)
        {
            Deferred deferred = new Deferred();
            //BacktoryCloudUrl.Url = "http://localhost:8086";
            UpdateItemsRequest request = new UpdateItemsRequest();
            List<ServerPurchasableItem> serverPurchasableItems = new List<ServerPurchasableItem>();
            foreach (
                PurchasableItemInfo purchasableItemInfo in
                    InfoResolver.Resolve<FortInfo>().Purchase.GetAllPurchasableItemInfos().Where(info => info != null))
            {
                NoneLevelBasePurchasableItemInfo noneLevelBasePurchasableItemInfo =
                    purchasableItemInfo as NoneLevelBasePurchasableItemInfo;
                if (noneLevelBasePurchasableItemInfo != null)
                {
                    serverPurchasableItems.Add(new ServerPurchasableItem
                    {
                        ItemId = noneLevelBasePurchasableItemInfo.Id,
                        Name = noneLevelBasePurchasableItemInfo.Name,
                        PurchaseCost = noneLevelBasePurchasableItemInfo.Costs.Purchase.Values,
                        RentCost = noneLevelBasePurchasableItemInfo.Costs.Rent.Values
                    });
                }
                else
                {
                    LevelBasePurchasableItemInfo levelBasePurchasableItemInfo =
                        (LevelBasePurchasableItemInfo)purchasableItemInfo;
                    PurchasableLevelInfo[] purchasableLevelInfos = levelBasePurchasableItemInfo.GetPurchasableLevelInfos();
                    for (int i = 0; i < purchasableLevelInfos.Length; i++)
                    {
                        serverPurchasableItems.Add(new ServerPurchasableItem
                        {
                            ItemId = purchasableLevelInfos[i].Id,
                            Name = purchasableLevelInfos[i].Name,
                            PurchaseCost = purchasableLevelInfos[i].Costs.Purchase.Values,
                            RentCost = purchasableLevelInfos[i].Costs.Rent.Values
                        });
                    }
                }
            }
            request.Items = serverPurchasableItems.ToArray();
            EditorUtility.DisplayProgressBar("Syncronizing Purchasable items", "Syncronizing Purchasable items", 0);
            InfoResolver.Resolve<FortInfo>()
                .ServerConnectionProvider.EditorConnection.Call<object>("UpdateItems", request)
                .Then(
                    o =>
                    {
                        EditorUtility.ClearProgressBar();
                        if (showDialog)
                            EditorUtility.DisplayDialog("Syncronizing Purchasable items",
                                "Purchasable items syncronization succeeded", "Ok");
                        deferred.Resolve();
                    }, error =>
                    {
                        EditorUtility.ClearProgressBar();
                        deferred.Reject();
                        throw new Exception("purchasable items syncronization failed");
                    });
            return deferred.Promise();
        }

        [MenuItem("Fort/Publish/Syncronize Server Settings")]
        public static void UpdateServerSettings()
        {
            InternalUpdateServerSettings(true);
        }

        private static Promise InternalUpdateServerSettings(bool showDialog)
        {
            Deferred deferred = new Deferred();
            ServerSettings serverSettings = new ServerSettings();
            serverSettings.ValuesDefenition = InfoResolver.Resolve<FortInfo>().ValueDefenitions;
            serverSettings.InvitationPrize = InfoResolver.Resolve<FortInfo>().InvitationInfo.InvitationPrize;
            serverSettings.StartupBalance = InfoResolver.Resolve<FortInfo>().StartupBalance;

            AdvertisementSettings advertisementSettings = new AdvertisementSettings();
            advertisementSettings.VideoPriority = InfoResolver.Resolve<FortInfo>().Advertisement.AdvertisementProviders.Where(
                priority =>
                    priority.AdvertisementProvider != null && priority.AdvertisementProvider.IsVideoSupported)
                .Select((priority, i) => new { Priority = priority, Index = i })
                .OrderBy(arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                .Select(arg => arg.Priority.AdvertisementProvider.Name)
                .ToArray();
            AdvertisementPriority advertisementPriority =
                InfoResolver.Resolve<FortInfo>().Advertisement.AdvertisementProviders.Where(
                    priority =>
                        priority.AdvertisementProvider != null &&
                        priority.AdvertisementProvider.IsStandardBannerSupported)
                    .Select((priority, i) => new { Priority = priority, Index = i })
                    .OrderBy(
                        arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                    .Select(arg => arg.Priority)
                    .FirstOrDefault();
            if (advertisementPriority != null)
                advertisementSettings.StandardBannerPriority = advertisementPriority.AdvertisementProvider.Name;
            else
                advertisementSettings.StandardBannerPriority = string.Empty;
            advertisementPriority =
                InfoResolver.Resolve<FortInfo>().Advertisement.AdvertisementProviders.Where(
                    priority =>
                        priority.AdvertisementProvider != null &&
                        priority.AdvertisementProvider.IsInterstitialBannerSupported)
                    .Select((priority, i) => new { Priority = priority, Index = i })
                    .OrderBy(
                        arg => arg.Priority.VideoPriority < 0 ? arg.Index * 10000 : arg.Priority.VideoPriority)
                    .Select(arg => arg.Priority)
                    .FirstOrDefault();
            if (advertisementPriority != null)
                advertisementSettings.InterstiatialBannerPriority = advertisementPriority.AdvertisementProvider.Name;
            else
                advertisementSettings.InterstiatialBannerPriority = string.Empty;
            serverSettings.AdvertisementSettings = advertisementSettings;
            EditorUtility.DisplayProgressBar("Syncronizing Server settings", "Syncronizing Server settings", 0);
            InfoResolver.Resolve<FortInfo>()
                .ServerConnectionProvider.EditorConnection.Call<object>("UpdateSettings",
                    new UpdateServerSettingRequest { ServerSettings = serverSettings })
                .Then(
                    o =>
                    {
                        EditorUtility.ClearProgressBar();
                        if (showDialog)
                            EditorUtility.DisplayDialog("Syncronizing Server settings", "Server settings syncronization succeeded",
                                "Ok");
                        deferred.Resolve();
                    }, error =>
                    {
                        EditorUtility.ClearProgressBar();
                        deferred.Reject();
                        throw new Exception("Server settings syncronization failed");
                    });
            return deferred.Promise();
        }

        [MenuItem("Fort/Publish/Build")]
        public static void Build()
        {
            LanguageFixPreprocessBuild.FixLanguage();
            string bundleVersion = PlayerSettings.bundleVersion;
            AssetDatabaseHelper.CreateFolderRecursive("/Assets/Fort/Resources");
            string path = Path.Combine(Application.dataPath, "Fort/Resources/Version.txt");
            using (StreamWriter writer = new StreamWriter(path))
            {
                writer.Write(bundleVersion);
            }
            AssetDatabase.Refresh();
            BuildPipeline.BuildPlayer(EditorBuildSettings.scenes.Select(scene => scene.path).ToArray(), EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget), EditorUserBuildSettings.activeBuildTarget, BuildOptions.None);
            string buildLocation = EditorUserBuildSettings.GetBuildLocation(EditorUserBuildSettings.activeBuildTarget);
            string argument = "/select, \"" + buildLocation.Replace("/","\\") + "\"";

            Process.Start("explorer.exe", argument);
            
        }

        [MenuItem("Fort/Publish/Complete Build")]
        public static void CompleteBuild()
        {
            AssetBundleBuilder.SyncAssetBundles().Then(() =>
            {
                InternalUpdateAchievementToServer(false).Then(() =>
                {
                    InternalUpdatePurchasableItemsToServer(false).Then(() =>
                    {
                        InternalUpdateServerSettings(false).Then(() =>
                        {
                            Build();
                        });
                    });
                });
            });
        }
        class UpdateAchievementsRequest
        {
            public ServerAchievement[] Items { get; set; }
        }

        class UpdateItemsRequest
        {
            public ServerPurchasableItem[] Items { get; set; }
        }

        class UpdateServerSettingRequest
        {
            public ServerSettings ServerSettings { get; set; }
        }
    }
}
