using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

    bool VRenabled = false; // true if VR eyeWidth is true and useable for renderTexture

    public float sizeDiff = 24;

    [Header("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    [Header("Hands Related")]
    PortalSlicing portalSlicing; // Object slicing parameters

    private void Awake()
    {
        portalSlicing = GetComponent<PortalSlicing>();
        portalSlicing.Initialize(here,there);
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

    public void Render()
    {
        if (!CameraUtils.VisibleFromCamera(screen, here.cam))
        {
            return;
        }

        PrePortalRender();

        float playerD = Vector3.Distance(here.cam.transform.position, here.planeView.transform.position);
        // The following lines manually adapt the scale, deprecated because of the sizeDiff variable
        // float worldD = Vector3.Distance(there.cam.transform.parent.position, centerMultiplier.transform.position);
        // multilier = worldD / playerD;

        CreateViewTexture();

        var localToWorldMatrix = here.cam.transform.localToWorldMatrix;

        Vector3 renderPosition;
        Quaternion renderRotation;

        there.cam.projectionMatrix = here.cam.projectionMatrix;
        there.planeView.transform.localScale = Vector3.one * sizeDiff;
        localToWorldMatrix = there.planeView.transform.localToWorldMatrix * screen.transform.worldToLocalMatrix * localToWorldMatrix;
        renderPosition = localToWorldMatrix.GetColumn(3);
        renderRotation = localToWorldMatrix.rotation;

        there.cam.transform.SetPositionAndRotation(renderPosition, renderRotation);
        //SetNearClipPlane();
        portalSlicing.HandleClipping();
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

            viewTexture = new RenderTexture(XRSettings.eyeTextureWidth, XRSettings.eyeTextureHeight, 0);
            there.cam.targetTexture = viewTexture;
            screen.material.SetTexture("_MainTex", viewTexture);
        }
        else if(!VRenabled)
        {
            if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
            {
                Debug.LogError("Gen RenderTexture Normal : " + viewTexture?.width + " " +  Screen.width + " " + viewTexture?.height + " "  + Screen.height);
                if (viewTexture != null)
                {
                    Debug.LogError("Release View");
                    viewTexture.Release();
                }
                viewTexture = new RenderTexture(Screen.width, Screen.height, 0);   
                // Render the view from the portal camera to the view texture
                there.cam.targetTexture = viewTexture;
                // Display the view texture on the screen of the linked portal
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
