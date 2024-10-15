using Assets.Source.Render.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class TriggerArea : MonoBehaviour
    {
        public int id;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == My3DHandlerPlayer.Instance.Character.gameObject.name) {
                Debug.Log($"Gameobject {other.name} has entered");
                GameEvents.Instance?.ComputerTriggerEnter(id);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == My3DHandlerPlayer.Instance.Character.gameObject.name)
            {
                Debug.Log($"Gameobject {other.name} is Leaving");
                GameEvents.Instance?.ComputerTriggerExit(id);
            }
        }
    }
}
