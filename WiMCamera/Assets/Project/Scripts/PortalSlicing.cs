using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSlicing : MonoBehaviour
{
    RelateObjects here;
    RelateObjects there;
    MeshFilter screenMeshFilter;
    List<PortalTraveller> trackedTravellers;
    public float screenThickness = 0.01f;

    void LateUpdate()
    {
        HandleTravellers();
    }

    public void Initialize(RelateObjects local, RelateObjects remote)
    {
        here = local;
        there = remote;
        screenMeshFilter = here.planeView.GetComponent<MeshFilter>();
        trackedTravellers = new List<PortalTraveller>();
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;

            var m = there.planeView.transform.localToWorldMatrix * here.planeView.transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - here.planeView.transform.position;

            traveller.graphicsClone?.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
            traveller.previousOffsetFromPortal = offsetFromPortal;
        }
    }

    void UpdateSliceParams(PortalTraveller traveller)
    {
        Vector3 sliceNormal = here.planeView.transform.forward;
        Vector3 cloneSliceNormal = -here.planeView.transform.forward;

        Vector3 slicePos = here.planeView.transform.position;
        Vector3 cloneSlicePos = there.planeView.transform.position;

        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;

        cloneSliceOffsetDst = -screenThickness;

        for (int i = 0; i < traveller.originalMaterials.Length; i++)
        {
            traveller.originalMaterials[i].SetVector("_sliceCenter", slicePos);
            traveller.originalMaterials[i].SetVector("_sliceNormal", sliceNormal);
            traveller.originalMaterials[i].SetFloat("_sliceOffset", sliceOffsetDst);

            traveller.cloneMaterials[i].SetVector("_sliceCenter", cloneSlicePos);
            traveller.cloneMaterials[i].SetVector("_sliceNormal", cloneSliceNormal);
            traveller.cloneMaterials[i].SetFloat("_sliceOffset", cloneSliceOffsetDst);
        }
    }

    public void HandleClipping()
    {
        const float hideDst = -1000;
        const float showDst = 1000;
        float screenThickness = 0.001f;

        foreach (var traveller in trackedTravellers)
        {
            traveller.SetSliceOffsetDst(-screenThickness, true);
            traveller.SetSliceOffsetDst(screenThickness, false);
        }
    }

    public void UpdateTravelers()
    {
        foreach (var traveller in trackedTravellers)
        {
            UpdateSliceParams(traveller);
        }
    }

    int SideOfPortal(Vector3 pos)
    {
        return System.Math.Sign(Vector3.Dot(pos - here.planeView.transform.position, here.planeView.transform.forward));
    }

    bool SameSideOfPortal(Vector3 posA, Vector3 posB)
    {
        return SideOfPortal(posA) == SideOfPortal(posB);
    }


    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            traveller.EnterPortalThreshold();
            traveller.previousOffsetFromPortal = traveller.transform.position - here.planeView.transform.position;
            trackedTravellers.Add(traveller);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller)
        {
            OnTravellerEnterPortal(traveller);
        }
    }

    void OnTriggerExit(Collider other)
    {
        var traveller = other.GetComponent<PortalTraveller>();
        if (traveller && trackedTravellers.Contains(traveller))
        {
            traveller.ExitPortalThreshold();
            trackedTravellers.Remove(traveller);
        }
    }

}
