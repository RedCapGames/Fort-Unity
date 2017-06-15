using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Fort
{
    [Service(ServiceType = typeof(ISceneLoaderService))]
    public class SceneLoaderService : MonoBehaviour,ISceneLoaderService
    {
        private readonly Stack<string> _sceneStack = new Stack<string>(); 
        private ComplitionDeferred<object> _lastLoadDeferred;
        private bool _captureReturnKey;
        private object _lastContext;

        void Update()
        {
            if(_captureReturnKey && Input.GetKeyUp(KeyCode.Escape) && IsReturnCapable)
                Return(null);
        }

        #region Implementation of ISceneLoaderService

        
        public bool IsReturnCapable { get { return _sceneStack.Count > 0; } }

        public ComplitionPromise<object> Load(SceneLoadParameters parameters)
        {
            ServiceLocator.Resolve<IAnalyticsService>().StatSceneLoaded(parameters.SceneName);
            _lastContext = null;
            if (_lastLoadDeferred != null)
            {
                ComplitionDeferred<object> lastLoadDeferred = _lastLoadDeferred;
                _lastLoadDeferred = null;
                lastLoadDeferred.Resolve(parameters.Context);
            }
            _captureReturnKey = parameters.CaptureReturnKey;
            _lastLoadDeferred = new ComplitionDeferred<object>();
            if(parameters.FlushSceneStack)
                _sceneStack.Clear();
            if(parameters.AddToSceneStack)
                _sceneStack.Push(SceneManager.GetActiveScene().name);
            _lastContext = parameters.Context;
            SceneManager.LoadScene(parameters.SceneName);
            return _lastLoadDeferred.Promise();
        }

        public object GetLastLoadContext()
        {
            return _lastContext;
        }

        public void Return(object context)
        {
            _lastContext = null;
            if (_lastLoadDeferred != null)
            {
                ComplitionDeferred<object> lastLoadDeferred = _lastLoadDeferred;
                _lastLoadDeferred = null;
                lastLoadDeferred.Resolve(context);
            }

        }

        #endregion
    }
}