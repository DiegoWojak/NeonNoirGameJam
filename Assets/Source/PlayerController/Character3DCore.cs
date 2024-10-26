
using Assets.Source.Managers;
using Assets.Source.Utilities;
using KinematicCharacterController;

using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Windows;


#if UNITY_EDITOR
using static UnityEditor.IMGUI.Controls.CapsuleBoundsHandle;
#endif

namespace Assets.Source.Render.Characters
{
    public enum CharacterState
    {
        Default,
        Swimming,
        Climbing,
        NoClip // No affected by Physics like wall of gravity
    }

    public enum ClimbingState
    {
        Anchoring,
        Climbing,
        DeAnchoring
    }

    public struct PlayerCharacterInputs
    {
        public float MoveAxisForward;
        public float MoveAxisRight;
        public Quaternion CameraRotation;
        public bool JumpDown;
        public bool JumpHeld;
        public bool CrouchDown;
        public bool CrouchUp;
        public bool CrouchHeld;
        public bool NoClipDown;
        public bool ClimbLadder;
        public bool ShootHeld;
        public bool Interaction;
        public bool Dash;
        public bool RunDownHeld;
    }

    public class Character3DCore : MonoBehaviour, ICharacterController
    {
        public KinematicCharacterMotor Motor;

        [Header("Stable Movement")]
        public float MaxStableMoveSpeed = 10f;
        public float StableMovementSharpness = 15;
        public float OrientationSharpness = 10;
        /**/
        public float MaxStableDistanceFromLedge = 5f;
        [Range(0f, 180f)]
        public float MaxStableDenivelationAngle = 180f;

        [Header("Air Movement")]
        public float MaxAirMoveSpeed = 10f;
        public float AirAccelerationSpeed = 5f;
        public float Drag = 0.1f;

        [Header("Jumping")]
        public bool AllowJumpingWhenSliding = false;
        public bool AllowDoubleJump 
        {
            get
            {
                if (!GameStarterManager.Instance.IsLoaded()) return false;
                return GameStarterManager.Instance._EffectsComponent.CanDoubleJump();
            }
        }
        public bool AllowWallJump
        {
            get
            {
                if (!GameStarterManager.Instance.IsLoaded()) return false;
                return GameStarterManager.Instance._EffectsComponent.CanWallJump();
            }
        }

        public float JumpSpeed = 10f;
        public float JumpPreGroundingGraceTime = 0f;
        public float JumpPostGroundingGraceTime = 0f;

        [Header("Ladder Climbing")]
        public float ClimbingSpeed = 4f;
        public float AnchoringDuration = 0.25f;
        public LayerMask ClimbingLayer;

        [Header("Swimming")]
        public Transform SwimmingReferencePoint;
        public LayerMask WaterLayer;
        public float SwimmingSpeed = 4f;
        public float SwimmingMovementSharpness = 3;
        public float SwimmingOrientationSharpness = 2f;

        [Header("Animation Parameters")]
        public Animator CharacterAnimator;
        public float ForwardAxisSharpness = 10;
        public float TurnAxisSharpness = 5;

        [Header("NoClip CHEATS")]
        public float NoClipMoveSpeed = 10f;
        public float NoClipSharpness = 15;

        [Header("Misc")]
        public Vector3 Gravity = new Vector3(0, -30f, 0);
        public bool OrientTowardsGravity = true;
        public bool FramePerfectRotation = true;
        public Transform MeshRoot;
        public List<Collider> IgnoredColliders = new List<Collider>();
        public CharacterState CurrentCharacterState { get; private set; }
        [Space(10)]
        [Header("Extras")]
        public Vector3 AdditionalDirectionForceFromJumpWall;
        
        public bool AllowDash
        {
            get
            {
                if (!GameStarterManager.Instance.IsLoaded()) return false;
                return GameStarterManager.Instance._EffectsComponent.CanDash();
            }
        }

        [Space(10)]
        [Header("Dash attributes")]
        public int maxChargeDash = 3;
        public float periodOfDashRecovery = 5f;
        [SerializeField]
        private DashFrameBehaviour _dashFrameBehavior;

        [Header("Visor")]
        public GameObject Visor;

        [Header("SoundSteps")]
        public float m_StepDistance = 2.0f;

        private Collider[] _probedColliders = new Collider[8];
        private Vector3 _moveInputVector;
        private Vector3 _lookInputVector;
        private bool _jumpInputIsHeld = false;
        private bool _crouchInputIsHeld = false;
        private bool _jumpRequested = false;
        private bool _jumpConsumed = false;
        private bool _jumpedThisFrame = false;
        private float _timeSinceJumpRequested = Mathf.Infinity;
        private float _timeSinceLastAbleToJump = 0f;
        private bool _doubleJumpConsumed = false;
        private bool _canWallJump = false;
        private Vector3 _wallJumpNormal;
        private Vector3 _internalVelocityAdd = Vector3.zero;
        private bool _shouldBeCrouching = false;
        private bool _isCrouching = false;
        private Collider _waterZone;

        // Ladder vars
        private float _ladderUpDownInput;
        private My3DClimbingLadder _activeLadder { get; set; }
        private ClimbingState _internalClimbingState;
        private ClimbingState _climbingState
        {
            get
            {
                return _internalClimbingState;
            }
            set
            {
                _internalClimbingState = value;
                _anchoringTimer = 0f;
                _anchoringStartPosition = Motor.TransientPosition;
                _anchoringStartRotation = Motor.TransientRotation;
            }
        }
        private Vector3 _ladderTargetPosition;
        private Quaternion _ladderTargetRotation;
        private float _onLadderSegmentState = 0;
        private float _anchoringTimer = 0f;
        private Vector3 _anchoringStartPosition = Vector3.zero;
        private Quaternion _anchoringStartRotation = Quaternion.identity;
        private Quaternion _rotationBeforeClimbing = Quaternion.identity;

        private bool _shootingHeld = false;
        [SerializeField]
        private Transform FromPoint;
        RaycastHit[] hitsAlloc;
        const float MAX_HITDISTANCE = 100f;

        #region AnimationVariables
        private float _forwardAxis;
        private float _rightAxis;
        private float _targetForwardAxis;
        private float _targetRightAxis;
        private Vector3 _rootMotionPositionDelta;
        private Quaternion _rootMotionRotationDelta;
        #endregion

        public static Action<LayerMask> OnCollisionDetected;
        public LayerMask DeviceNpcLayer;
        public LayerMask HumanoideNpcLayer;
        public bool DisableInputsFromPlayer { get { return UIManager.Instance.IsAnyUIOpened; } }

        private float m_DistanceTraveled = 0f;
        private float m_StepRand;
        private Vector3 m_PrevPos;

        private bool b_dashing =false;
        private int currentChargeDash; // Current available dashes
        private float recoveryTimer; // Tracks time passed to restore charges
        private bool isRecovering = false; // Tracks if recovery is in process
        [SerializeField]
        public struct DashFrameBehaviour {
            [HideInInspector]
            public float lastDashTime;
            [HideInInspector]
            public float dashStarttime;
            [SerializeField]
            public float framerateCooldown;

            public void Clear() {
                lastDashTime = 0f;
                dashStarttime = 0f;
                framerateCooldown = 0.3f;
            }
        }
        bool _isWalking = false;

        void Start() {
            Motor.CharacterController = this;
            TransitionToState(CharacterState.Default);

            _rootMotionPositionDelta = Vector3.zero;
            _rootMotionRotationDelta = Quaternion.identity;

            m_StepRand = UnityEngine.Random.Range(0.0f, 0.5f);
            m_PrevPos = Motor.InitialTickPosition;

            _dashFrameBehavior = new DashFrameBehaviour();
            _dashFrameBehavior.Clear();

            currentChargeDash = maxChargeDash; // Start with full dash charges
            recoveryTimer = 0f;
        }

        void Update() {
            
            _forwardAxis = Mathf.Lerp(_forwardAxis, _targetForwardAxis, 1f - Mathf.Exp(-ForwardAxisSharpness * Time.deltaTime));
            _rightAxis = Mathf.Lerp(_rightAxis, _targetRightAxis, 1f - Mathf.Exp(-TurnAxisSharpness * Time.deltaTime));
            _forwardAxis *= (b_dashing ? 2 : 1);
            CharacterAnimator.SetFloat("Forward", _forwardAxis);
            CharacterAnimator.SetFloat("Turn", _rightAxis);
            CharacterAnimator.SetBool("OnGround", Motor.GroundingStatus.IsStableOnGround && CurrentCharacterState == CharacterState.Default);
            CharacterAnimator.SetBool("OnLiquid", CurrentCharacterState==CharacterState.Swimming);
            CharacterAnimator.SetBool("OnDash", b_dashing);
            
            
            if (b_dashing) {
                GameSoundMusicManager.Instance.PlaySoundByPredefinedKey(PredefinedSounds.PlayerDash);
                currentChargeDash--;
                CronometricConsumeDashFrameRate();
            }

            m_DistanceTraveled += (Motor.InitialTickPosition - m_PrevPos).magnitude;
            if (Motor.GroundingStatus.IsStableOnGround && m_DistanceTraveled >= m_StepDistance + m_StepRand)
            {
                GameSoundMusicManager.Instance?.PlayPlayerFootStep();
                m_StepRand = UnityEngine.Random.Range(0.0f, 0.5f);
                m_DistanceTraveled = 0.0f;
            }
            m_PrevPos = Motor.InitialTickPosition;

            if (currentChargeDash < maxChargeDash)
            {
                StartDashRecovery();
            }
        }

        public void TransitionToState(CharacterState newState)
        {            
            CharacterState tmpInitialState = CurrentCharacterState;
            OnStateExit(tmpInitialState, newState);
            CurrentCharacterState = newState;
            OnStateEnter(newState, tmpInitialState);
        }

        public void SetInputs(ref PlayerCharacterInputs inputs)
        {
            if (inputs.NoClipDown)
            {
                if (CurrentCharacterState == CharacterState.Default)
                {
                    TransitionToState(CharacterState.NoClip);
                }
                else if (CurrentCharacterState == CharacterState.NoClip)
                {
                    TransitionToState(CharacterState.Default);
                }
            }

            // Handle ladder transitions
            _ladderUpDownInput = inputs.MoveAxisForward;
            if (inputs.Interaction)
            {
                if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, ClimbingLayer, QueryTriggerInteraction.Collide) > 0)
                {
                    if (_probedColliders[0] != null)
                    {
                        // Handle ladders
                        My3DClimbingLadder ladder = _probedColliders[0].gameObject.GetComponent<My3DClimbingLadder>();
                        if (ladder)
                        {
                            // Transition to ladder climbing state
                            if (CurrentCharacterState == CharacterState.Default)
                            {
                                _activeLadder = ladder;
                                TransitionToState(CharacterState.Climbing);
                            }
                            // Transition back to default movement state
                            else if (CurrentCharacterState == CharacterState.Climbing)
                            {
                                _climbingState = ClimbingState.DeAnchoring;
                                _ladderTargetPosition = Motor.TransientPosition;
                                _ladderTargetRotation = _rotationBeforeClimbing;
                            }
                        }
                    }
                }
                //Interact with a computer
                if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, DeviceNpcLayer, QueryTriggerInteraction.Collide) > 0) {
                    GameEvents.Instance?.RequestInteractInteractable();
                } else if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, HumanoideNpcLayer, QueryTriggerInteraction.Collide) > 0) {
                    GameEvents.Instance?.RequestInteractInteractable();
                }
            }
            else {
                _jumpInputIsHeld = inputs.JumpHeld;
                _crouchInputIsHeld = inputs.CrouchHeld;
            }
            //

            Vector3 moveInputVector = Vector3.ClampMagnitude(new Vector3(inputs.MoveAxisRight, 0f, inputs.MoveAxisForward), 1f);

            Vector3 cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.forward, Motor.CharacterUp).normalized;
            if (cameraPlanarDirection.sqrMagnitude == 0f)
            {
                cameraPlanarDirection = Vector3.ProjectOnPlane(inputs.CameraRotation * Vector3.up, Motor.CharacterUp).normalized;
            }
            Quaternion cameraPlanarRotation = Quaternion.LookRotation(cameraPlanarDirection, Motor.CharacterUp);

            _shootingHeld = inputs.ShootHeld;

            _isWalking = inputs.RunDownHeld;
            if (_isWalking) 
            {
                inputs.MoveAxisForward /= 2.5f;
                inputs.MoveAxisRight /= 2.5f;
            }

            switch(CurrentCharacterState){
                case CharacterState.Default:
                    AxisInputs(inputs.MoveAxisForward, inputs.MoveAxisRight);

                    _moveInputVector = cameraPlanarRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;
                    InputJumpDown(inputs.JumpDown);
                    InputCrouchDown(inputs.CrouchDown, inputs.CrouchUp);
                    //Only dash animation when no swimming or climbing
                    b_dashing = inputs.Dash;
                    break;
                case CharacterState.Swimming:
                    _jumpRequested = inputs.JumpHeld;
                    _moveInputVector = inputs.CameraRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;
                    break;
                case CharacterState.NoClip:
                    _moveInputVector = inputs.CameraRotation * moveInputVector;
                    _lookInputVector = cameraPlanarDirection;
                    break;
            }

            void AxisInputs(float axisForward, float axisRight) {
                _targetForwardAxis = axisForward;
                _targetRightAxis = axisRight;
            }

            void InputJumpDown(bool inputPress) {
                if (inputPress)
                {
                    _timeSinceJumpRequested = 0f;
                    _jumpRequested = true;
                }
            }
            void InputCrouchDown(bool inputCrouchDown, bool inputCrouchUp) {
                if (inputCrouchDown)
                {
                    _shouldBeCrouching = true;

                    if (!_isCrouching)
                    {
                        _isCrouching = true;
                        Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                        MeshRoot.localScale = new Vector3(1f, 0.5f, 1f);
                    }
                }
                else if (inputCrouchUp)
                {
                    _shouldBeCrouching = false;
                }
            }
        }

        public void PostInputUpdate(float deltaTime, Vector3 cameraForward,ref Camera cam)
        {
            if (FramePerfectRotation)
            {
                _lookInputVector = Vector3.ProjectOnPlane(cameraForward, Motor.CharacterUp);

                Quaternion newRotation = default;
                HandleRotation(ref newRotation, deltaTime);
                MeshRoot.rotation = newRotation;
            }

            if (_shootingHeld)
            {
                PerformHitScan(deltaTime, Vector3.ProjectOnPlane(cameraForward, Motor.CharacterUp));
            }
        }

        private void HandleRotation(ref Quaternion rot, float deltaTime) {
            if (_lookInputVector != Vector3.zero)
            {
                rot = Quaternion.LookRotation(_lookInputVector, Motor.CharacterUp);
            }
        }

        public void OnStateEnter(CharacterState state, CharacterState fromState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        Motor.SetGroundSolvingActivation(true);
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        _rotationBeforeClimbing = Motor.TransientRotation;

                        Motor.SetMovementCollisionsSolvingActivation(false);
                        Motor.SetGroundSolvingActivation(false);
                        _climbingState = ClimbingState.Anchoring;

                        // Store the target position and rotation to snap to
                        _ladderTargetPosition = _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition, out _onLadderSegmentState);
                        _ladderTargetRotation = _activeLadder.transform.rotation;
                        break;
                    }
                case CharacterState.Swimming:
                    {
                        Motor.SetGroundSolvingActivation(false);
                        GameStarterManager.Instance?.CameraFluidChange(true);
                        break;
                    }
                case CharacterState.NoClip:
                    {
                        Motor.SetCapsuleCollisionsActivation(false);
                        Motor.SetMovementCollisionsSolvingActivation(false);
                        Motor.SetGroundSolvingActivation(false);
                        break;
                    }
            }
        }

        public void OnStateExit(CharacterState state, CharacterState toState)
        {
            switch (state)
            {
                case CharacterState.Default:
                    {
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        Motor.SetMovementCollisionsSolvingActivation(true);
                        Motor.SetGroundSolvingActivation(true);
                        break;
                    }
                case CharacterState.Swimming:
                    {
                        GameStarterManager.Instance?.CameraFluidChange(false);
                        break;
                    }
                case CharacterState.NoClip:
                    {
                        Motor.SetCapsuleCollisionsActivation(true);
                        Motor.SetMovementCollisionsSolvingActivation(true);
                        Motor.SetGroundSolvingActivation(true);
                        break;
                    }
            }
        }

        public void AfterCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (_jumpRequested && _timeSinceJumpRequested > JumpPreGroundingGraceTime)
                        {
                            _jumpRequested = false;
                        }

                        if (AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround)
                        {

                            if (!_jumpedThisFrame)
                            {
                                _doubleJumpConsumed = false;
                                _jumpConsumed = false;
                            }
                            _timeSinceLastAbleToJump = 0f;
                        }
                        else
                        {
                            _timeSinceLastAbleToJump += deltaTime;
                        }

                        if (_isCrouching && !_shouldBeCrouching)
                        {
                            Motor.SetCapsuleDimensions(0.5f, 2f, 1f);
                            if (Motor.CharacterOverlap(
                                    Motor.TransientPosition,
                                    Motor.TransientRotation,
                                    _probedColliders,
                                    Motor.CollidableLayers,
                                    QueryTriggerInteraction.Ignore) > 0)
                            {
                                Motor.SetCapsuleDimensions(0.5f, 1f, 0.5f);
                            }
                            else
                            {
                                MeshRoot.localScale = new Vector3(1f, 1f, 1f);
                                _isCrouching = false;
                            }
                        }

                        _rootMotionPositionDelta = Vector3.zero;
                        _rootMotionRotationDelta = Quaternion.identity;
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                // Detect getting off ladder during climbing
                                _activeLadder.ClosestPointOnLadderSegment(Motor.TransientPosition, out _onLadderSegmentState);
                                if (Mathf.Abs(_onLadderSegmentState) > 0.05f)
                                {
                                    _climbingState = ClimbingState.DeAnchoring;

                                    // If we're higher than the ladder top point
                                    if (_onLadderSegmentState > 0)
                                    {
                                        _ladderTargetPosition = _activeLadder.TopReleasePoint.position;
                                        _ladderTargetRotation = _activeLadder.TopReleasePoint.rotation;
                                    }
                                    // If we're lower than the ladder bottom point
                                    else if (_onLadderSegmentState < 0)
                                    {
                                        _ladderTargetPosition = _activeLadder.BottomReleasePoint.position;
                                        _ladderTargetRotation = _activeLadder.BottomReleasePoint.rotation;
                                    }
                                }
                                break;
                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                // Detect transitioning out from anchoring states
                                if (_anchoringTimer >= AnchoringDuration)
                                {
                                    if (_climbingState == ClimbingState.Anchoring)
                                    {
                                        _climbingState = ClimbingState.Climbing;
                                    }
                                    else if (_climbingState == ClimbingState.DeAnchoring)
                                    {
                                        TransitionToState(CharacterState.Default);
                                    }
                                }

                                // Keep track of time since we started anchoring
                                _anchoringTimer += deltaTime;
                                break;
                        }
                        break;
                    }
            }
        }

        private void PerformHitScan(float deltaTime,Vector3 cameraForward) {
            Vector3 screenCenter = new Vector3(Screen.width / 2, Screen.height / 2, 0);
            
            Ray ray = Camera.main.ScreenPointToRay(screenCenter);

            hitsAlloc = Physics.RaycastAll(FromPoint.position, ray.direction, MAX_HITDISTANCE);

            Physics.RaycastNonAlloc(ray, hitsAlloc, MAX_HITDISTANCE, 0, QueryTriggerInteraction.Ignore );

            for (int i = 0; i < hitsAlloc.Length; i++)
            {
                if (hitsAlloc[i].collider.gameObject != gameObject) 
                {
                    Vector3 hitPoint = hitsAlloc[i].point; // Aquí está tu "Wall(x, y, z)"
                    DrawHitEffect(FromPoint.position, hitPoint, Color.green);
                    return;
                }
            }

            DrawHitEffect(FromPoint.position, ray.direction * MAX_HITDISTANCE, Color.blue); 
        }

        private void DrawHitEffect(Vector3 from ,Vector3 to, Color color) 
        {
            Debug.DrawLine(from, to, color);
        }

        public void BeforeCharacterUpdate(float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Swimming:
                case CharacterState.Default:
                    {
                        CheckCharacterOverlapToDetectWater();
                        break;
                    }
            }
        }

        public bool IsColliderValidForCollisions(Collider coll)
        {
            if (IgnoredColliders.Contains(coll))
            {
                return false;
            }
            return true;
        }

        public void OnDiscreteCollisionDetected(Collider hitCollider)
        {
            
        }

        public void OnGroundHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        public void OnMovementHit(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, ref HitStabilityReport hitStabilityReport)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        if (AllowWallJump && !Motor.GroundingStatus.IsStableOnGround && !hitStabilityReport.IsStable)
                        {
                            _canWallJump = true;
                            _wallJumpNormal = hitNormal;
                        }
                        break;
                    }
            }
        }

        public void PostGroundingUpdate(float deltaTime)
        {
            if (Motor.GroundingStatus.IsStableOnGround && !Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLanded();
            }
            else if (!Motor.GroundingStatus.IsStableOnGround && Motor.LastGroundingStatus.IsStableOnGround)
            {
                OnLeaveStableGround();
            }
        }

        public void ProcessHitStabilityReport(Collider hitCollider, Vector3 hitNormal, Vector3 hitPoint, Vector3 atCharacterPosition, Quaternion atCharacterRotation, ref HitStabilityReport hitStabilityReport)
        {
            
        }

        private void OnAnimatorMove()
        {
            _rootMotionPositionDelta += CharacterAnimator.deltaPosition;
            _rootMotionRotationDelta = CharacterAnimator.deltaRotation * _rootMotionRotationDelta;
        }

        public void UpdateRotation(ref Quaternion currentRotation, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                case CharacterState.Swimming:
                case CharacterState.NoClip:
                    { 
                        if (_lookInputVector != Vector3.zero && OrientationSharpness > 0f)
                        {
                            Vector3 smoothedLookInputDirection = Vector3.Slerp(Motor.CharacterForward, _lookInputVector, 1 - Mathf.Exp(-OrientationSharpness * deltaTime)).normalized;

                            currentRotation = Quaternion.LookRotation(smoothedLookInputDirection, Motor.CharacterUp);
                        }
                        if (OrientTowardsGravity)
                        {
                            // Rotate from current up to invert gravity
                            currentRotation = Quaternion.FromToRotation((currentRotation * Vector3.up), -Gravity) * currentRotation;
                        }
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                currentRotation = _activeLadder.transform.rotation;
                                break;
                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                currentRotation = Quaternion.Slerp(_anchoringStartRotation, _ladderTargetRotation, (_anchoringTimer / AnchoringDuration));
                                break;
                        }
                        break;
                    }
            }

        }

        public void UpdateVelocity(ref Vector3 currentVelocity, float deltaTime)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        Vector3 targetMovementVelocity = Vector3.zero;
                        if (Motor.GroundingStatus.IsStableOnGround)
                        {
                            currentVelocity = Motor.GetDirectionTangentToSurface(currentVelocity, Motor.GroundingStatus.GroundNormal) * currentVelocity.magnitude;

                            Vector3 inputRight = Vector3.Cross(_moveInputVector, Motor.CharacterUp);
                            Vector3 reorientedInput = Vector3.Cross(Motor.GroundingStatus.GroundNormal, inputRight).normalized * _moveInputVector.magnitude;
                            targetMovementVelocity = reorientedInput * MaxStableMoveSpeed;

                            currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-StableMovementSharpness * deltaTime));
                        }
                        else
                        {
                            if (_moveInputVector.sqrMagnitude > 0f)
                            {
                                targetMovementVelocity = _moveInputVector * MaxAirMoveSpeed;

                                if (Motor.GroundingStatus.FoundAnyGround)
                                {
                                    Vector3 perpenticularObstructionNormal = Vector3.Cross(Vector3.Cross(Motor.CharacterUp, Motor.GroundingStatus.GroundNormal), Motor.CharacterUp).normalized;
                                    targetMovementVelocity = Vector3.ProjectOnPlane(targetMovementVelocity, perpenticularObstructionNormal);
                                }

                                Vector3 velocityDiff = Vector3.ProjectOnPlane(targetMovementVelocity - currentVelocity, Gravity);
                                currentVelocity += velocityDiff * AirAccelerationSpeed * deltaTime;
                            }

                            currentVelocity += Gravity * deltaTime;
                            currentVelocity *= (1f / (1f + (Drag * deltaTime)));
                        }
                        #region JUMP mechanics
                        _jumpedThisFrame = false;
                        _timeSinceJumpRequested += deltaTime;
                        if (_jumpRequested)
                        {
                            if (AllowDoubleJump)
                            {
                                if (_jumpConsumed && !_doubleJumpConsumed && (AllowJumpingWhenSliding ? !Motor.GroundingStatus.FoundAnyGround : !Motor.GroundingStatus.IsStableOnGround))
                                {
                                    Motor.ForceUnground(0.1f);

                                    currentVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                    CharacterAnimator.SetTrigger("OnDoubleJump");
                                    GameSoundMusicManager.Instance?.PlaySoundByPredefinedKey(PredefinedSounds.PlayerJump);
                                    _jumpRequested = false;
                                    _doubleJumpConsumed = true;
                                    _jumpedThisFrame = true;
                                }
                            }

                            if (_canWallJump || canJump())
                            {
                                Vector3 jumpDirection = Motor.CharacterUp;
                                if (_canWallJump)
                                {
                                    jumpDirection = _wallJumpNormal + (new Vector3(
                                        Mathf.Sign(currentVelocity.x) * Motor.CharacterRight.x * AdditionalDirectionForceFromJumpWall.x, 
                                        AdditionalDirectionForceFromJumpWall.y,
                                        Motor.CharacterForward.z * AdditionalDirectionForceFromJumpWall.z));//
                                    
                                    CharacterAnimator.SetTrigger("OnDoubleJump");
                                }
                                else if (Motor.GroundingStatus.FoundAnyGround && !Motor.GroundingStatus.IsStableOnGround)
                                {
                                    jumpDirection = Motor.GroundingStatus.GroundNormal;
                                }
                                GameSoundMusicManager.Instance?.PlaySoundByPredefinedKey(PredefinedSounds.PlayerJump);

                                Motor.ForceUnground(0.1f);

                                currentVelocity += (jumpDirection * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                _jumpRequested = false;
                                _jumpConsumed = true;
                                _jumpedThisFrame = true;
                            }
                        }
                        _canWallJump = false;
                        #endregion
                        //Additive jump
                        if (_internalVelocityAdd.sqrMagnitude > 0f)
                        {
                            Vector3 _dir = currentVelocity.normalized;
                            currentVelocity.x += _dir.x * _internalVelocityAdd.sqrMagnitude;
                            currentVelocity.z += _dir.z * _internalVelocityAdd.sqrMagnitude;
                            _internalVelocityAdd = Vector3.zero;
                        }

                        if (_isWalking)
                        {
                            currentVelocity.x /= 1.7f;
                            currentVelocity.z /= 1.3f;
                        }

                        break;
                    }
                case CharacterState.Swimming:
                    {
                        float verticalInput = 0f + (_jumpInputIsHeld ? 1f: 0f) + (_crouchInputIsHeld ? -1f: 0f);

                        // Smoothly interpolate to target swimming velocity
                        Vector3 targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * SwimmingSpeed;
                        Vector3 smoothedVelocity = Vector3.Lerp(currentVelocity,targetMovementVelocity, 1 - Mathf.Exp(-SwimmingMovementSharpness * deltaTime));
                        {
                            // See if our swimming reference point would be out of water after the movement from our velocity has been applied
                            Vector3 resultingSwimmingReferancePosition = Motor.TransientPosition + (smoothedVelocity * deltaTime) + (SwimmingReferencePoint.position - Motor.TransientPosition);
                            Vector3 closestPointWaterSurface = Physics.ClosestPoint(resultingSwimmingReferancePosition, _waterZone, _waterZone.transform.position, _waterZone.transform.rotation);
                            // if our position would be outside the water surface on next update, project the velocity on the surface normal so that it would not take us out of the water
                            if (closestPointWaterSurface != resultingSwimmingReferancePosition)
                            {
                                Vector3 waterSurfaceNormal = (resultingSwimmingReferancePosition - closestPointWaterSurface).normalized;
                                smoothedVelocity = Vector3.ProjectOnPlane(smoothedVelocity, waterSurfaceNormal);
                                // Jump out of water
                                if (_jumpRequested) 
                                {
                                    smoothedVelocity += (Motor.CharacterUp * JumpSpeed) - Vector3.Project(currentVelocity, Motor.CharacterUp);
                                }
                            }
                        }
                        currentVelocity = smoothedVelocity;
                        break;
                    }
                case CharacterState.NoClip:
                    {
                        float verticalInput = 0f + (_jumpInputIsHeld ? 1f : 0f) + (_crouchInputIsHeld ? -1f : 0f);

                        // Smoothly interpolate to target velocity
                        Vector3 targetMovementVelocity = (_moveInputVector + (Motor.CharacterUp * verticalInput)).normalized * NoClipMoveSpeed;
                        currentVelocity = Vector3.Lerp(currentVelocity, targetMovementVelocity, 1 - Mathf.Exp(-NoClipSharpness * deltaTime));
                        break;
                    }
                case CharacterState.Climbing:
                    {
                        currentVelocity = Vector3.zero;
                        
                        switch (_climbingState)
                        {
                            case ClimbingState.Climbing:
                                currentVelocity = (_ladderUpDownInput * _activeLadder.transform.up).normalized * ClimbingSpeed;
                                break;
                            case ClimbingState.Anchoring:
                            case ClimbingState.DeAnchoring:
                                Vector3 tmpPosition = Vector3.Lerp(_anchoringStartPosition, _ladderTargetPosition, (_anchoringTimer / AnchoringDuration));
                                currentVelocity = Motor.GetVelocityForMovePosition(Motor.TransientPosition, tmpPosition, deltaTime);
                                break;
                        }
                        break;
                    }
            }
            
        }

        private bool canJump() {
            return (!_jumpConsumed && ((AllowJumpingWhenSliding ? Motor.GroundingStatus.FoundAnyGround : Motor.GroundingStatus.IsStableOnGround) || _timeSinceLastAbleToJump <= JumpPostGroundingGraceTime));
        }

        protected void OnLanded()
        {
            GameSoundMusicManager.Instance?.PlayPlayerFootStep();
        }

        protected void OnLeaveStableGround()
        {
            
        }

        public void AddVelocity(Vector3 velocity)
        {
            switch (CurrentCharacterState)
            {
                case CharacterState.Default:
                    {
                        _internalVelocityAdd += velocity;
                        break;
                    }
            }
        }

        private void CheckCharacterOverlapToDetectWater()
        {
            if (Motor.CharacterOverlap(Motor.TransientPosition, Motor.TransientRotation, _probedColliders, WaterLayer, QueryTriggerInteraction.Collide) > 0)
            {
                // If a water surface was detected
                if (_probedColliders[0] != null)
                {
                    // If the swimming reference point is inside the box, make sure we are in swimming state
                    if (Physics.ClosestPoint(SwimmingReferencePoint.position, _probedColliders[0], _probedColliders[0].transform.position, _probedColliders[0].transform.rotation) == SwimmingReferencePoint.position)
                    {
                        if (CurrentCharacterState == CharacterState.Default)
                        {
                            TransitionToState(CharacterState.Swimming);
                            _waterZone = _probedColliders[0];
                        }
                    }
                    // otherwise; default state
                    else
                    {
                        if (CurrentCharacterState == CharacterState.Swimming)
                        {
                            TransitionToState(CharacterState.Default);
                        }
                    }

                }   
            }

        }

        private void StartDashRecovery() {
            if (!isRecovering)
            {
                isRecovering = true; // Start recovery if not already in process
                recoveryTimer = 0f; // Reset timer
            }

            recoveryTimer += Time.deltaTime;
            
            // When enough time has passed, restore 1 dash and reset timer
            if (recoveryTimer >= periodOfDashRecovery)
            {
                recoveryTimer = 0f; // Reset the timer
                currentChargeDash++; // Gain one dash charge
                Debug.Log("Dash Recovered! Current Dashes: " + currentChargeDash);

                // Stop recovering if we reach max charges
                if (currentChargeDash >= maxChargeDash)
                {
                    currentChargeDash = maxChargeDash;
                    isRecovering = false; // Stop recovery process
                }
            }
        }

        public void UpdateUIDash(ref UnityEngine.UI.Image[] _imgs) {
            var percent = Mathf.Clamp((recoveryTimer / periodOfDashRecovery), 0f, 1f);
            for (int i = 0; i < maxChargeDash; i++) {
                _imgs[i].fillAmount = !AllowDash?0:(currentChargeDash) < i ? 0 : currentChargeDash == i ? percent : 1;
            }
            
        }

        public bool CanIAvailableToDash() {
            return currentChargeDash > 0 && CanDashFrameRate();
        }

        public void EnableVision(bool enable) { 
            Visor.SetActive(enable);
        }

        void CronometricConsumeDashFrameRate() {
            _dashFrameBehavior.lastDashTime = Time.time;
            _dashFrameBehavior.dashStarttime = Time.time;
        }

        bool CanDashFrameRate() {
            return Time.time >= _dashFrameBehavior.lastDashTime + _dashFrameBehavior.framerateCooldown;
        }

    }
}
