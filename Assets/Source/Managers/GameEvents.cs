
using Assets.Source.Utilities.Events;
using Assets.Source.Utilities.Events.ComputerDoor;
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Source.Utilities
{
    public class GameEvents: LoaderBase<GameEvents>
    {
        private MonoBehaviour _bufferComponent;
        public MonoBehaviour BufferIdComputer { get { return _bufferComponent; } }
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
        public event Action<string> onComputerTriggerExit;
        public event Action<string> onDoorTriggerEnter;
        public event Action<string> onDoorTriggerExit;
        public event Action<string> onNpcTriggerEnter;
        public event Action<string> onNpcTriggerExit;

        public event Action<InventoryItemData> onPickableItemEnter;


        public void RequestInteractInteractable() {
            DialogManager.Instance?.RequestOpen();
            if(BufferIdComputer != null)
            {
                switch (BufferIdComputer) {
                    case ComputerSecurityController c:
                        c.UnlockDoor();
                        break;
                    default:
                        Debug.LogError("There is not component register, activate debug");
                        break;
                }
            }
        }


        public void OnPickableItemEnter(InventoryItemData data) {
            if (data != null) {
                onPickableItemEnter(data);
            }
        }

        public void OnComponentWithTriggerEnter(MonoBehaviour _go, string id) { 
            switch (_go)
            {
                case DoorController _door:
                    if (onDoorTriggerEnter != null) { 
                        onDoorTriggerEnter?.Invoke(id);
                    }
                    break;
                case ComputerSecurityController _sccomputer:
                    if (onComputerTriggerEnter != null) {
                        _bufferComponent = _go;
                        onComputerTriggerEnter?.Invoke(id);
                    }
                    break;
                case ComputerController _computer:
                    if (onComputerTriggerEnter != null)
                    {
                        onComputerTriggerEnter?.Invoke(id);
                    }
                    break;
                case NpcController _npc:
                    if (onNpcTriggerEnter != null)
                    {
                        _bufferComponent = _go;
                        onNpcTriggerEnter?.Invoke(id);
                    }
                    break;
                default:
                    Debug.LogWarning($"The current: {_go.name} is trying to TriggerEnter but has not Observer");
                    break;
            }
        }

        public void OnComponentWithTriggerExit(MonoBehaviour _go, string id)
        {
            switch (_go)
            {
                case DoorController _door:
                    if (onDoorTriggerExit != null)
                    {
                        onDoorTriggerExit?.Invoke(id);
                    }
                    break;
                case ComputerSecurityController _sccomputer:
                    if (onComputerTriggerExit != null)
                    {
                        onComputerTriggerExit?.Invoke(id);
                        _bufferComponent = null;
                    }
                    break;
                case ComputerController _computer:
                    if (onComputerTriggerExit != null)
                    {
                        onComputerTriggerExit?.Invoke(id);
                    }
                    break;
                case NpcController _npc:
                    if (onNpcTriggerExit != null)
                    {
                        onNpcTriggerExit?.Invoke(id);
                        _bufferComponent = null;
                    }
                    break;
                default:
                    Debug.LogWarning($"The current: {_go.name} is trying to TriggerExit but has not Observer");
                    break;
            }
        }
        
    }   
}
