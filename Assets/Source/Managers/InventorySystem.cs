using Assets.Source.Data.Models;
using Assets.Source.Utilities;
using Assets.Source.Utilities.Helpers.Gizmo;

using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Source.Managers
{

    [Serializable]
    public class InventorySystem : LoaderBase<InventorySystem>
    {
        public Dictionary<InventoryItemData, InventoryItem> d_InventoryDictionary 
        { get 
            { if (_inventoryDictionary == null)
                {
                    Debug.LogError(DebugUtils.GetMessageFormat($"Inventory System got a null, we are mitigatin problem, please research the problem",0));
                    _inventoryDictionary = new Dictionary<InventoryItemData, InventoryItem>();
                }
                return _inventoryDictionary;
            } 
        }

        public List<InventoryItem> L_inventory { get; private set; }

        public List<InventoryItem> L_equipedItems {
            get 
                {
                    return L_inventory.FindAll(x => x.IsEquipped == true);
                } 
        }

        public Action<InventoryItem> OnInventoryUpdated;
        public Action<InventoryItem> OnInventoryCreate;
        public Action<InventoryItem> OnInventoryDelete;
#if UNITY_EDITOR
        public InventoryItemData TestData1;
        public InventoryItemData TestData2;
#endif
        private Dictionary<InventoryItemData, InventoryItem>  _inventoryDictionary;

        public bool IsInventaryOpen { get { return UIManager.Instance._currentManagerUsing.Contains(this); } }
        public override void Init()
        {
            _inventoryDictionary = new Dictionary<InventoryItemData, InventoryItem>();
            L_inventory = new List<InventoryItem>();
#if UNITY_EDITOR
            //Add(TestData1);
#endif            
            isLoaded = true;
        }

        private void OnEnable()
        {
            GameEvents.Instance.onPickableItemEnter += Add;
        }

        private void OnDisable()
        {
            GameEvents.Instance.onPickableItemEnter -= Add;
        }

        public void Add(InventoryItemData refData)
        {
            InventoryItem _item;
            if (d_InventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.AddToStack();
                _item = value;
                OnInventoryUpdated?.Invoke(_item);
            }
            else
            {
                InventoryItem _newitem = new InventoryItem(refData);
                L_inventory.Add(_newitem);
                d_InventoryDictionary.Add(refData, _newitem);
                _item = _newitem;
                OnInventoryCreate?.Invoke(_item);
            }

        }

        public void Remove(InventoryItemData refData)
        {
            InventoryItem _item;

            if (d_InventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.RemoveFromStack();
                _item = value;
                if (value.stack == 0)
                {
                    L_inventory.Remove(value);
                    d_InventoryDictionary.Remove(refData);
                    OnInventoryDelete?.Invoke(_item);
                    return;
                }
                OnInventoryUpdated?.Invoke(_item);
            }
            else {
                var msg = "You dont event have the item";
                UnityEngine.Debug.Log(DebugUtils.GetMessageFormat(msg,4));
                _item = null;
            }
        }

        public InventoryItem Get(InventoryItemData referenceData)
        {
            if (d_InventoryDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                return value;
            }
            return null;
        }

        public Action RequestOpenInventory;
        public Action RequestCloseInventory;

        public void OpenInventory() {
            if (RequestOpenInventory != null) {
                RequestOpenInventory?.Invoke();
                UIManager.Instance.RequestOpenUI(this);
            }
        }

        public void CloseInventory() {
            if (RequestCloseInventory != null) {
                RequestCloseInventory?.Invoke();
                UIManager.Instance.RequestCloseUI(this);    
            }
        }

        bool TVGlass = false;
        public bool HasTVGlasses()
        {
            return TVGlass;
        }

#if UNITY_EDITOR
        [ContextMenu("Test Inventory Glass")]
        public void AllowTVGlasses() {
            TVGlass = true;
        }

        [ContextMenu("DoCreate")]
        public void TestAdd() {
            Add(TestData2);
        }

        [ContextMenu("Editcreated")]
        public void Testedit() {
            InventoryItem _o = L_inventory.Find(x=>x.ItemData == TestData2);
            _o.Equip();
        }

        [ContextMenu("DoRemove")]
        public void TestRemove()
        {
            Remove(TestData2);
        }
#endif 
    }
}

