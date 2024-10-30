
using Assets.Source.Utilities.Events;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Source.Utilities.Helpers.BoidsBehaviour
{
    public class DolphinBoidBehaviour : MonoBehaviour
    {


        public Transform[] points;
        public float animationSpeedVariation = 0.2f;
        public float controllerVelocity = 0.2f;
        public float rotationCoeff = 4.0f;
        public float neighborDist = 2.0f;
        public float velocityVariation = 0.5f;
        public float velocity = 6.0f;
        public float periodUpdateNoise = 5f;
        public float periodUpdateNearObjective = 5f;
        private float noiseOffset;

        private float _currentTime;
        private float _currentTimeNearObjective;
        private int _pointsLenght;
        private int iT;

        [System.Serializable]
        public enum DolphinStatus
        {
            Sad,
            Happy
        }

        [SerializeField]
        private DolphinStatus dolphinStatus = DolphinStatus.Sad;

        private void Start()
        {
            UpdateNoiseEffect();
            _currentTime = Time.time + periodUpdateNoise;
            _currentTimeNearObjective = Time.time + periodUpdateNearObjective;
            _pointsLenght = points.Length;
            iT = GetNewIndex();
        }

        private void Update()
        {
            if (dolphinStatus == DolphinStatus.Sad) return;

            var currentPosition = transform.position;
            var currentRotation = transform.rotation;
            var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
            float velocity = controllerVelocity * (1.0f + noise * animationSpeedVariation);

            var separation = Vector3.zero;
            var alignment = Vector3.zero;
            var cohesion = points[iT].transform.position;

            var avg = 1.0f; // can be used like a mirror
            alignment *= avg;
            cohesion *= avg;
            cohesion = (cohesion - currentPosition).normalized;

            var direction = (separation + alignment + cohesion);
            var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

            if (rotation != currentRotation)
            {
                var ip = Mathf.Exp(-rotationCoeff * Time.deltaTime);
                transform.rotation = Quaternion.Slerp(rotation, currentRotation, ip);
            }
            transform.position = currentPosition + transform.forward / 5 * (velocity * Time.deltaTime);

            if (_currentTime < Time.time) {
                _currentTime = Time.time + periodUpdateNoise;
                UpdateNoiseEffect();
            }


            if (Vector3.Distance(transform.position, points[iT].position) < rotationCoeff * 1f)
            {
                if (_currentTimeNearObjective < Time.time) {
                    _currentTimeNearObjective = Time.time + periodUpdateNearObjective;
                    iT = GetNewIndex();
                }
            }
            
        }

        public void GetHappy() {
            dolphinStatus = DolphinStatus.Happy;
            var _v = GetComponent<NpcController>();
            if(_v != null)
            {
                _v.DialogText = "Yo, my friend got me this glasses know i can see them. Friends can be so cool, right?";
            }
        }

        int GetNewIndex() {
            return Random.Range(0, _pointsLenght);
        }
        void UpdateNoiseEffect() {
            noiseOffset = Random.value * 10.0f;
        }
    }
}
