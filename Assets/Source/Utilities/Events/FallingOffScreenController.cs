
using Assets.Source.Managers;
using UnityEngine;

namespace Assets.Source.Utilities.Events
{
    public class FallingOffScreenController : MonoBehaviour
    {
        [SerializeField]
        string id;

        private void Start()
        {
            var _te = GetComponent<TriggerArea>();
            _te.RelatedActionOnEnter = delegate { GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.FallingFromVoid); GameEvents.Instance?.OnComponentWithTriggerEnter(this, id); };
        }


        private void OnDestroy()
        {
            
        }
    }
}
