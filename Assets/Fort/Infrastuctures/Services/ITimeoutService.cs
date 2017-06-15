using System;
using UnityEngine;
using System.Collections;

namespace Fort
{
    public interface ITimeoutService
    {
        Promise TimeOut(TimeSpan duration);
        Promise TimeOut(TimeSpan duration, Func<bool> breakCondition);
        Promise TimeOut(TimeSpan duration, out object token);
        void Rearange(TimeSpan duration, object token);
        TimeoutInfo ResolveTimeoutInfo(object token);
        void RemoveTimeOut(object token);
    }
    public class TimeoutInfo
    {
        public TimeoutInfo(TimeSpan duration, TimeSpan position)
        {
            Duration = duration;
            Position = position;
        }

        public TimeSpan Duration { get; private set; }
        public TimeSpan Position { get; private set; }

        public float NormalPosition
        {
            get { return (float)(Position.TotalMilliseconds / Duration.TotalMilliseconds); }
        }
    }
}
