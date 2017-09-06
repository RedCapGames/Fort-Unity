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
        private ILevelFinishStat _lastFinishStat;
        private GameLevelInfo _lastLoadGameAsync;
        #region Implementation of ILevelManagementService

        public void GameLevelFinished(LevelFinishParameters parameters)
        {
            _lastFinishStat = parameters.LevelFinishStat;
            GameLevelInfo level = GetLastLoadedLevel();
            ServiceLocator.Resolve<IAnalyticsService>().StatGameLevelFinished(level,parameters.LevelFinishStat);
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                                          new GameSavedData();
            gameSavedData.LastFinishedLevelId = level.Id;
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
                        Context= GetLastLoadedLevel(),
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

        public ILevelFinishStat GetLastGameFinishStat()
        {
            return _lastFinishStat;
        }

        public void LoadGameLevel(GameLevelInfo level)
        {
            string gameLevelSceneName = ResolveGameLevelSceneName(level);
            if(string.IsNullOrEmpty(gameLevelSceneName))
                throw new Exception(string.Format("Scene name of Level {0} cannot be resloved.",level.Name));
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                              new GameSavedData();
            gameSavedData.LastLoadedLevelId = level.Id;
            ServiceLocator.Resolve<IStorageService>().UpdateData(gameSavedData);
            ServiceLocator.Resolve<ISceneLoaderService>().Load(new SceneLoadParameters(gameLevelSceneName)
            {
                AddToSceneStack = false,
                CaptureReturnKey = false,
                Context = level,
                FlushSceneStack = true
            });
        }

        public void LoadGameLevelAsync(GameLevelInfo level)
        {
            if(FortScene.IsNullOrEmpty(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value))
                throw new Exception("No Loader Scene is defined in Game Level config");
            _lastLoadGameAsync = level;
            ServiceLocator.Resolve<ISceneLoaderService>().Load(new SceneLoadParameters(InfoResolver.Resolve<FortInfo>().GameLevel.LoaderScene.Value.SceneName)
            {
                AddToSceneStack = false,
                CaptureReturnKey = false,
                Context = level,
                FlushSceneStack = true
            });

        }

        public AsyncOperation ContinueLoadGameLevelAsync()
        {
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                  new GameSavedData();
            gameSavedData.LastLoadedLevelId = _lastLoadGameAsync.Id;
            ServiceLocator.Resolve<IStorageService>().UpdateData(gameSavedData);
            return
                ServiceLocator.Resolve<ISceneLoaderService>()
                    .LoadAsync(new SceneLoadParameters(ResolveGameLevelSceneName(_lastLoadGameAsync))
                    {
                        AddToSceneStack = false,
                        CaptureReturnKey = false,
                        Context = GetLastLoadedLevel(),
                        FlushSceneStack = true
                    });
        }

        public GameLevelInfo GetLastLoadedLevel()
        {
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                                          new GameSavedData();
            if(string.IsNullOrEmpty(gameSavedData.LastLoadedLevelId))
                return null;
            if(!FortInfo.Instance.GameLevel.GameLevelInfos.ContainsKey(gameSavedData.LastLoadedLevelId))
                return null;
            return FortInfo.Instance.GameLevel.GameLevelInfos[gameSavedData.LastLoadedLevelId];            
        }

        public GameLevelInfo GetLastFinishedLevel()
        {
            GameSavedData gameSavedData = ServiceLocator.Resolve<IStorageService>().ResolveData<GameSavedData>() ??
                                          new GameSavedData();
            if (string.IsNullOrEmpty(gameSavedData.LastFinishedLevelId))
                return null;
            if (!FortInfo.Instance.GameLevel.GameLevelInfos.ContainsKey(gameSavedData.LastFinishedLevelId))
                return null;
            return FortInfo.Instance.GameLevel.GameLevelInfos[gameSavedData.LastFinishedLevelId];
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
            public string LastLoadedLevelId { get; set; }
            public string LastFinishedLevelId { get; set; }
        }
    }
    
}