
using UnityEngine;

public class BoidsBehaviour : MonoBehaviour
{
    public Transform point;
    public float animationSpeedVariation = 0.2f;
    public float controllerVelocity = 0.2f;
 
    public float rotationCoeff = 4.0f;
    public LayerMask searchLayer;
    public float neighborDist = 2.0f;
    public float velocityVariation = 0.5f;
    public float velocity = 6.0f;
    public float orbitDistance = 5.0f; // Fixed distance from the point
    public float orbitSpeed = 2.0f;    // Speed of rotation

    float noiseOffset;

    private void Start()
    {
        
    }

    private void Update()
    {
        CSGBoids();
    }

    Vector3 GetSeparationVector(Vector3 pos)
    {
        Vector3 diff = transform.position - pos;
        float diffLen = Vector3.Magnitude(diff);
        float scaler = Mathf.Clamp(1.0f - diffLen / neighborDist, 0, 1);
        return diff * (scaler / diffLen);
    }

    public void CSGBoids() {
        var currentPosition = transform.position;
        var currentRotation = transform.rotation;
        noiseOffset = Random.value * 10.0f;
        var noise = Mathf.PerlinNoise(Time.time, noiseOffset) * 2.0f - 1.0f;
        float velocity = controllerVelocity * (1.0f + noise * animationSpeedVariation);

        var separation = Vector3.zero;
        var alignment = Vector3.zero;
        var cohesion = point.transform.position;
        Debug.Log($"TAR {alignment}");
   
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
        transform.position = currentPosition + transform.forward/5 * (velocity * Time.deltaTime);
       
    }
    
}
