using System;
using Fort.Inspector;

namespace Fort.Info.GameLevel
{
    public abstract class GameLevelCategory
    {
        protected GameLevelCategory()
        {
            Id = Guid.NewGuid().ToString();
            ChildrenCategory = new GameLevelCategory[0];
            GameLevelInfos = new GameLevelInfo[0];
            DefaultScene = new FortScene();
        }
        public FortScene DefaultScene { get; set; }
        [IgnoreProperty]
        public string Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public GameLevelCategory[] ChildrenCategory { get; set; }
        public GameLevelInfo[] GameLevelInfos { get; set; }

    }
}