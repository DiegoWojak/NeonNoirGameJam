
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class ComputerController : MonoBehaviour
    {
        [HideInInspector]
        public int id;
        public string DialogText;
        public void Start()
        {
            GameEvents.Instance.onComputerTriggerEnter += OnComputerStuffs;
            GameEvents.Instance.onComputerTriggerExit += OnComputerExitClean;
            id = GetComponent<TriggerArea>().id;
        }

        private void OnComputerStuffs(int id) {
            if (id == this.id) 
            {
                DialogManager.Instance?.OnRequestStringChange.Invoke(DialogText);
            }
        }

        private void OnComputerExitClean(int id) {
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
