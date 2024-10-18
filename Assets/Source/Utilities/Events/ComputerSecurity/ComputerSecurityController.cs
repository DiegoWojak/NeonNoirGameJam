using Assets.Source.Utilities.Events.ComputerSecurity;
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
        [SerializeField]
        protected override void OnComputerEnterAreaToInteract(string id)
        {
            base.OnComputerEnterAreaToInteract(id);            
        }
        
        
    }
}
