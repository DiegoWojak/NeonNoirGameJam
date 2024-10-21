
using UnityEngine;

[CreateAssetMenu(menuName  = "Inventory Item Data")]
public class InventoryItemData : ScriptableObject
{
    [System.Serializable]
    public struct ToolTipData { 
        public string tooltipHeader;
        public string tooltipText;
    }

    public string id;
    public string displayName;
    public Sprite icon;
    public GameObject prefab_Game;
    public ToolTipData tooltíp_Data;
}
