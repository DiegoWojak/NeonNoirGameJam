
using UnityEngine;

public class MaterialPropertyChange : MonoBehaviour
{
    public int ColorIndex;

    [SerializeField] 
    
    // Start is called before the first frame update
    private void Start()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        props.SetInt("_ColorIndex", ColorIndex);

        Renderer renderer = GetComponent<MeshRenderer>();
        if(renderer == null ) renderer = GetComponent<SkinnedMeshRenderer>();
        renderer.SetPropertyBlock(props);
    }


}
