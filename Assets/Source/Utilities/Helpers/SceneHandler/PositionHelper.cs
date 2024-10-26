
using System.Collections.Generic;

using UnityEngine;

public class PositionHelper : MonoBehaviour
{
    [SerializeField]
    List<Vector3> PositionToUpdate = new List<Vector3>();
    [SerializeField]
    private int To;

    [ContextMenu("Go To Target")]
    public void ChangeToTarget() {
        int ind = Mathf.Min(Mathf.Max(0, PositionToUpdate.Count), To);
        transform.position = PositionToUpdate[ind];
    }
}
