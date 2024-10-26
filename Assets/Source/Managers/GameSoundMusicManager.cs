
using Assets.Source.Render.Characters;
using Assets.Source.Utilities.Events;
using Assets.Source.Utilities.Helpers;
using Assets.Source.Utilities.Helpers.Gizmo;
using FMODUnity;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace Assets.Source.Managers
{
    [Serializable]
    public class GameSoundMusicManager : LoaderBase<GameSoundMusicManager>
    {
        public Dictionary<PredefinedSounds, EventReference> SoundDictionary;

        /*public List<GameObject> computers = new List<GameObject>();
        [SerializeField]
        public GameObjectSoundConfig Doors = new GameObjectSoundConfig();*/

        public AudioIntensityComponent _audioIntensityController { get; private set; }
        [SerializeField]
        private StudioEventEmitter _audioEmiter;
        public override void Init()
        {
            if (_audioEmiter == null)
            {
                var go = GameObject.Find("BackgroundMusic");
                if (go != null) _audioEmiter = go.GetComponent<StudioEventEmitter>();
            }
            _audioIntensityController = new AudioIntensityComponent(_audioEmiter);
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
            /*yield return InitComputers();
            yield return InitDoors();*/

            Callback?.Invoke();

        }

        public void PlaySoundByPredefinedKey(PredefinedSounds _key)
        {
            RuntimeManager.PlayOneShot(SoundDictionary[_key]);
        }


        /*IEnumerator InitComputers()
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

        IEnumerator InitDoors()
        {
            for (int i = 0; i < Doors._CAV.Count; i++)
            {
                if (Doors._CAV[i].GetComponent<StudioEventEmitter>() == null)
                {
                    GameObject _b = Doors._CAV[i];
                    yield return FmodAutomaticHelper.CreateSoundEmitirForDoor(ref _b,
                        SoundDictionary[PredefinedSounds.OpenDoor], SoundDictionary[PredefinedSounds.CloseDoor]
                        , Doors.ForOpen, Doors.ForLeave);
                }
            }
        }*/

        IEnumerator InitDictionary()
        {
            SoundDictionary = new Dictionary<PredefinedSounds, EventReference>();

            var _sound = EventReference.Find("event:/VO/Welcome");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerTurning);

            _sound = EventReference.Find("event:/UI/Okay");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerInteracting);

            _sound = EventReference.Find("event:/UI/Cancel");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerClose);

            _sound = EventReference.Find("event:/Character/Door Open");
            EvaluateEventRef(ref _sound, PredefinedSounds.OpenDoor);

            _sound = EventReference.Find("event:/Character/Door Close");
            EvaluateEventRef(ref _sound, PredefinedSounds.CloseDoor);

            _sound = EventReference.Find("event:/Character/Player Footsteps");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerFootStep);

            _sound = EventReference.Find("event:/Character/Dash");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerDash);

            _sound = EventReference.Find("event:/Character/Jump");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerJump);

            yield return null;
        }

        public void PlayPlayerFootStep()
        {
            if (!isLoaded) return;
            if (!SoundDictionary[PredefinedSounds.PlayerFootStep].IsNull)
            {
                FMOD.Studio.EventInstance e = RuntimeManager.CreateInstance(SoundDictionary[PredefinedSounds.PlayerFootStep]);
                e.set3DAttributes(RuntimeUtils.To3DAttributes((Vector3)My3DHandlerPlayer.Instance?.Character.Motor.InitialTickPosition));

                e.start();
                e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
            }
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
        PlayerFootStep,
        PlayerJump,
        PlayerDash,
        ComputerTurning,
        ComputerInteracting,
        ComputerClose,
        OpenDoor,
        CloseDoor
    }

    [Serializable]
    public class GameObjectSoundConfig {
        public List<GameObject> _CAV;
        public EmitterGameEvent ForOpen = EmitterGameEvent.None;
        public EmitterGameEvent ForLeave = EmitterGameEvent.None;
    }
}
