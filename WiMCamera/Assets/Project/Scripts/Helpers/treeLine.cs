using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class treeLine : MonoBehaviour
{
    public float frequency;
    public GameObject spawner;

    public Transform store;


    // Start is called before the first frame update
    void Start()
    {
        if(transform.childCount < 2)
        {
            return;
        }

        for(int i = 0; i < transform.childCount - 1; i++)
        {
            FormLine(i, i+1);
        }
    }

    private void FormLine(int ini, int end)
    {
        Vector3 iniPoint = transform.GetChild(ini).position;
        Vector3 endPoint = transform.GetChild(end).position;

        float distance = Vector3.Distance(endPoint, iniPoint);
        Vector3 direction = (endPoint - iniPoint).normalized;
        while (distance > 0)
        {
            Instantiate(spawner, iniPoint + (direction * distance), transform.GetChild(ini).rotation, store);
            distance -= frequency;
        }
    }
}
