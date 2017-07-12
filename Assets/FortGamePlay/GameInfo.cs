using System;
using UnityEngine;

namespace Fort.GamePlay
{
    public class GameInfo:Actor
    {
        private LevelInfo _levelInfo;
        public string PlayerControllerClass;
        public GameObject PlayerCharacterPrefab;

        public PlayerController PlayerController { get; private set; }
        public new LevelInfo LevelInfo
        {
            get { return _levelInfo = _levelInfo ?? FindObjectOfType<LevelInfo>(); }
        }
        public bool PlayerControllerSpawned { get; private set; }

        public virtual Type GetPlayerControllerType()
        {
            return Type.GetType(PlayerControllerClass);
        }

        public virtual GameObject CreatePlayerPawn()
        {
            GameObject inventoryPrefabObject = Instantiate(PlayerCharacterPrefab);
            inventoryPrefabObject.transform.position = GetPawnSpawnPosition();
            return inventoryPrefabObject;
        }

        public virtual void PawnDied(Controller causer)
        {
            SpawnPlayer();
        }
        protected virtual Vector3 GetPawnSpawnPosition()
        {
            return Vector3.zero;
        }
        protected void SpawnPlayer()
        {
            GameObject playerPawn = CreatePlayerPawn();
            Pawn pawn = playerPawn.GetComponent<Pawn>();
            if (!PlayerControllerSpawned)
            {
                Type playerControllerType = GetPlayerControllerType();
                Controller controller = Controller.CreateControllerAndPosse(playerControllerType, pawn);
                PlayerController = (PlayerController)controller;
                Vector3 pawnSpawnPosition = GetPawnSpawnPosition();
            }
            else
            {
                PlayerController.PossePawn(pawn);
            }
            OnPawnSpawned(pawn);
        }
        protected virtual void OnPawnSpawned(Pawn pawn)
        {

        }

        protected override void BeginPlay()
        {
            base.BeginPlay();
            SpawnPlayer();
        }
    }
}
