using System;
using UnityEngine;
using System.Collections;

namespace Fort
{
    public interface ISceneLoaderService
    {
        bool IsReturnCapable { get; }
        ComplitionPromise<object> Load(SceneLoadParameters parameters);
        AsyncOperation LoadAsync(SceneLoadParameters parameters);
        object GetLastLoadContext();
        void Return(object context);
    }

    public class SceneLoadParameters
    {
        public SceneLoadParameters(string sceneName)
        {
            SceneName = sceneName;
            CaptureReturnKey = true;
            AddToSceneStack = true;
        }

        public string SceneName { get; private set; }
        public object Context { get; set; }
        public bool CaptureReturnKey { get; set; }
        public bool AddToSceneStack { get; set; }
        public bool FlushSceneStack { get; set; }
    }
}
