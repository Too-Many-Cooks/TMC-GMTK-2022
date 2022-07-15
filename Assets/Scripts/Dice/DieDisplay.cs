using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDisplay : MonoBehaviour
{
    public Die Die
    {
        get { return _die; }
        set
        {
            if (_die != value)
            {
                _die = value;
                CreateDie();
            }
        }
    }
    [SerializeField]
    private Die _die;
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
        if (dieObject != null)
        {
            Destroy(dieObject);
        }
        CreateDie();

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

    void CreateDie()
    {
        if(dieObject != null)
        {
            if (Application.IsPlaying(dieObject))
            {
                Destroy(dieObject);
            } else {
                DestroyImmediate(dieObject);
            }
        }
        if(Die == null)
        {
            return;
        }
        if (Die.NumberOfSides >= 12 && d12Prefab != null)
        {
            dieObject = GameObject.Instantiate(d12Prefab, transform);
        }
        else if (Die.NumberOfSides >= 10 && d10Prefab != null)
        {
            dieObject = GameObject.Instantiate(d10Prefab, transform);
        }
        else if (Die.NumberOfSides >= 8 && d8Prefab != null)
        {
            dieObject = GameObject.Instantiate(d8Prefab, transform);
        }
        else if (Die.NumberOfSides >= 6 && d6Prefab != null)
        {
            dieObject = GameObject.Instantiate(d6Prefab, transform);
        }
        else if (Die.NumberOfSides >= 4 && d4Prefab != null)
        {
            dieObject = GameObject.Instantiate(d4Prefab, transform);
        } else
        {
            dieObject = null;
        }
    }

    public void OnApplicationQuit()
    {
        if (dieObject != null)
        {
            Destroy(dieObject);
        }
    }

    public void OnValidate()
    {
        if (Application.isPlaying)
        {
            UnityEditor.EditorApplication.delayCall += CreateDie; //Must wait until after inspector updates to make structural changes
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
