using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelateCams : MonoBehaviour
{
    public Camera portalCam;
    public Camera playerCam;
    public MeshRenderer screen;
    public Transform centerMultiplier;
    RenderTexture viewTexture;
    public float multilier = 0.1f;

    [Header("Advanced Settings")]
    public float nearClipOffset = 0.05f;
    public float nearClipLimit = 0.2f;

    [Header("Hands Related")]
    MeshFilter screenMeshFilter;
    List<PortalTraveller> trackedTravellers;

    private void Awake()
    {
        //portalCam.enabled = false;
        screenMeshFilter = screen.GetComponent<MeshFilter>();
        trackedTravellers = new List<PortalTraveller>();
        screen.material.SetInt("displayMask", 1);
    }

    void LateUpdate()
    {
        HandleTravellers();
    }

    public void PrePortalRender()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams(traveller);
        }
    }
    public void Render()
    {
        if (!CameraUtils.VisibleFromCamera(screen, playerCam))
        {
            return;
        }

        float playerD = Vector3.Distance(playerCam.transform.position, screen.transform.position);
        float worldD = Vector3.Distance(portalCam.transform.parent.position, centerMultiplier.transform.position);
        multilier = worldD / playerD;

        CreateViewTexture();

        var localToWorldMatrix = playerCam.transform.localToWorldMatrix;

        Vector3 renderPosition;
        Quaternion renderRotation;

        portalCam.projectionMatrix = playerCam.projectionMatrix;
        localToWorldMatrix = transform.localToWorldMatrix * screen.transform.worldToLocalMatrix * localToWorldMatrix;
        renderPosition = localToWorldMatrix.GetColumn(3);// multilier;
        renderRotation = localToWorldMatrix.rotation;

        portalCam.transform.SetPositionAndRotation(renderPosition, renderRotation);
        SetNearClipPlane();
        HandleClipping();
        //portalCam.Render();
    }

    public void PosPortalRender()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams(traveller);
        }
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;

            var m = transform.localToWorldMatrix * screen.transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - transform.position;
          //  int portalSide = System.Math.Sign(Vector3.Dot(offsetFromPortal, transform.forward));
          //  int portalSideOld = System.Math.Sign(Vector3.Dot(traveller.previousOffsetFromPortal, transform.forward));

            traveller.graphicsClone?.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
            traveller.previousOffsetFromPortal = offsetFromPortal;
        }
    }

    void HandleClipping()
    {
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = 0.01f;

        foreach (var traveller in trackedTravellers)
        {
            int cloneSideOfLinkedPortal = -SideOfPortal(traveller.transform.position);
            traveller.SetSliceOffsetDst(-screenThickness, true);
            traveller.SetSliceOffsetDst(screenThickness, false);
        }
    }

        
    void UpdateSliceParams(PortalTraveller traveller)
    {
        int side = SideOfPortal(traveller.transform.position);
        Vector3 sliceNormal = transform.forward * -side;
        Vector3 cloneSliceNormal = portalCam.transform.parent.forward * side;

        Vector3 slicePos = transform.position;
        Vector3 cloneSlicePos = portalCam.transform.parent.position;

        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;
        float screenThickness = 0.01f;

        cloneSliceOffsetDst = -screenThickness;

        for (int i = 0; i < traveller.originalMaterials.Length; i++)
        {
            traveller.originalMaterials[i].SetVector("sliceCenter", slicePos);
            traveller.originalMaterials[i].SetVector("sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat("sliceOffset", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector("sliceCenter", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector("sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat("sliceOffset", cloneSliceOffsetDst);
        }
    }

    void CreateViewTexture()
    {
        if (viewTexture == null || viewTexture.width != Screen.width || viewTexture.height != Screen.height)
        {
            if (viewTexture != null)
            {
                viewTexture.Release();
            }
            viewTexture = new RenderTexture(Screen.width, Screen.height, 0);
            // Render the view from the portal camera to the view texture
            portalCam.targetTexture = viewTexture;
            // Display the view texture on the screen of the linked portal
            screen.material.SetTexture("_MainTex", viewTexture);
        }
    }

    void SetNearClipPlane()
    {
        // Learning resource:
        // http://www.terathon.com/lengyel/Lengyel-Oblique.pdf
        Transform clipPlane = transform;
        int dot = System.Math.Sign(Vector3.Dot(clipPlane.forward, transform.position - portalCam.transform.position));

        Vector3 camSpacePos = portalCam.worldToCameraMatrix.MultiplyPoint(clipPlane.position);
        Vector3 camSpaceNormal = portalCam.worldToCameraMatrix.MultiplyVector(clipPlane.forward) * dot;
        float camSpaceDst = -Vector3.Dot(camSpacePos, camSpaceNormal) + nearClipOffset;

        // Don't use oblique clip plane if very close to portal as it seems this can cause some visual artifacts
        if (Mathf.Abs(camSpaceDst) > nearClipLimit)
        {
            Vector4 clipPlaneCameraSpace = new Vector4(camSpaceNormal.x, camSpaceNormal.y, camSpaceNormal.z, camSpaceDst);

            // Update projection based on new clip plane
            // Calculate matrix with player cam so that player camera settings (fov, etc) are used
            portalCam.projectionMatrix = playerCam.CalculateObliqueMatrix(clipPlaneCameraSpace);
        }
        else
        {
            portalCam.projectionMatrix = playerCam.projectionMatrix;
        }
    }

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - transform.position, transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }
}
