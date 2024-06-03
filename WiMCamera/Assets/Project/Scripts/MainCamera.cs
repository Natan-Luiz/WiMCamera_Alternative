using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class MainCamera : MonoBehaviour
{
    public bool drawMinimapEnabled = true;

    public RelateCams camControl;

    // Start is called before the first frame update
    void OnEnable()
    {
        RenderPipelineManager.beginCameraRendering += CustomOnPostRender;
    }

    protected void CustomOnPostRender(ScriptableRenderContext context, Camera camera)
    {
        if (drawMinimapEnabled)
        {
            camControl.Render();
        }
    }

    private void OnDisable()
    {
        RenderPipelineManager.beginCameraRendering -= CustomOnPostRender;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= CustomOnPostRender;
    }
}
