using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class minimapPiece : MonoBehaviour
{
    public GameObject reference;
    public string originalName;

    // Start is called before the first frame update
    void Start()
    {
        originalName = new string(name);
        gameObject.name = "m_" + gameObject.name;
        GetReference();
    }

    public void GetReference()
    {
        reference = GameObject.Find(originalName);
    }

    // Update is called once per frame
    void Update()
    {
        if (reference.transform.hasChanged)
        {
            transform.localPosition = reference.transform.localPosition;
            transform.localRotation = reference.transform.localRotation;
            transform.localScale = reference.transform.localScale;
            reference.transform.hasChanged = false;
            transform.hasChanged = false;
        }
        else if (transform.hasChanged)
        {
            reference.transform.localPosition = transform.localPosition;
            reference.transform.localRotation = transform.localRotation;
            reference.transform.localScale = transform.localScale;
            reference.transform.hasChanged = false;
            transform.hasChanged = false;
        }
    }
}
