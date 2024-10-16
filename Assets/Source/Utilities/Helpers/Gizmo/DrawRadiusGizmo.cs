using UnityEngine;

public class DrawRadiusGizmo : MonoBehaviour
{
    public float radius = 5.0f; // The radius you want to visualize
    public Color gizmoColor = Color.green; // Color of the gizmo

    private void OnDrawGizmos()
    {
        // Set the color for the gizmo
        Gizmos.color = gizmoColor;

        // Draw a wire sphere around the GameObject's position
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}