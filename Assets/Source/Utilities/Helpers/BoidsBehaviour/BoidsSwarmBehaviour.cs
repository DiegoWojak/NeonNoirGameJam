

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.WSA;


namespace Assets.Source.Utilities.Helpers.BoidsBehaviour
{
    [System.Serializable]
    public class BoidsSwarmBehaviour : MonoBehaviour
    {
        [SerializeField] float _velocity = 6;
        [SerializeField, Range(0, 1)] float _velocityVariance = 0.5f;
        [SerializeField] Vector3 _scroll = Vector3.zero;

        [SerializeField] float _rotationSpeed = 4;
        [SerializeField] float _neighborDistance = 2;

        [SerializeField]
        List<Boid> _boids = new List<Boid>();

        [SerializeField]
        Transform[] points;
        int _pointsLenght;
        private int iT;
        private float _currentTimeNearObjective;
        public float periodUpdateNearObjective = 5f;
        Vector3 GetSeparationVector(Boid self, Boid target)
        {
            var diff = target.position - self.position;
            var diffLen = diff.magnitude;
            var scaler = Mathf.Clamp01(1 - diffLen / _neighborDistance);
            return diff * scaler / diffLen;
        }

        // Reynolds' steering behavior
        void SteerBoid(Boid self)
        {
            // Steering vectors
            var separation = Vector3.zero;
            var alignment = points[iT].forward;
            var cohesion = points[iT].position;

            // Looks up nearby boids.
            var neighborCount = 0;
            foreach (var neighbor in _boids)
            {
                if (neighbor == self) continue;

                var dist = Vector3.Distance(self.position, neighbor.position);
                if (dist > _neighborDistance) continue;

                // Influence from this boid
                separation += GetSeparationVector(self, neighbor);
                alignment += neighbor.rotation * Vector3.forward;
                cohesion += neighbor.position;

                neighborCount++;
            }

            // Normalization
            var div = 1.0f / (neighborCount + 1);
            alignment *= div;
            cohesion = (cohesion * div - self.position).normalized;

            // Calculate the target direction and convert to quaternion.
            var direction = separation + alignment * 0.667f + cohesion;
            var rotation = Quaternion.FromToRotation(Vector3.forward, direction.normalized);

            // Applys the rotation with interpolation.
            if (rotation != self.rotation)
            {
                var ip = Mathf.Exp(-_rotationSpeed * Time.deltaTime);
                self.rotation = Quaternion.Slerp(rotation, self.rotation, ip);
            }

        }

        void AdvanceBoid(Boid self)
        {
            var noise = Mathf.PerlinNoise(Time.time * 0.5f, self.noiseOffset) * 2 - 1;
            var velocity = _velocity * (1 + noise * _velocityVariance);
            var forward = self.rotation * Vector3.forward;
            self.position += (forward * velocity + _scroll) * Time.deltaTime;
        }

      
        private void Start()
        {
            _pointsLenght = points.Length;
            foreach (var item in _boids)
            {
                //item.gameObject.transform.SetParent(transform);
                item.gameObject.transform.position += Random.insideUnitSphere;
                item.position = item.gameObject.transform.position;
                item.rotation = Quaternion.Slerp(transform.rotation, Random.rotation, 0.3f);
                item.noiseOffset = Random.value * 10;
            }
            iT = GetNewIndex();
            _currentTimeNearObjective = Time.time + periodUpdateNearObjective;
        }


        void Update()
        {
            foreach (var boid in _boids) SteerBoid(boid);
            foreach (var boid in _boids) AdvanceBoid(boid);

            foreach (var boid in _boids)
            {
                var tr = boid.gameObject.transform;
                tr.position = boid.position;
                tr.rotation = boid.rotation;
            }

            if (Vector3.Distance(_boids[0].position, points[iT].position) < _neighborDistance * 1f)
            {
                if (_currentTimeNearObjective < Time.time)
                {
                    _currentTimeNearObjective = Time.time + periodUpdateNearObjective;
                    iT = GetNewIndex();
                }
            }
        }

        int GetNewIndex()
        {
            return Random.Range(0, _pointsLenght);
        }
    }



    [System.Serializable]
    public class Boid
    {
        public Vector3 position;
        public Quaternion rotation;
        public float noiseOffset;
        public GameObject gameObject;
    }
}
