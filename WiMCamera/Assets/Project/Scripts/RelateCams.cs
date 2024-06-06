using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.XR;

[Serializable]
public struct RelateObjects
{
    public Camera cam;
    public Transform worldCenter;
    public GameObject planeView;
}

public class RelateCams : MonoBehaviour
{
    public RelateObjects here; // Local related objects

    public RelateObjects there; // Remote related objects


    public MeshRenderer screen; // Where To Render

    RenderTexture viewTexture;

    public bool VRenabled = false; // true if VR eyeWidth is true and useable for renderTexture

    public bool singleRes = false;

    public float sizeDiff = 24;

    public float LensIPD = 0.07f;

    [Header("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    [Header("Hands Related")]
    PortalSlicing portalSlicing; // Object slicing parameters

    private void Awake()
    {
        portalSlicing = GetComponent<PortalSlicing>();
        portalSlicing.Initialize(here,there, this);
        screen.material.SetInt("displayMask", 1);
    }

    public void PrePortalRender()
    {
        portalSlicing.UpdateTravelers();
    }

    public void PosPortalRender()
    {
        portalSlicing.UpdateTravelers();
    }

    public void SetTransform(Transform t, Matrix4x4 localToWorld)
    {
        Vector3 renderPosition = localToWorld.GetColumn(3);
        Quaternion renderRotation = localToWorld.rotation;

        t.SetPositionAndRotation(renderPosition, renderRotation);
    }

    public void Render()
    {
        if (!CameraUtils.VisibleFromCamera(screen, here.cam))
        {
            return;
        }

        PrePortalRender();

        CreateViewTexture();

        there.planeView.transform.localPosition = here.planeView.transform.localPosition;
        there.planeView.transform.localRotation = here.planeView.transform.localRotation;

        var localToWorldMatrix = here.cam.transform.localToWorldMatrix;

        //there.cam.projectionMatrix = here.cam.projectionMatrix;
        there.worldCenter.transform.localScale = Vector3.one * sizeDiff * here.planeView.transform.localScale.x;
        localToWorldMatrix = there.worldCenter.transform.localToWorldMatrix * here.worldCenter.worldToLocalMatrix * localToWorldMatrix;

        SetTransform(there.cam.transform,localToWorldMatrix);


        


        //SetNearClipPlane();
        portalSlicing.HandleClipping();
        //UniversalRenderPipeline.RenderSingleCamera(there.cam);
        //there.cam.Render();
    }

    void CreateViewTexture()
    {
        if (!VRenabled && XRSettings.eyeTextureWidth != 0)
        {
            Debug.LogError("Gen RenderTexture VR");
            VRenabled = true;

            if (viewTexture != null)
            {
                viewTexture.Release();
            }
            if(singleRes)
                viewTexture = new RenderTexture(XRSettings.eyeTextureWidth * 1, XRSettings.eyeTextureHeight, 0);
            else
                viewTexture = new RenderTexture(XRSettings.eyeTextureWidth * 2, XRSettings.eyeTextureHeight, 0);
            there.cam.targetTexture = viewTexture;
            screen.material.SetTexture("_MainTex", viewTexture);
            screen.material.SetFloat("_multiplierValue", LensIPD);
        }
        else if(!VRenabled)
        {
            if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
            {
                if (viewTexture != null)
                {
                    viewTexture.Release();
                }
                viewTexture = new RenderTexture(Screen.width, Screen.height, 0);   

                there.cam.targetTexture = viewTexture;

                screen.material.SetFloat("_multiplierValue", 0);
                screen.material.SetTexture("_MainTex", viewTexture);
            }
        }


    }

    void SetNearClipPlane()
    {      
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = there.planeView.transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, there.planeView.transform.position - there.cam.transform.position));

        Vector3 camSpacePos = there.cam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = there.cam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            there.cam.projectionMatrix = here.cam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            there.cam.projectionMatrix = here.cam.projectionMatrix;
        }
        
    }

}
