using System;
using Fort.Inspector;

namespace Fort.Info.GameLevel
{
    public abstract class GameLevelCategory
    {
        protected GameLevelCategory()
        {
            Id = Guid.NewGuid().ToString();
        }

        public string DefaultSceneName { get; set; }
        [IgnoreProperty]
        public string Id { get; set; }

        public string Name { get; set; }
        public string DisplayName { get; set; }
        public GameLevelCategory[] ChildrenCategory { get; set; }
        public GameLevelInfo[] GameLevelInfos { get; set; }

    }
}