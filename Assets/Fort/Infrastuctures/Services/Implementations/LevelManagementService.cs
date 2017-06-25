using System;
using System.Collections.Generic;
using Fort.Info;
using Fort.Info.GameLevel;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public void LoadGameLevelAsync(GameLevelInfo level)
        {
            if(FortScene.IsNullOrEmpty(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value))
                throw new Exception("No Loader Scene is defined in Game Level config");
            ServiceLocator.Resolve<ISceneLoaderService>().Load(new SceneLoadParameters(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value.SceneName)
            {
                AddToSceneStack = true,
                CaptureReturnKey = false,
                Context = level,
                FlushSceneStack = true
            });

        }

        public AsyncOperation ContinueLoadGameLevelAsync()
        {

            return
                ServiceLocator.Resolve<ISceneLoaderService>()
                    .LoadAsync(new SceneLoadParameters(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value.SceneName)
                    {
                        AddToSceneStack = true,
                        CaptureReturnKey = false,
                        Context = GetLastLoadedLevel(),
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
            if (!FortScene.IsNullOrEmpty(level.Scene))
                return level.Scene.Value.SceneName;
            string categorySceneName = GetCategorySceneName(InfoResolver.Resolve<FortInfo>().GameLevel.LevelCategoriesParentMap[level.Id]);
            if (!string.IsNullOrEmpty(categorySceneName))
                return categorySceneName;
            return FortScene.IsNullOrEmpty(InfoResolver.Resolve<FortInfo>().GameLevel.DefaultScene)?null: InfoResolver.Resolve<FortInfo>().GameLevel.DefaultScene.Value.SceneName;
        }

        private string GetCategorySceneName(GameLevelCategory category)
        {
            if (!FortScene.IsNullOrEmpty(category.DefaultScene))
                return category.DefaultScene.Value.SceneName;
            if (InfoResolver.Resolve<FortInfo>().GameLevel.LevelCategoriesParentMap.ContainsKey(category.Id))
            {
                return GetCategorySceneName(InfoResolver.Resolve<FortInfo>().GameLevel.LevelCategoriesParentMap[category.Id]);
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