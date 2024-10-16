
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Source.Render.Characters
{

    //Removed extra for layer obstruction detection
    //THE CAMERA 
    public class Character3DCamera : MonoBehaviour
    {
        [Header("Framing")]
        public Camera Camera;
        public Vector2 FollowPointFraming = new Vector2(0f, 0f);
        public float FollowingSharpness = 10000f;

        [Header("Distance")]
        public float DefaultDistance = 6f;
        public float MinDistance = 0f;
        public float MaxDistance = 10f;
        public float DistanceMovementSpeed = 5f;
        public float DistanceMovementSharpness = 10f;

        [Header("Rotation")]
        public bool InvertX = false;
        public bool InvertY = false;
        [Range(-90f, 90f)]
        public float DefaultVerticalAngle = 20f;
        [Range(-90f, 90f)]
        public float MinVerticalAngle = -90f;
        [Range(-90f, 90f)]
        public float MaxVerticalAngle = 90f;
        public float RotationSpeed = 1f;
        public float RotationSharpness = 10000f;
        public bool RotateWithPhysicsMover = false;

        [Header("Obstruction")]
        public float ObstructionCheckRadius = 0.2f;
        public LayerMask ObstructionLayers = -1;
        public float ObstructionSharpness = 10000f;
        public List<Collider> IgnoredColliders = new List<Collider>();

        public Transform Transform { get; private set; }
        public Transform FollowTransform { get; private set; }
        public Vector3 PlanarDirection { get; set; }
        public float TargetDistance { get; set; }

        private bool _distanceIsObstructed;
        private float _currentDistance;
        private float _targetVerticalAngle;
        private Vector3 _currentFollowPosition;
        //
        private RaycastHit _obstructionHit;
        private int _obstructionCount;
        private RaycastHit[] _obstructions = new RaycastHit[MaxObstructions];

        private const int MaxObstructions = 32;
        private void OnValidate()
        {
            DefaultDistance = Mathf.Clamp(DefaultDistance, MinDistance, MaxDistance);
            //Between these two angles
            DefaultVerticalAngle = Mathf.Clamp(DefaultVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
        }


        private void Awake()
        {//Initialization?
            Transform = this.transform;

            _currentDistance = DefaultDistance;
            TargetDistance = _currentDistance;

            _targetVerticalAngle = 0f;

            PlanarDirection = Vector3.forward;
        }

        /// <summary>
        /// Target (The player) to Follow
        /// </summary>
        /// <param name="t"></param>
        public void SetFollowTranform(Transform t) {
            FollowTransform = t;
            PlanarDirection = FollowTransform.forward;
            _currentFollowPosition = FollowTransform.position;
        }

        public bool IsCameraClose() {
            return TargetDistance == 0f;
        }


        public void UpdateWithInput(float deltaTime, float zoomInput, Vector3 rotationInput) {
            if (FollowTransform) {
                if (InvertX) {
                    rotationInput.x *= -1f;
                }
                if (InvertY)
                {
                    rotationInput.y *= -1f;
                }

                #region Rotation Input
                Quaternion rotationFromInput = Quaternion.Euler(FollowTransform.up * (rotationInput.x * RotationSpeed)); //<-- you get the rotation of InputR on X and unit in Y rotation
                PlanarDirection = rotationFromInput * PlanarDirection; // Vector3 multiply Quaternion (Rotation) Equals a vector3 rotated
                PlanarDirection = Vector3.Cross(FollowTransform.up, Vector3.Cross(PlanarDirection, FollowTransform.up)); // vec3(x,y) cross Unit vector y, to get an perpedicular Vector in XZ, then again to get a new perpendicular vector for the vector VecUP
                Quaternion planarRot = Quaternion.LookRotation(PlanarDirection, FollowTransform.up); // The goal of this double cross product is to adjust PlanarDirection such that it lies within the plane perpendicular to FollowTransform.up

                _targetVerticalAngle -= (rotationInput.y * RotationSpeed);
                _targetVerticalAngle = Mathf.Clamp(_targetVerticalAngle, MinVerticalAngle, MaxVerticalAngle);
                Quaternion verticalRot = Quaternion.Euler(_targetVerticalAngle, 0, 0);
                Quaternion targetRotation = Quaternion.Slerp(Transform.rotation, planarRot * verticalRot, 1f - Mathf.Exp(-RotationSharpness * deltaTime));

                Transform.rotation = targetRotation;
                #endregion

                #region Proces Distance Input
                if (_distanceIsObstructed && Mathf.Abs(zoomInput) > 0f)
                {
                    TargetDistance = _currentDistance;
                }
                TargetDistance += zoomInput * DistanceMovementSpeed;
                TargetDistance = Mathf.Clamp(TargetDistance, MinDistance, MaxDistance);
                #endregion

                #region moothed follow position
                _currentFollowPosition = Vector3.Lerp(_currentFollowPosition, FollowTransform.position, 1f - Mathf.Exp(-FollowingSharpness * deltaTime));
                #endregion

                #region Handle Obstruction
                RaycastHit closestHit = new RaycastHit();
                closestHit.distance = Mathf.Infinity;

                _obstructionCount = Physics.SphereCastNonAlloc(
                    origin: _currentFollowPosition, // Where is the reference point from the maxdistance
                    radius: ObstructionCheckRadius, // how big the sphere is
                    direction: -Transform.forward, // Positive rot or negative rot
                    results: _obstructions, //All the hit obj catched
                    maxDistance: TargetDistance, // How far from the origin point i will start casting sphere with radius
                    layerMask: ObstructionLayers,
                    queryTriggerInteraction: QueryTriggerInteraction.Ignore
                    );

                for (int i = 0; i < _obstructionCount; i++) { //<-- Let find the closes hit
                    bool isIgnored = false;

                    for (int j = 0; j < IgnoredColliders.Count; j++)
                    {
                        if (IgnoredColliders[j] == _obstructions[i].collider)
                        {
                            isIgnored = true;
                            break;
                        }
                    }
                    if (!isIgnored 
                        && _obstructions[i].distance < closestHit.distance
                        && _obstructions[i].distance > 0)
                    {
                        closestHit = _obstructions[i];
                    }

                    // Interpolation --> FuncLerp(a, b , t) = a * (1 - t) + b * t
                    if (closestHit.distance < Mathf.Infinity) // If obstructions detecter
                    {
                        _distanceIsObstructed = true;
                        _currentDistance = Mathf.Lerp( a: _currentDistance, b: closestHit.distance, 
                            t: 1 - Mathf.Exp(-ObstructionSharpness * deltaTime));
                    }
                    else // If no
                    {
                        _distanceIsObstructed = false;
                        _currentDistance = Mathf.Lerp(a: _currentDistance, b: TargetDistance,
                            t: 1 - Mathf.Exp(-DistanceMovementSharpness * deltaTime));
                    }
                }

                #endregion

                #region Applying the Framing position for an orbit Camera
                Vector3 targetPosition = _currentFollowPosition - ((targetRotation * Vector3.forward) * _currentDistance);

                targetPosition += Transform.right * FollowPointFraming.x;
                targetPosition += Transform.up * FollowPointFraming.y;

                Transform.position = targetPosition;
                #endregion
            }
        }
    }
}
