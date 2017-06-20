using System;
using Fort.Info.Language;
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
            DefaultScene = new InfoLanguageItem<FortScene> {UseOverridedValue = true};
            DisplayName = new InfoLanguageItem<string>();
        }
        [OverridableLanguage]
        public LanguageItem<FortScene> DefaultScene { get; set; }
        [IgnorePresentation]
        public string Id { get; set; }

        public string Name { get; set; }
        public LanguageItem<string> DisplayName { get; set; }
        public GameLevelCategory[] ChildrenCategory { get; set; }
        public GameLevelInfo[] GameLevelInfos { get; set; }

    }
}