using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class LightmapLODManager : MonoBehaviour
{
    private MeshRenderer m_Renderer;

    private void Awake()
    {
        m_Renderer = gameObject.GetComponent<MeshRenderer>();
        this.ConfigureRenderers();
    }

    void ConfigureRenderers()
    {
        if (!GetComponentInParent<LODGroup>() || !m_Renderer) return;

        var lods = GetComponentInParent<LODGroup>().GetLODs();
        LOD? currentLod = null;

        foreach (var lod in lods)
        {
            foreach (var lodRenderer in lod.renderers) if (m_Renderer == lodRenderer) currentLod = lod;
        }

        if (currentLod == null) return;

        var renderers = currentLod?.renderers;
        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i] != null)
            {
                try
                {
                    var lod0Renderer = lods[0].renderers[i];

                    renderers[i].lightProbeUsage = lod0Renderer.lightProbeUsage;
                    renderers[i].lightmapIndex = lod0Renderer.lightmapIndex;
                    renderers[i].lightmapScaleOffset = lod0Renderer.lightmapScaleOffset;
                    renderers[i].realtimeLightmapIndex = lod0Renderer.realtimeLightmapIndex;
                    renderers[i].realtimeLightmapScaleOffset = lod0Renderer.realtimeLightmapScaleOffset;
                }
                catch
                {
#if DEBUG
                    Debug.Log("Failed to set lightmap settings on " + gameObject.name);
#endif
                }
            }
        }
    }

#if UNITY_EDITOR
    void OnBecameVisible() { if (!Application.isPlaying) this.ConfigureRenderers(); }
#endif
}
