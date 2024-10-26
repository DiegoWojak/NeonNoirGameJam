
using UnityEngine;

public class BoidsBehaviourRigid : MonoBehaviour
{
    public Transform point;

    public float radius = 5f;      // Distance from the center
    public float speed = 1f;       // Speed of circular motion
    public float rotationSpeed = 15f; // Speed of self-rotation (degrees per second)

    private float angle = 0f;

    private Rigidbody m_Rigidbody;
    private void Start()
    {
        m_Rigidbody=GetComponent<Rigidbody>();
    }

    private void Update()
    {
        CSGBoids();
    }


    public void CSGBoids() {
        angle += speed * Time.fixedDeltaTime;  // Increment the angle
        float x = point.position.x + radius * Mathf.Cos(angle);
        float y = point.position.y + radius * Mathf.Sin(angle);

        // Update the Rigidbody's position (manually setting the position for precise circular motion)
        m_Rigidbody.MovePosition(new Vector3(x, y, m_Rigidbody.position.z));

        // 2. Self-rotation: Rotate the object by 15 degrees per second (around its local Z-axis)
        Quaternion rotation = Quaternion.Euler(0, 0, rotationSpeed * Time.fixedDeltaTime);
        m_Rigidbody.MoveRotation(m_Rigidbody.rotation * rotation);

    }
    
}
