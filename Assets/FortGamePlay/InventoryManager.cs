using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Fort.GamePlay
{
    public class InventoryManager:Actor
    {
        #region Fields

        public string DefaultInventoryPrefab;
        public string[] InventoryPrefabs;
        protected readonly List<Inventory> _inventories;
        private Pawn _pawn;

        #endregion

        #region Constructors

        public InventoryManager()
        {
            _inventories = new List<Inventory>();
        }

        #endregion

        #region Properties

        public Pawn Pawn
        {
            get { return _pawn = _pawn ?? GetComponent<Pawn>(); }

        }

        public Inventory[] Inventories
        {
            get { return _inventories.ToArray(); }
        }

        public Inventory ActiveInventory
        {
            get { return _inventories.FirstOrDefault(inventory => inventory.IsActive); }
        }

        public Inventory[] ActiveInventories
        {
            get { return _inventories.Where(inventory => inventory.IsActive).ToArray(); }
        }


        public Inventory this[string inventoryName]
        {
            get { return Inventories.First(inventory => inventory.Name == inventoryName); }
        }

        #endregion

        #region Private Methods

        #region Overrides of Actor

        protected override void BeginPlay()
        {
            base.BeginPlay();
            Initialize();
        }

        #endregion

        protected virtual void Initialize()
        {
            if (InventoryPrefabs != null)
            {
                foreach (string inventoryPrefab in InventoryPrefabs)
                {
                    GameObject instantiate =
                        Resources.Load<GameObject>(inventoryPrefab);
                    Inventory inventory = instantiate.GetComponent<Inventory>();
                    inventory.InventoryManager = this;
                    instantiate.transform.parent = Pawn.transform;
                    instantiate.SetActive(false);
                    _inventories.Add(inventory);
                }
            }
            Pawn.InventoryInitialized();
        }

        #endregion

        public void AddInventory(Inventory inventory)
        {
            inventory.InventoryManager = this;
            inventory.transform.parent = Pawn.transform;
            //inventory.gameObject.SetActive(false);
            _inventories.Add(inventory);
        }

        public void RemoveInventory(Inventory inventory)
        {
            inventory.InventoryManager = null;
            transform.parent = null;
            _inventories.Remove(inventory);
        }

        public void DestroyAllInventories()
        {
            foreach (Inventory inventory in Inventories)
            {
                DestroyObject(inventory.gameObject);
            }
            _inventories.Clear();
        }

    }
}
