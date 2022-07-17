using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfDistant : MonoBehaviour
{
    [SerializeField]
    float maxDistance = 3.5f;

    private void Start()
    {
        GetComponent<Canvas>().enabled = false;
    }

    void FixedUpdate()
    {
        bool isVisible = Vector3.Distance(Camera.main.transform.position, transform.position) < maxDistance;
        GetComponent<Canvas>().enabled = isVisible;
    }
}
