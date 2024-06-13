using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class ChangeRendering : MonoBehaviour
{
    public Mesh miniatureExample;
    public Material[] miniatureMaterials;
    Material[] originalMaterials;
    bool hasmesh;
    Camera cam;

    void Awake()
    {
        for(int i = 0; i < transform.childCount; i++)
        {
            if(transform.GetChild(i).GetComponent<ChangeRendering>() == null)
            {
                transform.GetChild(i).gameObject.AddComponent<ChangeRendering>();
            }
        }

        if (GetComponent<Renderer>() == null)
        {
            hasmesh = false;
            return;
        }
        tag = "Piece";
        hasmesh = true;
        originalMaterials = GetComponent<Renderer>().materials;
        if(miniatureMaterials == null)
        {
            List<Material> miniatureMats = new List<Material>();
            for (int i = 0; i < originalMaterials.Length; i++)
            {
                miniatureMats.Add(new Material(Shader.Find("Shader Graphs/SimpleCut")));
                miniatureMats[miniatureMats.Count -1].mainTexture = originalMaterials[i].mainTexture;
                miniatureMats[miniatureMats.Count - 1].color = originalMaterials[i].color;
            }
            miniatureMaterials = miniatureMats.ToArray();
        }
    }

    void Start()
    {
        RenderPipelineManager.beginCameraRendering += ChangeColors;
        RenderPipelineManager.endCameraRendering += ChangeColorsBack;
    }

    private void OnDestroy()
    {
        RenderPipelineManager.beginCameraRendering -= ChangeColors;
        RenderPipelineManager.endCameraRendering -= ChangeColorsBack;
    }

    private void ChangeColorsBack(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 == cam)
        {
            ChangeMaterial(false);
        }
    }

    private void ChangeColors(ScriptableRenderContext arg1, Camera arg2)
    {
        if (arg2 == cam)
        {
            ChangeMaterial(true);
        }
    }

    public void ChangeMesh()
    {
        if (!hasmesh)
        {
            return;
        }
        GetComponent<MeshFilter>().mesh = miniatureExample;
    }

    public void ChangeMaterial(bool toMiniature)
    {
        if (!hasmesh)
        {
            return;
        }
        if (toMiniature)
        {
            GetComponent<Renderer>().materials = miniatureMaterials;
        }
        else
        {
            GetComponent<Renderer>().materials = originalMaterials;
        }
    }

    internal void SetupMe(Camera _cam)
    {
        cam = _cam;
    }
}
