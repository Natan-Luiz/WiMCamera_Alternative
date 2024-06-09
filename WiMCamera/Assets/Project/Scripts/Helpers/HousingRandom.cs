using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HousingRandom : MonoBehaviour
{
    public List<GameObject> elems;
    // Start is called before the first frame update
    void Start()
    {
        int i = Random.Range(0, elems.Count);
        Instantiate(elems[i], transform.position, transform.rotation, this.transform);
    }
}
