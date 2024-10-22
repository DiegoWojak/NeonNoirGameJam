
using Assets.Source.Utilities.Helpers.Gizmo;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Managers
{
    public class PoolManager : LoaderBase<PoolManager>
    {
        [System.Serializable]
        public struct PoolData {
            public InventoryItemData inventoryItemData;
            public Transform poolindex;
            public int initialPoolSize;
        }
        [Header("UI Elements")]
        public RectTransform UIPrefab;
        private Queue<RectTransform> UIPool = new Queue<RectTransform>();
        public int UIPoolSize = 10;
        public Transform UIPoolIndex;

        [Space(10)]
        [Header("InventoryItemData Elements")]
        private Dictionary<InventoryItemData, Queue<GameObject>> InventoryItemsPool = new Dictionary<InventoryItemData, Queue<GameObject>>();

        [SerializeField]
        private PoolData GlassesRGB;
        [SerializeField]
        private PoolData GlassesTV;

        [Space(10)]
        [Header ("Particle System")]
        public int initialPool = 10;
        [SerializeField]
        private GameObject _DashPrefabRender;
        private Queue<GameObject> _DashPool = new Queue<GameObject>();
        [SerializeField]
        private Transform _dashPoolIndex;
        public override void Init()
        {
            for (int i = 0; i < UIPoolSize; i++)
            {
                RectTransform icon = Instantiate(UIPrefab, UIPoolIndex);
                icon.gameObject.name = "pooled";
                icon.gameObject.SetActive(false); // Inactivos por defecto
                UIPool.Enqueue(icon);
            }

            for (int i = 0; i < initialPool; i++) 
            {
                GameObject _go = Instantiate(_DashPrefabRender, _dashPoolIndex);
                _go.gameObject.name = "pooled";
                _go.gameObject.SetActive(false);
                _DashPool.Enqueue(_go);
            }

            AddToDictionaryAndQueu(ref GlassesRGB);
            AddToDictionaryAndQueu(ref GlassesTV);

            isLoaded = true;
        }

        public RectTransform GetUIDraggable(RectTransform _parent)
        {
            if (UIPool.Count > 0)
            {
                RectTransform icon = UIPool.Dequeue();
                icon.gameObject.SetActive(true); // Activar el ícono
                icon.parent = _parent;
                return icon;
            }
            else
            {
                // Si no hay íconos disponibles, crea uno nuevo
                RectTransform newIcon = Instantiate(UIPrefab, _parent);
                return newIcon;
            }
        }

        // Devolver un ícono al pool
        public void ReturnIcon(RectTransform icon)
        {
            icon.SetParent(UIPoolIndex);
            icon.gameObject.name = "pooled";
            icon.gameObject.SetActive(false);
            UIPool.Enqueue(icon);
        }

        public GameObject GetDashObject(Transform _t)
        {
            if (_DashPool.Count > 0)
            {
                GameObject _go = _DashPool.Dequeue();
                _go.SetActive(true); // Activar el ícono
                _go.transform.SetParent(_t);
                _go.transform.localPosition = Vector3.zero;
                _go.transform.localRotation = Quaternion.identity;
                return _go;
            }
            else
            {
                // Si no hay íconos disponibles, crea uno nuevo
                GameObject _go = Instantiate(_DashPrefabRender,_t);
                return _go;
            }
        }

        public void ReturnDashObject(GameObject obj) {
            obj.transform.SetParent(_dashPoolIndex);
            obj.SetActive(false);
            _DashPool.Enqueue(obj);
        }

        public GameObject GetInventoryGameobjectPool(InventoryItemData _inventoryItemData) {
            if (!InventoryItemsPool.ContainsKey(_inventoryItemData)) {
                string msg = $"There is no Gameobject with {_inventoryItemData.name} name, Create one Before";
                Debug.Log(DebugUtils.GetMessageFormat(msg,0));
                return null;
            }

            if (InventoryItemsPool[_inventoryItemData].Count > 0)
            {
                var _go = InventoryItemsPool[_inventoryItemData].Dequeue();
                _go.SetActive(true);
                _go.transform.SetParent(null);
                return _go;
            }
            else 
            { 
                GameObject _go = Instantiate(_inventoryItemData.prefab_Game);            
                return _go;
            }
        }

        public void ReturnInventoryGo(InventoryItemData _inventoryItemData, GameObject go)
        {
            switch (_inventoryItemData)
            {
                case var item when item == GlassesRGB.inventoryItemData:
                    go.SetActive(false);
                    go.gameObject.name = "pooled";
                    go.transform.SetParent(GlassesRGB.poolindex);
                    InventoryItemsPool[_inventoryItemData].Enqueue(go);
                    break;

                case var item when item == GlassesTV.inventoryItemData:
                    go.SetActive(false);
                    go.gameObject.name = "pooled";
                    go.transform.SetParent(GlassesTV.poolindex);
                    InventoryItemsPool[_inventoryItemData].Enqueue(go);
                    break;

                // Add more cases if needed
                default:
                    Debug.LogWarning("Unknown inventory item data");
                    break;
            }
        }

        private void AddToDictionaryAndQueu(ref PoolData _poolData) {
            InventoryItemsPool.TryAdd(_poolData.inventoryItemData, new Queue<GameObject>());
            
            for (int i = 0; i < _poolData.initialPoolSize; i++)
            {
                GameObject _go = Instantiate(_poolData.inventoryItemData.prefab_Game, _poolData.poolindex);
                _go.SetActive(false);
                InventoryItemsPool[_poolData.inventoryItemData].Enqueue(_go);
            }
        }

    }
}
