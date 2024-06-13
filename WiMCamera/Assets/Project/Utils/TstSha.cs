using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TstSha : MonoBehaviour
{
    public Transform p1;
    public Transform p2;
    public Transform p0;

    Material material;

    private void Start()
    {
        material = GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        material.SetVector("_wp1", p1.position);
        material.SetVector("_wp2", p2.position);
        material.SetVector("_wp0", p0.position);
    }
}
