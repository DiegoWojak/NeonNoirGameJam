﻿using Assets.Source.Managers.Components;
using System;
using UnityEngine;

namespace Assets.Source.Data.Models
{
    [Serializable]
    public class InventoryItem
    {
        public int stack { get; private set; }
        public InventoryItemData ItemData { get; private set; }
       
        public bool IsEquipped { get; private set; }
        public InventoryItem(InventoryItemData _dataref) 
        {
            stack = 0;
            ItemData = _dataref;
            AddToStack();
        }

        public void AddToStack() { 
            stack++;
        }

        public void RemoveFromStack()
        {
            stack--;
        }


        public void Equip() {
            IsEquipped = true;
        }

        public void UnEquip() {
            IsEquipped = false;
        }
    }
}
