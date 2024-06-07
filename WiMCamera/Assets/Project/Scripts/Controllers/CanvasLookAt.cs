using System.Collections;
using System.Collections.Generic;
using Unity.XR.CoreUtils;
using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    public float positionDiff = 0.5f;
    public float scaleDiff = 1.0f;
    public float scaleZ = 0.0f;
    public Transform origin;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale = Vector3.one * scaleDiff  + scaleZ * Vector3.forward;
        transform.position = origin.position + (target.position - origin.position) * positionDiff;
        transform.LookAt((transform.position - target.position) + transform.position);
    }
}
