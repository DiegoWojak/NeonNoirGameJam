

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Assets.Source.Managers
{
    public class UIManager: LoaderBase<UIManager>
    {
        [HideInInspector]
        public GameObject Panel { get { return _panel; } }
        [SerializeField]
        private GameObject _panel;
        public Stack<IInitiable> _currentManagerUsing;

        public Action<bool> OnRequestOpen;
        public Action<bool> OnRequestClose;

        public override void Init()
        {
            _currentManagerUsing = new Stack<IInitiable>();
            Panel.SetActive(false);
            isLoaded = true;
        }

        public void RequestOpenUI(IInitiable _requireEnt) { 
            if(_requireEnt != null)
            {
                _currentManagerUsing.Push(_requireEnt);
                OnRequestOpen?.Invoke(true);
            }
            { 
            
                OnRequestOpen?.Invoke(false); 
            }

            if (_currentManagerUsing.Count > 0) { 
                Panel.SetActive(true);
            }
        }

        public void RequestCloseUI(IInitiable _requireEnt) {
            if (_requireEnt != null && _currentManagerUsing.Peek() == _requireEnt)
            {
                _currentManagerUsing.Pop();
                OnRequestClose?.Invoke(true);
            }
            else {
                OnRequestClose?.Invoke(false);
            }

            if (_currentManagerUsing.Count < 1) { 
                Panel.SetActive(false);
            }
        }

    }
}
