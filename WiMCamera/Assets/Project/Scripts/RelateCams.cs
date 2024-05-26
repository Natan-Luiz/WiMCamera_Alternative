using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelateCams : MonoBehaviour
{
    public Camera reference;
    public Camera mine;

    public Vector3 origRef;
    public Vector3 origMine;

    public float scale;

    // Start is called before the first frame update
    void Start()
    {
        origRef = reference.transform.position - transform.position;
        origMine = mine.transform.position;
        scale = origMine.magnitude - origRef.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 pos = reference.transform.position - transform.position;
        mine.transform.position = ((pos - origRef) * scale) + origMine;
        mine.transform.rotation = reference.transform.rotation;
    }

    Vector3 Project(Vector3 vec, Vector3 a, Vector3 b)
    {
        return Mul(Div(vec, a), b);
    }

    Vector3 Mul(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x * b.x, a.y * b.y, a.z * b.z);
    }

    Vector3 Div(Vector3 a, Vector3 b)
    {
        return new Vector3(a.x/b.x, a.y/b.y, a.z/b.z);
    }
}
