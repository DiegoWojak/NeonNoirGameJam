
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class DragableItem : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    private DragManager _manager { get { return DragManager.Instance; } }

    private Vector2 _centerPoint;
    private Vector2 _worldCenterPoint => transform.TransformPoint(_centerPoint);
    
    public RectTransform RectTranform { get { return _rect; } }
    private RectTransform _rect;

    private RectTransform _parent;
    public RectTransform RctParent => _parent;

    bool _hasInited = false;
    public bool HasInited { get { return _hasInited; } }

    public string id;
    public Image Icon;
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;
        _manager.RegisterDraggedObject(this);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;

        if (_manager.IsWithinBounds(_worldCenterPoint + eventData.delta))
        {
            transform.Translate(eventData.delta);
            //transform.Translate(Input.mousePosition);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;

        _manager.UnregisterDraggedObject(this);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!_hasInited) { 
            Init();    
        }
    }

    void Init() { 
        //_manager = DragManager.Instance;
        _rect = transform as RectTransform;
        _centerPoint = _rect.rect.center;
        _parent = transform.parent as RectTransform;
        _hasInited = true;
    }

    public void UpdateParent() {
        _parent = transform.parent as RectTransform;
        if (!_hasInited) {
            Init();
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;

        DragManager.Instance.OtherDragItemSlotSelected(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;
        _manager.OnDraggableitemHover(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;
        _manager.OnExitItemHover(this);
    }
}
