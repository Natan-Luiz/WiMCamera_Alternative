using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parameterizing : MonoBehaviour
{
    [Range(0,1)]
    public int dimensions;
    public float scaleRelation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnValidate()
    {
        if (UnityEditor.EditorApplication.isPlaying)
        {
            RelateCams portal = FindObjectOfType<RelateCams>();
            if (portal != null)
            {
                portal.SetScaleAndTransformer(dimensions, scaleRelation);
            }
        }
    }
}
