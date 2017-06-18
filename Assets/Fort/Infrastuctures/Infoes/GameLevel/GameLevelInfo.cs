using System;
using Fort.Inspector;

namespace Fort.Info.GameLevel
{
    public abstract class GameLevelInfo
    {
        public GameLevelInfo()
        {
            Id = Guid.NewGuid().ToString();
        }
        [IgnoreProperty]
        public string Id { get; set; }

        public string Name { get; set; }
        public FortScene Scene { get; set; }
        public string DisplayName { get; set; }
        
    }
}