using Assets.Source.Data.Models;
using Assets.Source.Utilities.Helpers.Gizmo;
using System;
using System.Collections.Generic;
using UnityEngine;




namespace Assets.Source.Managers
{

    [Serializable]
    public class InventorySystem : LoaderBase<InventorySystem>
    {
        public Dictionary<InventoryItemData, InventoryItem> d_inventoryDictionary { get;  private set; }
        public List<InventoryItem> L_inventory { get; private set; }

        public List<InventoryItem> L_equipedItems { get; private set; }

        public Action<InventoryItem> OnInventoryUpdated;
        public Action<InventoryItem> OnInventoryCreate;
        public Action<InventoryItem> OnInventoryDelete;
#if UNITY_EDITOR
        public InventoryItemData TestData1;
        public InventoryItemData TestData2;
#endif

        public bool IsInventaryOpen { get { return UIManager.Instance._currentManagerUsing.Contains(this); } }
        public override void Init()
        {
            d_inventoryDictionary = new Dictionary<InventoryItemData, InventoryItem>();
            L_inventory = new List<InventoryItem>();
            L_equipedItems = new List<InventoryItem>();
#if UNITY_EDITOR
            Add(TestData1);
#endif            

            isLoaded = true;
        }
        
        public void Add(InventoryItemData refData)
        {
            InventoryItem _item;
            if (d_inventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.AddToStack();
                _item = value;
                OnInventoryUpdated?.Invoke(_item);
            }
            else
            {
                InventoryItem _newitem = new InventoryItem(refData);
                L_inventory.Add(_newitem);
                d_inventoryDictionary.Add(refData, _newitem);
                _item = _newitem;
                OnInventoryCreate?.Invoke(_item);
            }

        }

        public void Remove(InventoryItemData refData)
        {
            InventoryItem _item;

            if (d_inventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.RemoveFromStack();
                _item = value;
                if (value.stack == 0)
                {
                    L_inventory.Remove(value);
                    d_inventoryDictionary.Remove(refData);
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
            if (d_inventoryDictionary.TryGetValue(referenceData, out InventoryItem value))
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

#if UNITY_EDITOR
        [ContextMenu("DoCreate")]
        public void TestAdd() {
            Add(TestData2);
        }

        [ContextMenu("DoRemove")]
        public void TestRemove()
        {
            Remove(TestData2);
        }
#endif 
    }
}

