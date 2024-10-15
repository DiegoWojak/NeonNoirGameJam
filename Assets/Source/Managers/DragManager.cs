using Assets.Source;
using Assets.Source.UI.Ordering;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragManager : LoaderBase<DragManager>
{
    private static DragManager _instance;
    public static DragManager Instance { get { return _instance; } }

    [HideInInspector]
    public RectTransform DefaultParentLayer { get { return InventoryGroup._defaultParentLayer; } }
    [HideInInspector]
    public RectTransform SlotLayer { get { return InventoryGroup._slotLayer; } }
    [HideInInspector]
    public RectTransform DragLayer { get { return _dragLayer; } }
    [HideInInspector]
    public RectTransform PlayerSlotLayer { get { return PlayerEquipedGroup._slotLayer; } }
    [HideInInspector]
    public RectTransform PlayerEquippedLayer { get { return PlayerEquipedGroup._defaultParentLayer; } }


    [SerializeField]
    private LayerGroup InventoryGroup;
    [SerializeField]
    private LayerGroup PlayerEquipedGroup;

    [SerializeField]
    private RectTransform _dragLayer = null;

    private Rect _boundingBox;

    private DragableItem _currentDraggedObject = null;
    public DragableItem CurrentDraggedObject { get { return _currentDraggedObject; } }

    /// <summary>
    /// IsDragging In Process 
    /// False = NO True YES
    /// </summary>
    public bool IsDragging { get { return CurrentDraggedObject == null ? false : true; } }
    [System.Serializable]
    private struct LayerGroup {
        public string name;
        public RectTransform _defaultParentLayer;
        public RectTransform _slotLayer;
    }

    public Action<bool> RequestUI;

    /// <summary>
    ///                   DragItem
    ///                    ______
    ///                    |    |
    ///                    |    |   --> OnDraggedItem
    ///               > > >|____|                          
    ///               |      None(World outside canvas)  
    ///               |
    ///               |                   DragItem
    ///               ^                    ________
    ///               ^                    |      |
    ///               ^                    |______|
    ///               ^   > > > > > > > >   ________   --> OnOtherDragItemSelected --> OnDraggItem
    ///               ^   |                 |      |
    ///               ^   |                 |______|
    ///               ^   |                 DragItem           
    ///               ^   |                                     
    ///       OnBegingDrag                                 
    ///               |      DragItem
    ///               |      ______                        
    ///               |      |    |                        
    ///               |      |    |  --> OnEmptySlotDropped -> OnDraggedItem
    ///               |      |____|
    ///               > > > xxxxxxx 
    ///                     SlotItem
    /// 
    /// </summary>
    public Action<RectTransform, DragableItem> OnDraggedItem;
    public Action<RectTransform, DragableItem> OnBeginDrag;
    public Action<RectTransform, DragableItem> OnOtherDragItemSelected;
    public Action<RectTransform, DragSlot> OnEmptySlotDropped;

    private DragOrderFix _dragOrderFix;

    [SerializeField]
    private Canvas _mainCanvas;

    public Canvas MainCanvas { get { return _mainCanvas; } }

    private bool isGameLoaded = false;

    public bool IsGameReady { get { return isGameLoaded; } }
    private void Awake()
    {
        
    }

    public override void Init()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        if (_dragOrderFix == null)
        {
            _dragOrderFix = new DragOrderFix(DefaultParentLayer);
        }

        SetBoundingBoxRect(_dragLayer);
        SetDimensionCanvasToScale();

        InitLayerGroup(DefaultParentLayer, SlotLayer);
        InitLayerGroup(PlayerEquippedLayer, PlayerSlotLayer);

        LoaderManager.OnEverythingLoaded += AllowInteraction;

        isLoaded = true;
    }

    private void OnEnable()
    {
        RequestUI += DragShowUI;
    }


    private void OnDisable()
    {
        RequestUI -= DragShowUI;
    }

    void AllowInteraction() {
        isGameLoaded = true;
        LoaderManager.OnEverythingLoaded -= AllowInteraction;
    }

    void SetDimensionCanvasToScale() {
        if (DragLayer == null || MainCanvas == null) 
        {
            throw new Exception("To continue be sure to assign the Canvas interaction for their Drag and Main components");
        }

        CanvasScaler DragcanvasScaler = DragLayer.GetComponent<CanvasScaler>();
        CanvasScaler mainScaler = MainCanvas.GetComponent<CanvasScaler>();

        if (DragcanvasScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize || 
            mainScaler.uiScaleMode != CanvasScaler.ScaleMode.ScaleWithScreenSize) 
        {
            throw new Exception("To continue be sure to set ScaleWithScreenSize as a option in both canvas scale components");
        }

        // To explain this imagine the Camera view is 1920 x 600 , so the adjusment for the canvas will be 600 is 1 pixel for 3.2 to be compared to 1920 or width / height
        // to a configurated canvas 1080
        // por 1 pixel to Up or Down it will be 3.2 pixel Left or Right

        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        int ScaleValue = screenWidth / screenHeight; 



    }

    private void InitLayerGroup(RectTransform _TargetDefaultLayer, RectTransform _targetSlotLayer) 
    {
        if (_TargetDefaultLayer != null)
        {
            PlayerSlotLayer.gameObject.SetActive(true);
        }

        if (_targetSlotLayer != null && DragManager.Instance != null)
        {
            var _childs = _targetSlotLayer.childCount;
            for (int i = 0; i < _childs; i++)
            {
                _targetSlotLayer.GetChild(i).gameObject.SetActive(true);
            }
        }
    }

    public void RegisterDraggedObject(DragableItem drag)
    {
        OnBeginDrag?.Invoke(drag.RctParent, drag);
        _currentDraggedObject = drag;
        UnParentItem(drag);
    }

    private void UnParentItem(DragableItem _item) => _dragOrderFix.UnParent(_item,DragLayer);

    public void UnregisterDraggedObject(DragableItem dragItem)
    {
        //dragItem.transform.SetParent(DefaultParentLayer);
        _currentDraggedObject = null;

        OnDraggedItem?.Invoke(dragItem.RctParent, dragItem);
    }

    public void SlotDropEvent(DragSlot _dragSlot)
    {
        OnEmptySlotDropped?.Invoke(_dragSlot.RctParent, _dragSlot);
    }

    public void OtherDragItemSlotSelected(DragableItem _otherItem) 
    {
        OnOtherDragItemSelected?.Invoke(_otherItem.RctParent, _otherItem);
    }  

    public bool IsWithinBounds(Vector2 position)
    {
        return _boundingBox.Contains(position);
    }

    private void SetBoundingBoxRect(RectTransform rectTransform)
    {
        var corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        var position = corners[0];

        Vector2 size = new Vector2(
            rectTransform.lossyScale.x * rectTransform.rect.size.x,
            rectTransform.lossyScale.y * rectTransform.rect.size.y);

        _boundingBox = new Rect(position, size);
    }

    public RectTransform GetRelativeSlot(DragSlot _slot) {
        if(_slot.RctParent == InventoryGroup._slotLayer)
            return InventoryGroup._defaultParentLayer;
        if (_slot.RctParent == PlayerEquipedGroup._slotLayer)
            return PlayerEquipedGroup._defaultParentLayer;

        throw new NotImplementedException("Slot Selected has no Layer defined ");
    }

    private void DragShowUI(bool isOn) {
        if (isGameLoaded)
        {
            DragLayer.gameObject.SetActive(isOn);
            MainCanvas.gameObject.SetActive(isOn);
        }
    }

}