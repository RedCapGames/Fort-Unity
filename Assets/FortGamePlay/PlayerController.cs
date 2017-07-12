using System;
using UnityEngine;

namespace Fort.GamePlay
{
    public class PlayerController:Controller
    {
        private HUD _hud;
        public HUD HUD
        {
            get { return _hud = _hud ?? FindObjectOfType<HUD>(); }
        }
        protected GameObject MainCamera { get; private set; }
        public override void PawnDied(Controller causer)
        {
            Pawn.Posse(null);
            Pawn = null;
            if (GameInfo != null)
                GameInfo.PawnDied(causer);
        }
        virtual protected void CalculateCamera(ref Vector3 cameraPosition, ref Quaternion rotation)
        {
        }

        #region Overrides of Actor

        protected override void BeginPlay()
        {
            base.BeginPlay();
            MainCamera = ResolveMainCamera();
        }

        #endregion

        protected virtual GameObject ResolveMainCamera()
        {
            Camera cam = gameObject.transform.FindComponentRecursive<Camera>();
            if (cam != null)
                return cam.gameObject;
            if (Pawn != null)
            {
                cam = Pawn.gameObject.transform.FindComponentRecursive<Camera>();
                if (cam != null)
                    return cam.gameObject;
            }
            cam = FindObjectOfType<Camera>();
            if (cam != null)
                return cam.gameObject;
            throw new Exception("Camera cannot be found.");
        }

        protected override void Tick()
        {
            base.Tick();
            if (MainCamera != null)
            {
                Quaternion rotation = new Quaternion();
                Vector3 cameraPosition = new Vector3();
                CalculateCamera(ref cameraPosition, ref rotation);
                MainCamera.transform.position = cameraPosition;
                MainCamera.transform.rotation = rotation;
            }
        }
    }
}
