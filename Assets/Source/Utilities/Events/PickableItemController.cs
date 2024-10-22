

using Assets.Source.Managers;
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class PickableItemController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public InventoryItemData inventoryItemData;
        protected void Start()
        {
            var _ta = GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { 
                GameEvents.Instance?.OnPickableItemEnter(inventoryItemData);
                PoolManager.Instance.ReturnInventoryGo(inventoryItemData, gameObject);
            };   
        }
    }
}
