using Assets.Source.Data.Models;
using Assets.Source.Utilities.Helpers.Gizmo;
using System;
using System.Collections.Generic;
using TreeEditor;



namespace Assets.Source.Managers
{
    [Serializable]
    public class InventorySystem : LoaderBase<InventorySystem>
    {
        public Dictionary<InventoryItemData, InventoryItem> d_inventoryDictionary { get;  private set; }
        public List<InventoryItem> L_inventory { get; private set; }

        public List<InventoryItem> L_equipedItems { get; private set; }

        public Action OnInventoryUpdated;

#if UNITY_EDITOR
        public InventoryItemData TestData1;
        public InventoryItemData TestData2;
#endif

        public override void Init()
        {
            d_inventoryDictionary = new Dictionary<InventoryItemData, InventoryItem>();
            L_inventory = new List<InventoryItem>();
            L_equipedItems = new List<InventoryItem>();
#if UNITY_EDITOR
            Add(TestData1);
            Add(TestData2);
#endif            

            isLoaded = true;
        }

        public void Add(InventoryItemData refData)
        {
            if (d_inventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.AddToStack();
            }
            else
            {
                InventoryItem _newitem = new InventoryItem(refData);
                L_inventory.Add(_newitem);
                d_inventoryDictionary.Add(refData, _newitem);

            }
            OnInventoryUpdated?.Invoke();
        }

        public void Remove(InventoryItemData refData)
        {
            if (d_inventoryDictionary.TryGetValue(refData, out InventoryItem value))
            {
                value.RemoveFromStack();
                if (value.stack == 0)
                {
                    L_inventory.Remove(value);
                    d_inventoryDictionary.Remove(refData);
                }
            }
            else {
                var msg = "You dont event have the item";
                UnityEngine.Debug.Log(DebugUtils.GetMessageFormat(msg,4));
            }
            OnInventoryUpdated?.Invoke();
        }

        public InventoryItem Get(InventoryItemData referenceData)
        {
            if (d_inventoryDictionary.TryGetValue(referenceData, out InventoryItem value))
            {
                return value;
            }
            return null;
        }

        

    }
}
