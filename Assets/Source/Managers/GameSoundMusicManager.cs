
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
        public float delayBetweenFootStepSound;
        public Dictionary<PredefinedSounds, EventReference> SoundDictionary;
        public Dictionary<PredefinedMusics, EventReference> MusicsDictionary;
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

            Callback?.Invoke();

        }

        public void PlaySoundByPredefinedKey(PredefinedSounds _key)
        {
            RuntimeManager.PlayOneShot(SoundDictionary[_key]);
        }


        IEnumerator InitDictionary()
        {
            SoundDictionary = new Dictionary<PredefinedSounds, EventReference>();

            var _sound = EventReference.Find("event:/UI/Okay2");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerTurning);

            _sound = EventReference.Find("event:/Interactables/ComputerInteract");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerInteracting);

            _sound = EventReference.Find("event:/UI/Cancel");
            EvaluateEventRef(ref _sound, PredefinedSounds.ComputerClose);

            _sound = EventReference.Find("event:/Character/Door Open");
            EvaluateEventRef(ref _sound, PredefinedSounds.OpenDoor);

            _sound = EventReference.Find("event:/Character/Door Close");
            EvaluateEventRef(ref _sound, PredefinedSounds.CloseDoor);

            _sound = EventReference.Find("event:/Character/Player Footsteps");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerFootStep);

            _sound = EventReference.Find("event:/Character/Dash2");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerDash);

            _sound = EventReference.Find("event:/Character/Jump");
            EvaluateEventRef(ref _sound, PredefinedSounds.PlayerJump);

            _sound = EventReference.Find("event:/Interactables/MaleNpc");
            EvaluateEventRef(ref _sound, PredefinedSounds.NpcMaleInteract);

            _sound = EventReference.Find("event:/Character/NewItem");
            EvaluateEventRef(ref _sound, PredefinedSounds.NewItem);

            _sound = EventReference.Find("event:/Interactables/Checkpoint");
            EvaluateEventRef(ref _sound, PredefinedSounds.Checkpoint);
            
            _sound = EventReference.Find("event:/UI/Cancel");
            EvaluateEventRef(ref _sound, PredefinedSounds.FallingFromVoid);

            MusicsDictionary = new Dictionary<PredefinedMusics, EventReference>();

            var _music = EventReference.Find("event:/Music/Introduction");
            if (!_music.IsNull) {
                MusicsDictionary.Add(PredefinedMusics.Introduction,_music);
            }

            _music = EventReference.Find("event:/Music/NIvel1");
            if (!_music.IsNull)
            {
                MusicsDictionary.Add(PredefinedMusics.Nivel1, _music);
            }

            yield return null; 
        }


        public void PlayPlayerFootStep()
        {
            if (!isLoaded) return;

             { 
                if (!SoundDictionary[PredefinedSounds.PlayerFootStep].IsNull)
                {
                    FMOD.Studio.EventInstance e = RuntimeManager.CreateInstance(SoundDictionary[PredefinedSounds.PlayerFootStep]);
                    e.set3DAttributes(RuntimeUtils.To3DAttributes((Vector3)My3DHandlerPlayer.Instance?.Character.Motor.InitialTickPosition));

                    e.start();
                    e.release();//Release each event instance immediately, there are fire and forget, one-shot instances. 
                }
            }
        }

        public void StartBackgroundMusic() {
            int level= SceneLoaderManager.Instance.SceneTarget;
            var key = SceneLoaderManager.Instance.Scenes[level].MusicBackgroundURL;
            _audioEmiter.EventReference = MusicsDictionary[key];
            _audioIntensityController.CleanAudioIntensity();
            _audioEmiter.Play();
        }

        public void StopBackgroundMusic() {
            _audioEmiter.Stop();
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
        CloseDoor,
        NpcMaleInteract,
        NewItem,
        Checkpoint,
        FallingFromVoid
    }

    [Serializable]
    public enum PredefinedMusics 
    {
        Introduction,
        Nivel1
    }

    [Serializable]
    public class GameObjectSoundConfig {
        public List<GameObject> _CAV;
        public EmitterGameEvent ForOpen = EmitterGameEvent.None;
        public EmitterGameEvent ForLeave = EmitterGameEvent.None;
    }
}
