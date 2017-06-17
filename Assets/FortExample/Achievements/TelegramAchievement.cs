using System;
using System.Collections.Generic;
using Fort.Info.Achievement;

[Serializable]
public class TelegramAchievement : NoneLevelBaseAchievementInfo
{
    public string Value;
    public string[] Datas;
    public Dictionary<string, int> Ghaz;
}
