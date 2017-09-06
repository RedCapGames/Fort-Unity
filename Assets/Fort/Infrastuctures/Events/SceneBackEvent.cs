using System;
using Fort.Aggregator;

namespace Fort.Events
{
    public class SceneBackEvent:PubSubEvent<SceneBackEventArgs>
    {
    }

    public class SceneBackEventArgs : EventArgs
    {
        public string CurrentSceneName { get; private set; }
        public string ReturnSceneName { get; private set; }
        public bool ReturnToSceneCapable { get; private set; }
        public bool AutoReturn { get; set; }

        public SceneBackEventArgs(string currentSceneName,string returnSceneName,bool returnToSceneCapable)
        {
            AutoReturn = true;
            CurrentSceneName = currentSceneName;
            ReturnSceneName = returnSceneName;
            ReturnToSceneCapable = returnToSceneCapable;
        }
    }
}
