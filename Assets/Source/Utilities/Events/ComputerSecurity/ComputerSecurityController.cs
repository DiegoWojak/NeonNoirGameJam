
using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Source.Utilities.Events.ComputerDoor
{
    [Serializable]
    public class ComputerSecurityController : ComputerController
    {
        public List<DoorController> Doors { get { return _doors; }}
        [SerializeField]
        private List<DoorController> _doors = new List<DoorController>(); //Should be an IUnlockableObject or IInteratableObject

        public Action OnInteract;

        protected override void LinkTriggers()
        { 
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };
            _ta.RelatedActionOnLeave = delegate { GameEvents.Instance?.OnComponentWithTriggerExit(this, id); };
        }


        public void UnlockDoor() { 
            for (int i=0;i<Doors.Count;i++)
            {
                Doors[i].Unlock();
            }
        }


        protected override void PostDestroy()
        {
            OnInteract -= UnlockDoor;
        }

    }
}
