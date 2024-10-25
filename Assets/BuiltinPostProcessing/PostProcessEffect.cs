
using UnityEngine;

public class PostProcessEffect : MonoBehaviour
{
    public Material postProcessEffectMaterial;
    bool AditionalShaderTest = false;
    public Material Testmaterial;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessEffectMaterial != null)
        {

            if (AditionalShaderTest)
            {
                RenderTexture temp = RenderTexture.GetTemporary(source.width, source.height);

                // Apply the ASCII shader first
                Graphics.Blit(source, temp, postProcessEffectMaterial);
                // Apply the Edge Detection shader after ASCII
                Graphics.Blit(temp, destination, Testmaterial);

                RenderTexture.ReleaseTemporary(temp);
            }
            else { 
                Graphics.Blit(source, destination,postProcessEffectMaterial);
            }
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }


#if UNITY_EDITOR
    [ContextMenu("Test Shader")]
    public void TestShader(){
        AditionalShaderTest = true;
    }
#endif
}
