
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
    private Image BtnBG;

    public Color OnDragColor;
    public Color OnHoverColor;
    public Color OnNormalColor;

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
            SetColorBtn(OnDragColor);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;

        _manager.UnregisterDraggedObject(this);
        
        SetColorBtn(OnNormalColor);
    }

    // Start is called before the first frame update
    void Awake()
    {
        if (!_hasInited) { 
            Init();    
        }   
    }

    private void OnEnable()
    {
        if(BtnBG != null)
        {
            OnNormalColor.a = 1;
            BtnBG.color = OnNormalColor;
        }
    }

    void Init() { 
        //_manager = DragManager.Instance;
        _rect = transform as RectTransform;
        _centerPoint = _rect.rect.center;
        _parent = transform.parent as RectTransform;
        _hasInited = true;
        BtnBG = GetComponent<Image>();
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

        SetColorBtn(OnHoverColor);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!_manager.IsGameReady) return;
        _manager.OnExitItemHover(this);
        SetColorBtn(OnNormalColor);
    }
    private LTDescr myLTDescr;
    private void SetColorBtn(Color color) 
    {
        color.a = 1;
        myLTDescr = LeanTween.color(BtnBG.rectTransform,color, 0.1f);
    }

}
