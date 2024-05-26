using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugPositions : MonoBehaviour
{
    public GameObject cam;
    public GameObject target;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("Position: " + (100*(cam.transform.position - target.transform.position)));
        Debug.Log("Rotation: " + (cam.transform.eulerAngles - target.transform.eulerAngles));
    }
}
