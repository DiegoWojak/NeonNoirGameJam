
using System;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine;



namespace Assets.Source.Managers.Components
{
    public class EffectsManagerComponent
    {
        private List<EffectDictionary> d_Effect;
        private int ItemCapacity;

        public EffectsManagerComponent(int _ItemsCapacity, List<EffectDictionary> _dic) { 
            ItemCapacity = _ItemsCapacity;
            d_Effect = _dic;
        }

        public void ApplyAllEffectsFromEquippedInventory() {
            var _list_equipped = InventorySystem.Instance.L_equipedItems;
            int _to = Mathf.Min(ItemCapacity,_list_equipped.Count);

            for (int i = 0; i < _to; i++)
            {
                if (_list_equipped[i] != null){
                    var _result = d_Effect.Find(x => x._relatedItem == _list_equipped[i].ItemData);
                    _result.OnEquippingItemEffect?.Invoke();
                }
            }
        }

        public void RemoveEffectFromInventory(EffectDictionary _effect) {
            _effect.OnRemoveItemEffect?.Invoke();
        }

        public void ApplyEffectFromEquippedItem(EffectDictionary _effect) {
            _effect.OnEquippingItemEffect?.Invoke();
        }

    }




    [Serializable]
    public struct EffectDictionary
    {
        public InventoryItemData _relatedItem;
        [SerializeField]
        public UnityEvent OnEquippingItemEffect;
        [SerializeField]
        public UnityEvent OnRemoveItemEffect;

    }


    public interface IEffects {

        void ApplyEffect();
        void RemoveEffect();
    }

}
