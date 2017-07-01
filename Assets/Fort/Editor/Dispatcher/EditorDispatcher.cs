using System;
using System.Collections.Generic;
using Fort.Dispatcher;
using UnityEditor;
using UnityEngine;

namespace Assets.Fort.Editor.Dispatcher
{
    public class EditorDispatcher: IDispatcher
    {
        private readonly Queue<Action> _actionQueue = new Queue<Action>();
        private static IDispatcher CreateDispatcher()
        {
            EditorApplication.update +=Update;
            return new EditorDispatcher();
        }

        private static void Update()
        {
            EditorDispatcher editorDispatcher = (EditorDispatcher) _dispatcher;
            Action[] actions;
            lock (editorDispatcher)
            {
                actions = editorDispatcher._actionQueue.ToArray();
                editorDispatcher._actionQueue.Clear();
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
    }
}
