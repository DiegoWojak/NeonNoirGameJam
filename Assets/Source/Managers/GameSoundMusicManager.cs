
using Assets.Source.Utilities.Helpers;
using Assets.Source.Utilities.Helpers.Gizmo;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Assets.Source.Managers
{
    public class GameSoundMusicManager : LoaderBase<GameSoundMusicManager>
    {
        public Dictionary<PredefinedSounds, EventReference> SoundDictionary;

        public List<GameObject> computers = new List<GameObject>();

        public override void Init()
        {

            StartCoroutine(
                SoundInitialization(
                    () => {
                        isLoaded = true;
                    }
                )
             );
        }
        IEnumerator SoundInitialization(Action Callback)
        {
            yield return InitDictionary();
            yield return InitComputers();
            Callback?.Invoke();
        }

        public void PlayComputerInteracting(PredefinedSounds _key)
        {
            RuntimeManager.PlayOneShot(SoundDictionary[_key]);
        }


        IEnumerator InitComputers()
        {
            if (SoundDictionary.TryGetValue(PredefinedSounds.ComputerTurning, out EventReference _sound))
            {
                for (int i = 0; i < computers.Count; i++)
                {
                    if (computers[i].GetComponent<StudioEventEmitter>() == null)
                    {
                        GameObject _b = computers[i];
                        yield return FmodAutomaticHelper.CreateSoundEmitirForComputer(ref _b, _sound);
                    }
                }
            }
        }

        IEnumerator InitDictionary()
        {
            SoundDictionary = new Dictionary<PredefinedSounds, EventReference>();

            var _sound = EventReference.Find("event:/VO/Welcome");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerTurning);

            _sound = EventReference.Find("event:/UI/Okay");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerInteracting);


            _sound = EventReference.Find("event:/UI/Cancel");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerClose);

            yield return null;
        }


        private void EvaluateEventRef(ref EventReference _sound, PredefinedSounds _key)
        {
            if (_sound.IsNull)
            {
                string _msg = DebugUtils.GetMessageFormat($"Sound not Found for {_sound.Path} ", 0);
                Debug.Log(_msg);
            }
            else
            {
                string _msg = DebugUtils.GetMessageFormat($"Found Sounds for {_sound.Path} ", 2);
                SoundDictionary.Add(_key, _sound);
                Debug.Log(_msg);
            }
        }
    }

    public enum PredefinedSounds
    {
        ComputerTurning,
        ComputerInteracting,
        ComputerClose
    }
}
