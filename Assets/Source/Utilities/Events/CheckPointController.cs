

using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class CheckPointController : MonoBehaviour
    {
        [SerializeField]
        string id;
        private void Start()
        {
            var _te = GetComponent<TriggerArea>();
            _te.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };
        }
    }
}
