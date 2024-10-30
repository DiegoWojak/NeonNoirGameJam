using UnityEngine;

public class RotateGameObject : MonoBehaviour
{
    public float rotationSpeed = 90f;  // Rotation speed in degrees per second
    public float animationDuration = 2f;  // Duration for one full spin
    public AnimationType animationType = AnimationType.EaseInOut;  // Choose ease type
    public bool loopRotation = true;  // Whether to loop indefinitely

    private float elapsedTime = 0f;
    private Quaternion startRotation;

    // Enum for different easing types
    public enum AnimationType
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut
    }

    void Start()
    {
        startRotation = transform.rotation;
    }

    void Update()
    {
        elapsedTime += Time.deltaTime;

        if (elapsedTime > animationDuration && !loopRotation)
            return;

        // Calculate normalized time (0 to 1) based on elapsed time and animation duration
        float normalizedTime = Mathf.Clamp01(elapsedTime / animationDuration);

        // Apply selected easing function
        float easedTime = ApplyEasing(normalizedTime, animationType);

        // Perform Y-axis rotation based on easedTime and rotationSpeed
        float angle = easedTime * 360f * (rotationSpeed / 360f);
        transform.rotation = startRotation * Quaternion.Euler(0, 0, angle);

        // Reset for looping
        if (loopRotation && normalizedTime >= 1f)
        {
            elapsedTime = 0f;
            startRotation = transform.rotation;
        }
    }

    // Easing function based on selected type
    float ApplyEasing(float time, AnimationType type)
    {
        switch (type)
        {
            case AnimationType.Linear:
                return time;  // No easing
            case AnimationType.EaseIn:
                return Mathf.Pow(time, 3);  // Cubic ease-in
            case AnimationType.EaseOut:
                return 1 - Mathf.Pow(1 - time, 3);  // Cubic ease-out
            case AnimationType.EaseInOut:
                return Mathf.SmoothStep(0f, 1f, time);  // SmoothStep for ease-in-out
            default:
                return time;
        }
    }
}