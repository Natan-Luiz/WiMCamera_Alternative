using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovableObject : MonoBehaviour
{
    public SpawnEnvironment parentEnv;
    Vector3 originalPos;
    public Vector3 direction = Vector3.forward;
    float originalTime;

    private void Start()
    {
        originalPos = transform.position;
        originalTime = parentEnv.time;
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = parentEnv.time - originalTime;
        transform.position = originalPos + direction * currentTime;

        if(Vector3.Distance(transform.position,Vector3.zero) > 50)
        {
            Destroy(gameObject);
        }
    }
}
