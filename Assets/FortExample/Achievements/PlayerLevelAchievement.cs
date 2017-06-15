using UnityEngine;
using System.Collections;
using Fort.Info;

public class PlayerLevelAchievement : LevelBaseAchievementInfo<PlayerLevelAchievementLevelInfo>
{
    
}

public class PlayerLevelAchievementLevelInfo: AchievementLevelInfo
{
    public int Level { get; set; }
}