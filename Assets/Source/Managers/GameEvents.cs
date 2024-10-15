using System;
using System.Collections.Generic;

using UnityEngine;

namespace Assets.Source.Utilities
{
    public class GameEvents: LoaderBase<GameEvents>
    {
        public override void Init()
        {
            isLoaded = true;
        }

        public event Action<int> onComputerTriggerEnter;
        public void ComputerTriggerEnter(int id)
        {
            if (onComputerTriggerEnter != null)
            {
                onComputerTriggerEnter(id);
            }
        }

        public event Action<int> onComputerTriggerExit;
        public void ComputerTriggerExit(int id) {
            if (onComputerTriggerEnter != null)
            {
                onComputerTriggerExit(id);
            }
        }
    }
}
