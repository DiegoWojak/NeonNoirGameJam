
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class ComputerController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public string DialogText;
        public void Start()
        {
            GameEvents.Instance.onComputerTriggerEnter += OnComputerStuffs;
            GameEvents.Instance.onComputerTriggerExit += OnComputerExitClean;
            var _ta = GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.ComputerTriggerEnter(id);};
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.ComputerTriggerExit(id); };
        }

        private void OnComputerStuffs(string id) {
            if (id == this.id) 
            {
                DialogManager.Instance?.OnRequestStringChange.Invoke(DialogText);
            }
        }

        private void OnComputerExitClean(string id) {
            if (id == this.id)
            {
                DialogManager.Instance?.OnRequestClean.Invoke();
            }
        }

        private void OnDestroy()
        {
            if (GameEvents.Instance != null) { 
                GameEvents.Instance.onComputerTriggerEnter -= OnComputerStuffs;
                GameEvents.Instance.onComputerTriggerExit -= OnComputerExitClean;
            }
        }
    }
}
