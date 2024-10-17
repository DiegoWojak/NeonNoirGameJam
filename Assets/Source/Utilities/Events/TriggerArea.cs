using Assets.Source.Render.Characters;
using System;
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class TriggerArea : MonoBehaviour
    {
        public string id;
        public Action<string> RelatedActionOnEnter;
        public Action<string> RelatedActionOnLeave;
        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.name == My3DHandlerPlayer.Instance.Character.gameObject.name) {
                Debug.Log($"Gameobject {other.name} has entered to Trigger Area with ID {id} ");
                RelatedActionOnEnter?.Invoke(id);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.gameObject.name == My3DHandlerPlayer.Instance.Character.gameObject.name)
            {
                Debug.Log($"Gameobject {other.name} is Leaving the Trigger Area with ID {id}");
                RelatedActionOnLeave?.Invoke(id);
            }
        }
    }
}
