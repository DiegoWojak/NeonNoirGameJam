
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class ComputerController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public string DialogText;
        protected void Start()
        {
            GameEvents.Instance.onComputerTriggerEnter += OnComputerEnterAreaToInteract;
            GameEvents.Instance.onComputerTriggerExit += OnComputerExitAreaToInteract;
            var _ta = GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.ComputerTriggerEnter(id);};
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.ComputerTriggerExit(id); };
        }

        protected virtual void OnComputerEnterAreaToInteract(string id) {
            if (id == this.id) 
            {
                DialogManager.Instance?.OnRequestStringChange.Invoke(DialogText);
            }
        }

        protected virtual void OnComputerExitAreaToInteract(string id) {
            if (id == this.id)
            {
                DialogManager.Instance?.OnRequestClean.Invoke();
            }
        }

        protected void OnDestroy()
        {
            if (GameEvents.Instance != null) { 
                GameEvents.Instance.onComputerTriggerEnter -= OnComputerEnterAreaToInteract;
                GameEvents.Instance.onComputerTriggerExit -= OnComputerExitAreaToInteract;
            }
        }
    }
}
