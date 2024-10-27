using UnityEngine;

namespace Assets.BuiltinPostProcessing
{
    public class PostProcessEffectGhostTrail : MonoBehaviour
    {
        public Shader ghostTrailShader;
        private Material ghostTrailMaterial;

        public float fadeAmount = 0.8f;

        private RenderTexture ghostTexture;  // Store previous player frames
        public RenderTexture ghostRenderTexture; // The texture from the "Ghost Camera"

        [ContextMenu("Try")]
        void Start()
        {
            if (ghostTrailShader != null)
            {
                ghostTrailMaterial = new Material(ghostTrailShader);
            }

            ghostTexture = new RenderTexture(Screen.width, Screen.height, 0, RenderTextureFormat.ARGB32);
        }

        void OnRenderImage(RenderTexture source, RenderTexture destination)
        {
            if (ghostTrailMaterial != null && ghostRenderTexture != null)
            {
                // Pass the ghostRenderTexture (player camera texture) to the shader
                ghostTrailMaterial.SetTexture("_MainTex", source); // The full scene texture
                ghostTrailMaterial.SetTexture("_GhostTex", ghostRenderTexture); // The player render texture

                // Pass the fade amount to the shader
                ghostTrailMaterial.SetFloat("_FadeAmount", fadeAmount);

                // Update the ghost texture for the player
                Graphics.Blit(ghostRenderTexture, ghostTexture, ghostTrailMaterial);

                // Composite the ghost texture with the full scene texture
                Graphics.Blit(source, destination, ghostTrailMaterial);
            }
            else
            {
                // If no shader, just render normally
                Graphics.Blit(source, destination);
            }
        }

        void OnDestroy()
        {
            if (ghostTexture != null)
            {
                Destroy(ghostTexture);
            }
        }


    }


}
