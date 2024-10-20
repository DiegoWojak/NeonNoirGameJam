
using Assets.Source.Managers;
using System.Linq;

using UnityEngine;

namespace Assets.Source.Render.Characters
{
    //MyPlayer
    public  class My3DHandlerPlayer : LoaderBase<My3DHandlerPlayer>
    {
        public Character3DCamera OrbitCamera;
        public Transform CameraFollowPoint;
        public Character3DCore Character;

        private Vector3 _lookInputVector = Vector3.zero;

        private const string MouseXInput = "Mouse X";
        private const string MouseYInput = "Mouse Y";
        private const string MouseScrollInput = "Mouse ScrollWheel";
        private const string HorizontalInput = "Horizontal";
        private const string VerticalInput = "Vertical";

        private bool isGameLoaded = false;

        public override void Init()
        {
            isLoaded = true;
            LoaderManager.OnEverythingLoaded += AllowInteraction;
        }

        void AllowInteraction()
        {
            Debug.Log("Loaded");
            isGameLoaded = true;
            LoaderManager.OnEverythingLoaded -= AllowInteraction;
        }

        private void Start()
        {
            //
            Cursor.lockState = CursorLockMode.Locked;

            OrbitCamera.SetFollowTranform(CameraFollowPoint);

            OrbitCamera.IgnoredColliders = Character.GetComponentsInChildren<Collider>().ToList();
        }

        private void Update()
        {
            if (!isGameLoaded) return;

            if (Input.GetMouseButtonDown(0) && !Character.DisableInputsFromPlayer && UIManager.Instance.IsAnyUIOpened) 
            {
                Cursor.lockState = CursorLockMode.Locked;
            }

            HandleCharacterInput();
        }

        private void LateUpdate()
        {
            HandleCameraInput();
            Character.PostInputUpdate(Time.deltaTime, OrbitCamera.transform.forward, ref OrbitCamera.Camera);
        }

        private void HandleCameraInput() {
            #region Look Input Vector
            float mouseLookAxisUp = Input.GetAxisRaw(MouseYInput);
            float mouseLookAxisRight = Input.GetAxisRaw(MouseXInput);
            _lookInputVector = new Vector3(mouseLookAxisRight, mouseLookAxisUp, 0f);
            
            if (Cursor.lockState != CursorLockMode.Locked) // Prevent moving the camera while the cursor isn't locked
            {
                _lookInputVector = Vector3.zero;
            }
            float scrollInput = -Input.GetAxis(MouseScrollInput);
            #if UNITY_WEBGL
            scrollInput = 0f;
            #endif

            OrbitCamera.UpdateWithInput(Time.deltaTime, scrollInput, _lookInputVector); // Apply ingputs

            if (Input.GetMouseButtonDown(1)) 
            {
                OrbitCamera.TargetDistance = (OrbitCamera.TargetDistance == 0f)? OrbitCamera.DefaultDistance : 0f;
                //
                GameStarterManager.Instance?.CameraDistanceChange((OrbitCamera.TargetDistance == 0f));
            }
            #endregion
        }

        private void HandleCharacterInput()
        {
            PlayerCharacterInputs characterInputs = new PlayerCharacterInputs();
            if (!Character.DisableInputsFromPlayer) { 
                characterInputs.MoveAxisForward = Input.GetAxisRaw(VerticalInput);
                characterInputs.MoveAxisRight = Input.GetAxisRaw(HorizontalInput);
                characterInputs.CameraRotation = OrbitCamera.Transform.rotation;
                characterInputs.JumpDown = Input.GetKeyDown(KeyCode.Space);
                characterInputs.JumpHeld = Input.GetKey(KeyCode.Space); 
                characterInputs.CrouchDown = Input.GetKeyDown(KeyCode.LeftControl);
                characterInputs.CrouchUp = Input.GetKeyUp(KeyCode.LeftControl);
                characterInputs.CrouchHeld = Input.GetKey(KeyCode.LeftControl);
                characterInputs.NoClipDown = Input.GetKeyUp(KeyCode.O);
                //characterInputs.ClimbLadder = Input.GetKeyUp(KeyCode.E);
                characterInputs.Interaction = Input.GetKeyUp(KeyCode.E);
                characterInputs.ShootHeld = Input.GetButton("Fire1");
            }

            if(Input.GetKeyDown(KeyCode.I))
            {
                if (!InventorySystem.Instance.IsInventaryOpen)
                {
                    InventorySystem.Instance.OpenInventory();   
                }
                else {
                    InventorySystem.Instance.CloseInventory();
                }
            }

            if (Character.AllowDash && Input.GetKeyDown(KeyCode.LeftShift))
            {
                characterInputs.Dash = Input.GetKeyDown(KeyCode.LeftShift);
                Character.Motor.ForceUnground(0.1f);
                Character.AddVelocity(Vector3.one * 5f);
            }

            Character.SetInputs(ref characterInputs);
        }



    }
}
