using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDisplay : MonoBehaviour
{
    public Die die;
    private GameObject dieObject;

    // Start is called before the first frame update
    void Start()
    {
        switch(die.NumberOfSides)
        {
            case 6:
                dieObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
                dieObject.transform.parent = transform;
                break;
            default:
                dieObject = null;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
