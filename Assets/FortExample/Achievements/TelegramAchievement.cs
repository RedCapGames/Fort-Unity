using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Fort.Info;

[Serializable]
public class TelegramAchievement : NoneLevelBaseAchievementInfo
{
    public string Value;
    public string[] Datas;
    public Dictionary<string, int> Ghaz;
}
