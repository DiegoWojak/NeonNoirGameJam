
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class ComputerController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public string DialogText;
        protected TriggerArea _ta;
        protected void Start()
        {
            GameEvents.Instance.onComputerTriggerEnter += OnComputerEnterAreaToInteract;
            GameEvents.Instance.onComputerTriggerExit += OnComputerExitAreaToInteract;
            _ta = GetComponent<TriggerArea>();
            id = _ta.id;
            LinkTriggers();
        }



        protected virtual void OnComputerEnterAreaToInteract(string id) {
            if (id == this.id) 
            {
                DialogManager.Instance?.OnRequestStringChange.Invoke(this.id,DialogText);
            }
        }

        protected virtual void OnComputerExitAreaToInteract(string id) {
            if (id == this.id)
            {
                DialogManager.Instance?.OnRequestClean.Invoke();
            }
        }

        void OnDestroy()
        {
            if (GameEvents.Instance != null) { 
                GameEvents.Instance.onComputerTriggerEnter -= OnComputerEnterAreaToInteract;
                GameEvents.Instance.onComputerTriggerExit -= OnComputerExitAreaToInteract;
            }
            PostDestroy();
        }

        protected virtual void PostDestroy() { 
            //clean
        }

        protected virtual void LinkTriggers() 
        {
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.OnComponentWithTriggerExit(this, id); };
        }
    }
}
