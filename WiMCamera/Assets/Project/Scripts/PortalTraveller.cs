using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalTraveller : MonoBehaviour
{
    public GameObject graphicsObject;
    public GameObject graphicsClone { get; set; }

    public Vector3 previousOffsetFromPortal { get; set; }

    public Material[] originalMaterials { get; set; }
    public Material[] cloneMaterials { get; set; }

    public virtual bool EnterPortalThresholdFirstTime()
    {
        if (graphicsClone == null)
        {
            graphicsClone = Instantiate(graphicsObject);
            graphicsClone.transform.parent = graphicsObject.transform.parent;
            graphicsClone.transform.localScale = graphicsObject.transform.localScale;
            originalMaterials = GetMaterials(graphicsObject);
            cloneMaterials = GetMaterials(graphicsClone);
            return true;
        }
        else
        {
            graphicsClone.SetActive(true);
        }
        return false;
    }

    // Called once no longer touching portal (excluding when teleporting)
    public virtual void ExitPortalThreshold()
    {
        graphicsClone.SetActive(false);
        // Disable slicing
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            originalMaterials[i].SetVector("sliceNormal", Vector3.zero);
        }
    }

    public void SetSliceOffsetDst(float dst, bool clone)
    {
        for (int i = 0; i < originalMaterials.Length; i++)
        {
            if (clone)
            {
                cloneMaterials[i].SetFloat("sliceOffset", dst);
            }
            else
            {
                originalMaterials[i].SetFloat("sliceOffset", dst);
            }

        }
    }

    Material[] GetMaterials(GameObject g)
    {
        var renderers = g.GetComponentsInChildren<MeshRenderer>();

        var matList = new List<Material>();
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.materials)
            {
                matList.Add(mat);
            }
        }

        var rds2 = g.GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (var renderer in rds2)
        {
            foreach (var mat in renderer.materials)
            {
                matList.Add(mat);
            }
        }
        return matList.ToArray();
    }

    internal void SetCloneSize(float sizeDiff)
    {
        graphicsClone.transform.localScale *= sizeDiff;
    }
}
