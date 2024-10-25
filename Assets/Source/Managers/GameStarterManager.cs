using Assets.Source.Managers.Components;
using Assets.Source.Render.Characters;
using Assets.Source.Utilities;
using Assets.Source.Utilities.Helpers;
using System;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;

namespace Assets.Source.Managers
{
    public class GameStarterManager : LoaderBase<GameStarterManager>
    {

        public event Action OnCameraChangeRequiered;

        [Space(10)]
        [Header("Light Source")]
        public GameObject FromAbove;
        public GameObject FromScreenPlayer;

        [Header("Material PostProcessing")]
        public Material FromDistance;
        public Material FromClose;
        public Material FromReading;
        public Material FromWater;
        public Material FromRGBGlasses;

        public CharacterViewState _currentCharacterViewState { private set; get; }

        private PostProcessEffect _camComponent;

        bool bufferisWatering = false;
        bool bufferIsReading = false;

        [Space(10)]
        [Header("Effect Table")]
        [SerializeField]
        private System.Collections.Generic.List<EffectDictionary> d_Effect;

        [SerializeField]
        private ApplyEffectFunctionsHelper Helper;
        public EffectsManagerComponent _EffectsComponent { private set; get; }

        public PlayableDirector Director;

        [SerializeField]
        public bool Begin { get; private set; }


        public Vector3 PointToSpawmSaved { get; private set; }
        public Quaternion RotationToSpawmSaved { get; private set; }

        public override void Init()
        {
            _camComponent = Camera.main.GetComponent<PostProcessEffect>();
            OnCameraChangeRequiered += ChangeShader;
            LoaderManager.OnEverythingLoaded += AllowInteraction;

            _EffectsComponent = new EffectsManagerComponent(d_Effect, Helper);
            _EffectsComponent.ApplyAllVisualEffectsFromEquippedInventory();

            PointToSpawmSaved = My3DHandlerPlayer.Instance.Character.Motor.Transform.position;
            RotationToSpawmSaved = My3DHandlerPlayer.Instance.Character.Motor.Transform.rotation;
            GameEvents.Instance.onPlayerFallingOffScreen += RespawntoCheckPoint;

            ChangeShader();
            isLoaded = true;
        }

        public void CameraDistanceChange(bool _distanceZero) {
            if (OnCameraChangeRequiered != null) {
                if (!bufferisWatering && !bufferIsReading)
                {
                    _currentCharacterViewState = _distanceZero ? CharacterViewState.OnCameraClose : CharacterViewState.Default;
                }

                OnCameraChangeRequiered();
            }
        }

        public void CameraFluidChange(bool isWatering) {
            if (OnCameraChangeRequiered != null)
            {
                bufferisWatering = isWatering;
                _currentCharacterViewState = isWatering ? CharacterViewState.OnWater : CharacterViewState.Default;
                OnCameraChangeRequiered();
            }
        }

        public void CameraReadingChange(bool isReading) {
            if (OnCameraChangeRequiered != null)
            {
                bufferIsReading = isReading;
                if (isReading)
                {
                    _currentCharacterViewState = CharacterViewState.OnReading;
                }
                else {
                    if (!bufferisWatering)
                    {
                        _currentCharacterViewState = My3DHandlerPlayer.Instance.OrbitCamera.IsCameraClose() ? CharacterViewState.OnCameraClose : CharacterViewState.Default;
                    }
                    else {
                        _currentCharacterViewState = CharacterViewState.OnWater;
                    }
                }
                OnCameraChangeRequiered();
            }
        }


        private void OnEnable()
        {

        }

        private void OnDisable()
        {

        }

        void ChangeShader() {
            _camComponent.postProcessEffectMaterial = GetMaterialFromViewStatus();
            _camComponent.postIntermedialMaterial = FromRGBGlasses;
        }

        void AllowInteraction()
        {
            if (Director != null)
            {
                Debug.Log($"PlayCinematic");
                Director.Play();
            }
            else {
                Debug.Log($"Not Cinematic Applied, Game just running");
                StartGame();
            }

            LoaderManager.OnEverythingLoaded -= AllowInteraction;
        }

        Material GetMaterialFromViewStatus() {
            return _currentCharacterViewState == CharacterViewState.OnCameraClose ? FromClose : //FromRange
                _currentCharacterViewState == CharacterViewState.OnWater ? FromWater :
                _currentCharacterViewState == CharacterViewState.OnReading ? FromReading :
                FromDistance;
        }

        [ContextMenu("Let the player Move")]
        public void StartGame() {
            Begin = true;
        }

        void RespawntoCheckPoint() {
            My3DHandlerPlayer.Instance.Character.Motor.SetPositionAndRotation(PointToSpawmSaved,RotationToSpawmSaved);
        }
    }
    public enum CharacterViewState
    {
        Default, //FromRange
        OnWater,
        OnReading,
        OnCameraClose
    }
}
