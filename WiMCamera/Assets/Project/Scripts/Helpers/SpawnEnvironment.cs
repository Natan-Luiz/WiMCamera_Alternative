using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct SpawnerElement
{
    public Transform spawnPosition;
    public GameObject element;
    public float frequency;
    public int elements;
    public float timeInit;
}


public class SpawnEnvironment : MonoBehaviour
{
    public List<SpawnerElement> envList;
    public float speed;
    public float time;

    private void Start()
    {
        for (int i = 0; i < envList.Count; i++)
        {
            SpawnerElement spawnerElement = envList[i];
            spawnerElement.timeInit = 0;
            envList[i] = spawnerElement;
        }
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime * speed;

        for(int i = 0; i< envList.Count; i++)
        {
            if ( Mathf.Abs(time - envList[i].timeInit) > envList[i].frequency)
            {
                GameObject go = Instantiate(envList[i].element, envList[i].spawnPosition.position, envList[i].spawnPosition.rotation, envList[i].spawnPosition);
                go.GetComponent<MovableObject>().parentEnv = this;
                SpawnerElement spawnerElement = envList[i];
                spawnerElement.timeInit = time;
                envList[i] = spawnerElement;
            }
        }
    }
}
