using Fort.Info.Achievement;

public class PlayerLevelAchievement : LevelBaseAchievementInfo<PlayerLevelAchievementLevelInfo>
{
    
}

public class PlayerLevelAchievementLevelInfo: AchievementLevelInfo
{
    public int Level { get; set; }
}