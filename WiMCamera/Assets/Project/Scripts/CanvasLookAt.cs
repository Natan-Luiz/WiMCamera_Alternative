using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasLookAt : MonoBehaviour
{
    public Transform origin;
    public Transform target;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = origin.position + (target.position - origin.position) / 2f;
        transform.LookAt((transform.position - target.position) + transform.position);
    }
}
