using System;
using System.Collections.Generic;
using Fort.Info;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(ILevelManagementService))]
    public class LevelManagementService : MonoBehaviour,ILevelManagementService
    {
        #region Implementation of ILevelManagementService

        public void GameLevelFinished(LevelFinishParameters parameters)
        {
            GameLevelInfo level = GetLastLoadedLevel();
            ServiceLocator.Resolve<IAnalyticsService>().StatGameLevelFinished(level,parameters.LevelFinishStat);
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                                          new GameSavedData();
            if (gameSavedData.LevelFinishStats.ContainsKey(level.Id))
            {
                if (gameSavedData.LevelFinishStats[level.Id].CompareTo(parameters.LevelFinishStat) == -1)
                {
                    gameSavedData.LevelFinishStats[level.Id] = parameters.LevelFinishStat;
                }
            }
            else
            {
                gameSavedData.LevelFinishStats[level.Id] = parameters.LevelFinishStat;
            }
            ServiceLocator.Resolve<IStorageService>().UpdateData(gameSavedData);
            switch (parameters.TransitionType)
            {
                case LevelFinishSceneTransitionType.Stay:
                    break;
                case LevelFinishSceneTransitionType.MoveToScene:
                    ServiceLocator.Resolve<ISceneLoaderService>().Load(new SceneLoadParameters(parameters.TransitionScene)
                    {
                        AddToSceneStack = true,
                        CaptureReturnKey = false,
                        Context= parameters.LevelFinishStat,
                        FlushSceneStack = true
                    });
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public ILevelFinishStat GetGameFinishStat(GameLevelInfo level)
        {
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                              new GameSavedData();
            if (gameSavedData.LevelFinishStats.ContainsKey(level.Id))
                return gameSavedData.LevelFinishStats[level.Id];
            return null;
        }

        public void LoadGameLevel(GameLevelInfo level)
        {
            string gameLevelSceneName = ResolveGameLevelSceneName(level);
            if(string.IsNullOrEmpty(gameLevelSceneName))
                throw new Exception(string.Format("Scene name of Level {0} cannot be resloved.",level.Name));
            ServiceLocator.Resolve<ISceneLoaderService>().Load(new SceneLoadParameters(gameLevelSceneName)
            {
                AddToSceneStack = true,
                CaptureReturnKey = false,
                Context = level,
                FlushSceneStack = true
            });
        }

        public GameLevelInfo GetLastLoadedLevel()
        {
            return (GameLevelInfo) ServiceLocator.Resolve<ISceneLoaderService>().GetLastLoadContext();
        }

        #endregion

        private string ResolveGameLevelSceneName(GameLevelInfo level)
        {
            if (!string.IsNullOrEmpty(level.SceneName))
                return level.SceneName;
            string categorySceneName = GetCategorySceneName(InfoResolver.FortInfo.GameLevel.LevelCategoriesParentMap[level.Id]);
            if (!string.IsNullOrEmpty(categorySceneName))
                return categorySceneName;
            return InfoResolver.FortInfo.GameLevel.DefaultSceneName;
        }

        private string GetCategorySceneName(GameLevelCategory category)
        {
            if (!string.IsNullOrEmpty(category.DefaultSceneName))
                return category.DefaultSceneName;
            if (InfoResolver.FortInfo.GameLevel.LevelCategoriesParentMap.ContainsKey(category.Id))
            {
                return GetCategorySceneName(InfoResolver.FortInfo.GameLevel.LevelCategoriesParentMap[category.Id]);
            }
            return string.Empty;
        }
        private class GameSavedData
        {
            public GameSavedData()
            {
                LevelFinishStats = new Dictionary<string, ILevelFinishStat>();
            }
            public Dictionary<string,ILevelFinishStat> LevelFinishStats { get; set; } 
        }
    }
    
}