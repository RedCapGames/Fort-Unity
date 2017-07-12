using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Fort.GamePlay
{
    public class Inventory:Actor
    {
        private InventoryManager _inventoryManager;
        protected Transform PawnAttachmentBoneTransform { get; private set; }

        public string Name
        {
            get { return GetType().Name; }
        }

        public Pawn Pawn
        {
            get { return InventoryManager == null ? null : InventoryManager.Pawn; }
        }

        #region Properties

        public InventoryManager InventoryManager
        {
            get { return _inventoryManager; }
            set
            {
                _inventoryManager = value;
                UpdatePawnInfos();
            }
        }

        private void UpdatePawnInfos()
        {

        }

        public bool IsActive
        {
            get { return gameObject.activeSelf; }
        }

        #endregion

        #region  Public Methods

        public void Activate(bool deactivateOther = true)
        {
            if (gameObject.activeSelf)
                return;
            if (deactivateOther)
                foreach (Inventory inventory in InventoryManager.Inventories.Except(new[] { this }))
                {
                    inventory.DeActivate();
                }
            gameObject.SetActive(true);
            ActivationStateChanged();
        }

        public virtual void DeActivate()
        {
            if (!gameObject.activeSelf)
                return;
            gameObject.SetActive(false);
            ActivationStateChanged();
        }

        #endregion

        #region Protected Methods

        protected virtual void ActivationStateChanged()
        {
        }



        #endregion



        public virtual void PlayIdleAnimation()
        {

        }

        protected virtual bool IsAttachToPawn()
        {
            return true;
        }

        public virtual void OnAttackCanceled()
        {

        }
    }
}
