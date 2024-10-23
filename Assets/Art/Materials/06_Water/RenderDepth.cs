using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderDepth : MonoBehaviour
{
    private void OnEnable() {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.DepthNormals;
    }
}
