using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDisplay : MonoBehaviour
{
    public Die die;
    public Vector3 rotationRate;
    public bool rotateInWorldSpace = true;
    public GameObject d4Prefab;
    public GameObject d6Prefab;
    public GameObject d8Prefab;
    public GameObject d10Prefab;
    public GameObject d12Prefab;
    public float cameraDistance = 3f;

    private GameObject dieObject;
    private new Camera camera;

    // Start is called before the first frame update
    void Start()
    {
        switch(die.NumberOfSides)
        {
            case >= 12:
                dieObject = GameObject.Instantiate(d12Prefab, transform);
                break;
            case >= 10:
                dieObject = GameObject.Instantiate(d10Prefab, transform);
                break;
            case >= 8:
                dieObject = GameObject.Instantiate(d8Prefab, transform);
                break;
            case >= 6:
                dieObject = GameObject.Instantiate(d6Prefab, transform);
                break;
            case >= 4:
                dieObject = GameObject.Instantiate(d4Prefab, transform);
                break;
            default:
                dieObject = null;
                break;
        }

        dieObject.layer = gameObject.layer;
        
        CreateCamera();
    }

    // Update is called once per frame
    void Update()
    {
        if (dieObject != null)
        {
            dieObject.transform.Rotate(rotationRate * Time.deltaTime, rotateInWorldSpace ? Space.World : Space.Self);
        }
    }

    private void CreateCamera()
    {
        if (dieObject == null) return;
        
        if (camera == null)
        {
            GameObject obj = new("Camera");
            camera = obj.AddComponent<Camera>();
        }

        Transform cameraTransform = camera.transform;
        cameraTransform.SetParent(transform, false);
        cameraTransform.localPosition = Vector3.forward * cameraDistance;
        cameraTransform.localRotation = Quaternion.LookRotation(-cameraTransform.localPosition.normalized, Vector3.up);
    }
}
