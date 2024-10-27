
using Assets.Source.Managers;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Source.Utilities.Events
{
    [System.Serializable]
    public class NpcController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public string MyName;
        public string DialogText;
        public Sprite photo;
        public Sprite btnIcon;
        public InventoryItemData Gift;
        public GameObject IconAlert;

        [SerializeField]
        public UnityEvent PostAction;
        public bool HasNewInteractions { get; private set; }
        private void Start()
        {
            HasNewInteractions = true;
            #region Callbacks
            GameEvents.Instance.onNpcTriggerEnter += OnPlayerEnterNpcArea;
            GameEvents.Instance.onNpcTriggerExit += OnPlayerExitNpcArea;
            #endregion
            var _ta = GetComponent<TriggerArea>();
            id = _ta.id;

            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.OnComponentWithTriggerExit(this, id); };

            GameEvents.Instance.OnInteract += OnInteract;
        }

        public virtual void OnPlayerEnterNpcArea(string id) {
            if (id == this.id) 
            {
                DialogManager.Instance?.OnRequestStringChange.Invoke(MyName, DialogText, photo, btnIcon);
            }
        }

        public virtual void OnPlayerExitNpcArea(string id)
        {
            if (id == this.id)
            {
                DialogManager.Instance?.OnRequestClean.Invoke();
            }
        }

        public bool IsConditionMeet() {
            if (!GameStarterManager.Instance.IsLoaded()) return false;        
            return InventorySystem.Instance.d_InventoryDictionary.ContainsKey(Gift);
        }

        public void PostAlgo() {
            PostAction?.Invoke();
            DialogManager.Instance.OnUIClose -= PostAlgo;
        }

        public void RequestEnding() {
            GameStarterManager.Instance.CompletetLevel();
        }

        private void OnDestroy()
        {
            if (GameEvents.Instance != null) 
            {
                GameEvents.Instance.onNpcTriggerEnter -= OnPlayerEnterNpcArea;
                GameEvents.Instance.onNpcTriggerExit -= OnPlayerExitNpcArea;
            }
        }

        private void OnInteract(MonoBehaviour _itself) {
            if (_itself == this && ((NpcController)_itself).id == this.id) {
                DialogManager.Instance.OnUIClose += PostAlgo;
            }
        }

    }
}
