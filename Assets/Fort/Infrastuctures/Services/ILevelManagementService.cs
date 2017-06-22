using System;
using Fort.Info;
using Fort.Info.GameLevel;
using UnityEngine;

namespace Fort
{
    public interface ILevelManagementService
    {
        void GameLevelFinished(LevelFinishParameters parameters);
        ILevelFinishStat GetGameFinishStat(GameLevelInfo level);
        void LoadGameLevel(GameLevelInfo level);
        void LoadGameLevelAsync(GameLevelInfo level);
        AsyncOperation ContinueLoadGameLevelAsync();
        GameLevelInfo GetLastLoadedLevel();
    }

    public interface ILevelFinishStat:IComparable<ILevelFinishStat>
    {
        
    }

    public class LevelFinishParameters
    {
        public LevelFinishParameters(ILevelFinishStat levelFinishStat)
        {
            LevelFinishStat = levelFinishStat;
            TransitionType = LevelFinishSceneTransitionType.Stay;
        }

        public ILevelFinishStat LevelFinishStat { get; private set; }
        public LevelFinishSceneTransitionType TransitionType { get; set; }
        public string TransitionScene { get; set; }        
    }

    public enum LevelFinishSceneTransitionType
    {
        Stay,        
        MoveToScene
    }
    
}
