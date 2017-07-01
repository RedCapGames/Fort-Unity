using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fort.Dispatcher
{
    public class GameDispatcher : MonoBehaviour, IDispatcher
    {
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        private static IDispatcher CreateDispatcher()
        {
            GameObject o = new GameObject("Dispatcher");
            return o.AddComponent<GameDispatcher>();
        }

        private static IDispatcher _dispatcher;

        public static IDispatcher Dispatcher
        {
            get { return _dispatcher = _dispatcher ?? CreateDispatcher(); }

        }
        #region Implementation of IDispatcher

        public void Dispach(Action action)
        {
            lock (this)
            {
                _actionQueue.Enqueue(action);
            }
        }

        #endregion

        void Update()
        {
            Action[] actions;
            lock (this)
            {
                actions = _actionQueue.ToArray();
                _actionQueue.Clear();
            }
            foreach (Action action in actions)
            {
                try
                {
                    action();
                }
                catch (Exception e)
                {
                    Debug.LogException(e);
                }
                
            }
        }
    }
}