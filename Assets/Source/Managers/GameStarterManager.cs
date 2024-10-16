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

        public event Action<bool> OnCameraChangeRequiered;

        [Space(10)]
        [Header("Light Source")]
        public GameObject FromAbove;
        public GameObject FromScreenPlayer;

        [Header("Material PostProcessing")]
        public Material FromDistance;
        public Material FromCloses;
        public Material FromWater;
        private PostProcessEffect _camComponent;

        bool IsOnWater = false;
        public override void Init()
        {
            _camComponent = Camera.main.GetComponent<PostProcessEffect>();
            OnCameraChangeRequiered += ChangeShader;
            LoaderManager.OnEverythingLoaded += AllowInteraction;
            isLoaded = true;
        }

        public void CameraDistanceChange(bool _distanceZero) {
            if (OnCameraChangeRequiered != null) {
                OnCameraChangeRequiered(_distanceZero);
            }
        }

        public void CameraFluidChange(bool isWatering) {
            if (OnCameraChangeRequiered != null)
            {
                IsOnWater = isWatering;
                OnCameraChangeRequiered(My3DHandlerPlayer.Instance.OrbitCamera.IsCameraClose());
            }
        }


        private void OnEnable()
        {
            

        }

        private void OnDisable()
        {
            
        }

        void ChangeShader(bool _IsCloseDistance) {
            
            _camComponent.postProcessEffectMaterial = IsOnWater ? FromWater
                :_IsCloseDistance ? FromCloses : FromDistance;
        }

        void AllowInteraction()
        {
            _isGameLoaded = true;
            LoaderManager.OnEverythingLoaded -= AllowInteraction;
        }
    }
}
