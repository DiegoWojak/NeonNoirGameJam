
using Assets.Source.Utilities.Events.ComputerSecurity;

using UnityEngine;
using UnityEngine.Events;

namespace Assets.Source.Utilities.Events
{
    public class DoorController : MonoBehaviour , IActivableObject
    {
        [HideInInspector]
        public string id;
        public GameObject FakeWall;
        public ParticleSystem Particle;
        [SerializeField]
        public UnityEvent PostAction;

        public DoorStatus status = DoorStatus.Unlocked;
        public void Start()
        {
            GameEvents.Instance.onDoorTriggerEnter += OnDoorTriggerEnter;
            GameEvents.Instance.onDoorTriggerExit += OnCDoorExitTriggerExit;

            var _ta =GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this,id); };
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.OnComponentWithTriggerExit(this,id); };
        }


        protected void OnDoorTriggerEnter(string id)
        {
            if (id == this.id && status == DoorStatus.Unlocked)
            {
                FakeWall.SetActive(false);
            }
        }

        protected void OnCDoorExitTriggerExit(string id)
        {
            if (id == this.id)
            {
                FakeWall.SetActive(true);
            }
        }

        protected virtual void OnDestroy()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onComputerTriggerEnter -= OnDoorTriggerEnter;
                GameEvents.Instance.onComputerTriggerExit -= OnCDoorExitTriggerExit;
            }
        }

        public virtual void Unlock()
        {
            status = DoorStatus.Unlocked;
            PostAction?.Invoke();
        }
     
    }

    public enum DoorStatus { 
        Unlocked,
        Locked
    }

}
