
using FMODUnity;

namespace Assets.Source.Utilities.Events
{
    public class AudioIntensityComponent
    {
        private StudioEventEmitter _audioEmiter;

        public AudioIntensityComponent(StudioEventEmitter audioEmiter)
        {
            GameEvents.Instance.onAudioItensityController += SetAudioIntensity;
            _audioEmiter = audioEmiter;
        }



        public void SetAudioIntensity(string id, AudioIntensityController _c )
        {
            _audioEmiter.SetParameter(_c.parameter, _c.value);

            _c.PostAction?.Invoke();
        }


    }



}
