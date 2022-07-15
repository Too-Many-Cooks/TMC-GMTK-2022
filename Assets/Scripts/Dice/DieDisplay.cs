using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieDisplay : MonoBehaviour
{
    public Die die;
    public GameObject d4Prefab;
    public GameObject d6Prefab;
    public GameObject d8Prefab;
    public GameObject d10Prefab;
    public GameObject d12Prefab;

    private GameObject dieObject;

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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
