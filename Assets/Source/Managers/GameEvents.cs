
using Assets.Source.Utilities.Events;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Utilities
{
    public class GameEvents: LoaderBase<GameEvents>
    {
        private string _bufferIDCComputerFocus = "none";
        public string BufferIdComputer { get { return _bufferIDCComputerFocus; } }
        private Dictionary<string, DoorController> Doors;

        public override void Init()
        {
            var _objs = FindObjectsOfType(typeof(DoorController));
            Doors = new Dictionary<string, DoorController>();

            foreach (DoorController door in _objs)
            {
                Doors.TryAdd(door.id, door);
            }

            isLoaded = true;
        }

        public event Action<string> onComputerTriggerEnter;
        public void ComputerTriggerEnter(string id)
        {
            if (onComputerTriggerEnter != null)
            {
                _bufferIDCComputerFocus = id;
                onComputerTriggerEnter(id);
            }
        }

        public event Action<string> onComputerTriggerExit;
        public void ComputerTriggerExit(string id) {
            if (onComputerTriggerEnter != null)
            {
                _bufferIDCComputerFocus = "none";
                onComputerTriggerExit(id);
            }
        }


        public event Action<string> onDoorTriggerEnter;

        public void DoorTriggerEnter(string id) {
            if (onDoorTriggerEnter != null) {
                onDoorTriggerEnter(id);
            }
        }
        public event Action<string> onDoorTriggerExit;

        public void DoorrTriggerExit(string id) {
            if (onDoorTriggerExit != null)
            {
                onDoorTriggerExit(id);
            }
        }


        public void RequestInteractComputer() {
            DialogManager.Instance?.RequestOpen();
            if (Doors.ContainsKey(_bufferIDCComputerFocus)) { 
                Doors[_bufferIDCComputerFocus].Unlock();
            }
        }
    }

    
}
