using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine.SceneManagement;

namespace Fort.Info
{
    public class GameLevel
    {
        private Dictionary<string, GameLevelInfo> _gameLevelInfos = new Dictionary<string, GameLevelInfo>();
        private Dictionary<string, GameLevelCategory> _gameLevelCategoriesMap = new Dictionary<string, GameLevelCategory>();
        private Dictionary<string, GameLevelCategory> _gameLevelCategoriesParentMap = new Dictionary<string, GameLevelCategory>();
        private GameLevelCategory[] _gameLevelCategories;
        public string DefaultSceneName { get; set; }

        public Dictionary<string, GameLevelInfo> GameLevelInfos
        {
            get
            {
                return _gameLevelInfos;
            }
        }
        public Dictionary<string, GameLevelCategory> LevelCategoriesParentMap
        {
            get
            {
                return _gameLevelCategoriesParentMap;
            }
        }

        public Dictionary<string, GameLevelCategory> LevelCategories
        {
            get
            {
                return _gameLevelCategoriesMap;
            }
        }
        public GameLevelCategory[] GameLevelCategories
        {
            get { return _gameLevelCategories; }
            set
            {
                _gameLevelCategories = value;
                Sync();
            }
        }

        private void Sync()
        {
            if (_gameLevelCategories == null)
                return;
            _gameLevelInfos = new Dictionary<string, GameLevelInfo>();
            _gameLevelCategoriesMap = new Dictionary<string, GameLevelCategory>();
            _gameLevelCategoriesParentMap = new Dictionary<string, GameLevelCategory>();

            Dictionary<GameLevelCategory, GameLevelCategory> gameLevelCategoriesDic = new Dictionary<GameLevelCategory, GameLevelCategory>();
            GetAllCategories(_gameLevelCategories, gameLevelCategoriesDic);
            GameLevelCategory[] allGameLevelCategories = gameLevelCategoriesDic.Keys.ToArray();
            foreach (GameLevelCategory gameLevelCategory in allGameLevelCategories)
            {
                _gameLevelCategoriesMap[gameLevelCategory.Id] = gameLevelCategory;
                if (gameLevelCategory.ChildrenCategory != null)
                {
                    foreach (GameLevelCategory childGameLevelCategory in gameLevelCategory.ChildrenCategory)
                    {
                        _gameLevelCategoriesParentMap[childGameLevelCategory.Id] = gameLevelCategory;
                    }
                }
                if (gameLevelCategory.GameLevelInfos != null)
                {
                    foreach (GameLevelInfo gameLevelInfo in gameLevelCategory.GameLevelInfos)
                    {
                        _gameLevelInfos[gameLevelInfo.Id] = gameLevelInfo;
                        _gameLevelCategoriesParentMap[gameLevelInfo.Id] = gameLevelCategory;
                    }
                }
            }
        }

        private void GetAllCategories(GameLevelCategory[] categories, Dictionary<GameLevelCategory, GameLevelCategory> gameLevelCategories)
        {
            foreach (GameLevelCategory category in categories)
            {
                if (!gameLevelCategories.ContainsKey(category))
                {
                    gameLevelCategories.Add(category, category);
                    if (category.ChildrenCategory != null)
                        GetAllCategories(category.ChildrenCategory, gameLevelCategories);
                }
            }
        }
    }
}
