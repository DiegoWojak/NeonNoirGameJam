
using System;
using System.Collections.Generic;

using UnityEngine;


namespace Assets.Source.Managers
{
    public class UIManager: LoaderBase<UIManager>
    {
        [HideInInspector]
        public GameObject Panel { get { return _panel; } }
        [SerializeField]
        private GameObject _panel;
        public Stack<IInitiable> _currentManagerUsing;     

        public bool IsAnyUIOpened { get { return _currentManagerUsing.Count > 0; } }
        public override void Init()
        {
            _currentManagerUsing = new Stack<IInitiable>();
            ClearUI();
            isLoaded = true;
        }

        private void ClearUI() { 
            Panel.SetActive(false);
            for(int i=0; i<Panel.transform.childCount; i++)
            {
                Panel.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        public void RequestOpenUI(IInitiable _requireEnt, Action<bool> RequestMessage = null) { 
            if(_requireEnt != null)
            {
                _currentManagerUsing.Push(_requireEnt);
                RequestMessage?.Invoke(true);
            }
            {
                RequestMessage?.Invoke(false);
                //OnAfterRequestOpen?.Invoke(false); 
            }

            if (_currentManagerUsing.Count > 0) {
#if UNITY_EDITOR
                Debug.Log("Unlocking");
#endif
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Panel.SetActive(true);
            }
        }

        public void RequestCloseUI(IInitiable _requireEnt, Action<bool> RequestMessage = null) {
            if (_requireEnt != null && _currentManagerUsing.Peek() == _requireEnt)
            {
                _currentManagerUsing.Pop();
                RequestMessage?.Invoke(true);
            }
            else {
                RequestMessage?.Invoke(false);
            }

            if (_currentManagerUsing.Count < 1) {
#if UNITY_EDITOR
                Debug.Log("locked");
#endif
                Panel.SetActive(false);
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

    }
}
