using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fort
{
    [Service(ServiceType = typeof(ITimeoutService))]
    public class TimeoutService : MonoBehaviour,ITimeoutService
    {
        private List<TimeOutToken> _timeOutTokens = new List<TimeOutToken>();

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            List<TimeOutToken> expiredTimeOutTokens = new List<TimeOutToken>();
            foreach (TimeOutToken timeOutToken in _timeOutTokens.ToArray())
            {
                timeOutToken.RemainingTime -= TimeSpan.FromSeconds(Time.deltaTime);
                if (timeOutToken.RemainingTime <= TimeSpan.Zero)
                {
                    try
                    {
                        timeOutToken.Deferred.Resolve();
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }

                    expiredTimeOutTokens.Add(timeOutToken);
                }
                else
                {
                    if (timeOutToken.BreakCondition != null)
                    {
                        if (timeOutToken.BreakCondition())
                        {
                            try
                            {
                                timeOutToken.Deferred.Resolve();
                            }
                            catch (Exception e)
                            {
                                Debug.LogException(e);
                            }
                            expiredTimeOutTokens.Add(timeOutToken);
                        }
                    }
                }
            }
            foreach (TimeOutToken expiredTimeOutToken in expiredTimeOutTokens)
            {
                _timeOutTokens.Remove(expiredTimeOutToken);
            }
        }

        #region Implementation of ITimeOutService

        public Promise TimeOut(TimeSpan duration)
        {
            Deferred deferred = new Deferred();
            _timeOutTokens.Add(new TimeOutToken
            {
                Deferred = deferred,
                RemainingTime = duration,
                Duration = duration
            });
            return deferred.Promise();
        }

        public Promise TimeOut(TimeSpan duration, Func<bool> breakCondition)
        {
            Deferred deferred = new Deferred();
            _timeOutTokens.Add(new TimeOutToken
            {
                Deferred = deferred,
                RemainingTime = duration,
                Duration = duration,
                BreakCondition = breakCondition
            });
            return deferred.Promise();
        }

        public Promise TimeOut(TimeSpan duration, out object token)
        {
            Deferred deferred = new Deferred();
            TimeOutToken timeOutToken = new TimeOutToken
            {
                Deferred = deferred,
                RemainingTime = duration,
                Duration = duration
            };
            token = timeOutToken;
            _timeOutTokens.Add(timeOutToken);
            return deferred.Promise();
        }


        public void Rearange(TimeSpan duration, object token)
        {
            TimeOutToken timeOutToken = (TimeOutToken)token;
            if (!_timeOutTokens.Contains(timeOutToken))
                throw new Exception("Token not found in TimeOutService");
            timeOutToken.RemainingTime = duration;
            timeOutToken.Duration = duration;
        }

        public TimeoutInfo ResolveTimeoutInfo(object token)
        {
            TimeOutToken timeOutToken = (TimeOutToken)token;
            if (!_timeOutTokens.Contains(timeOutToken))
                throw new Exception("Token not found in TimeOutService");
            return new TimeoutInfo(timeOutToken.Duration, timeOutToken.Duration - timeOutToken.RemainingTime);
        }

        public void RemoveTimeOut(object token)
        {
            _timeOutTokens.Remove((TimeOutToken)token);
        }

        #endregion

        class TimeOutToken
        {
            public Deferred Deferred { get; set; }
            public TimeSpan Duration { get; set; }
            public TimeSpan RemainingTime { get; set; }
            public Func<bool> BreakCondition { get; set; }
        }
    }
}

