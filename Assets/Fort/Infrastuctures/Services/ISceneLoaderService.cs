using UnityEngine;

namespace Fort
{
    /// <summary>
    /// Service to manage loading of scene. Ability such as stacking level loaded sequences and return to last scene and passing context to newly loaded scene
    /// </summary>
    public interface ISceneLoaderService
    {
        /// <summary>
        /// Determine if any scene exists in stack to return
        /// </summary>
        bool IsReturnCapable { get; }
        /// <summary>
        /// Loading scene and returning a complition promise. Whenever another load accurred or return call the new context will pass to this promise and resolve it
        /// </summary>
        /// <param name="parameters">Scene loading parameters</param>
        /// <returns></returns>
        ComplitionPromise<object> Load(SceneLoadParameters parameters);
        /// <summary>
        /// Load scene async.
        /// </summary>
        /// <param name="parameters">Scene loading parameters</param>
        /// <returns>Unity load scene async operation</returns>
        AsyncOperation LoadAsync(SceneLoadParameters parameters);
        /// <summary>
        /// Returning last load context
        /// </summary>
        /// <returns>Last load context</returns>
        object GetLastLoadContext();
        /// <summary>
        /// Return to last stacked scene
        /// </summary>
        /// <param name="context">The context of Load() Method promise</param>
        void Return(object context);
    }

    /// <summary>
    /// Parameters for loading scene
    /// </summary>
    public class SceneLoadParameters
    {
        public SceneLoadParameters(string sceneName)
        {
            SceneName = sceneName;
            CaptureReturnKey = true;
            AddToSceneStack = true;
        }
        /// <summary>
        /// Scene name
        /// </summary>
        public string SceneName { get; private set; }
        /// <summary>
        /// Context that will pass to GetLastLoadContext() Method of ISceneLoaderService and can be resolve by GetLastLoadContext() of ISceneLoaderService
        /// </summary>
        public object Context { get; set; }
        /// <summary>
        /// Capture return key and automatically return to last stacked scene if possible
        /// </summary>
        public bool CaptureReturnKey { get; set; }
        /// <summary>
        /// Add current scene to scene stack
        /// </summary>
        public bool AddToSceneStack { get; set; }
        /// <summary>
        /// Clear all scene stack
        /// </summary>
        public bool FlushSceneStack { get; set; }
    }
}
