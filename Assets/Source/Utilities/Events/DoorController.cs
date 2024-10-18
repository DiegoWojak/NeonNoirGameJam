
using System;
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class DoorController : MonoBehaviour
    {
        [HideInInspector]
        public string id;
        public GameObject FakeWall;
        public ParticleSystem Particle;

        public DoorStatus status = DoorStatus.Unlocked;
        public void Start()
        {
            GameEvents.Instance.onDoorTriggerEnter += OnDoorTriggerEnter;
            GameEvents.Instance.onDoorTriggerExit += OnCDoorExitTriggerExit;

            var _ta =GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.DoorTriggerEnter(id); };
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.DoorrTriggerExit(id); };
        }


        private void OnDoorTriggerEnter(string id)
        {
            if (id == this.id)
            {
                FakeWall.SetActive(false);
            }
        }

        private void OnCDoorExitTriggerExit(string id)
        {
            if (id == this.id)
            {
                FakeWall.SetActive(true);
            }
        }

        private void OnDestroy()
        {
            if (GameEvents.Instance != null)
            {
                GameEvents.Instance.onComputerTriggerEnter -= OnDoorTriggerEnter;
                GameEvents.Instance.onComputerTriggerExit -= OnCDoorExitTriggerExit;
            }
        }
    }

    public enum DoorStatus { 
        Unlocked,
        Locked
    }

}
