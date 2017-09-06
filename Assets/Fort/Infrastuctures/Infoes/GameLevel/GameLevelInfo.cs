﻿using System;
using Fort.Info.Language;
using Fort.Inspector;

namespace Fort.Info.GameLevel
{
    public abstract class GameLevelInfo
    {
        protected GameLevelInfo()
        {
            Id = Guid.NewGuid().ToString();
            Scene = new InfoLanguageItem<FortScene> { UseOverridedValue = true,OverridedValue = new FortScene()};
            
            DisplayName = new InfoLanguageItem<string>();
        }
        [IgnorePresentation]
        public string Id { get; set; }

        public string Name { get; set; }
        [OverridableLanguage]
        public LanguageItem<FortScene> Scene { get; set; }
        public LanguageItem<string> DisplayName { get; set; }
        
    }
}