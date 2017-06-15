using System;

namespace Fort.Info
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