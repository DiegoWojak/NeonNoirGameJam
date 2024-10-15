using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragSlot : MonoBehaviour, IDropHandler
{
    [SerializeField]
    private int _indexSlot;
    public Action<DragSlot> OnDropSlotEvent;
    public RectTransform RectTransform { get { return _rect; } }
    private RectTransform _rect;

    private RectTransform _parent;
    public RectTransform RctParent => _parent;
    private void Awake()
    {
        _indexSlot = transform.GetSiblingIndex();
        if(_rect == null){
            gameObject.name = $"Slot{transform.GetSiblingIndex()}";
            _rect = GetComponent<RectTransform>();
            _parent = transform.parent as RectTransform;
        }
    }

    private void OnEnable()
    {
         OnDropSlotEvent += DragManager.Instance.SlotDropEvent;
    }

    private void OnDisable()
    {
        OnDropSlotEvent -= DragManager.Instance.SlotDropEvent;
    }

    /// <summary>
    /// Drag Slot store the points where the Drag items should allocate
    /// </summary>
    /// <param name="eventData"></param>
    public void OnDrop(PointerEventData eventData)
    {
        //--> This is the Dragged ObjetGameObject _go = eventData.pointerDrag;
        //Use this if needed, currently you dont need it
        OnDropSlotEvent?.Invoke(this);
    }
}
