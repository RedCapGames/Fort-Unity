using System.Collections.Generic;
using UnityEngine;

namespace Fort.GamePlay
{
    public class ActorTickManager:MonoBehaviour
    {
        private GameInfo _gameInfo;
        public GameInfo GameInfo
        {
            get
            {
                if (_gameInfo == null)
                {
                    _gameInfo = FindObjectOfType<GameInfo>();
                }
                return _gameInfo;
            }
        }
        Dictionary<Actor,Actor> _actors = new Dictionary<Actor, Actor>(); 
        public void RegisterActor(Actor actor)
        {
            _actors[actor] = actor;
        }

        public void UnRegisterActor(Actor actor)
        {
            _actors.Remove(actor);
        }

        void Update()
        {
            foreach (KeyValuePair<Actor, Actor> pair in _actors)
            {
                if(pair.Key.IsTickable)
                    pair.Key.InternalTick();
            }
        }
    }
}
