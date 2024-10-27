
using Assets.Source.Managers;
using Assets.Source.Utilities.Helpers;
using UnityEngine;

public class PostProcessEffect : MonoBehaviour
{
    public Material postProcessEffectMaterial;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessEffectMaterial != null)
        {
            Graphics.Blit(source, destination,postProcessEffectMaterial);   
        }
        else
        {
            Graphics.Blit(source, destination);
        }



    }


}
