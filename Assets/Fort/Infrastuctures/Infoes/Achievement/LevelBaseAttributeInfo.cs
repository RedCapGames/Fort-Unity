using System;

namespace Fort.Info.Achievement
{
    public class LevelBaseAttributeInfo
    {
        public object Value { get; set; }
        public object MaxValue { get; set; }
        public object MinValue { get; set; }
        public bool Invert { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public AttributeType AttributeType { get; set; }

        public float ResolveFloatValue()
        {
            float value = ResolveFloat(Value);
            float maxValue = ResolveFloat(MaxValue);
            float minValue = ResolveFloat(MinValue);
            if (!Invert)
            {
                if (maxValue > 0)
                    return value / maxValue;
                return 0;
            }
            if (maxValue <= 0)
                return 0;
            return (maxValue - value) / (maxValue - minValue);
        }

        public static float ResolveFloat(object val)
        {
            if (val == null)
                return 0;
            if (val is float)
                return (float)val;
            if (val is TimeSpan)
                return (float)((TimeSpan)val).TotalSeconds;
            object changeType = Convert.ChangeType(val, typeof(float));
            if (changeType != null) return (float)changeType;
            return 0;
        }
    }
}