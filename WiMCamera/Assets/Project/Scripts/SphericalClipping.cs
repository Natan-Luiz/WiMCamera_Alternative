using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SphericalClipping : MonoBehaviour
{
    public Transform wimCenter;  // The center of the WIM
    public float cullingRadius = 10.0f;  // The radius within which objects will be rendered
    public Camera cam;

    void Start()
    {
        OnValueChange();
        RenderPipelineManager.beginCameraRendering += ChangeColors;
        RenderPipelineManager.endCameraRendering += ChangeColorsBack;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= ChangeColors;
        RenderPipelineManager.endCameraRendering -= ChangeColorsBack;
    }

    private void ApplyColors(bool b)
    {
        OnValueChange();
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Piece");

        for (int i = 0; i < objs.Length; i++)
        {
            if (objs[i].activeInHierarchy)
            {
                objs[i].GetComponent<ChangeRendering>()?.ChangeMaterial(b);
            }
        }
    }

    private void ChangeColorsBack(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 == cam)
        {
            ApplyColors(false);
        }
    }

    private void ChangeColors(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 == cam)
        { 
            ApplyColors(true);
        }
    }

    private void OnValueChange()
    {
        Shader.SetGlobalFloat("_GLOBALClipRadius", cullingRadius);
        Shader.SetGlobalVector("_GLOBALwimCenter", wimCenter.position);
    }
}