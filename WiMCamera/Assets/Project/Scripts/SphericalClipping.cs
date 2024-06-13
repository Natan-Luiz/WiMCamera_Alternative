using System;
using UnityEngine;
using UnityEngine.Rendering;

public class SphericalClipping : MonoBehaviour
{
    public Transform wimCenter;  // The center of the WIM
    public float cullingRadius = 10.0f;  // The radius within which objects will be rendered
    public Camera cam;

    GameObject[] objsList;

    void Start()
    {
        OnValueChange();
       // RenderPipelineManager.beginCameraRendering += ChangeColors;
       // RenderPipelineManager.endCameraRendering += ChangeColorsBack;

        objsList = GameObject.FindGameObjectsWithTag("Piece");

        for (int i = 0; i < objsList.Length; i++)
        { 
            objsList[i].GetComponent<ChangeRendering>()?.SetupMe(cam);
        }
    }

    private void OnDestroy()
    {
       // RenderPipelineManager.beginCameraRendering -= ChangeColors;
       // RenderPipelineManager.endCameraRendering -= ChangeColorsBack;
    }

    private void Update()
    {
        OnValueChange();
    }

    private void OnValueChange()
    {
        Shader.SetGlobalFloat("_GLOBALClipRadius", cullingRadius);
        Shader.SetGlobalVector("_GLOBALwimCenter", wimCenter.position);
    }
}