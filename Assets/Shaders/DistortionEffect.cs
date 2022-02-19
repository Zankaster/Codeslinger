using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ImageEffectAllowedInSceneView]
[ExecuteInEditMode]
public class DistortionEffect : MonoBehaviour {
    private Material m_Material;

    [SerializeField]
    private Shader m_Shader = null;

    public float Distortion = 0.1f;

    private void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture) {
        if (m_Material == null) {
            m_Material = new Material(m_Shader);

            if (!m_Shader.isSupported) {
                Debug.Log("This shader is not supported.");
                enabled = false;
                return;
            }
        }

        if (m_Material != null) {
            m_Material.SetFloat("_Distortion", Distortion);
            Graphics.Blit(sourceTexture, destTexture, m_Material);
        }
        else
            Graphics.Blit(sourceTexture, destTexture);
    }
}