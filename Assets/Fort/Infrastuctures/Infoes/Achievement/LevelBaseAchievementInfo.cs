using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Fort.Info.Achievement
{
    public class LevelBaseAchievementInfo: AchievementInfo
    {
        public AchievementLevelInfo[] GetAchievementLevelInfos()
        {
            return
                ((IEnumerable) GetType().GetProperty("LevelInfoes").GetValue(this, new object[0]))
                    .Cast<AchievementLevelInfo>().ToArray();
        }
    }
    public abstract class LevelBaseAchievementInfo<T> : LevelBaseAchievementInfo where T : AchievementLevelInfo
    {
        public T[] LevelInfoes { get; set; }
/*        public virtual string ResolveDisplayNameTag(string tag, int levelIndex)
        {
            AchivementLevelInfo achivementLevelInfo = GetAchivementLevelInfos()[levelIndex];
            FieldInfo fieldInfo = achivementLevelInfo.GetType().GetField(tag);
            if (fieldInfo == null)
                return null;

            AchievementLevelParameterAttribute attribute = (AchievementLevelParameterAttribute)fieldInfo.GetCustomAttributes(typeof(AchievementLevelParameterAttribute), true).FirstOrDefault();
            if (attribute != null)
            {
                switch (attribute.ParameterType)
                {
                    case AchievementLevelParameterType.None:
                        return fieldInfo.GetValue(achivementLevelInfo).ToString();
                    case AchievementLevelParameterType.Duration:
                        int value = (int)LevelBaseAttributeInfo.ResolveFloat(fieldInfo.GetValue(achivementLevelInfo));
                        int second = value % 60;
                        int minute = value / 60;

                        if (minute > 0 && second > 0)
                        {
                            return string.Format("{0} دقیقه و {1} ثانیه", minute, second).Persian();
                        }
                        if (minute == 0 && second > 0)
                        {
                            return string.Format("{0} ثانیه", second).Persian();
                        }
                        if (minute > 0 && second == 0)
                        {
                            return string.Format("{0} دقیقه", minute).Persian();
                        }
                        return string.Format("{0} ثانیه", 0).Persian();
                    case AchievementLevelParameterType.Distance:
                        return fieldInfo.GetValue(achivementLevelInfo).ToString();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            return fieldInfo.GetValue(achivementLevelInfo).ToString();
        }
        public string GetDisplayName(int levelIndex)
        {
            string displayName = DisplayName;
            Regex regex = new Regex("\\{(\\w+)\\}");
            foreach (Match match in regex.Matches(displayName))
            {
                string value = match.Groups[1].Value;
                string tagDisplayName = ResolveDisplayNameTag(value, levelIndex);
                displayName = displayName.Replace(match.Value, tagDisplayName);
            }
            return displayName;
        }*/
    }
}