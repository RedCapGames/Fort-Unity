using System;
using Fort.Info;
using Fort.Info.GameLevel;
using UnityEngine;

namespace Fort
{
    /// <summary>
    /// Service to manage game levels
    /// </summary>
    public interface ILevelManagementService
    {
        /// <summary>
        /// Call this method whenever the game is finished
        /// </summary>
        /// <param name="parameters">Level finish parameter</param>
        void GameLevelFinished(LevelFinishParameters parameters);
        /// <summary>
        /// Resolve cached level finish stat of a game level info
        /// </summary>
        /// <param name="level">Corresponding game level info</param>
        /// <returns>Level finish stat</returns>
        ILevelFinishStat GetGameFinishStat(GameLevelInfo level);
        /// <summary>
        /// Get Last game level stat that is passed to GameLevelFinished() method
        /// </summary>
        /// <returns>Level finish stat.(If no cached data null value will be returned)</returns>
        ILevelFinishStat GetLastGameFinishStat();
        /// <summary>
        /// Load Game Level
        /// </summary>
        /// <param name="level">Corresponding game level info</param>
        void LoadGameLevel(GameLevelInfo level);
        /// <summary>
        /// Load game level async.(Loader scene of GameLevel in fort will be load)
        /// </summary>
        /// <param name="level">Corresponding game level info</param>
        void LoadGameLevelAsync(GameLevelInfo level);
        /// <summary>
        /// Continue loading game scene after loader scene
        /// </summary>
        /// <returns>Unity load scene async operation</returns>
        AsyncOperation ContinueLoadGameLevelAsync();
        /// <summary>
        /// Resolve last loaded game level info
        /// </summary>
        /// <returns>last loaded game level info</returns>
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
