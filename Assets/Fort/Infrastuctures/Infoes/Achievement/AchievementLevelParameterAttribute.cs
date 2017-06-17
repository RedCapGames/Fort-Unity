using System;

namespace Fort.Info.Achievement
{
    [AttributeUsage(AttributeTargets.Field)]
    public class AchievementLevelParameterAttribute : Attribute
    {
        public AchievementLevelParameterType ParameterType { get; set; }

        public AchievementLevelParameterAttribute(AchievementLevelParameterType parameterType)
        {
            ParameterType = parameterType;
        }
    }
}