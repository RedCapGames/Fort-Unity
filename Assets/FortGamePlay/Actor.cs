using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fort.GamePlay
{
    public class Actor : MonoBehaviour
    {
        #region Fields

        private static ActorTickManager _tickManager;
        private readonly List<FrameInfo> _everyFrameInfoes = new List<FrameInfo>();
        private readonly List<LateInvokeData> _lateInvokeDatas = new List<LateInvokeData>();
        private readonly List<TimerInfo> _timerInfos = new List<TimerInfo>();

        #endregion

        #region Properties

        public bool IsTickable { get; protected set; }
        public static bool Paused { get; private set; }

        public GameInfo GameInfo
        {
            get
            {
                InitializeTickManager();
                return _tickManager.GameInfo;
            }
        }

        public LevelInfo LevelInfo
        {
            get { return GameInfo.LevelInfo; }
        }

        #endregion

        #region  Public Methods

        public virtual void ApplyDamage(DamageInfo damageInfo)
        {
        }

        public static void Pause()
        {
            Time.timeScale = 0;
            Paused = true;
        }

        public static void Resume()
        {
            Time.timeScale = 1;
            Paused = false;
        }

        public object SetTimer(TimeSpan duration, Action action, bool oneShot = false)
        {
            TimerInfo timerInfo = new TimerInfo
            {
                Action = action,
                Duration = duration,
                Timer = TimeSpan.Zero,
                OneShot = oneShot
            };
            _timerInfos.Add(timerInfo);
            return timerInfo;
        }

        public void InvokeLater(Action action)
        {
            _lateInvokeDatas.Add(new LateInvokeData
            {
                FrameIndex = Time.frameCount,
                Action = action
            });
        }

        public T Cast<T>() where T : Actor
        {
            return (T) this;
        }

        #endregion

        #region  Internal Methods

        internal void InternalTick()
        {
            if (Paused)
                return;

            _everyFrameInfoes.RemoveAll(frameInfo =>
            {
                if (frameInfo == null)
                    return true;
                frameInfo.Timer += TimeSpan.FromSeconds(Time.deltaTime);
                return frameInfo.EveryTimePredecate(frameInfo.Timer);
            });

            List<TimerInfo> removedTimers = new List<TimerInfo>();
            foreach (TimerInfo timerInfo in _timerInfos.ToArray())
            {
                timerInfo.Timer += TimeSpan.FromSeconds(Time.deltaTime);
                timerInfo.LateTimer += TimeSpan.FromSeconds(Time.deltaTime);
                if (timerInfo.Timer >= timerInfo.Duration)
                {
                    if (timerInfo.Action != null)
                        timerInfo.Action();
                    if (timerInfo.OneShot)
                        removedTimers.Add(timerInfo);
                    timerInfo.Timer = TimeSpan.Zero;
                }
            }
            foreach (TimerInfo removedTimer in removedTimers)
            {
                _timerInfos.Remove(removedTimer);
            }
            LateInvokeData[] triggerableInvokeDatas =
                _lateInvokeDatas.Where(data => data.FrameIndex < Time.frameCount).ToArray();
            foreach (LateInvokeData triggerableInvokeData in triggerableInvokeDatas)
            {
                _lateInvokeDatas.Remove(triggerableInvokeData);
            }
            foreach (LateInvokeData triggerableInvokeData in triggerableInvokeDatas)
            {
                triggerableInvokeData.Action();
            }
            Tick();
        }

        #endregion

        #region Protected Methods

        protected virtual void Tick()
        {
        }

        protected virtual void BeginPlay()
        {
        }

        protected virtual void EndPlay()
        {
        }

        protected void RemoveTimer(object token)
        {
            if (token is FrameInfo)
            {
                if (_everyFrameInfoes.Contains((FrameInfo) token))
                    _everyFrameInfoes.Remove((FrameInfo) token);
                return;
            }
            if (_timerInfos.Contains((TimerInfo) token))
                _timerInfos.Remove((TimerInfo) token);
        }

        protected object EveryFrame(Action action)
        {
            Func<TimeSpan, bool> everyFrame = span =>
            {
                action();
                return false;
            };
            FrameInfo result = new FrameInfo
            {
                EveryTimePredecate = everyFrame,
                Timer = TimeSpan.Zero
            };
            _everyFrameInfoes.Add(result);
            return result;
        }

        protected object EveryFrame(Func<TimeSpan, bool> action)
        {
            FrameInfo result = new FrameInfo
            {
                EveryTimePredecate = action,
                Timer = TimeSpan.Zero
            };
            _everyFrameInfoes.Add(result);
            return result;
        }

        protected object EveryFrame(Action<TimeSpan> action)
        {
            Func<TimeSpan, bool> everyFrame = span =>
            {
                action(span);
                return false;
            };
            FrameInfo result = new FrameInfo
            {
                EveryTimePredecate = everyFrame,
                Timer = TimeSpan.Zero
            };
            _everyFrameInfoes.Add(result);
            return result;
        }

        #endregion

        #region Private Methods

        private void Start()
        {
            InitializeTickManager();
            _tickManager.RegisterActor(this);
            BeginPlay();
        }

        private void OnDestroy()
        {
            _tickManager.UnRegisterActor(this);
            EndPlay();
        }

        private void InitializeTickManager()
        {
            if (_tickManager == null || Equals(_tickManager, "null"))
            {
                GameObject o = new GameObject("Tick Manager");
                _tickManager = o.AddComponent<ActorTickManager>();
            }
        }

        #endregion

        #region Nested types

        private class TimerInfo
        {
            #region Properties

            public TimeSpan Duration { get; set; }
            public Action Action { get; set; }
            public TimeSpan Timer { get; set; }
            public TimeSpan LateTimer { get; set; }
            public bool OneShot { get; set; }

            #endregion
        }

        private class FrameInfo
        {
            #region Properties

            public TimeSpan Timer { get; set; }
            public Func<TimeSpan, bool> EveryTimePredecate { get; set; }

            #endregion
        }

        private class LateInvokeData
        {
            #region Properties

            public int FrameIndex { get; set; }
            public Action Action { get; set; }

            #endregion
        }

        #endregion
    }

    public class DamageInfo
    {
        #region Properties

        public virtual float Amount { get; set; }
        public Controller CauseBy { get; set; }
        public Actor CauseWith { get; set; }

        #endregion
    }
}