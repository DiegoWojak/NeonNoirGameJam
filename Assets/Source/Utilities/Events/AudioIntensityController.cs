
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Source.Utilities.Events
{
    public class AudioIntensityController : MonoBehaviour
    {
        [SerializeField]
        public UnityEvent PostAction;
        public string parameter = "Intensity";
        public float value;
        public string id;
        private void Start()
        {
            var _ta = GetComponent<TriggerArea>();
            id = _ta.id;
            _ta.RelatedActionOnEnter = delegate { GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };

        }


    }
}
