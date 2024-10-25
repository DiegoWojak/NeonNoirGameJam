
using Assets.Source.Managers;
using Assets.Source.Utilities.Helpers;
using UnityEngine;

public class PostProcessEffect : MonoBehaviour
{
    public Material postProcessEffectMaterial;
    public Material postIntermedialMaterial;
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (postProcessEffectMaterial != null)
        {

            if (GameStarterManager.Instance._EffectsComponent.HasRGBGlasses() && postIntermedialMaterial!=null)
            {
                /*RenderTexture temp = RenderTexture.GetTemporary(source.width/2, source.height/2);

                // Apply the ASCII shader first
                Graphics.Blit(source, temp, postProcessEffectMaterial);
                // Apply the Edge Detection shader after ASCII
                Graphics.Blit(temp, destination, postIntermedialMaterial);

                RenderTexture.ReleaseTemporary(temp);*/
                Graphics.Blit(source, destination, postIntermedialMaterial);
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


}
