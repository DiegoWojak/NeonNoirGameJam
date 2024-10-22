
using Assets.Source.Managers;
using Assets.Source.Render.Characters;
using UnityEngine;

namespace Assets.Source.Utilities.Helpers
{
    public class ParticleDashBehavioir : MonoBehaviour
    {
        private float _currentTime;
        [SerializeField]
        private float _duration;
        [SerializeField]
        ParticleSystem render;
        private void OnEnable()
        {
            _currentTime = Time.time;
            render.Play();
        }


        private void FixedUpdate()
        {   
            if ( _currentTime + _duration < Time.time )
            {
                PoolManager.Instance?.ReturnDashObject(gameObject);
            }
        }


    }
}
