using Fort.Inspector;
using UnityEditor;

namespace Fort.Info
{
    public static class FortInfoAsest
    {
/*        [MenuItem("Assets/Create/FortInfo")]
        public static void CreateAsset()
        {
            CustomAssetUtility.CreateAsset<FortInfoScriptable>();
        }*/
/*        [MenuItem("Fort/Server Sync/Generate synchronization file")]
        public static void GenerateSynchronizationFile()
        {
            string path = EditorUtility.SaveFilePanel("ServerFortInfo", "", "ServerFortInfo.json", "json");
            if (string.IsNullOrEmpty(path))
                return;

            ServerInfo serverInfo = new ServerInfo();
            FortInfoScriptable fortInfoScriptable = Resources.Load<FortInfoScriptable>("FortInfo");
            FortInfo fortInfo = (FortInfo)fortInfoScriptable.Load(typeof (FortInfo));
            Dictionary<string, int> defaultBalance = fortInfo.ValueDefenitions.ToDictionary(s => s, s => 0);
            serverInfo.ValueDefenition = fortInfo.ValueDefenitions;
            serverInfo.ServerAchievements =
                fortInfo.Achievement.AchievementInfos.OfType<NoneLevelBaseAchievementInfo>()
                    .Select(info => new ServerAchievement
                    {
                        Name = info.GetType().Name,
                        AchievementId = info.Id,
                        Score = info.Score,
                        Values = info.Balance==null? defaultBalance: info.Balance.Values
                    })
                    .Concat(
                        fortInfo.Achievement.AchievementInfos.OfType<LevelBaseAchievementInfo>()
                            .SelectMany(info => info.GetAchievementLevelInfos().Select((levelInfo, i) => new Tuple<int, Tuple<AchievementLevelInfo, LevelBaseAchievementInfo>>(i, new Tuple<AchievementLevelInfo, LevelBaseAchievementInfo>(levelInfo, info))))
                            .Select(info => new ServerAchievement
                            {
                                Name = string.Format("{0}_{1}", info.Item2.Item2.GetType().Name, info.Item1),
                                AchievementId = info.Item2.Item1.Id,
                                Score = info.Item2.Item1.Score,
                                Values = info.Item2.Item1.Balance==null ? defaultBalance : info.Item2.Item1.Balance.Values
                            })).ToArray();
            PurchasableItemInfo[] allPurchasableItemInfos = fortInfo.Purchase.GetAllPurchasableItemInfos();
            serverInfo.ServerPurchasableItems =
                allPurchasableItemInfos.OfType<NoneLevelBasePurchasableItemInfo>()
                    .Select(info => new ServerPurchasableItem
                    {
                        ItemId = info.Id,
                        Costs = info.PurchaseCost == null ? defaultBalance : info.PurchaseCost.Values
                    })
                    .Concat(
                        allPurchasableItemInfos.OfType<LevelBasePurchasableItemInfo>()
                            .SelectMany(info  => info.GetPurchasableLevelInfos().Select((levelInfo, i) => new Tuple<int, Tuple<PurchasableLevelInfo, LevelBasePurchasableItemInfo>>(i, new Tuple<PurchasableLevelInfo, LevelBasePurchasableItemInfo>(levelInfo, info))))
                            .Select(token => new ServerPurchasableItem
                            {
                                Name = string.Format("{0}_{1}",token.Item2.Item2.GetType().Name,token.Item1),
                                ItemId = token.Item2.Item1.Id,
                                Costs = token.Item2.Item1.PurchaseCost==null ? defaultBalance : token.Item2.Item1.PurchaseCost.Values
                            })).ToArray();
            using (StreamWriter writer = new StreamWriter(path,false))
            {
                writer.Write(JsonConvert.SerializeObject(serverInfo));
            }
        }*/

    }

}
