
using System;

using UnityEngine;

namespace Assets.Source.Utilities
{
    public class GameEvents: LoaderBase<GameEvents>
    {

        public override void Init()
        {
            isLoaded = true;
        }

        public event Action<string> onComputerTriggerEnter;
        public void ComputerTriggerEnter(string id)
        {
            if (onComputerTriggerEnter != null)
            {
                onComputerTriggerEnter(id);
            }
        }

        public event Action<string> onComputerTriggerExit;
        public void ComputerTriggerExit(string id) {
            if (onComputerTriggerEnter != null)
            {
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
    }

    
}
