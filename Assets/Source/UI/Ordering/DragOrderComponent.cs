using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Source.UI.Ordering
{
    public class DragOrderComponent
    {
        private Rect _rect;
        TargetsData _currentTargetRect;
        TargetsData _otherTarget;

        private struct TargetsData {
            public Vector2 _vector2;
            public RectTransform _parent;

            public void Clear() {
                _vector2 = Vector2.zero;
                _parent = null;
            }
        }

        RectTransform[] _stacks = new RectTransform[2];
        public DragOrderComponent(RectTransform _comp, Action<DragableItem, DragableItem> ActionSwitch, Action<DragableItem> ActionMoving) { 
            
            _rect = (_comp).rect;
            var _width = _rect.width;
            var _height = _rect.height;
#if UNITY_EDITOR
            Debug.Log(string.Format(" Object : {0} \n Information: \n " +
                "_Width : {1} _Heigh {2}",
                _comp.gameObject.name,
                _width, _height));
#endif
            DragManager.Instance.OnBeginDrag+= StoreFirstDragInformation ;
            DragManager.Instance.OnDraggedItem += HandleUI;
            DragManager.Instance.OnEmptySlotDropped += StorSlotInfotmation;
            DragManager.Instance.OnOtherDragItemSelected += StoreOtherDragItemInformation;

            OnSwitchingItem += ActionSwitch;
            OnMovingItem += ActionMoving;
        }

        public Action<DragableItem, DragableItem> OnSwitchingItem;
        public Action<DragableItem> OnMovingItem;

        private void HandleUI(RectTransform _parent, DragableItem _itemInfo) {
            if (_stacks[1] != null)
            {
                var _other = _stacks[1];
                var _current = _stacks[0];

                _current.SetParent(_otherTarget._parent);
                _other.SetParent(_currentTargetRect._parent);

                if (_otherTarget._parent != _currentTargetRect._parent) { 
                    _current.GetComponent<DragableItem>().UpdateParent();
                    _other.GetComponent<DragableItem>().UpdateParent();
                }

                _current.anchoredPosition = _otherTarget._vector2;
                _other.anchoredPosition = _currentTargetRect._vector2;
                OnSwitchingItem?.Invoke(_current.GetComponent<DragableItem>(), _other.GetComponent<DragableItem>());
            }
            else {
                _itemInfo.transform.SetParent(_currentTargetRect._parent);
                _itemInfo.RectTranform.anchoredPosition = _currentTargetRect._vector2;
                _itemInfo.UpdateParent();

                OnMovingItem?.Invoke(_itemInfo);
            }

            ClearStack();
        }

        private void StoreFirstDragInformation(RectTransform _parent, DragableItem _itemInfo)
        {
            _currentTargetRect._vector2 = _itemInfo.RectTranform.anchoredPosition;
            _currentTargetRect._parent = _parent;

            _stacks[0] = _itemInfo.RectTranform;
        }

        private void StoreOtherDragItemInformation(RectTransform _parent, DragableItem _otherInfo) {
            _otherTarget._vector2 = _otherInfo.RectTranform.anchoredPosition;
            _otherTarget._parent = _parent;
            _stacks[1] = _otherInfo.RectTranform;
        }
        

        private void StorSlotInfotmation(RectTransform _parent, DragSlot _slotInfo) {
            _currentTargetRect._vector2 = _slotInfo.RectTransform.anchoredPosition;
            _currentTargetRect._parent = GetRelativeParent(_slotInfo);
        }

        private void ClearStack() 
        {
            //clear
            for (int i = 0; i < _stacks.Length; i++)
            {
                _stacks[i] = null;
            }

            _currentTargetRect.Clear();
            _otherTarget.Clear();
        }

        public void UnParent(DragableItem _item,Transform DragabbleLayerParent) {
            _item.transform.SetParent(DragabbleLayerParent);
        }

        private RectTransform GetRelativeParent(DragSlot _slot) {
            return DragManager.Instance?.GetRelativeSlot(_slot);
        }
    }
}
