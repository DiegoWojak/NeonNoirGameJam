using Assets.Source.Render.Characters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Managers
{
    public class GameStarterManager : LoaderBase<GameStarterManager>
    {
        private bool _isGameLoaded = false;

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

        public CharacterViewState _currentCharacterViewState { private set; get; }

        private PostProcessEffect _camComponent;

        bool bufferisWatering = false;
        bool bufferIsReading = false;
        public override void Init()
        {
            _camComponent = Camera.main.GetComponent<PostProcessEffect>();
            OnCameraChangeRequiered += ChangeShader;
            LoaderManager.OnEverythingLoaded += AllowInteraction;
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
                _currentCharacterViewState = isWatering?CharacterViewState.OnWater: CharacterViewState.Default;
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

        void Priority() {
        
        }

        void ChangeShader() {
            _camComponent.postProcessEffectMaterial = GetMaterialFromViewStatus();
        }

        void AllowInteraction()
        {
            _isGameLoaded = true;
            LoaderManager.OnEverythingLoaded -= AllowInteraction;
        }

        Material GetMaterialFromViewStatus() {
            return _currentCharacterViewState==CharacterViewState.OnCameraClose? FromClose: //FromRange
                _currentCharacterViewState == CharacterViewState.OnWater ? FromWater:
                _currentCharacterViewState == CharacterViewState.OnReading ? FromReading :
                FromDistance;
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
