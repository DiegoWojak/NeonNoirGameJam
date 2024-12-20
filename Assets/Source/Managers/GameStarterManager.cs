﻿using Assets.Source.Managers.Components;
using Assets.Source.Render.Characters;
using Assets.Source.Utilities;
using Assets.Source.Utilities.Helpers;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

using UnityEngine.Playables;

using static Assets.Source.Managers.SceneLoaderManager;

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
        public bool GameBegin { get; private set; }

        public Vector3 PointToSpawmSaved { get; private set; }
        public Quaternion RotationToSpawmSaved { get; private set; }


        private Material _bufferMat;
        [SerializeField]
        private GameObject MainCamera;
        [SerializeField]
        private GameObject Player;
        [SerializeField]
        private TextWritingBehavior _textWritingBehavior;
        public override void Init()
        {
            _camComponent = MainCamera.GetComponent<PostProcessEffect>();
            OnCameraChangeRequiered += ChangeShader;
            _bufferMat = FromDistance;
            _EffectsComponent = new EffectsManagerComponent(d_Effect, Helper);

            GameEvents.Instance.onPlayerFallingOffScreen += RespawntoCheckPoint;
            GameEvents.Instance.onCheckPointEnter += SaveNewPosition;
            
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
        }

        Material GetMaterialFromViewStatus() {
            return _currentCharacterViewState == CharacterViewState.OnCameraClose ? FromClose : //FromRange
                _currentCharacterViewState == CharacterViewState.OnWater ? FromWater :
                _currentCharacterViewState == CharacterViewState.OnReading ? FromReading :
                !_change?FromDistance:FromRGBGlasses;
        }

        [ContextMenu("Let the player Move")]
        public void StartGame() {
            //Find StartPoint
            //Find EndPoint
            _EffectsComponent.ApplyAllVisualEffectsFromEquippedInventory();
            ChangeShader();
            //PointToSpawmSaved = My3DHandlerPlayer.Instance.Character.Motor.Transform.position;
            //RotationToSpawmSaved = My3DHandlerPlayer.Instance.Character.Motor.Transform.rotation;
            MainCamera.SetActive(true);
            Player.SetActive(true);

            RespawntoCheckPointForce(true);
            My3DHandlerPlayer.Instance.SetCamera();
            GameSoundMusicManager.Instance.StartBackgroundMusic();

            if (Director != null)
            {
                BlockInteraction();
                var _s = SceneLoaderManager.Instance;
                var _s2 = _s.Scenes[_s.SceneTarget];
                _textWritingBehavior.fullTextA = _s2.ChapterIntro;
                _textWritingBehavior.fullTextB = _s2.ChapterSubName;
                Debug.Log($"PlayCinematic");
                Director.Play();
            }
            else
            {
                Debug.Log($"Not Cinematic Applied, Game just running");
                GameBegin = true;
            }
            
        }


        public void PreStartGame() {
            GameObject _go = GameObject.Find("STARTPOINT");
            SaveNewPosition("", _go.transform.position, _go.transform.rotation);
        }

        private void BlockInteraction() {
            UIManager.Instance.RequestOpenUI(this, false);
        }

        public void AllowInteraction() {
            UIManager.Instance.RequestCloseUI(this);
            GameBegin = true;
        }


        void RespawntoCheckPoint() {
            My3DHandlerPlayer.Instance.Character.Motor.SetPositionAndRotation(PointToSpawmSaved,RotationToSpawmSaved);
        }

        void RespawntoCheckPointForce(bool force = false)
        {
            My3DHandlerPlayer.Instance.Character.Motor.SetPositionAndRotation(PointToSpawmSaved, RotationToSpawmSaved, force);
        }

        void SaveNewPosition(string id, Vector3 position, Quaternion rotation)
        {
            PointToSpawmSaved = position;
            RotationToSpawmSaved = rotation;
        }

        bool _change = false;
        public void ChangedefaultMaterial(bool change) {
            _change = change;
            ChangeShader();
        }

        public void CompletetLevel() {
            UIManager.Instance.RequestOpenUI(this, true, (resu) => {
                var _status = UIManager.Instance.ShowStatus();

                SceneInformation _scene = SceneLoaderManager.Instance.Scenes[SceneLoaderManager.Instance.SceneTarget];

                _status.RequestWriteMessage(_scene.FinalMessageOnGameEnd,_scene.SomeRanking ,() => {
                    StartCoroutine(DelayTime(2, () => {
                        MainCamera.SetActive(false);
                        Player.SetActive(false);
                        UIManager.Instance.RequestCloseUI(this, (bo) => {
                            UIManager.Instance.CloseStatus();
                        });
                        GameBegin = false;
                        GameSoundMusicManager.Instance.StopBackgroundMusic();
                        SceneLoaderManager.Instance.LoadNextLevel();
                    }));
                });
            });
        }

        IEnumerator DelayTime(float time ,Action _do) {
            yield return time;
            _do.Invoke();
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
