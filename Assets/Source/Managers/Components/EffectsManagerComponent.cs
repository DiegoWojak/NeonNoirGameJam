
using System;
using System.Collections.Generic;

using UnityEngine.Events;
using UnityEngine;
using Assets.Source.Utilities.Helpers;



namespace Assets.Source.Managers.Components
{
    public class EffectsManagerComponent
    {
        private List<EffectDictionary> d_Effect;
        
        [SerializeField]
        private ApplyEffectFunctionsHelper Helper;
        public EffectsManagerComponent(List<EffectDictionary> _dic, ApplyEffectFunctionsHelper Helper) { 
            d_Effect = _dic;
            this.Helper = Helper;
            ApplyAllVisualEffectsFromEquippedInventory();

            DragManager.OnItemHasCHanged += ApplyAllVisualEffectsFromEquippedInventory;
        }    

        public void ApplyAllVisualEffectsFromEquippedInventory() {
            var _items = InventorySystem.Instance.d_InventoryDictionary;
            foreach (var item in _items)
            {
                var _i = d_Effect.Find(x => x._relatedItem == item.Key);
                if (item.Value.IsEquipped)
                {
                    _i.OnEquippingItemEffect?.Invoke();
                }
                else {
                    _i.OnRemoveItemEffect?.Invoke();
                }
            }
        }

        public void RemoveEffectFromInventory(EffectDictionary _effect) {
            _effect.OnRemoveItemEffect?.Invoke();
        }

        public void ApplyEffectFromEquippedItem(EffectDictionary _effect) {
            _effect.OnEquippingItemEffect?.Invoke();
        }

        public bool HasRGBGlasses() {
            return Helper._HasRGBGlasses;
        }
        public bool HasEquippedTvGlasses()
        {
            return Helper._hasTVGlasses;
        }

        public bool CanWallJump() {
            return Helper._CanWallJump;
        }

        public bool CanDoubleJump()
        {
            return Helper._CanDoubleJump;
        }

        public bool CanDash() {
            return Helper._CanDash;
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
