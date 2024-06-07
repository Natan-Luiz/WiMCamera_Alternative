using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalSlicing : MonoBehaviour
{
    RelateObjects here;
    RelateObjects there;
    RelateCams portalController;
    List<PortalTraveller> trackedTravellers;
    public float screenThickness = 0.01f;

    void LateUpdate()
    {
        HandleTravellers();
    }

    public void Initialize(RelateObjects local, RelateObjects remote, RelateCams portal)
    {
        portalController = portal;
        here = local;
        there = remote;
        trackedTravellers = new List<PortalTraveller>();
    }

    void HandleTravellers()
    {
        for (int i = 0; i < trackedTravellers.Count; i++)
        {
            PortalTraveller traveller = trackedTravellers[i];
            Transform travellerT = traveller.transform;

            var m = there.worldCenter.transform.localToWorldMatrix * here.worldCenter.transform.worldToLocalMatrix * travellerT.localToWorldMatrix;

            Vector3 offsetFromPortal = travellerT.position - here.worldCenter.transform.position;

            traveller.graphicsClone?.transform.SetPositionAndRotation(m.GetColumn(3), m.rotation);
        }
    }

    void UpdateSliceParams(PortalTraveller traveller)
    {
        traveller.SetCloneSize(portalController.sizeDiff / here.worldCenter.transform.lossyScale.x);
        Vector3 sliceNormal = here.planeView.transform.forward;
        Vector3 cloneSliceNormal = -there.planeView.transform.forward;

        Vector3 slicePos = here.planeView.transform.position;
        Vector3 cloneSlicePos = there.planeView.transform.position;

        float sliceOffsetDst = 0;
        float cloneSliceOffsetDst = 0;

        cloneSliceOffsetDst = -screenThickness;
        return;
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

    void OnTravellerEnterPortal(PortalTraveller traveller)
    {
        if (!trackedTravellers.Contains(traveller))
        {
            if (traveller.EnterPortalThresholdFirstTime())
            {
                traveller.SetCloneSize(portalController.sizeDiff);
            }
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
